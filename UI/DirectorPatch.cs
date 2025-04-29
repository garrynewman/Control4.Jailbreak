using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;
using Renci.SshNet;

namespace Garry.Control4.Jailbreak.UI
{
    public partial class DirectorPatch : UserControl
    {
        private readonly MainWindow _mainWindow;

        public DirectorPatch(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;

            InitializeComponent();
        }


        private bool PatchDirector(LogWindow log)
        {
            var sshConnectionInfo = new ConnectionInfo(Address.Text, Username.Text,
                new PasswordAuthenticationMethod(Username.Text, Password.Text))
            {
                RetryAttempts = 1,
                Timeout = TimeSpan.FromSeconds(2)
            };

            using (var ssh = new ScpClient(sshConnectionInfo))
            {
                log.WriteNormal("Connecting to director via SCP.. ");

                try
                {
                    ssh.Connect();
                }
                catch (Exception e)
                {
                    log.WriteError(e);
                    return false;
                }

                log.WriteSuccess(" .. connected!\n");

                // Get the existing certificate
                using (var stream = new MemoryStream())
                {
                    log.WriteNormal("Downloading /etc/openvpn/clientca-prod.pem\n");
                    ssh.Download("/etc/openvpn/clientca-prod.pem", stream);
                    log.WriteSuccess($"Done - got {stream.Length} bytes\n\n");

                    stream.Position = 0;

                    var backupName = $"/etc/openvpn/clientca-prod.{DateTime.Now:yyyy-dd-M--HH-mm-ss}.backup";
                    log.WriteNormal($"Uploading {backupName}\n");
                    ssh.Upload(stream, backupName);
                    log.WriteSuccess("Done!\n\n");

                    log.WriteNormal("Constructing new clientca-prod.pem\n");
                    using (var reader = new StreamReader(stream))
                    {
                        stream.Position = 0;

                        var certificate = reader.ReadToEnd();

                        certificate += "\n";

                        log.WriteNormal("  Reading Certs/public.pem\n");
                        var localCert = File.ReadAllText("Certs/public.pem");

                        var localBackupName = $"Certs/clientca-prod.{DateTime.Now:yyyy-dd-M--HH-mm-ss}.backup";
                        log.WriteNormal($"  Downloading to {localBackupName}\n");
                        File.WriteAllText(localBackupName, certificate);

                        if (certificate.Contains(localCert))
                        {
                            log.WriteError("The certificate on the director already contains our public key!\n");
                            return false;
                        }

                        //
                        // We just add our public key to the end
                        //
                        certificate += localCert;

                        //
                        // This serves no purpose, but it doesn't hurt to have it hanging around
                        //
                        localBackupName += ".new";
                        log.WriteNormal($"  Downloading to {localBackupName}\n");
                        File.WriteAllText(localBackupName, certificate);


                        //
                        // Upload the modded certificates to the director
                        //
                        log.WriteNormal("Uploading New Certificate..\n");
                        using (var wstream = new MemoryStream())
                        {
                            using (var writer = new StreamWriter(wstream))
                            {
                                writer.Write(certificate);
                                writer.Flush();

                                wstream.Position = 0;
                                ssh.Upload(wstream, "/etc/openvpn/clientca-prod.pem");
                            }
                        }

                        log.WriteSuccess("Done!\n");
                    }
                }
            }

            return true;
        }

        private void PatchDirectorCertificates(object sender, EventArgs e)
        {
            var log = new LogWindow(_mainWindow);

            try
            {
                log.WriteNormal("Copying To Director\n");
                if (!PatchDirector(log))
                {
                    return;
                }

                log.WriteNormal("\n\n");
            }
            catch (Exception ex)
            {
                log.WriteError(ex);
            }
        }

        private void OnAddressChanged(object sender, EventArgs e)
        {
            _ = WorkoutPassword();
        }

        private async Task WorkoutPassword()
        {
            var address = Address.Text;

            await Task.Run(() =>
                {
                    var password = GetDirectorRootPassword(address);
                    if (password != null)
                    {
                        Invoke((Action)(() =>
                        {
                            if (Address.Text == address)
                            {
                                Password.Text = password;
                            }
                        }));
                    }
                }
            );
        }

        // ReSharper disable InconsistentNaming
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        private static extern int SendARP(int DestIP, int SrcIP, [Out] byte[] pMacAddr, ref int PhyAddrLen);
        // ReSharper restore InconsistentNaming

        private static string GetDirectorRootPassword(string address)
        {
            var salt = Convert.FromBase64String("STlqJGd1fTkjI25CWz1hK1YuMURseXA/UnU5QGp6cF4=");

            try
            {
                var hostIpAddress = IPAddress.Parse(address);
                var ab = new byte[6];
                var len = ab.Length;
                var r = SendARP(BitConverter.ToInt32(hostIpAddress.GetAddressBytes(), 0), 0, ab, ref len);
                if (r != 0) return null;
                var macAddress = BitConverter.ToString(ab, 0, 6).Replace("-", "");

                var password = Convert.ToBase64String(
                    new Rfc2898DeriveBytes(macAddress, salt, macAddress.Length * 397, HashAlgorithmName.SHA384)
                        .GetBytes(33));
                return password;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void RebootDirector(object sender, EventArgs e)
        {
            var log = new LogWindow(_mainWindow);

            try
            {
                var sshConnectionInfo = new ConnectionInfo(Address.Text, Username.Text,
                    new PasswordAuthenticationMethod(Username.Text, Password.Text))
                {
                    RetryAttempts = 1,
                    Timeout = TimeSpan.FromSeconds(5)
                };

                log.WriteTrace("Connecting To Director..\n");

                using (var ssh = new SshClient(sshConnectionInfo))
                {
                    ssh.Connect();

                    log.WriteTrace("Connected!\n");

                    log.WriteTrace("Running Reboot Command..\n");
                    var r = ssh.RunCommand("reboot");
                    log.WriteTrace($"Response Was: {r.Result}\n");

                    log.WriteSuccess(
                        "Your system is rebooting - it can take a while - don't panic, give it 10 minutes!");
                }
            }
            catch (Exception ex)
            {
                log.WriteError(ex);
            }
        }
    }
}
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Garry.Control4.Jailbreak.UI
{
    public partial class Composer : UserControl
    {
        private readonly MainWindow _mainWindow;

        public Composer(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;

            InitializeComponent();
        }

        private void PatchComposer(object sender, EventArgs eventargs)
        {
            const string oldLine = @"  <system.net>
    <connectionManagement>
      <add address=""*"" maxconnection=""20"" />
    </connectionManagement>
  </system.net>";
            const string newLine = @"   <system.net>
    <connectionManagement>
      <add address=""*"" maxconnection=""20"" />
    </connectionManagement>
    <defaultProxy>
      <proxy usesystemdefault=""false"" proxyaddress=""http://127.0.0.1:31337/"" bypassonlocal=""True""/>
    </defaultProxy>
  </system.net>

";

            var log = new LogWindow(_mainWindow);

            log.WriteTrace("Asking for ComposerPro.exe.config location\n");

            var open = new OpenFileDialog();
            open.Filter = @"Config Files|*.config";
            open.Title = @"Find Original ComposerPro.exe.config";
            open.InitialDirectory = "C:\\Program Files (x86)\\Control4\\Composer\\Pro";
            open.FileName = "ComposerPro.exe.config";

            if (open.ShowDialog() != DialogResult.OK)
            {
                log.WriteError("Cancelled\n");
                return;
            }

            if (string.IsNullOrEmpty(open.FileName))
            {
                log.WriteError("Filename was invalid\n");
                return;
            }

            log.WriteNormal("Opening ");
            log.WriteHighlight($"{open.FileName}\n");

            var contents = System.IO.File.ReadAllText(open.FileName);

            if (!contents.Contains(oldLine))
            {
                log.WriteHighlight("Couldn't find the line - probably already patched??");
                return;
            }

            log.WriteHighlight("Writing Backup..\n");
            System.IO.File.WriteAllText(open.FileName + $".backup-{DateTime.Now:yyyy-dd-M--HH-mm-ss}", contents);

            log.WriteHighlight("Writing New File..\n");
            contents = contents.Replace(oldLine, newLine);

            System.IO.File.WriteAllText(open.FileName, contents);
            log.WriteHighlight("Done!\n");
        }

        private void SearchGoogleForComposer(object sender, EventArgs e)
        {
            Process.Start("https://www.google.com/search?q=ComposerPro-3.1.3.574885-res.exe");
        }

        private void OpenControl4Reddit(object sender, EventArgs e)
        {
            Process.Start("https://www.reddit.com/r/C4diy/");
        }

        private void UpdateCertificates(object sender, EventArgs e)
        {
            var log = new LogWindow(_mainWindow);

            try
            {
                log.WriteNormal("Copying To Composer\n");
                UpdateComposerCertificate(log);
            }
            catch (Exception ex)
            {
                log.WriteError(ex);
            }
        }


        private static void UpdateComposerCertificate(LogWindow log)
        {
            var configFolder =
                $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Control4\\Composer";

            log.WriteNormal("\nCreating new Composer Key\n");
            var exitCode = RunProcessPrintOutput(
                log,
                Constants.OpenSslExe,
                "genrsa " +
                "-out Certs/composer.key " +
                "1024 " +
                $"-config \"{Constants.OpenSslConfig}\"");

            if (exitCode != 0)
            {
                log.WriteError("Failed.");
                return;
            }

            log.WriteNormal("\nCreating Signing Request\n");
            exitCode = RunProcessPrintOutput(
                log,
                Constants.OpenSslExe,
                "req -new -nodes " +
                "-key Certs/composer.key " +
                $"-subj \"/C=US/ST=Utah/L=Draper/CN={Constants.CertificateCn}/\" " +
                "-out Certs/composer.csr " +
                $"-config \"{Constants.OpenSslConfig}\"");

            if (exitCode != 0)
            {
                log.WriteError("Failed.");
                return;
            }

            log.WriteNormal("\nSigning Request\n");
            exitCode = RunProcessPrintOutput(
                log,
                Constants.OpenSslExe,
                "ca " +
                $"-subj \"/C=US/ST=Utah/L=Draper/CN={Constants.CertificateCn}/\" " +
                "-preserveDN " +
                "-days 365 " +
                "-batch " +
                "-create_serial " +
                "-cert Certs/public.pem " +
                "-keyfile Certs/private.key " +
                "-out Certs/composer.pem " +
                "-in Certs/composer.csr " +
                $"-config \"{Constants.OpenSslConfig}\"");

            if (exitCode != 0)
            {
                log.WriteError("Failed.");
                return;
            }

            //
            // Create the composer.p12 (public key) which sits in your composer config folder
            //
            log.WriteNormal("Creating composer.p12\n");
            exitCode = RunProcessPrintOutput(
                log,
                Constants.OpenSslExe,
                "pkcs12 " +
                "-export " +
                "-out \"Certs/composer.p12\" " +
                "-inkey \"Certs/composer.key\" " +
                "-in \"Certs/composer.pem\" " +
                $"-passout pass:{Constants.CertPassword}");

            if (exitCode != 0)
            {
                log.WriteError("Failed.");
                return;
            }

            //
            // Get the text for the composer cacert-*.pem
            //
            log.WriteNormal($"Creating {Constants.ComposerCertName}\n");
            var output = RunProcessGetOutput(Constants.OpenSslExe, "x509 -in \"Certs/public.pem\" -text");
            System.IO.File.WriteAllText($"Certs/{Constants.ComposerCertName}", output);

            CopyFile(log, $"Certs/{Constants.ComposerCertName}", $"{configFolder}\\{Constants.ComposerCertName}");
            CopyFile(log, "Certs/composer.p12", $"{configFolder}\\composer.p12");

            log.WriteNormal("\n\n");
            log.WriteSuccess("Success - composer should be good for 30 days\n\n");
            log.WriteSuccess(
                "Once it starts complaining that you had x days left to renew, just run this step again\n\n");
            log.WriteSuccess(
                "You shouldn't need to patch your Director again unless you update to a new version or delete the Certs folder next to this exe.\n\n");
        }

        private static void CopyFile(LogWindow log, string a, string b)
        {
            log.WriteNormal("Copying ");
            log.WriteHighlight(a);
            log.WriteNormal(" to ");
            log.WriteHighlight(b);
            log.WriteNormal("\n");

            System.IO.File.Copy(a, b, true);
        }

        private static int RunProcessPrintOutput(LogWindow log, string exe, string arguments)
        {
            log.WriteNormal(System.IO.Path.GetFileName(exe));
            log.WriteNormal(" ");
            log.WriteHighlight(arguments);
            log.WriteNormal("\n");

            var startInfo = new ProcessStartInfo(exe, arguments)
            {
                WorkingDirectory = Environment.CurrentDirectory,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var process = Process.Start(startInfo);
            if (process == null)
            {
                log.WriteError($"Failed to start {exe} {arguments}\n");
                return -1;
            }

            log.WriteTrace(process.StandardOutput.ReadToEnd());
            log.WriteTrace(process.StandardError.ReadToEnd());

            process.WaitForExit();

            log.WriteTrace(process.StandardError.ReadToEnd());
            log.WriteTrace(process.StandardOutput.ReadToEnd());

            log.WriteNormal("\n");

            return process.ExitCode;
        }

        private static string RunProcessGetOutput(string exe, string arguments)
        {
            var startInfo = new ProcessStartInfo(exe, arguments)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            var process = Process.Start(startInfo);

            return process?.StandardOutput.ReadToEnd();
        }
    }
}
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Garry.Control4.Jailbreak.UI
{
    public partial class Certificates : UserControl
    {
        private readonly MainWindow _mainWindow;

        public Certificates(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;

            InitializeComponent();
        }

        private void GenerateCertificates(object sender, EventArgs e)
        {
            var log = new LogWindow(_mainWindow);

            log.WriteNormal("Generating Certificates\n");
            if (!GenerateCertificates(log))
            {
                return;
            }

            log.WriteSuccess("Certificate Generation Successful");
            log.WriteNormal("\n\n");
        }

        private void ViewCertificates(object sender, EventArgs e)
        {
            var folder = System.IO.Path.GetFullPath("Certs");

            if (!System.IO.Directory.Exists(folder))
            {
                var log = new LogWindow(_mainWindow);
                log.WriteError($"{folder}doesn't exist - did you generate certificates yet?\n");
                return;
            }

            Process.Start("explorer.exe", folder);
        }

        private static bool GenerateCertificates(LogWindow log)
        {
            //
            // Don't regenerate the certificates. They might be copying the folder
            // over to another computer or some shit.
            //
            if (System.IO.File.Exists($"Certs/{Constants.ComposerCertName}") &&
                System.IO.File.Exists("Certs/composer.p12") &&
                System.IO.File.Exists("Certs/private.key") &&
                System.IO.File.Exists("Certs/public.pem"))
            {
                log.WriteSuccess("\nThe certificates already exist - so we're going to use them.\n");
                System.Threading.Thread.Sleep(1000);
                log.WriteSuccess("If you want to generate new certificates delete the Certs folder.\n\n");
                System.Threading.Thread.Sleep(1000);
                return true;
            }

            if (!System.IO.File.Exists(Constants.OpenSslExe))
            {
                log.WriteError($"Couldn't find {Constants.OpenSslExe} - do you have composer installed?");
                return false;
            }

            if (!System.IO.File.Exists(Constants.OpenSslConfig))
            {
                log.WriteError($"Couldn't find {Constants.OpenSslConfig} - do you have composer installed?");
                return false;
            }

            if (!System.IO.Directory.Exists("Certs"))
            {
                log.WriteTrace("Creating Certs Folder\n");
                System.IO.Directory.CreateDirectory("Certs");
            }

            //
            // generate a self-signed private and public key
            //
            log.WriteNormal("\nGenerating private + public keys\n");
            var exitCode = RunProcessPrintOutput(
                log,
                Constants.OpenSslExe,
                "req -new -x509 -sha256 -nodes " +
                $"-days {Constants.CertificateExpireDays} " +
                "-newkey rsa:1024 " +
                "-keyout \"Certs/private.key\" " +
                $"-subj \"/C=US/ST=Utah/L=Draper/O=Control4/OU=Controller Certificates/CN={Constants.CertificateCn}/\" " +
                "-out \"Certs/public.pem\" " +
                $"-config \"{Constants.OpenSslConfig}\"");

            if (exitCode != 0)
            {
                log.WriteError("Failed.");
                return false;
            }

            //
            // Create the composer.p12 (public key) which sits in your composer config folder
            //
            log.WriteNormal("Creating composer.p12\n");
            exitCode = RunProcessPrintOutput(
                log,
                Constants.OpenSslExe,
                "pkcs12 -export " +
                "-out \"Certs/composer.p12\" " +
                "-inkey \"Certs/private.key\" " +
                "-in \"Certs/public.pem\" " +
                $"-passout pass:{Constants.CertPassword}");

            if (exitCode != 0)
            {
                log.WriteError("Failed.");
                return false;
            }

            //
            // Get the text for the composer cacert-*.pem
            //
            log.WriteNormal($"Creating {Constants.ComposerCertName}\n");
            var output = RunProcessGetOutput(
                Constants.OpenSslExe,
                "x509 -in \"Certs/public.pem\" -text"
            );
            System.IO.File.WriteAllText($"Certs/{Constants.ComposerCertName}", output);

            return true;
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

        private static int RunProcessPrintOutput(LogWindow log, string exe, string arguments)
        {
            log.WriteNormal(System.IO.Path.GetFileName(exe));
            log.WriteNormal(" ");
            log.WriteHighlight(arguments);
            log.WriteNormal("\n");

            var startInfo = new ProcessStartInfo(exe, arguments)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            var process = Process.Start(startInfo);
            if (process == null)
            {
                log.WriteError($"Failed to start {exe} {arguments}\n");
                return -1;
            }

            process.WaitForExit();

            var text = process.StandardOutput.ReadToEnd();

            log.WriteTrace(text);

            log.WriteNormal("\n");

            return process.ExitCode;
        }
    }
}
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            checkBoxBlockSplitIo.CheckedChanged += checkBoxBlockSplitIo_CheckedChanged;
            Load += Composer_Load;

        }
        
        private void Composer_Load(object sender, EventArgs e)
        {
            // Load the checkbox state from the application settings
            checkBoxBlockSplitIo.Checked = Properties.Settings.Default.BlockSplitIoChecked;
        }
        
        private void checkBoxBlockSplitIo_CheckedChanged(object sender, EventArgs e)
        {
            // Save the checkbox state to the application settings
            Properties.Settings.Default.BlockSplitIoChecked = checkBoxBlockSplitIo.Checked;
            Properties.Settings.Default.Save();
        }


        private void PatchComposer(object sender, EventArgs eventargs)
        {
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

            try
            {
                // Load the XML document
                var xmlDoc = System.Xml.Linq.XDocument.Load(open.FileName);

                // Find the <system.net> element
                var systemNet = xmlDoc.Root?.Element("system.net");
                if (systemNet == null)
                {
                    log.WriteError("Could not find the <system.net> node in the configuration file.\n");
                    return;
                }

                // Find or add <defaultProxy> element
                var defaultProxy = systemNet.Element("defaultProxy");
                if (defaultProxy != null)
                {
                    log.WriteHighlight("The <defaultProxy> node already exists. The configuration appears to be already patched. No changes were made.");
                    return;
                }

                log.WriteNormal("<defaultProxy> node not found. Adding it...\n");

                defaultProxy = new System.Xml.Linq.XElement("defaultProxy",
                    new System.Xml.Linq.XElement("proxy",
                        new System.Xml.Linq.XAttribute("usesystemdefault", "false"),
                        new System.Xml.Linq.XAttribute("proxyaddress", "http://127.0.0.1:31337/"),
                        new System.Xml.Linq.XAttribute("bypassonlocal", "true")
                    )
                );

                systemNet.Add(defaultProxy);

                log.WriteHighlight("Added <defaultProxy> node.\n");

                // Backup and save the modified XML
                var backupPath = open.FileName + $".backup-{DateTime.Now:yyyy-dd-M--HH-mm-ss}";
                log.WriteHighlight("Writing Backup..\n");
                File.Copy(open.FileName, backupPath);

                log.WriteHighlight("Writing New File..\n");
                xmlDoc.Save(open.FileName);

                log.WriteHighlight("Done!\n");
            }
            catch (Exception ex)
            {
                log.WriteError($"An error occurred: {ex.Message}\n");
            }
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
            var log = new LogWindow(_mainWindow, "Update Composer Certificates");
            try
            {
                UpdateComposerCertificate(log, checkBoxBlockSplitIo.Checked);
            }
            catch (Exception ex)
            {
                log.WriteError(ex);
            }
        }


        private static void UpdateComposerCertificate(LogWindow log, bool blockSplitIo = false)
        {
            if (!File.Exists(Constants.OpenSslExe))
            {
                log.WriteError($"Couldn't find {Constants.OpenSslExe} - do you have composer installed?");
                return;
            }

            if (!File.Exists(Constants.OpenSslConfig))
            {
                log.WriteError($"Couldn't find {Constants.OpenSslConfig} - do you have composer installed?");
                return;
            }
            if (Process.GetProcessesByName("ComposerPro").Length > 0)
            {
                log.WriteError("ComposerPro.exe is currently running. Please close Composer and try again.");
                return;
            }

            log.WriteNormal("\nCreating new Composer Key\n");
            var exitCode = RunProcessPrintOutput(
                log,
                Constants.OpenSslExe,
                "genrsa " +
                $"-out {Constants.CertsFolder}/composer.key " +
                "1024"
            );
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
                $"-key {Constants.CertsFolder}/composer.key " +
                $"-subj \"/C=US/ST=Utah/L=Draper/CN={Constants.CertificateCn}\" " +
                $"-out {Constants.CertsFolder}/composer.csr"
            );

            if (exitCode != 0)
            {
                log.WriteError("Failed.");
                return;
            }

            log.WriteNormal("\nSigning Request\n");
            exitCode = RunProcessPrintOutput(
                log,
                Constants.OpenSslExe,
                "x509 -req " +
                $"-in {Constants.CertsFolder}/composer.csr " +
                $"-CA {Constants.CertsFolder}/public.pem " +
                $"-CAkey {Constants.CertsFolder}/private.key " +
                "-CAcreateserial " +
                $"-out {Constants.CertsFolder}/composer.pem " +
                "-days 365 " +
                "-sha256"
            );
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
                $"-out \"{Constants.CertsFolder}/composer.p12\" " +
                $"-inkey \"{Constants.CertsFolder}/composer.key\" " +
                $"-in \"{Constants.CertsFolder}/composer.pem\" " +
                $"-passout pass:{Constants.CertPassword}"
            );

            if (exitCode != 0)
            {
                log.WriteError("Failed.");
                return;
            }

            var configFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/Control4";
            // These directories won't exist if Composer has not yet been opened.
            if (!Directory.Exists(configFolder))
            {
                Directory.CreateDirectory(configFolder);
            }

            if (!Directory.Exists($"{configFolder}/Composer"))
            {
                Directory.CreateDirectory($"{configFolder}/Composer");
            }

            CopyFile(log, $"{Constants.CertsFolder}/{Constants.ComposerCertName}",
                $"{configFolder}/Composer/{Constants.ComposerCertName}");
            CopyFile(log, $"{Constants.CertsFolder}/composer.p12", $"{configFolder}/Composer/composer.p12");

            // This section is a temporary workaround to allow upgrading to X4 without a valid
            // dealer account. Essentially all that is involved is to block split.io (cloud service
            // for experiment rollouts) and delete the features cache file. The user will then be
            // opted into the "control" which does not require a dealer to upgrade.
            WriteFile(log, $"{configFolder}/Composer/FeaturesConfiguration.json", @"{}");
            if (blockSplitIo)
            {
                AddLineToFile(log, Constants.WindowsHostsFile, Constants.BlockSplitIoHostsEntry);
            }
            else
            {
                RemoveLineFromFile(log, Constants.WindowsHostsFile, Constants.BlockSplitIoHostsEntry);
            }

            // The first time opening Composer will stick you in a login loop
            // without this file present.
            if (!File.Exists($"{configFolder}/dealeraccount.xml"))
            {
                // This is just the file contents after entering username=no and password=way
                WriteFile(log, $"{configFolder}/dealeraccount.xml", @"<?xml version=""1.0"" encoding=""utf-8""?>
<DealerAccount>
  <Username>no</Username>
  <Employee>False</Employee>
  <Password>+bJjU5zcsEI=</Password>
  <UserHash>9390298f3fb0c5b160498935d79cb139aef28e1c47358b4bbba61862b9c26e59</UserHash>
</DealerAccount>");
            }

            log.WriteNormal("\n\n");
            log.WriteSuccess("Success - composer should be good for 30 days\n\n");
            log.WriteSuccess(
                "Once it starts complaining that you have x days left to renew, just run this step again\n\n");
            log.WriteSuccess(
                $"You shouldn't need to patch your Director again unless you update to a new version or delete the {Constants.CertsFolder} folder next to this exe.\n\n");
        }

        private static void CopyFile(LogWindow log, string a, string b)
        {
            log.WriteNormal("Copying ");
            log.WriteHighlight(a);
            log.WriteNormal(" to ");
            log.WriteHighlight(b);
            log.WriteNormal("\n");

            File.Copy(a, b, true);
        }

        private static void WriteFile(LogWindow log, string file, string content)
        {
            log.WriteNormal("Writing ");
            log.WriteHighlight(file);
            log.WriteNormal("\n");

            File.WriteAllText(file, content);
        }

        private static void RemoveLineFromFile(LogWindow log, string file, string line, bool ignoreWhitespace = true)
        {
            log.WriteNormal("Removing line '");
            log.WriteTrace(line);
            log.WriteNormal("' from ");
            log.WriteHighlight(file);
            log.WriteNormal("\n");

            if (!File.Exists(file))
            {
                return;
            }
            var lines = File.ReadAllLines(file);
            var newLines = lines.Where(s => ignoreWhitespace 
                ? s.Trim() != line.Trim() 
                : s != line).ToArray();
                
            if (newLines.Length != lines.Length)
            {
                File.WriteAllLines(file, newLines);
            }
        }
                
        private static void AddLineToFile(LogWindow log, string file, string line, bool ignoreWhitespace = true)
        {
            log.WriteNormal("Adding line '");
            log.WriteTrace(line);
            log.WriteNormal("' to ");
            log.WriteHighlight(file);
            log.WriteNormal("\n");

            if (File.Exists(file))
            {
                var lines = File.ReadAllLines(file);
                if (!lines.Select(s => ignoreWhitespace ? s.Trim() : s).Contains(ignoreWhitespace ? line.Trim() : line))
                {
                    File.AppendAllText(file, line.TrimEnd() + Environment.NewLine);
                }
            }
            else
            {
                File.WriteAllText(file, line.TrimEnd() + Environment.NewLine);
            }
        }

        private static int RunProcessPrintOutput(LogWindow log, string exe, string arguments)
        {
            log.WriteNormal(Path.GetFileName(exe));
            log.WriteNormal(" ");
            log.WriteHighlight(arguments);
            log.WriteNormal("\n");

            var startInfo = new ProcessStartInfo(exe, arguments)
            {
                WorkingDirectory = Environment.CurrentDirectory,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                EnvironmentVariables = { ["OPENSSL_CONF"] = Path.GetFullPath(Constants.OpenSslConfig) }
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
    }
}
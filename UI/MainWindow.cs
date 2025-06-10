using System;
using System.Windows.Forms;
using Garry.Control4.Jailbreak.Properties;

namespace Garry.Control4.Jailbreak.UI
{
    public partial class MainWindow : Form
    {
        private Certificates Certificates { get; }
        private Composer Composer { get; }
        private Director Director { get; }

        public DirectorPatch DirectorPatch { get; }


        public MainWindow()
        {
            InitializeComponent();

            if (!System.IO.Directory.Exists(Constants.CertsFolder))
            {
                System.IO.Directory.CreateDirectory(Constants.CertsFolder);
            }

            if (!System.IO.Directory.Exists(Constants.KeysFolder))
            {
                System.IO.Directory.CreateDirectory(Constants.KeysFolder);
            }

            System.IO.File.WriteAllBytes($"{Constants.CertsFolder}/openssl.cfg", Resources.openssl);

            Text += $@" - v{Constants.Version} - For C4 v{Constants.TargetDirectorVersion}";

            TabControl.TabPages.Clear();

            Director = new Director(this);

            Certificates = new Certificates(this);
            TabControl.TabPages.Add("Certificates");
            Certificates.Parent = TabControl.TabPages[0];
            Certificates.Dock = DockStyle.Fill;

            Composer = new Composer(this);
            TabControl.TabPages.Add("Composer");
            Composer.Parent = TabControl.TabPages[1];
            Composer.Dock = DockStyle.Fill;

            DirectorPatch = new DirectorPatch(this);
            TabControl.TabPages.Add("Director");
            DirectorPatch.Parent = TabControl.TabPages[2];
            DirectorPatch.Dock = DockStyle.Fill;

            CenterToScreen();

            Load += OnLoaded;
        }

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            DirectorDisconnected();

            Director.RefreshList();
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        public void SetStatusRight(string txt)
        {
            StatusTextRight.Text = txt;
        }

        private void OpenComposerFolder(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("C:\\Program Files (x86)\\Control4\\Composer");
        }

        private void OpenComposerSettingsFolder(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(
                $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Control4");
        }

        private void DirectorDisconnected()
        {
            Director.DirectorDisconnected();
        }

        private void ViewOnGithub(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/garrynewman/Control4.Jailbreak");
        }

        private void FileAndQuit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void VisitC4Diy(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.reddit.com/r/C4diy/");
        }
    }
}
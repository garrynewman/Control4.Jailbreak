using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace  Garry.Control4.Jailbreak
{
	public partial class MainWindow : Form
	{
		public Director Director { get; set; }
		public Backup Backup { get; set; }
		public Composer Composer { get; set; }
		public DirectorPatch DirectorPatch { get; set; }
		public Restore Restore { get; set; }

		public DirectorManager ConnectedDirector { get; set; }

		public MainWindow()
		{
			InitializeComponent();

			this.Text += $" - v{Constants.Version} - For C4 v{Constants.TargetDirectorVersion}";

			MainTabControl.TabPages.Clear();

			Director = new Director( this );
			Director.Parent = AddTab( "Director" );
			Director.Dock = DockStyle.Fill;

			Backup = new Backup( this );
			Backup.Parent = AddTab( "Backup" );
			Backup.Dock = DockStyle.Fill;

			Composer = new Composer( this );
			Composer.Parent = AddTab( "Composer" );
			Composer.Dock = DockStyle.Fill;

			DirectorPatch = new DirectorPatch( this );
			DirectorPatch.Parent = AddTab( "Director Patch" );
			DirectorPatch.Dock = DockStyle.Fill;

			Restore = new Restore( this );
			Restore.Parent = AddTab( "Restore" );
			Restore.Dock = DockStyle.Fill;

			MainTabControl.SelectedTab = MainTabControl.TabPages[0];
			
			CenterToScreen();

			Load += OnLoaded;
		}

		private async void OnLoaded( object sender, EventArgs e )
		{
			DirectorDisconnected();

			await Director.RefreshList();
		}

		private Control AddTab( string v )
		{
			MainTabControl.TabPages.Add( v );

			var tabPage = MainTabControl.TabPages[MainTabControl.TabPages.Count - 1];
			tabPage.UseVisualStyleBackColor = true;
			tabPage.Padding = new Padding( 10 );

			return tabPage;
		}

		private void OnFormClosed( object sender, FormClosedEventArgs e )
		{
			Application.Exit();
		}

		public void SetStatusRight( string txt )
		{
			this.StatusTextRight.Text = txt;
		}

		private void OpenComposerFolder( object sender, EventArgs e )
		{
			System.Diagnostics.Process.Start( $"C:\\Program Files (x86)\\Control4\\Composer" );
		}

		private void OpenComposerSettingsFolder( object sender, EventArgs e )
		{
			System.Diagnostics.Process.Start( $"{Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData )}\\Control4" );
		}

		internal void DirectorDisconnected()
		{
			ConnectedDirector = null;

			Director.DirectorDisconnected();
			Backup.Enabled = false;
			DirectorPatch.Enabled = false;
			Restore.Enabled = false;
		}

		private void ViewOnGithub( object sender, EventArgs e )
		{
			System.Diagnostics.Process.Start( $"https://github.com/garrynewman/Control4.Jailbreak" );
		}

		private void FileAndQuit( object sender, EventArgs e )
		{
			Application.Exit();
		}
	}
}

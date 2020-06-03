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
		public Composer Composer { get; set; }
		public DirectorPatch DirectorPatch { get; set; }
		public DirectorManager ConnectedDirector { get; set; }

		public MainWindow()
		{
			InitializeComponent();

			this.Text += $" - v{Constants.Version} - For C4 v{Constants.TargetDirectorVersion}";

			Director = new Director( this );
			Director.Parent = FlowPanel;
			Director.Hide();

			Composer = new Composer( this );
			Composer.Parent = FlowPanel;

			DirectorPatch = new DirectorPatch( this );
			DirectorPatch.Parent = FlowPanel;
		
			CenterToScreen();

			Load += OnLoaded;
		}

		private async void OnLoaded( object sender, EventArgs e )
		{
			DirectorDisconnected();

			await Director.RefreshList();
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
		}

		private void ViewOnGithub( object sender, EventArgs e )
		{
			System.Diagnostics.Process.Start( $"https://github.com/garrynewman/Control4.Jailbreak" );
		}

		private void FileAndQuit( object sender, EventArgs e )
		{
			Application.Exit();
		}

		private void VisitC4Diy( object sender, EventArgs e )
		{
			System.Diagnostics.Process.Start( $"https://www.reddit.com/r/C4diy/" );
		}
	}
}

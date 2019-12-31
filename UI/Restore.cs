using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Mono.Cecil;
using System.Diagnostics;

namespace  Garry.Control4.Jailbreak
{
	public partial class Restore : UserControl
	{
		MainWindow MainWindow;

		public Restore( MainWindow MainWindow )
		{
			this.MainWindow = MainWindow;

			InitializeComponent();
		}

		protected override void OnVisibleChanged( EventArgs e )
		{
			base.OnVisibleChanged( e );

			if ( this.Visible )
			{
				UpdateBackupList();
			}
		}

		void UpdateBackupList()
		{
			if ( MainWindow.ConnectedDirector == null )
				return;

			backupList.Items.Clear();
			restoreFromDirector.Enabled = false;

			using ( var ssh = MainWindow.ConnectedDirector.SshClient )
			{
				ssh.Connect();

				var cmd = ssh.RunCommand( "ls /etc/openvpn/clientca-prod.*.backup" );

				foreach ( var line in cmd.Result.Split( new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries  ) )
				{
					backupList.Items.Add( System.IO.Path.GetFileName( line ) );
				}
			}
		}

		private void RestoreFromFile( object sender, EventArgs e )
		{
			var open = new OpenFileDialog();
			open.Filter = "Certificate File|*.pem";
			open.Title = "Find clientca-prod.pem";
			open.FileName = "clientca-prod-downloadedbackup.pem";

			if ( open.ShowDialog() != DialogResult.OK )
				return;

			var file = System.IO.File.ReadAllText( open.FileName );

			var log = new LogWindow( MainWindow );
			RestoreCert( log, file );
		}

		private void SelectedIndexChanged( object sender, EventArgs e )
		{
			restoreFromDirector.Enabled = backupList.SelectedIndex >= 0;
		}

		private void RestoreFromBackup( object sender, EventArgs e )
		{
			var log = new LogWindow( MainWindow );

			var selectedItem = "/etc/openvpn/" + backupList.SelectedItem.ToString();

			using ( var ssh = MainWindow.ConnectedDirector.ScpClient )
			{
				log.WriteNormal( $"Connecting to director via SCP.. " );
				ssh.Connect();
				log.WriteSuccess( $" .. connected!\n" );

				// Get the existing certificate
				using ( var stream = new MemoryStream() )
				{
					log.WriteNormal( $"Downloading {selectedItem}\n" );
					ssh.Download( selectedItem, stream );
					log.WriteSuccess( $"Done - got {stream.Length} bytes\n\n" );

					stream.Position = 0;

					using ( StreamReader reader = new StreamReader( stream ) )
					{
						stream.Position = 0;
						var certificate = reader.ReadToEnd();

						RestoreCert( log, certificate );
					}
				}
			}
		}

		private void RestoreCert( LogWindow log, string certificate )
		{
			if ( !certificate.Contains( "BEGIN CERTIFICATE" ) )
			{
				log.WriteError( "This file doesn't seem to be valid?" );
				return;
			}

			using ( var ssh = MainWindow.ConnectedDirector.ScpClient )
			{
				log.WriteNormal( $"Connecting to director via SCP.. " );
				ssh.Connect();
				log.WriteSuccess( $" .. connected!\n" );

				// Get the existing certificate
				using ( var stream = new MemoryStream() )
				{
					using ( StreamWriter writer = new StreamWriter( stream ) )
					{
						writer.Write( certificate );
						writer.Flush();

						stream.Position = 0;

						log.WriteNormal( $"Uploading /etc/openvpn/clientca-prod.pem\n" );
						ssh.Upload( stream, "/etc/openvpn/clientca-prod.pem" );
						log.WriteSuccess( $"Done!\n" );
					}
				}
			}

			MainWindow.ConnectedDirector.Reboot( log );
			MainWindow.DirectorDisconnected();
		}
	}
}

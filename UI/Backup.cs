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
	public partial class Backup : UserControl
	{
		MainWindow MainWindow;

		public Backup( MainWindow MainWindow )
		{
			this.MainWindow = MainWindow;

			InitializeComponent();
		}

		private void MakeABackupOfTheCertificate( object sender, EventArgs e )
		{
			if ( MainWindow.ConnectedDirector == null )
				return;

			using ( var ssh = MainWindow.ConnectedDirector.ScpClient )
			{
				ssh.Connect();

				using ( var stream = new MemoryStream() )
				{
					ssh.Download( "/etc/openvpn/clientca-prod.pem", stream );

					SaveFileDialog save = new SaveFileDialog();
					save.Filter = "Certificate File|*.pem";
					save.Title = "Save clientca-prod.pem";
					save.FileName = "clientca-prod-downloadedbackup.pem";
					save.ShowDialog();

					if ( !string.IsNullOrEmpty( save.FileName ) )
					{
						System.IO.File.WriteAllBytes( save.FileName, stream.ToArray() );
					}
				}

				ssh.Disconnect();
			}
		}
	}
}

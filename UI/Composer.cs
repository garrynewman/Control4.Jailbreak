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
using System.Diagnostics;

namespace  Garry.Control4.Jailbreak
{
	public partial class Composer : UserControl
	{
		const string NewExtension = ".modded.exe";

		MainWindow MainWindow;

		public Composer( MainWindow MainWindow )
		{
			this.MainWindow = MainWindow;

			InitializeComponent();
		}

		private void PatchComposer( object sender, EventArgs eventargs )
		{
			var oldLine = "<setting name=\"ComposerPro_LicensingService_Licensing\" serializeAs=\"String\">";
			var newLine = "<setting name=\"ComposerPro_LicensingService_Licensing\" serializeAs=\"0\">";

			var log = new LogWindow( MainWindow );

			log.WriteTrace( "Asking for ComposerPro.exe.config location\n" );

			OpenFileDialog open = new OpenFileDialog();
			open.Filter = "Config Files|*.config";
			open.Title = "Find Original ComposerPro.exe.config";
			open.InitialDirectory = "C:\\Program Files (x86)\\Control4\\Composer\\Pro";
			open.FileName = "ComposerPro.exe.config";

			if ( open.ShowDialog() != DialogResult.OK )
			{
				log.WriteError( "Cancelled\n" );
				return;
			}

			if ( string.IsNullOrEmpty( open.FileName ) )
			{
				log.WriteError( "Filename was invalid\n" );
				return;
			}

			log.WriteNormal( "Opening " );
			log.WriteHighlight( $"{open.FileName}\n" );

			var contents = System.IO.File.ReadAllText( open.FileName );

			if ( !contents.Contains( oldLine ) )
            {
				log.WriteHighlight( "Couldn't find the line - probably already patched??" );
				return;
            }

			log.WriteHighlight( $"Writing Backup..\n" );
			System.IO.File.WriteAllText( open.FileName + $".backup-{DateTime.Now.ToString( "yyyy-dd-M--HH-mm-ss" )}", contents );

			log.WriteHighlight( $"Writing New File..\n" );
			contents = contents.Replace( oldLine, newLine );

			System.IO.File.WriteAllText( open.FileName, contents );
			log.WriteHighlight( $"Done!\n" );
		}

		private void SearchGoogleForComposer( object sender, EventArgs e )
		{
			System.Diagnostics.Process.Start( $"https://www.google.com/search?q=ComposerPro-3.1.3.574885-res.exe" );
		}

		private void OpenControl4Reddit( object sender, EventArgs e )
		{
			System.Diagnostics.Process.Start( $"https://www.reddit.com/r/C4diy/" );
		}

        private void UpdateCertificates( object sender, EventArgs e )
        {
			var log = new LogWindow( MainWindow );

			log.WriteNormal( "Copying To Composer\n" );
			if ( !PatchComposer( log ) )
			{
				return;
			}
			log.WriteNormal( "\n\n" );
		}


		bool PatchComposer( LogWindow log )
		{
			var configFolder = $"{Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData )}\\Control4\\Composer";

			CopyFile( log, $"Certs/{Constants.ComposerCertName}", $"{configFolder}\\{Constants.ComposerCertName}" );
			CopyFile( log, $"Certs/composer.p12", $"{configFolder}\\composer.p12" );

			return true;
		}

		private void CopyFile( LogWindow log, string a, string b )
		{
			log.WriteNormal( $"Copying " );
			log.WriteHighlight( a );
			log.WriteNormal( $" to " );
			log.WriteHighlight( b );
			log.WriteNormal( $"\n" );

			System.IO.File.Copy( a, b, true );
		}
	}
}

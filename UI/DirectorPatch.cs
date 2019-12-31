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
	public partial class DirectorPatch : UserControl
	{
		MainWindow MainWindow;

		public DirectorPatch( MainWindow MainWindow )
		{
			this.MainWindow = MainWindow;

			InitializeComponent();
		}

		private void JailbreakDirector( object sender, EventArgs e )
		{
			var log = new LogWindow( MainWindow );

			log.WriteNormal( "Generating Certificates\n" );
			if ( !GenerateCertificates( log ) )
			{
				return;
			}
			log.WriteSuccess( "Certificate Generation Successful" );
			log.WriteNormal( "\n\n" );

			log.WriteNormal( "Copying To Composer\n" );
			if ( !PatchComposer( log ) )
			{
				return;
			}
			log.WriteNormal( "\n\n" );

			log.WriteNormal( "Copying To Director\n" );
			if ( !PatchDirector( log ) )
			{
				return;
			}
			log.WriteNormal( "\n\n" );

			log.WriteNormal( "Restarting Director\n" );
			if ( !RestartDirector( log ) )
			{
				return;
			}
			log.WriteNormal( "\n\n" );

			log.WriteSuccess( "100% All Done!" );
		}

		bool GenerateCertificates( LogWindow log )
		{ 
			//
			// Don't regenerate the certificates. They might be copying the folder
			// over to another computer or some shit.
			//
			if ( System.IO.File.Exists( $"Certs/{Constants.ComposerCertName}" ) &&
				System.IO.File.Exists( $"Certs/composer.p12" ) &&
				System.IO.File.Exists( $"Certs/private.key" ) &&
				System.IO.File.Exists( $"Certs/public.pem" ) )
			{
				log.WriteSuccess( $"\nThe certificates already exist - so we're going to use them.\n" );
				System.Threading.Thread.Sleep( 1000 );
				log.WriteSuccess( $"If you want to generate new certificates delete the Certs folder.\n\n" );
				System.Threading.Thread.Sleep( 1000 );
				return true;
			}

			if ( !System.IO.File.Exists( Constants.OpenSslExe ) )
			{
				log.WriteError( $"Couldn't find {Constants.OpenSslExe} - do you have composer installed?" );
				return false;
			}

			if ( !System.IO.File.Exists( Constants.OpenSslConfig ) )
			{
				log.WriteError( $"Couldn't find {Constants.OpenSslConfig} - do you have composer installed?" );
				return false;
			}

			if ( !System.IO.Directory.Exists( "Certs" ) )
			{
				log.WriteTrace( "Creating Certs Folder\n" );
				System.IO.Directory.CreateDirectory( "Certs" );
			}

			//
			// generate a self signed private and public key
			//
			log.WriteNormal( "\nGenerating private + public keys\n" );
			var exitCode = RunProcessPrintOutput( log, Constants.OpenSslExe, $"req -new -x509 -sha256 -nodes -days {Constants.CertificateExpireDays} -newkey rsa:1024 -keyout \"Certs/private.key\" -subj \"/C=US/ST=Utah/L=Draper/O=Control4/OU=Controller Certificates/CN={Constants.CertificateCN}/\" -out \"Certs/public.pem\" -config \"{Constants.OpenSslConfig}\"" );

			if ( exitCode != 0 )
			{
				log.WriteError( $"Failed." );
				return false;
			}

			//
			// Create the composer.p12 (public key) which sits in your composer config folder
			//
			log.WriteNormal( "Creating composer.p12\n" );
			exitCode = RunProcessPrintOutput( log, Constants.OpenSslExe, $"pkcs12 -export -out \"Certs/composer.p12\" -inkey \"Certs/private.key\" -in \"Certs/public.pem\" -passout pass:{Constants.CertPassword}" );

			if ( exitCode != 0 )
			{
				log.WriteError( $"Failed." );
				return false;
			}

			//
			// Get the text for the composer cacert-*.pem
			//
			log.WriteNormal( $"Creating {Constants.ComposerCertName}\n" );
			var output = RunProcessGetOutput( Constants.OpenSslExe, $"x509 -in \"Certs/public.pem\" -text" );
			System.IO.File.WriteAllText( $"Certs/{Constants.ComposerCertName}", output );

			return true;

		}

		bool PatchComposer( LogWindow log )
		{
			var configFolder = $"{Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData )}\\Control4\\Composer";

			CopyFile( log, $"Certs/{Constants.ComposerCertName}", $"{configFolder}\\{Constants.ComposerCertName}" );
			CopyFile( log, $"Certs/composer.p12", $"{configFolder}\\composer.p12" );

			return true;
		}

		bool PatchDirector( LogWindow log )
		{
			using ( var ssh = MainWindow.ConnectedDirector.ScpClient )
			{
				log.WriteNormal( $"Connecting to director via SCP.. " );
				ssh.Connect();
				log.WriteSuccess( $" .. connected!\n" );

				// Get the existing certificate
				using ( var stream = new MemoryStream() )
				{
					log.WriteNormal( $"Downloading /etc/openvpn/clientca-prod.pem\n" );
					ssh.Download( "/etc/openvpn/clientca-prod.pem", stream );
					log.WriteSuccess( $"Done - got {stream.Length} bytes\n\n" );

					stream.Position = 0;

					var backupName = $"/etc/openvpn/clientca-prod.{DateTime.Now.ToString( "yyyy-dd-M--HH-mm-ss" )}.backup";
					log.WriteNormal( $"Uploading {backupName}\n" );
					ssh.Upload( stream, backupName );
					log.WriteSuccess( $"Done!\n\n" );

					log.WriteNormal( $"Constructing new clientca-prod.pem\n" );
					using ( StreamReader reader = new StreamReader( stream ) )
					{
						stream.Position = 0;

						var certificate = reader.ReadToEnd();

						certificate += "\n";

						log.WriteNormal( $"  Reading Certs/public.pem\n" );
						var localCert = System.IO.File.ReadAllText( "Certs/public.pem" );

						if ( certificate.Contains( localCert ) )
						{
							log.WriteError( $"The certificate on the director already contains our public key!\n" );
						}
						else
						{
							//
							// We just add our public key to the end
							//
							certificate += localCert;
						}

						//
						// This serves no purpose but it doesn't hurt to have it hanging around
						//
						log.WriteNormal( $"  Writing to Certs/clientca-prod.pem\n" );
						System.IO.File.WriteAllText( "Certs/clientca-prod.pem", certificate );


						//
						// Upload the modded certificates to the director
						//
						log.WriteNormal( $"Uploading New Certificate..\n" );
						using ( var wstream = new MemoryStream() )
						{
							using ( StreamWriter writer = new StreamWriter( wstream ) )
							{
								writer.Write( certificate );
								writer.Flush();

								wstream.Position = 0;
								ssh.Upload( wstream, "/etc/openvpn/clientca-prod.pem" );
							}
						}

						log.WriteSuccess( $"Done!\n" );
					}
				}
			}

			return true;
		}

		bool RestartDirector( LogWindow log )
		{
			log.WriteNormal( $"Rebooting..\n" );

			MainWindow.ConnectedDirector.Reboot( log );
			MainWindow.DirectorDisconnected();

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

		string RunProcessGetOutput( string exe, string arguments )
		{
			ProcessStartInfo startInfo = new ProcessStartInfo( exe, arguments );
			startInfo.CreateNoWindow = true;
			startInfo.UseShellExecute = false;
			startInfo.RedirectStandardOutput = true;

			var process = System.Diagnostics.Process.Start( startInfo );

			return process.StandardOutput.ReadToEnd();
		}

		int RunProcessPrintOutput( LogWindow log, string exe, string arguments )
		{
			log.WriteNormal( System.IO.Path.GetFileName( exe ) );
			log.WriteNormal( " " );
			log.WriteHighlight( arguments );
			log.WriteNormal( "\n" );

			ProcessStartInfo startInfo = new ProcessStartInfo( exe, arguments );
			startInfo.CreateNoWindow = true;
			startInfo.UseShellExecute = false;
			startInfo.RedirectStandardOutput = true;

			var process = System.Diagnostics.Process.Start( startInfo );

			process.WaitForExit();

			var text = process.StandardOutput.ReadToEnd();

			log.WriteTrace( text );

			log.WriteNormal( "\n" );

			return process.ExitCode;
		}
	}
}

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
using Renci.SshNet;
using RestSharp;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Net;
using System.Security.Cryptography;

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



		bool PatchDirector( LogWindow log )
		{

			var SshConnectionInfo = new ConnectionInfo( Address.Text.ToString(), Username.Text, new PasswordAuthenticationMethod( Username.Text, Password.Text ) );
			SshConnectionInfo.RetryAttempts = 1;
			SshConnectionInfo.Timeout = TimeSpan.FromSeconds( 6 );

			using ( var ssh = new ScpClient( SshConnectionInfo ) )
			{
				log.WriteNormal( $"Connecting to director via SCP.. " );

				try
				{
					ssh.Connect();
				}
				catch ( System.Exception e )
                {
					
					log.WriteError( e );
					return false;
                }

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

						var localBackupName = $"Certs/clientca-prod.{DateTime.Now.ToString( "yyyy-dd-M--HH-mm-ss" )}.backup";
						log.WriteNormal( $"  Downloading to {localBackupName}\n" );
						System.IO.File.WriteAllText( localBackupName, certificate );

						if ( certificate.Contains( localCert ) )
						{
							log.WriteError( $"The certificate on the director already contains our public key!\n" );
							return false;
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
						localBackupName += ".new";
						log.WriteNormal( $"  Downloading to {localBackupName}\n" );
						System.IO.File.WriteAllText( localBackupName, certificate );


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




		private void OpenSystemManager( object sender, EventArgs e )
        {
			Process.Start( @"C:\Program Files (x86)\Control4\Composer\Pro\Sysman.exe" );
		}

        private void PatchDirectorCertificates( object sender, EventArgs e )
        {
			var log = new LogWindow( MainWindow );

			try
			{

				log.WriteNormal( "Copying To Director\n" );
				if ( !PatchDirector( log ) )
				{
					return;
				}
				log.WriteNormal( "\n\n" );
			}
			catch ( System.Exception ex )
            {
				log.WriteError( ex );
            }
		}

        private void OnAddressChanged( object sender, EventArgs e )
        {
			_ = WorkoutPassword();
        }

		async Task WorkoutPassword()
        {
			var address = Address.Text;

			await Task.Run( () =>
				{
					var password = GetDirectorRootPassword( address );
					if ( password != null )
					{
						Invoke( (Action)( () =>
						{
							if ( Address.Text == address )
							{
								Password.Text = password;
							}
						}) );
					}
				}
			);
		}

		[DllImport( "iphlpapi.dll", ExactSpelling = true )]
		public static extern int SendARP( int DestIP, int SrcIP, [Out] byte[] pMacAddr, ref int PhyAddrLen );

		private static string GetDirectorRootPassword( string address )
		{
			var salt = Convert.FromBase64String( "STlqJGd1fTkjI25CWz1hK1YuMURseXA/UnU5QGp6cF4=" );

			try
			{
				//ignore cert issues on controller
				ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

				var hostIPAddress = IPAddress.Parse( address );
				var ab = new byte[6];
				int len = ab.Length;

				//create request with RestSharp
				var client = new RestClient("https://"+hostIPAddress+"/api/v1/broker_info");
				//if you let this sit it will go on forever, I've given it a 2 sec windows which seems to suffice.
				client.Timeout = 2000;
				var request = new RestRequest(Method.GET);
				IRestResponse response = client.Execute(request);
				dynamic jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);

				//Command doesn't work across VPN or virtual machine
				//var r = SendARP( (int)hostIPAddress.Address, 0, ab, ref len );

				//if you get a 404, version is below 3.1, if brokerVersionDate = null then controller is blank and doesn't have a project.
				//if controller doesn't have a project the password is the default password. 
				if (response.StatusDescription == "OK" && jsonResponse.brokerVersionDate != null)
				{
					
					string responseMac = jsonResponse.macAddr;
					var password = Convert.ToBase64String(new Rfc2898DeriveBytes(responseMac, salt, responseMac.Length * 397, HashAlgorithmName.SHA384).GetBytes(33));
					return password;
				}
				else
				{
					return "t0talc0ntr0l4!";
				}

				
			}
			catch ( System.Exception )
            {
				return null;
            }
		}

        private void RebootDirector( object sender, EventArgs e )
        {
			var log = new LogWindow( MainWindow );

			try
			{
				var SshConnectionInfo = new ConnectionInfo( Address.Text.ToString(), Username.Text, new PasswordAuthenticationMethod( Username.Text, Password.Text ) );
				SshConnectionInfo.RetryAttempts = 1;
				SshConnectionInfo.Timeout = TimeSpan.FromSeconds( 10 );

				log.WriteTrace( "Connecting To Director..\n" );

				using ( var ssh = new SshClient( SshConnectionInfo ) )
				{
					ssh.Connect();

					log.WriteTrace( "Connected!\n" );

					log.WriteTrace( "Running Reboot Command..\n" );
					var r = ssh.RunCommand( "reboot" );
					log.WriteTrace( $"Response Was: {r.Result}\n" );

					log.WriteSuccess( $"Your system is rebooting - it can take a while - don't panic, give it 10 minutes!" );
				}
			}
			catch ( System.Exception ex )
            {
				log.WriteError( ex );
			}
		}
    }
}

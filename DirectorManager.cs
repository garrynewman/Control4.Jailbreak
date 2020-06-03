using  Garry.Control4.Jailbreak.Utility;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace  Garry.Control4.Jailbreak
{
	public class DirectorManager
	{
		private IPAddress address;

		SoapClient Soap;
		ConnectionInfo SshConnectionInfo;

		public string SystemName { get; private set; }
		public string CommonName { get; private set; }
		public string Version { get; private set; }

		public DirectorManager( IPAddress address )
		{
			this.address = address;

			SshConnectionInfo = new ConnectionInfo( address.ToString(), "jailbreak", new PasswordAuthenticationMethod( "jailbreak", "jailbreak" ) );
			SshConnectionInfo.RetryAttempts = 1;
			SshConnectionInfo.Timeout = TimeSpan.FromSeconds( 2 );
		}

		public ScpClient ScpClient => new ScpClient( SshConnectionInfo );
		public SshClient SshClient => new SshClient( SshConnectionInfo );

		public async Task<bool> TryInitialize()
		{

			//
			// Since 3.1.1 Soap doesn't work, we probably need a certificate or something
			//

			//Soap = new SoapClient( address );

			//var getItems = await Soap.Call<Soap.GetItems>( "GetItems", "filter", "1" );
			//var getVersionInfo = await Soap.Call<Soap.GetVersionInfo>( "GetVersionInfo" );
			//var getCommonName = await Soap.Call<Soap.GetCommonName>( "GetCommonName" );

			//SystemName = getItems.SystemItems.All.FirstOrDefault( x => x.Type == 1 ).Name;
			//CommonName = getCommonName.CommonName;
			//Version = getVersionInfo.Versions.All.Single( x => x.Name == "Director" ).VersionNumber;

			Trace.WriteLine( $"Version is {Version}" );

			return true;
		}

		internal async void Reboot( LogWindow log )
		{
			log.WriteNormal( $"\nRebooting Director\n\n" );

			using ( var ssh = SshClient )
			{
				log.WriteTrace( $"\nConnecting via SSH.. " );

				ssh.Connect();

				log.WriteTrace( $"\n .. connected!" );

				log.WriteSuccess( $"\n\nYour director is now rebooting. This can take a few minutes and it's a nervous wait - I know.\n" );
				log.WriteSuccess( $"But nothing we've done here will stop your director from booting up, so don't worry.\n" );

				var cmd = ssh.CreateCommand( "sysman reboot" );

				var task = cmd.BeginExecute();

				while( !task.IsCompleted )
				{
					await Task.Delay( 10 );
				}
			}
		}
	}
}

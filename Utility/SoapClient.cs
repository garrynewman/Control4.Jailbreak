using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace  Garry.Control4.Jailbreak.Utility
{
	class SoapClient
	{
		//
		// Apparently you can't use 5020 non ssl mode in 3.1
		//
		static int port = 5021;

		private IPAddress address;

		public SoapClient( IPAddress address )
		{
			this.address = address;
		}

		public async Task<string> Call( string methodName, params object[] args )
		{
			var argstr = "";

			if ( args.Length != 0 )
			{
				for ( int i = 0; i < args.Length; i += 2 )
				{
					argstr += $"<param name=\"{args[i]}\">{args[i+1]}</param>";
				}
			}

			var msg = $"<c4soap name=\"{methodName}\" async=\"False\">{argstr}M</c4soap>";

			Trace.WriteLine( $"Out: {msg}" );

			return await CallDirect( msg );
		}

		public async Task<T> Call<T>( string methodName, params object[] args )
		{
			var str = await Call( methodName, args );

			str = $"<?xml version=\"1.0\"?>\n{str}";

			XmlSerializer xmlSerializer = new XmlSerializer( typeof( T ), new XmlRootAttribute( "c4soap" ) );
			using ( var strReader = new StringReader( str ) )
			{
				using ( XmlReader reader = XmlReader.Create( strReader ) )
				{
					return (T)xmlSerializer.Deserialize( reader );
				}
			}
		}

		private async Task<string> CallDirect( string msg )
		{
			using ( var client = new TcpClient( address.ToString(), port ) )
			{
				client.ReceiveTimeout = 2000;
				client.SendTimeout = 2000;

				using ( var stream = new SslStream( client.GetStream(), false, ( a, b, c, d ) => true ) )
				{
					stream.ReadTimeout = 1000;
					stream.WriteTimeout = 1000;

					await stream.AuthenticateAsClientAsync( "" );

					var data = Encoding.UTF8.GetBytes( msg );
					stream.Write( data, 0, data.Length );
					stream.WriteByte( 0 );

					await stream.FlushAsync();

					return await ReadResponse( stream );
				}
			}
		}

		private async Task<string> ReadResponse( SslStream stream )
		{
			var data = new Byte[1024];
			int n;
			var done = false;
			var sb = new StringBuilder();
			while ( !done )
			{
				try
				{
					while ( (n = await stream.ReadAsync( data, 0, data.Length )) > 0 )
					{
						// check for last byte
						if ( data[n - 1] == 0 )
						{
							sb.Append( System.Text.Encoding.ASCII.GetString( data, 0, n - 1 ) );
							done = true;
							break;
						}
						sb.Append( System.Text.Encoding.ASCII.GetString( data, 0, n ) );
					}
				}
				catch ( System.IO.IOException )
				{
					return null;
				}

				await Task.Delay( 10 );
			}

			var r = sb.ToString();

			if ( r.Length < 1024 * 10 )
				Trace.WriteLine( $"Out: {r}" );
			else
				Trace.WriteLine( $"Out: {r.Substring( 0, 1024 * 10 )}" );

			return r;
		}
	}
}

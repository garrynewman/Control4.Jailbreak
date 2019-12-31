using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace  Garry.Control4.Jailbreak.Utility
{
	partial class Sddp : IDisposable
	{
		public Socket SearchSocket { get; private set; }
		public byte[] ReceiveBytes = new byte[1024];

		public Action<DeviceResponse> OnResponse;

		EndPoint endPoint;

		public Sddp()
		{
			endPoint = new IPEndPoint( IPAddress.Any, 0 );

			SearchSocket = new Socket( AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp );
			SearchSocket.SetSocketOption( SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1 );
			SearchSocket.SetSocketOption( SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 3 );
			SearchSocket.SetSocketOption( SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption( IPAddress.Parse( "239.255.255.250" ) ) );
			SearchSocket.Bind( endPoint );
			SearchSocket.BeginReceiveFrom( ReceiveBytes, 0, ReceiveBytes.Length, SocketFlags.None, ref endPoint, new AsyncCallback( SearchSocketRecv ), this );
		}

		public void Dispose()
		{
			SearchSocket?.Shutdown( SocketShutdown.Both );
			SearchSocket = null;
		}

		public void Search( string target )
		{
			SendMessage( $"M-SEARCH * HTTP/1.1\r\nHOST: 239.255.255.250:1900\r\nMAN: \"ssdp:discover\"\r\nMX: 5\r\nST: {target}\r\n\r\n\0" );
		}

		public void SendMessage( string message )
		{
			var bytes = Encoding.UTF8.GetBytes( message );
			var hostByName = Dns.GetHostEntry( Dns.GetHostName() );
			var addressList = hostByName.AddressList;
			var remoteEP = new IPEndPoint( IPAddress.Parse( "239.255.255.250" ), 1900 );
			var array = addressList;
			for ( int i = 0; i < array.Length; i++ )
			{
				var iPAddress = array[i];
				var addressBytes = iPAddress.GetAddressBytes();
				var optionValue = (int)addressBytes[0] + ((int)addressBytes[1] << 8) + ((int)addressBytes[2] << 16) + ((int)addressBytes[3] << 24);

				SearchSocket.SetSocketOption( SocketOptionLevel.IP, SocketOptionName.MulticastInterface, optionValue );
				SearchSocket.SendTo( bytes, 0, bytes.Length, SocketFlags.None, remoteEP );
			}
		}

		private void SearchSocketRecv( IAsyncResult Result )
		{
			if ( SearchSocket == null )
				return;

			try
			{
				EndPoint endPoint = new IPEndPoint( IPAddress.Any, 0 );
				int num = SearchSocket.EndReceiveFrom( Result, ref endPoint );

				if ( num <= 0 ) return;
				var msg = Encoding.UTF8.GetString( ReceiveBytes, 0, num );

				var lines = msg.Split( '\n', '\r' );

				var headers = lines.Where( x => x.Contains( ":" ) )
									.ToDictionary( 
										x => x.Substring( 0, x.IndexOf( ':' ) ).Trim(), 
										x => x.Substring( x.IndexOf( ':' ) + 1 ).Trim() );

				if ( headers.Count == 0 )
					return;

				var response = new DeviceResponse
				{
					FullHeaders = headers,
					EndPoint = endPoint as IPEndPoint,
				};

				OnResponse?.Invoke( response );
			}
			finally
			{
				SearchSocket.BeginReceiveFrom( ReceiveBytes, 0, ReceiveBytes.Length, SocketFlags.None, ref endPoint, new AsyncCallback( SearchSocketRecv ), this );
			}
		}
	}
}

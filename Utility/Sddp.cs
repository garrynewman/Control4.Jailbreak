using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Garry.Control4.Jailbreak.Utility
{
    internal partial class Sddp : IDisposable
    {
        private Socket SearchSocket { get; set; }
        private readonly byte[] _receiveBytes = new byte[1024];

        public Action<DeviceResponse> OnResponse;

        private EndPoint _endPoint;

        public Sddp()
        {
            _endPoint = new IPEndPoint(IPAddress.Any, 0);

            SearchSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            SearchSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            SearchSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 3);
            SearchSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
                new MulticastOption(IPAddress.Parse("239.255.255.250")));
            SearchSocket.Bind(_endPoint);
            SearchSocket.BeginReceiveFrom(_receiveBytes, 0, _receiveBytes.Length, SocketFlags.None, ref _endPoint,
                SearchSocketRecv, this);
        }

        public void Dispose()
        {
            SearchSocket?.Shutdown(SocketShutdown.Both);
            SearchSocket = null;
        }

        public void Search(string target)
        {
            SendMessage(
                $"M-SEARCH * HTTP/1.1\r\nHOST: 239.255.255.250:1900\r\nMAN: \"ssdp:discover\"\r\nMX: 5\r\nST: {target}\r\n\r\n\0");
        }

        private void SendMessage(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            var hostByName = Dns.GetHostEntry(Dns.GetHostName());
            var addressList = hostByName.AddressList;
            var remoteEp = new IPEndPoint(IPAddress.Parse("239.255.255.250"), 1900);
            foreach (var iPAddress in addressList)
            {
                var addressBytes = iPAddress.GetAddressBytes();
                if (addressBytes.Length > 4) continue;

                var optionValue = addressBytes[0] + (addressBytes[1] << 8) + (addressBytes[2] << 16) +
                                  (addressBytes[3] << 24);

                SearchSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, optionValue);
                SearchSocket.SendTo(bytes, 0, bytes.Length, SocketFlags.None, remoteEp);
            }
        }

        private void SearchSocketRecv(IAsyncResult result)
        {
            if (SearchSocket == null)
                return;

            try
            {
                EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                var num = SearchSocket.EndReceiveFrom(result, ref endPoint);

                if (num <= 0) return;
                var msg = Encoding.UTF8.GetString(_receiveBytes, 0, num);

                var lines = msg.Split('\n', '\r');

                var headers = lines.Where(x => x.Contains(":"))
                    .ToDictionary(
                        x => x.Substring(0, x.IndexOf(':')).Trim(),
                        x => x.Substring(x.IndexOf(':') + 1).Trim());

                if (headers.Count == 0)
                    return;

                var response = new DeviceResponse
                {
                    FullHeaders = headers,
                    EndPoint = endPoint as IPEndPoint
                };

                OnResponse?.Invoke(response);
            }
            finally
            {
                SearchSocket.BeginReceiveFrom(_receiveBytes, 0, _receiveBytes.Length, SocketFlags.None, ref _endPoint,
                    SearchSocketRecv, this);
            }
        }
    }
}
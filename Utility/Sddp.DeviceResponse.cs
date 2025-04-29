using System.Collections.Generic;
using System.Net;

namespace Garry.Control4.Jailbreak.Utility
{
    internal partial class Sddp
    {
        public class DeviceResponse
        {
            public Dictionary<string, string> FullHeaders;
            public IPEndPoint EndPoint;

            // ReSharper disable UnusedMember.Global
            public string Location => GetHeader("Location");
            public string Server => GetHeader("SERVER");
            public string St => GetHeader("ST");
            public string Usn => GetHeader("USN");
            public string Host => GetHeader("Host");
            public string Nts => GetHeader("NTS");
            // ReSharper restore UnusedMember.Global

            private string GetHeader(string name)
            {
                FullHeaders.TryGetValue(name, out var value);
                return value;
            }

            public override string ToString() => $"{EndPoint.Address}";
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace  Garry.Control4.Jailbreak.Soap
{
	public class GetCommonName
	{
		[XmlElement( "common_name" )]
		public string CommonName { get; set; }
	}
}

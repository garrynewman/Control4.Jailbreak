using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace  Garry.Control4.Jailbreak.Soap
{
	public class GetVersionInfo
	{
		public class Version
		{
			[XmlAttribute( "name" )]
			public string Name { get; set; }	
			[XmlAttribute( "version" )]
			public string VersionNumber { get; set; }		
		}

		public class VersionList
		{
			[XmlElement( "version" )]
			public Version[] All { get; set; }
		}

		[XmlElement( "versions" )]
		public VersionList Versions { get; set; }
	}
}

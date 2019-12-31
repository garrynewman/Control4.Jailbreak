using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace  Garry.Control4.Jailbreak.Soap
{
	public class GetItems
	{
		public class SystemItem
		{
			public class Item
			{
				[XmlElement( "id" )]
				public int Id { get; set; }

				[XmlElement( "name" )]
				public string Name { get; set; }

				[XmlElement( "type" )]
				public int Type { get; set; }

				// ItemData has some moire shit
			}

			[XmlElement( "item" )]
			public Item[] All;
		}

		[XmlElement( "systemitems" )]
		public SystemItem SystemItems { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace EMIEMC_Viewer
{

		[XmlRoot(ElementName = "Composite")]
		public class Composite
		{
			[XmlAttribute(AttributeName = "pid")]
			public string Pid { get; set; }
			[XmlAttribute(AttributeName = "collection")]
			public string Collection { get; set; }
			[XmlElement(ElementName = "Items")]
			public Items Items { get; set; }
		}

		[XmlRoot(ElementName = "Waveform")]
		public class Waveform
		{
			[XmlElement(ElementName = "TimeSpanStart")]
			public string TimeSpanStart { get; set; }
			[XmlElement(ElementName = "TimeSpanLength")]
			public string TimeSpanLength { get; set; }
			[XmlElement(ElementName = "InternalYUnits")]
			public string InternalYUnits { get; set; }
			[XmlElement(ElementName = "InternalXUnits")]
			public string InternalXUnits { get; set; }
			[XmlElement(ElementName = "YUnits")]
			public string YUnits { get; set; }
			[XmlElement(ElementName = "XUnits")]
			public string XUnits { get; set; }
			[XmlElement(ElementName = "MinMax")]
			public string MinMax { get; set; }
			[XmlElement(ElementName = "Count")]
			public string Count { get; set; }
			[XmlElement(ElementName = "x")]
			public List<string> X { get; set; }
			[XmlElement(ElementName = "y")]
			public List<string> Y { get; set; }
			[XmlElement(ElementName = "lastcontiguous")]
			public List<string> Lastcontiguous { get; set; }
			[XmlAttribute(AttributeName = "pid")]
			public string Pid { get; set; }
			[XmlAttribute(AttributeName = "name")]
			public string Name { get; set; }
		}

		[XmlRoot(ElementName = "Items")]
		public class Items
		{
			[XmlElement(ElementName = "Waveform")]
			public Waveform Waveform { get; set; }
			[XmlElement(ElementName = "Composite")]
			public List<Composite> Composite { get; set; }
	}

		[XmlRoot(ElementName = "Internal")]
		public class Internal
		{
			[XmlElement(ElementName = "Composite")]
			public Composite Composite { get; set; }
		}

		[XmlRoot(ElementName = "RSAPersist")]
		public class RSAPersist
		{
			[XmlElement(ElementName = "Application")]
			public string Application { get; set; }
			[XmlElement(ElementName = "Internal")]
			public Internal Internal { get; set; }
			[XmlAttribute(AttributeName = "version")]
			public string Version { get; set; }

		}


}

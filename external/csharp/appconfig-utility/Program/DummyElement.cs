using System;
using System.Xml;
using System.Configuration;

namespace ExampleApplication {
	// https://docs.microsoft.com/en-us/dotnet/api/system.configuration.configurationelement?view=netframework-4.5
	public class DummyElement: ConfigurationElement {
		// https://docs.microsoft.com/en-us/dotnet/api/system.configuration.configurationelement.deserializeelement?view=netframework-4.5
		protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey) {
			Value = reader.ReadElementContentAs(typeof(string), null) as string;
		}
		public string Value { get; private set; }
	}
}

using System;
using System.Configuration;

namespace Utils {
	// https://docs.microsoft.com/en-us/dotnet/api/system.configuration.configurationsection?view=netframework-4.5
	public class ScriptElement : ConfigurationSection {
		[ConfigurationProperty("source")]
		public ValueElement Source { get { return this["source"] as ValueElement; } }
		[ConfigurationProperty("name", DefaultValue = "", IsRequired = true, IsKey = true)]
		public string Name {
			get { return (string)this["name"]; }
			set { this["name"] = value; }
		}

	}
}

using System;
using System.Configuration;

namespace ExampleApplication {
	// https://docs.microsoft.com/en-us/dotnet/api/system.configuration.configurationsection?view=netframework-4.5
	public class SqlElement : ConfigurationSection {
		[ConfigurationProperty("query")]
		public QueryElement Query { get { return this["query"] as QueryElement; } }
		[ConfigurationProperty("name", DefaultValue = "", IsRequired = true, IsKey = true)]
		public string Name {
			get { return (string)this["name"]; }
			set { this["name"] = value; }
		}

	}
}

using System.Configuration;

namespace ExampleApplication {
	// https://docs.microsoft.com/en-us/dotnet/api/system.configuration.configurationelement?view=netframework-4.5
	public class UrlConfigElement : ConfigurationElement {
		[ConfigurationProperty("name", DefaultValue = "Microsoft", IsRequired = true, IsKey = true)]
		public string Name {
			get { return (string)this["name"]; }
			set { this["name"] = value; }
		}

		[ConfigurationProperty("url", DefaultValue = "http://www.microsoft.com", IsRequired = true)] [RegexStringValidator(@"\w+:\/\/[\w.]+\S*")]
		public string Url {
			get { return (string)this["url"]; }
			set { this["url"] = value; }
		}

		[ConfigurationProperty("port", DefaultValue = 0, IsRequired = false)]
		[IntegerValidator(MinValue = 0, MaxValue = 8080, ExcludeRange = false)]
		public int Port {
			get { return (int)this["port"]; }
			set { this["port"] = value; }
		}

		[ConfigurationProperty("content", IsRequired = false, IsKey = false)]
		public DummyElement Content {
			get {
				return
					(DummyElement)this["content"]; }
		}
	}
}

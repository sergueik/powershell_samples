using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace NanoTube.Configuration
{
	public class MetricPublishingConfiguration : ConfigurationSection, IMetricPublishingConfiguration
	{
		public static IMetricPublishingConfiguration FromConfig()
		{
			return (MetricPublishingConfiguration)ConfigurationManager.GetSection("nanoTubePublishing");
		}

		public static IMetricPublishingConfiguration FromConfig(string section)
		{
			return (MetricPublishingConfiguration)ConfigurationManager.GetSection(section);
		}

		[ConfigurationProperty("hostNameOrAddress", IsRequired = true)]
		public string HostNameOrAddress {
			get { return (string)this["hostNameOrAddress"]; }
			set { this["hostNameOrAddress"] = value; }
		}

		[ConfigurationProperty("port", DefaultValue = 8125, IsRequired = false)]
		public int Port {
			get { return (int)this["port"]; }
			set { this["port"] = value; }
		}

		[ConfigurationProperty("prefixKey", DefaultValue = "", IsRequired = false)]
		[RegexStringValidator(@"^[^!\s;:/\.\(\)\\#%\$\^]+$|^$")]
		public string PrefixKey {
			get { return (string)this["prefixKey"]; }
			set { this["prefixKey"] = value; }
		}

		[ConfigurationProperty("format", IsRequired = true)]
		public MetricFormat Format {
			get { return (MetricFormat)this["format"]; }
			set { this["format"] = value; }
		}
	}
}
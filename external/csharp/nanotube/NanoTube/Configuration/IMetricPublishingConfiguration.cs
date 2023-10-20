using System;
using System.Collections.Generic;
using System.Linq;

namespace NanoTube.Configuration {

	public interface IMetricPublishingConfiguration {
		string HostNameOrAddress { get; }		
		int Port { get; }		
		string PrefixKey { get; }
		MetricFormat Format { get; }
	}
}
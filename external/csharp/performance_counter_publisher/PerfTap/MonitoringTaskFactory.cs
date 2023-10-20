﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NanoTube.Configuration;
using NanoTube.Linq;
using NanoTube.Net;
using PerfTap.Configuration;
using PerfTap.Counter;

namespace PerfTap {

	public class MonitoringTaskFactory {
		private readonly ICounterSamplingConfiguration _counterSamplingConfig;
		private readonly IMetricPublishingConfiguration _metricPublishingConfig;
		private readonly List<string> _counterPaths;

		public MonitoringTaskFactory(ICounterSamplingConfiguration counterSamplingConfig, IMetricPublishingConfiguration metricPublishingConfig) {
			if (null == counterSamplingConfig) { throw new ArgumentNullException("counterSamplingConfig"); }
			if (null == metricPublishingConfig) { throw new ArgumentNullException("metricPublishingConfig"); }

			_counterSamplingConfig = counterSamplingConfig;
			_counterPaths = counterSamplingConfig.DefinitionFilePaths
				.SelectMany(path => CounterFileParser.ReadCountersFromFile(path.Path))
				.Union(_counterSamplingConfig.CounterNames.Select(name => name.Name.Trim()))
				.Distinct(StringComparer.CurrentCultureIgnoreCase)
				.ToList();
			_metricPublishingConfig = metricPublishingConfig;
		}

		public Task CreateContinuousTask(CancellationToken cancellationToken) {
			return new Task(() => {
				var reader = new PerfmonCounterReader();
				using (var messenger = new UdpMessenger(_metricPublishingConfig.HostName, _metricPublishingConfig.Port)) {
					foreach (var metricBatch in reader.StreamCounterSamples(_counterPaths, _counterSamplingConfig.SampleInterval, cancellationToken)
						.SelectMany(set => set.CounterSamples.ToGraphiteString(_metricPublishingConfig.PrefixKey))
						.Chunk(10)) {
						messenger.SendMetrics(metricBatch);
					}
				}
			}, cancellationToken);
		}

		public Task CreateTask(CancellationToken cancellationToken, int maximumSamples) {
			return new Task(() => {
					var reader = new PerfmonCounterReader();

				using (var messenger = new UdpMessenger(_metricPublishingConfig.HostName, _metricPublishingConfig.Port)) {
					foreach (var metricBatch in reader.GetCounterSamples(_counterPaths, _counterSamplingConfig.SampleInterval, maximumSamples, cancellationToken)
							.SelectMany(set => set.CounterSamples.ToGraphiteString(_metricPublishingConfig.PrefixKey))
							.Chunk(10)) {
						messenger.SendMetrics(metricBatch);
					}
				}
				}, cancellationToken);
		}
	}
}
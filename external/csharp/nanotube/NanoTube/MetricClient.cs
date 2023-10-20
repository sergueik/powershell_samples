using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using NanoTube.Collections;
using NanoTube.Configuration;
using NanoTube.Core;
using NanoTube.Net;

namespace NanoTube {
	public class MetricClient : IDisposable {
		private readonly static ConcurrentDictionary<string, SimpleObjectPool<UdpMessenger>> _messengerPool
			= new ConcurrentDictionary<string, SimpleObjectPool<UdpMessenger>>();

		private readonly UdpMessenger _messenger;
		private readonly string _key;
		private readonly MetricFormat _format;
		private bool _disposed;

		public MetricClient(IMetricPublishingConfiguration configuration) {
			if (null == configuration) {
				throw new ArgumentNullException("configuration");
			}
			if (string.IsNullOrEmpty(configuration.HostNameOrAddress)) {
				throw new ArgumentException("HostNameOrAddress cannot be null or empty", "configuration");
			}
			if (!configuration.PrefixKey.IsValidKey()) {
				throw new ArgumentException("PrefixKey contains invalid characters", "configuration");
			}

			_messenger = new UdpMessenger(configuration.HostNameOrAddress, configuration.Port);
			_key = configuration.PrefixKey;
			_format = configuration.Format;
		}
		public MetricClient(string hostNameOrAddress, int port, MetricFormat format, string key) {
			if (string.IsNullOrEmpty(hostNameOrAddress)) {
				throw new ArgumentException("cannot be null or empty", "hostNameOrAddress");
			}
			if (!key.IsValidKey()) {
				throw new ArgumentException("contains invalid characters", "key");
			}

			_messenger = new UdpMessenger(hostNameOrAddress, port);
			_key = key;
			_format = format;
		}


		public void Dispose() {
			if (!this._disposed) {
				Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (null != _messenger) {
					_messenger.Dispose();
				}
				this._disposed = true;
			}
		}

		public static void Send(IMetricPublishingConfiguration configuration, IEnumerable<IMetric> metrics) {
			if (null == configuration) {
				throw new ArgumentNullException("configuration");
			}
			if (null == metrics) {
				throw new ArgumentNullException("metrics");
			}

			Send(configuration.HostNameOrAddress, configuration.Port, configuration.Format, configuration.PrefixKey, metrics);
		}

		public static void Send(string hostNameOrAddress, int port, MetricFormat format, string key, IEnumerable<IMetric> metrics) {
			if (string.IsNullOrEmpty(hostNameOrAddress)) {
				throw new ArgumentException("cannot be null or empty", "hostNameOrAddress");
			}
			if (!key.IsValidKey()) {
				throw new ArgumentException("contains invalid characters", "key");
			}
			if (null == metrics) {
				throw new ArgumentNullException("metrics");
			}

			SendToServer(hostNameOrAddress, port, metrics.ToStrings(key, format), false);
		}


		public static void Stream(IMetricPublishingConfiguration configuration, IEnumerable<IMetric> metrics) {
			if (null == configuration) {
				throw new ArgumentNullException("configuration");
			}
			if (null == metrics) {
				throw new ArgumentNullException("metrics");
			}

			Stream(configuration.HostNameOrAddress, configuration.Port, configuration.Format, configuration.PrefixKey, metrics);
		}

		public static void Stream(string hostNameOrAddress, int port, MetricFormat format, string key, IEnumerable<IMetric> metrics) {
			if (string.IsNullOrEmpty(hostNameOrAddress)) {
				throw new ArgumentException("cannot be null or empty", "hostNameOrAddress");
			}
			if (!key.IsValidKey()) {
				throw new ArgumentException("contains invalid characters", "key");
			}
			if (null == metrics) {
				throw new ArgumentNullException("metrics");
			}

			SendToServer(hostNameOrAddress, port, metrics.ToStrings(key, format), true);
		}


		public static void Time(IMetricPublishingConfiguration configuration, Action action) {
			if (null == configuration) {
				throw new ArgumentNullException("configuration");
			}
			if (null == action) {
				throw new ArgumentNullException("action");
			}

			Time(configuration.HostNameOrAddress, configuration.Port, configuration.Format, configuration.PrefixKey, action);
		}

		public static void Time(string hostNameOrAddress, int port, MetricFormat format, string key, Action action) {
			if (string.IsNullOrEmpty(hostNameOrAddress)) {
				throw new ArgumentException("cannot be null or empty", "hostNameOrAddress");
			}
			if (!key.IsValidKey()) {
				throw new ArgumentException("contains invalid characters", "key");
			}
			if (null == action) {
				throw new ArgumentNullException("action");
			}

			Stopwatch timer = null;
			try {
				timer = new Stopwatch();
				timer.Start();
				action();
			} finally {
				if (null != timer) {
					timer.Stop();
					SendToServer(hostNameOrAddress, port, new[] { Metric.Timing(key, timer.Elapsed.TotalSeconds).ToString(null, format) }, false);
				}
			}
		}

		private static void SendToServer(string server, int port, IEnumerable<string> metrics, bool stream) {
			UdpMessenger messenger = null;
			SimpleObjectPool<UdpMessenger> serverPool = null;

			try {
				serverPool = _messengerPool.GetOrAdd(string.Format(CultureInfo.InvariantCulture, "{0}:{1}", server, port),
					new SimpleObjectPool<UdpMessenger>(3, pool => new UdpMessenger(server, port)));
				messenger = serverPool.Pop();

				//all used up, sorry!
				if (null == messenger) {
					return;
				}

				if (stream) {
					messenger.StreamMetrics(metrics);
				} else {
					messenger.SendMetrics(metrics);
				}
			} finally {
				if (null != serverPool && null != messenger) {
					serverPool.Push(messenger);
				}
			}
		}

		public void Send(IMetric metric) {
			if (null == metric) {
				throw new ArgumentNullException("metric");
			}

			_messenger.SendMetrics(new[] { metric.ToString(_key, _format) });
		}

		public void Send(IEnumerable<IMetric> metrics) {
			if (null == metrics) {
				throw new ArgumentNullException("metrics");
			}

			_messenger.SendMetrics(metrics.ToStrings(_key, _format));
		}

		public void Stream(IEnumerable<IMetric> metrics) {
			if (null == metrics) {
				throw new ArgumentNullException("metrics");
			}

			_messenger.StreamMetrics(metrics.ToStrings(_key, _format));
		}
	}
}
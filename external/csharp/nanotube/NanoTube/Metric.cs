using System;
using System.Collections.Generic;
using System.Linq;
using NanoTube.Core;

namespace NanoTube {
	public static class Metric {
		public static Counter Counter(string key, int adjustment)
		{
			if (!key.IsValidKey()) {
				throw new ArgumentException("Key contains invalid characters", "key");
			}

			return new Counter() { Key = key, Adjustment = adjustment };
		}

		public static Counter Increment(string key) {
			if (!key.IsValidKey()) {
				throw new ArgumentException("Key contains invalid characters", "key");
			}

			return new Counter() { Key = key, Adjustment = 1 };
		}

		public static Counter Decrement(string key) {
			if (!key.IsValidKey()) {
				throw new ArgumentException("Key contains invalid characters", "key");
			}

			return new Counter() { Key = key, Adjustment = -1 };
		}

		public static Sample Sample(string key, int @value, double frequency)
		{
			if (!key.IsValidKey()) {
				throw new ArgumentException("Key contains invalid characters", "key");
			}
			if (frequency > 1) {
				throw new ArgumentOutOfRangeException("frequency", "Frequency must be < 1");
			}

			return new Sample() { Key = key, Value = @value, Frequency = frequency };
		}

		public static Timing Timing(string key, double elapsed) {
			if (!key.IsValidKey()) {
				throw new ArgumentException("Key contains invalid characters", "key");
			}
			
			return new Timing() { Key = key, Duration = elapsed };
		}

		public static KeyValue KeyValue(string key, double @value, DateTime timestamp) {
			if (!key.IsValidKey()) {
				throw new ArgumentException("Key contains invalid characters", "key");
			}

			return new KeyValue() { Key = key, Value = @value, Timestamp = timestamp };
		}
	}
}
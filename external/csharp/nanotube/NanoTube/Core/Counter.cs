using System;

namespace NanoTube.Core {

	public struct Counter : IMetric {
		public string Key { get; set; }

		public int Adjustment { get; set; }

		public override int GetHashCode() {
			return Key.GetHashCode() ^ Adjustment;
		}

		public override bool Equals(object obj) {
			if (!(obj is Counter))
				return false;

			return Equals((Counter)obj);
		}

		public bool Equals(Counter other) {
			return Key == other.Key && Adjustment == other.Adjustment;
		}

		public static bool operator ==(Counter counter1, Counter counter2) {
			return counter1.Equals(counter2);
		}

		public static bool operator !=(Counter counter1, Counter counter2) {
			return !counter1.Equals(counter2);
		}
	}
}
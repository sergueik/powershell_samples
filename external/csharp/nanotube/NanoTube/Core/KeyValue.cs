using System;
namespace NanoTube.Core {

	public struct KeyValue : IMetric
	{
		public string Key { get; set; }
		
		public double Value { get; set; }
		
		public DateTime? Timestamp { get; set; }

		public override int GetHashCode()
		{
			return Key.GetHashCode() ^ Value.GetHashCode() ^ Timestamp.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is KeyValue))
				return false;

			return Equals((KeyValue)obj);
		}

		public bool Equals(KeyValue other)
		{
			return Key == other.Key && Value == other.Value && Timestamp == other.Timestamp;
		}

		public static bool operator ==(KeyValue keyValue1, KeyValue keyValue2)
		{
			return keyValue1.Equals(keyValue2);
		}

		public static bool operator !=(KeyValue keyValue1, KeyValue keyValue2)
		{
			return !keyValue1.Equals(keyValue2);
		}
	}
}
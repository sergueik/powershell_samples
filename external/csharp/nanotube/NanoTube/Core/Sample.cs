using System;

namespace NanoTube.Core {

	public struct Sample : IMetric
	{
		public string Key { get; set; }
		public double Value { get; set; }
		
		public double Frequency { get; set; }
		public override int GetHashCode()
		{
			return Key.GetHashCode() ^ Value.GetHashCode() ^ Frequency.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Sample))
				return false;

			return Equals((Sample)obj);
		}

		public bool Equals(Sample other)
		{
			return Key == other.Key && Value == other.Value && Frequency == other.Frequency;
		}

		public static bool operator ==(Sample sample1, Sample sample2)
		{
			return sample1.Equals(sample2);
		}

		public static bool operator !=(Sample sample1, Sample sample2)
		{
			return !sample1.Equals(sample2);
		}
	}
}
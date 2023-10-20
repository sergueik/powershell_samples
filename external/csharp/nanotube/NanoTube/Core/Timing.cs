using System;

namespace NanoTube.Core {

	public struct Timing : IMetric	{
		public string Key { get; set; }
		
		public double Duration { get; set; }

		public override int GetHashCode(){
			return Key.GetHashCode() ^ Duration.GetHashCode();
		}

		public override bool Equals(object obj)	{
			if (!(obj is Timing))
				return false;

			return Equals((Timing)obj);
		}

		public bool Equals(Timing other){
			return Key == other.Key && Duration == other.Duration;
		}

		public static bool operator ==(Timing timing1, Timing timing2){
			return timing1.Equals(timing2);
		}

		public static bool operator !=(Timing timing1, Timing timing2){
			return !timing1.Equals(timing2);
		}
	}
}
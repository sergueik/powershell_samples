using System;

namespace Utils{

	public class Data{
		public DateTime TimeStamp { get; set; }
		public Int32 Value { get; set; }
		public override string ToString() {
			return string.Format("TimeStamp={0} {1}, Value={2}", TimeStamp.ToLongDateString(), TimeStamp.ToLongTimeString(), Value);
		}
	}
}

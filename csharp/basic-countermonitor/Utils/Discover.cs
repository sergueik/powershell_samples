using System;
using System.Threading;
using System.Timers;


namespace Utils {
	// poll-until-success engine
	// will do WMI checks
	// and PDH checks before launching metric collection
	public class Discover {

		private int interval;
		private System.Timers.Timer timer;
		private string argument;
		private Predicate<string> checkCondition;
		public Discover(int interval, Predicate<string> checkCondition, string argument) {
			if (interval <= 0) {
				throw new ArgumentException("invalid interval");
			}
			if (argument == null || argument.Trim().Length == 0) {
				throw new ArgumentException("invalid argument");
			}
			this.interval = interval;
			this.checkCondition = checkCondition;
			this.argument = argument;
		}
		
		public void startPolling() {
			timer = new System.Timers.Timer();
			timer.Interval = this.interval; // every 500 ms
			timer.Elapsed += new ElapsedEventHandler(onTimerElapsed);
			timer.Start();
			Console.Error.WriteLine("started timer with interval: " + this.interval);
		}

		public void stop() {
				timer.Stop();
				timer.Dispose();
				timer = null;
		}

		private void onTimerElapsed(object source, ElapsedEventArgs args) {
			Console.Error.WriteLine("timer elapsed");
			bool done = this.checkCondition(this.argument);
			Console.Error.WriteLine("done: " + done);

			if (done) 
				stop();
		}
	}
}

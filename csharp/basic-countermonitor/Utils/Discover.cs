using System;
using System.Threading;
using System.Timers;


namespace Utils {
	// poll until success,poll to get result engine
	// will do WMI checks
	// and PDH checks before launching metric collection
	public class Discover {

		private int interval;
		private System.Timers.Timer timer;
		private string argument;
		private Predicate<string> checkCondition = null;
		private Func<string, string> getResult = null;
		private string result;
		public string Result { get { 
				Console.Error.WriteLine(String.Format("result: {0}", this.result));
				return result;}
		}
	
		public Discover(int interval, Func<string, string> getResult, string argument) {
			if (interval <= 0) {
				throw new ArgumentException("invalid interval");
			}
			if (argument == null || argument.Trim().Length == 0) {
				throw new ArgumentException("invalid argument");
			}
			if (getResult == null) {
				throw new ArgumentException("invalid getResult");
			}
			this.interval = interval;
			this.getResult = getResult;
			this.argument = argument;			
		}

		public Discover(int interval, Predicate<string> checkCondition, string argument) {
			if (interval <= 0) {
				throw new ArgumentException("invalid interval");
			}
			if (argument == null || argument.Trim().Length == 0) {
				throw new ArgumentException("invalid argument");
			}
			if (checkCondition == null) {
				throw new ArgumentException("invalid checkCondition");
			}
			this.interval = interval;
			this.checkCondition = checkCondition;
			this.argument = argument;
		}

		private void start(ElapsedEventHandler handler) {
			timer = new System.Timers.Timer();
			timer.Interval = interval;
			timer.AutoReset = true;
			timer.Elapsed += handler;
			timer.Start();

			Console.Error.WriteLine("started timer with interval: " + interval);
		}

		// This is useful to detect some long running operation has finished
		public void startPollingForResult() {
			start(this.resultPoll);

			/*
			timer = new System.Timers.Timer();
			timer.Interval = this.interval;
			timer.Elapsed += new ElapsedEventHandler(resultPoll);
			timer.Start();
			Console.Error.WriteLine("started timer with interval: " + this.interval);
		*/
		}

		private void resultPoll(object sender, ElapsedEventArgs e) {
			Console.Error.WriteLine("timer elapsed");
			string result = getResult(this.argument);
			Console.Error.WriteLine(String.Format("result: {0}", result));
			if (!string.IsNullOrEmpty(result)) {
				this.result = result;
				stop();
			}
		}

		// advanced - unused
		private void resultAdvancedPoll(object sender, ElapsedEventArgs e)
		{
			timer.Stop();

			string result = getResult(argument);

			if (!string.IsNullOrEmpty(result)) {
				this.result = result;
				stop();
			} else {
				timer.Start();
			}
		}
		
		// This is useful to detect some long running operation has finished
		public void startCheckingIfFinished() {
			start(this.checkIfFinished);
			/*
			timer = new System.Timers.Timer();
			timer.Interval = this.interval;
			timer.Elapsed += new ElapsedEventHandler(checkIfFinished);
			timer.Start();
			Console.Error.WriteLine("started timer with interval: " + this.interval);
		*/
		}

		private void checkIfFinished(object source, ElapsedEventArgs args) {
			Console.Error.WriteLine("timer elapsed");
			bool done = this.checkCondition(this.argument);
			Console.Error.WriteLine(done ?"not done" : "done");

			if (done) {
				stop();
			}
		}

		public void stop() {
			timer.Stop();
			timer.Dispose();
			timer = null;
		}

	}
}

using System;
using System.Threading;
using System.Timers;
using System.Diagnostics;


namespace Utils {
	// poll until success,poll to get result engine
	// will do WMI checks
	// and PDH checks before launching metric collection
	public class Discover {

		private int interval;
		private System.Timers.Timer timer;
		private string argument1;
		private string argument2;
		private Predicate<string> checkCondition1 = null;
		private Func<string, string> getResult1 = null;
		// While the built-in Predicate<T> delegate exists, it is limited to exactly one input parameter.
		// For two or more parameters, you must use the Func delegate
		private Func<string, string, bool> checkCondition2 = null;
		Func<string, string, string> getResult2 = null;
		private string result;
		public string Result { get { 
				Debug.WriteLine(String.Format("result: {0}", this.result));
				return result;}
		}
	
		public Discover(int interval, Func<string, string> getResult1, string argument1) {
			if (interval <= 0) {
				throw new ArgumentException("invalid interval");
			}
			if (argument1 == null || argument1.Trim().Length == 0) {
				throw new ArgumentException("invalid argument");
			}
			if (getResult1 == null) {
				throw new ArgumentException("invalid getResult");
			}
			this.interval = interval;
			this.getResult1 = getResult1;
			this.argument1 = argument1;			
		}

	
		public Discover(int interval, Func<string, string, string> getResult2, string argument1, string argument2) {
			if (interval <= 0) {
				throw new ArgumentException("invalid interval");
			}
			if (argument1 == null || argument1.Trim().Length == 0) {
				throw new ArgumentException("invalid argument 1");
			}
			if (argument2 == null || argument2.Trim().Length == 0) {
				throw new ArgumentException("invalid argument 2");
			}
			if (getResult2 == null) {
				throw new ArgumentException("invalid getResult");
			}
			this.interval = interval;
			this.getResult2 = getResult2;
			this.argument1 = argument1;			
			this.argument2 = argument2;			
		}

		public Discover(int interval, Func<string, string, bool> checkCondition2, string argument1, string argument2) {
			if (interval <= 0) {
				throw new ArgumentException("invalid interval");
			}
			if (argument1 == null || argument1.Trim().Length == 0) {
				throw new ArgumentException("invalid argument");
			}
			if (argument2 == null || argument2.Trim().Length == 0) {
				throw new ArgumentException("invalid argument");
			}
			if (checkCondition2 == null) {
				throw new ArgumentException("invalid checkCondition");
			}
			if (argument2 == null || argument2.Trim().Length == 0) {
				throw new ArgumentException("invalid argument");
			}
			this.interval = interval;
			this.checkCondition2 = checkCondition2;
			this.argument1 = argument1;			
			this.argument2 = argument2;			
		}

		public Discover(int interval, Predicate<string> checkCondition1, string argument1) {
			if (interval <= 0) {
				throw new ArgumentException("invalid interval");
			}
			if (argument1 == null || argument1.Trim().Length == 0) {
				throw new ArgumentException("invalid argument");
			}
			if (checkCondition1 == null) {
				throw new ArgumentException("invalid checkCondition");
			}
			this.interval = interval;
			this.checkCondition1 = checkCondition1;
			this.argument1 = argument1;
		}

		private void start(ElapsedEventHandler handler) {
			Debug.WriteLine("started timer with interval: " + interval);
			// Debug.WriteLine("started timer with interval: " + interval);
			timer = new System.Timers.Timer();
			timer.Interval = interval;
			timer.AutoReset = true;
			timer.Elapsed += handler;
			timer.Start();

		}

		// This is useful to detect some long running operation has finished
		public void startPollingForResult() {
			start(this.resultPoll);
		}

		private void resultPoll(object sender, ElapsedEventArgs e) {
			Debug.WriteLine("timer elapsed");
			// Debug.WriteLine("timer elapsed");
			timer.Stop();
			string result = getResult2 == null? getResult1(this.argument1):getResult2(this.argument1,this.argument2);
			Debug.WriteLine(String.Format("result: {0}", result));

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
		}

		private void checkIfFinished(object source, ElapsedEventArgs args) {
			Debug.WriteLine("timer elapsed");
			bool done = checkCondition2 == null ?  checkCondition1(argument1)  : checkCondition2(argument1,argument2);
			Debug.WriteLine(done ?"not done" : "done");

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

param(
[switch]$debug # currently unused
)

$cnt = 0
$code = @'
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

		
		// This is useful to detect some long running operation has finished
		public void startCheckingIfFinished() {
			start(this.checkIfFinished);
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

'@



try {
  add-type -typedefinition $code
} catch [Exception] {
  write-output ('{0} {1}' -f $_.Exception.getType().Name, $_.Exception.Message)
}

# will partially work
[int]$interval = 100
# NOTE: is the $intevral is 0 the  ArgumentException "invalid interval" will be thrown
[string]$argument = 'should not be empty'
# NOTE: is the $argument is empty the  ArgumentException "invalid argument" will be thrown
[System.Predicate[string]]$method1 = [System.Predicate[string]] {
   param(
     [string]$arg = ''
   )
   $true
}
[Func[string,string]]$method2 = [Func[string,string]] {
   param(
     [string]$argument = ''
   )
   # explicitly reference the script scope
   $script:cnt = $script:cnt + 1
   write-output ('cnt: {0}' -f $script:cnt)
   # implicit return is not possible
   # Windows powershell 5.1 does not support the C-style ternary operator: it is for 7.x+ only
   # return ( $script:cnt -ge 10 )? 'DONE': ''
   if ( $script:cnt -ge 10 ) {
     return  'DONE' 
   } else {
     return ''
   }

}

try {
  $caller1 = new-object Utils.Discover -argumentlist ($interval, $method1 , $argument)
  if ($caller1 -ne $null) {
    write-output 'created caller1'
  }
} catch [Exception] {
 # MethodInvocationException Exception calling ".ctor" with "3" argument(s): "invalid argument"
  write-output ('{0} {1}' -f $_.Exception.getType().Name, $_.Exception.Message)
}


try {
  $caller2 = new-object Utils.Discover -argumentlist ($interval, $method2 , $argument)
  if ($caller2 -ne $null) {
    write-output 'created caller2'
  }
} catch [Exception] {
  # MethodInvocationException Exception calling ".ctor" with "3" argument(s): "invalid argument"
  # at System.Management.Automation.DotNetAdapter.ConstructorInvokeDotNet(Type type, ConstructorInfo[] constructors, Object[] arguments)
  write-output ('{0} {1}' -f $_.Exception.getType().Name, $_.Exception.ToString())
}
# real seriously try
$discover = $null

try {
  $discover = [Activator]::CreateInstance( [Utils.Discover], [int]$interval, [System.Func[string,string]]$method2 , [string]$argument)
  if ($discover -ne $null) {
    write-output 'created caller3'
  }
} catch [MissingMethodException] {
  write-output ('{0} {1}' -f $_.Exception.getType().Name, $_.Exception.Message)
} catch [Exception] {
  write-output ('{0} {1}' -f $_.Exception.getType().Name, $_.Exception.ToString())
  # ArgumentException System.ArgumentException: invalid argument
  # at Utils.Discover..ctor(Int32 interval, Func`2 getResult, String argument)
# https://learn.microsoft.com/en-us/dotnet/api/system.exception?view=netframework-4.5
}
$script:cnt = 0
$discover.startPollingForResult()
start-sleep -seconds 3
if ($discover.Result -eq 'DONE') { 
  write-output 'it is working'
} else {
  write-output 'it is not working'
}
$discover.Stop()

<#
will log:

started timer with interval: 100
timer elapsed
....
timer elapsed
timer elapsed
timer elapsed
timer elapsed
result:
it is not working
#>
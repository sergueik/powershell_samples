param(
  [string]$pipe = 'demo',
  [string]$message = 'test'
)
# origin: https://www.codeproject.com/Articles/810030/IPC-with-Named-Pipes
# see also: https://docs.microsoft.com/en-us/dotnet/api/system.io.pipes.namedpipeclientstream?redirectedfrom=MSDN&view=net-6.0
# (hanging on reading response)
#
add-Type -typeDefinition @'


using System;
using System.IO;
using System.IO.Pipes;
using System.Text;

public class PipeClient {

	private NamedPipeClientStream pipeClient { get; set; }
	public void Test(string[] args) {
		pipeClient = new NamedPipeClientStream(".", args[0], PipeDirection.InOut, PipeOptions.Asynchronous);

		// Connect to the pipe or wait until the pipe is available.
		Console.Write("Attempting to connect to pipe {0} ...", args[0]);
		pipeClient.Connect();

		Console.WriteLine("Connected to pipe.");
		Console.WriteLine("There are currently {0} pipe server instances open.", pipeClient.NumberOfServerInstances);
		Send(args[1]);
		if (Boolean.Parse(args[2])){
			Console.Write("Press Enter to continue...");
			Console.ReadLine();
		}
	}

	public void Send(string value) {

		Console.WriteLine(String.Format("Sending {0} to pipe.",value));
		byte[] buffer = Encoding.UTF8.GetBytes(value);
		pipeClient.BeginWrite(buffer, 0, buffer.Length, this.SendCallback, pipeClient);
	}
        
	private void SendCallback(IAsyncResult result) {
		var pipeStream = (NamedPipeClientStream) result.AsyncState;
		pipeStream.EndWrite(result);
		Console.WriteLine("Send done.");
	}

 }

'@

$o = new-object PipeClient
$o.Test(@($pipe, $message, $false))

# see also: https://stackoverflow.com/questions/24096969/powershell-named-pipe-no-connection
# non-working

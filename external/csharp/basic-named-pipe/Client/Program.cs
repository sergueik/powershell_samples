using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using Utils;

namespace Client {
	public class Program {
		private static void Main(string[] args) {

			var pipeClient = new PipeClient("demo", PipeDirection.InOut);
			pipeClient.MessageReceived += (s, o) => Console.WriteLine("Client Received: value: {0}", o.Message);
			pipeClient.Connect();
			bool status = true;
			while (status == true) {
				string s = GetString().First();
				if ("quit".Equals(s, StringComparison.Ordinal))
					status = false;
				pipeClient.Send(s);
			} 
			
			// pipeClient.DisConnect();
		}

		private static IEnumerable<string> GetString()
		{
			string line;
			while (null != (line = Console.ReadLine())) {
				yield return line;
			}
		}
	}
}

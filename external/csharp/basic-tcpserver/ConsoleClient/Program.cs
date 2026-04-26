using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Utils;

namespace ConsoleClient {
	class Program {

		static void Main(string[] args) {
			var program = new Program();
			program.Run();
			Console.ReadLine();
            
		}


		private TcpServer tcpServer;

		public void Run() {

			Task.Run(() => {
				var echoServiceProvider = new EchoServiceProvider();
				tcpServer = new TcpServer(echoServiceProvider, 15555);
				tcpServer.Start();        
			});
		}
	}
}


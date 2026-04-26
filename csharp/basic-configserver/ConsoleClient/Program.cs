using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Utils;

namespace ConsoleClient
{
    class Program
    {
		private static SimpleHTTPServer pageServer;
		private static int port = 0;

		static void Main(string[] args)
        {
            Program p = new Program();
            p.Run();
            Console.ReadLine();
            
        }



        public void Run()
        {
            //build the container

            Task.Run(() =>
                {

   			String documentRoot = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", "");
			
			Console.Error.WriteLine(String.Format("Using document root path: {0}", documentRoot));
			pageServer = new SimpleHTTPServer(documentRoot);
			// NOTE: the constructor calls 
			// pageServer.Initialize() and pageServer.Listen();
			port = pageServer.Port;
                     });
        }


        
    }
}


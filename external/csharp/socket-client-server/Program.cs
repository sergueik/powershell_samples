/*
 * Created by SharpDevelop.
 * User: sollw2
 * Date: 10/18/2007
 * Time: 1:54 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net.Sockets;
using System.Text;

namespace NotifyClient
{
	class Program
	{
		public static void Main(string[] args)
		{
			
			// TODO: Implement Functionality Here
			if (args.Length != 6)
			{
				Console.Write("Usage: NotifyClient.exe host port timeout[milliseconds] icon[INFO|WARN|ERROR|NONE] title message\n\n");
				System.Environment.Exit(1);
			}
			StringBuilder myMessage = new System.Text.StringBuilder();
			myMessage.AppendFormat("{0}|{1}|{2}|{3}",args[2],args[3],args[4],args[5]);
			try
			{
				TcpClient client = new TcpClient();
				ASCIIEncoding encoder = new ASCIIEncoding();
				byte[] buffer = encoder.GetBytes(myMessage.ToString());
				client.Connect(args[0], Convert.ToInt16(args[1]));
				NetworkStream stream = client.GetStream();
				stream.Write(buffer, 0, buffer.Length);
				stream.Close();
				client.Close();
				Console.WriteLine("Message was sent to host " + args[0] + " on port " + args[1]);
			}
			catch(System.Exception e)
			{
				Console.WriteLine("Message was not sent");
			}
		}
	}
}

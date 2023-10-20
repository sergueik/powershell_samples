﻿using System;
using System.Text;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Threading;

namespace Zabbix
{
	public class Sender
	{
		private int port;
		private string server;
		private readonly string ZBX_HDR = "ZBXD\x01";
		private readonly int ZBX_HDR_SIZE = 13;
		
		public Sender(string server, int port = 10051)
		{
			this.server = server;
			this.port = port;
		}
		
		public SenderResponse Send(Message message, int timeout = 500)
		{
			return SendMessage(message, timeout);
		}

		public SenderResponse Send(string host, string key, string value, int timeout = 500)
		{
			return SendMessage(
				new Message(
					new SendValue{ Host = host, Key = key, Value = value }
					)
				);
		}

		public SenderResponse Send(string key, string value)
		{
			var host = System.Net.Dns.GetHostName();
			return Send(host, key, value);
		}

		private SenderResponse SendMessage(Message message, int timeout = 500)
		{
			var json = JsonConvert.SerializeObject(message);
			var data = Encoding.UTF8.GetBytes(json);

			byte[] header = Encoding.ASCII.GetBytes(ZBX_HDR);
			byte[] length = BitConverter.GetBytes((long)data.Length);
			
			byte[] all = new byte[header.Length + length.Length + data.Length];
			System.Buffer.BlockCopy(header, 0, all, 0, header.Length);
			System.Buffer.BlockCopy(length, 0, all, header.Length, length.Length);
			System.Buffer.BlockCopy(data, 0, all, header.Length + length.Length,
				data.Length);

			using (var tcp_client = new TcpClient(server, port))
			{
				using (var network_stream = tcp_client.GetStream())
				{
					network_stream.Write(data, 0, data.Length);
					network_stream.Flush();
					var counter = 0;
					while (!network_stream.DataAvailable)
					{
						if (counter < timeout / 50)
						{
							counter++;
							Thread.Sleep(50);
						}
						else
						{
							throw new TimeoutException();
						}
					}

					// Заголовок
					byte[] buffer = new byte[ZBX_HDR_SIZE];
					network_stream.Read(buffer, 0, buffer.Length);

					if( ZBX_HDR != Encoding.ASCII.GetString(buffer, 0, 5) )
					{
						throw new Exception("Invalid response");
					}

					int dataLength = BitConverter.ToInt32(buffer, 5);

					if (dataLength == 0)
					{
						throw new Exception("Invalid response");
					}

					var resbytes = new Byte[dataLength];
					network_stream.Read(resbytes, 0, resbytes.Length);
					var s = Encoding.UTF8.GetString(resbytes);
					var jsonRes = s.Substring(s.IndexOf('{'));
					//System.Console.WriteLine(jsonRes);
					
					return JsonConvert.DeserializeObject<SenderResponse>(jsonRes);
				}
			}
		}

	}
}

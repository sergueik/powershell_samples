using System;
using System.Text;
using System.Windows.Forms;
using TcpLib;

namespace TcpServerDemo
{
	/// <SUMMARY>
	/// EchoServiceProvider. Just replies messages received from the clients.
	/// </SUMMARY>
	public class EchoServiceProvider: TcpServiceProvider
	{
		private string _receivedStr;

		public override object Clone()
		{
			return new EchoServiceProvider();
		}

		public override void OnAcceptConnection(ConnectionState state)
		{
			_receivedStr = "";
			if(!state.Write(Encoding.UTF8.GetBytes("Hello World!\r\n"), 0, 14))
				state.EndConnection(); //if write fails... then close connection
		}


		public override void OnReceiveData(ConnectionState state)
		{
			byte[] buffer = new byte[1024];
			while(state.AvailableData > 0)
			{
				int readBytes = state.Read(buffer, 0, 1024);
				if(readBytes > 0)
				{
					_receivedStr += Encoding.UTF8.GetString(buffer, 0, readBytes);
					if(_receivedStr.IndexOf("<EOF>") >= 0)
					{
						state.Write(Encoding.UTF8.GetBytes(_receivedStr), 0,
							_receivedStr.Length);
						_receivedStr = "";
					}
				}
				else state.EndConnection(); //If read fails then close connection
			}
		}


		public override void OnDropConnection(ConnectionState state)
		{
			//Nothing to clean here
		}
	}
}

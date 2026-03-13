using System;
using System.Text;

namespace Utils {
	public class EchoServiceProvider: TcpServiceProvider {
		private string payload;

		public override object Clone() {
			return new EchoServiceProvider();
		}

		public override void OnAcceptConnection(ConnectionState state) {
			payload = "";
			if (!state.Write(Encoding.UTF8.GetBytes("Hello World!\r\n"), 0, 14))
				state.EndConnection(); //if write fails... then close connection
		}

		public override void OnReceiveData(ConnectionState state) {
			var buffer = new byte[1024];
			while (state.AvailableData > 0) {
				int readBytes = state.Read(buffer, 0, 1024);
				if (readBytes > 0) {
					payload += Encoding.UTF8.GetString(buffer, 0, readBytes);
					if (payload.IndexOf("<EOF>") >= 0) {
						state.Write(Encoding.UTF8.GetBytes(payload), 0,
							payload.Length);
						payload = "";
					}
				} else
					state.EndConnection(); //If read fails then close connection
			}
		}

		public override void OnDropConnection(ConnectionState state) {
			//Nothing to clean here
		}
	}
}

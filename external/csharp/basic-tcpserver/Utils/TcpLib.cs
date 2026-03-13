using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections;

namespace Utils {

	public class ConnectionState {
		internal Socket socket;
		internal TcpServer tcpServer;
		internal TcpServiceProvider tcpServiceProvider;
		internal byte[] buffer;

		public EndPoint RemoteEndPoint {
			get{ return socket.RemoteEndPoint; }
		}

		public int AvailableData {
			get{ return socket.Available; }
		}

		public bool Connected {
			get{ return socket.Connected; }
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			try {
				return (socket.Available > 0) ? socket.Receive(buffer, offset, count, SocketFlags.None) :0;
			} catch {
				return 0;
			}
		}

		public bool Write(byte[] buffer, int offset, int count)
		{
			try {
				socket.Send(buffer, offset, count, SocketFlags.None);
				return true;
			} catch {
				return false;
			}
		}


		public void EndConnection()
		{
			if (socket != null && socket.Connected) {
				socket.Shutdown(SocketShutdown.Both);
				socket.Close();
			}
			tcpServer.DropConnection(this);
		}
	}



	public abstract class TcpServiceProvider:ICloneable {
		public virtual object Clone() {
			throw new Exception("Derived clases must override Clone method.");
		}

		public abstract void OnAcceptConnection(ConnectionState state);

		public abstract void OnReceiveData(ConnectionState state);

		public abstract void OnDropConnection(ConnectionState state);
	}



	public class TcpServer {
		private int port;
		private Socket socket;
		private TcpServiceProvider tcpServiceProvider;
		private ArrayList connections;
		private int maxConnections = 100;

		private AsyncCallback ConnectionReady;
		private WaitCallback AcceptConnection;
		private AsyncCallback ReceivedDataReady;

		public TcpServer(TcpServiceProvider provider, int port) {
			tcpServiceProvider = provider;
			this.port = port;
			this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
				ProtocolType.Tcp);
			connections = new ArrayList();
			ConnectionReady = new AsyncCallback(ConnectionReady_Handler);
			AcceptConnection = new WaitCallback(AcceptConnection_Handler);
			ReceivedDataReady = new AsyncCallback(ReceivedDataReady_Handler);
		}


		public bool Start() {
			try {
				socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), port));
				socket.Listen(100);
				socket.BeginAccept(ConnectionReady, null);
				return true;
			} catch {
				return false;
			}
		}


		private void ConnectionReady_Handler(IAsyncResult ar) {
			lock (this) {
				if (socket == null)
					return;
				Socket conn = socket.EndAccept(ar);
				if (connections.Count >= maxConnections) {
					//Max number of connections reached.
					string msg = "SE001: Server busy";
					conn.Send(Encoding.UTF8.GetBytes(msg), 0, msg.Length, SocketFlags.None);
					conn.Shutdown(SocketShutdown.Both);
					conn.Close();
				} else {
					//Start servicing a new connection
					var connectionState = new ConnectionState();
					connectionState.socket = conn;
					connectionState.tcpServer = this;
					connectionState.tcpServiceProvider = (TcpServiceProvider)tcpServiceProvider.Clone();
					connectionState.buffer = new byte[4];
					connections.Add(connectionState);
					//Queue the rest of the job to be executed later
					ThreadPool.QueueUserWorkItem(AcceptConnection, connectionState);
				}
				//Resume the listening callback loop
				socket.BeginAccept(ConnectionReady, null);
			}
		}


		private void AcceptConnection_Handler(object state){
			var connectionState = state as ConnectionState;
			try {
				connectionState.tcpServiceProvider.OnAcceptConnection(connectionState);
			} catch {
				//report error in provider... Probably to the EventLog
			}
			//Starts the ReceiveData callback loop
			if (connectionState.socket.Connected)
				connectionState.socket.BeginReceive(connectionState.buffer, 0, 0, SocketFlags.None,
					ReceivedDataReady, connectionState);
		}


		private void ReceivedDataReady_Handler(IAsyncResult ar) {
			var connectionState = ar.AsyncState as ConnectionState;
			connectionState.socket.EndReceive(ar);
			//Im considering the following condition as a signal that the
			//remote host droped the connection.
			if (connectionState.socket.Available == 0)
				DropConnection(connectionState);
			else {
				try {
					connectionState.tcpServiceProvider.OnReceiveData(connectionState);
				} catch {
					//report error in the provider
				}
				//Resume ReceivedData callback loop
				if (connectionState.socket.Connected)
					connectionState.socket.BeginReceive(connectionState.buffer, 0, 0, SocketFlags.None,
						ReceivedDataReady, connectionState);
			}
		}


		public void Stop() {
			lock (this) {
				socket.Close();
				socket = null;
				//Close all active connections
				foreach (object obj in connections) {
					var connectionState = obj as ConnectionState;
					try {
						connectionState.tcpServiceProvider.OnDropConnection(connectionState);
					} catch {
						//some error in the provider
					}
					connectionState.socket.Shutdown(SocketShutdown.Both);
					connectionState.socket.Close();
				}
				connections.Clear();
			}
		}


		internal void DropConnection(ConnectionState connectionState)
		{
			lock (this) {
				connectionState.socket.Shutdown(SocketShutdown.Both);
				connectionState.socket.Close();
				if (connections.Contains(connectionState))
					connections.Remove(connectionState);
			}
		}


		public int MaxConnections {
			get {
				return maxConnections;
			}
			set {
				maxConnections = value;
			}
		}


		public int CurrentConnections {
			get {
				lock (this) {
					return connections.Count;
				}
			}
		}
	}
}

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections;

namespace Utils {

	public class ConnectionState {
		internal Socket _conn;
		internal TcpServer _server;
		internal TcpServiceProvider _provider;
		internal byte[] _buffer;

		public EndPoint RemoteEndPoint {
			get{ return _conn.RemoteEndPoint; }
		}

		public int AvailableData {
			get{ return _conn.Available; }
		}

		public bool Connected {
			get{ return _conn.Connected; }
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			try {
				if (_conn.Available > 0)
					return _conn.Receive(buffer, offset, count, SocketFlags.None);
				else
					return 0;
			} catch {
				return 0;
			}
		}

		public bool Write(byte[] buffer, int offset, int count)
		{
			try {
				_conn.Send(buffer, offset, count, SocketFlags.None);
				return true;
			} catch {
				return false;
			}
		}


		public void EndConnection()
		{
			if (_conn != null && _conn.Connected) {
				_conn.Shutdown(SocketShutdown.Both);
				_conn.Close();
			}
			_server.DropConnection(this);
		}
	}



	public abstract class TcpServiceProvider:ICloneable
	{
		public virtual object Clone()
		{
			throw new Exception("Derived clases must override Clone method.");
		}

		public abstract void OnAcceptConnection(ConnectionState state);

		public abstract void OnReceiveData(ConnectionState state);

		public abstract void OnDropConnection(ConnectionState state);
	}



	public class TcpServer {
		private int _port;
		private Socket _listener;
		private TcpServiceProvider _provider;
		private ArrayList _connections;
		private int _maxConnections = 100;

		private AsyncCallback ConnectionReady;
		private WaitCallback AcceptConnection;
		private AsyncCallback ReceivedDataReady;

		public TcpServer(TcpServiceProvider provider, int port) {
			_provider = provider;
			_port = port;
			_listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
				ProtocolType.Tcp);
			_connections = new ArrayList();
			ConnectionReady = new AsyncCallback(ConnectionReady_Handler);
			AcceptConnection = new WaitCallback(AcceptConnection_Handler);
			ReceivedDataReady = new AsyncCallback(ReceivedDataReady_Handler);
		}


		public bool Start() {
			try {
				_listener.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), _port));
				_listener.Listen(100);
				_listener.BeginAccept(ConnectionReady, null);
				return true;
			} catch {
				return false;
			}
		}


		private void ConnectionReady_Handler(IAsyncResult ar) {
			lock (this) {
				if (_listener == null)
					return;
				Socket conn = _listener.EndAccept(ar);
				if (_connections.Count >= _maxConnections) {
					//Max number of connections reached.
					string msg = "SE001: Server busy";
					conn.Send(Encoding.UTF8.GetBytes(msg), 0, msg.Length, SocketFlags.None);
					conn.Shutdown(SocketShutdown.Both);
					conn.Close();
				} else {
					//Start servicing a new connection
					var connectionState = new ConnectionState();
					connectionState._conn = conn;
					connectionState._server = this;
					connectionState._provider = (TcpServiceProvider)_provider.Clone();
					connectionState._buffer = new byte[4];
					_connections.Add(connectionState);
					//Queue the rest of the job to be executed later
					ThreadPool.QueueUserWorkItem(AcceptConnection, connectionState);
				}
				//Resume the listening callback loop
				_listener.BeginAccept(ConnectionReady, null);
			}
		}


		private void AcceptConnection_Handler(object state){
			var connectionState = state as ConnectionState;
			try {
				connectionState._provider.OnAcceptConnection(connectionState);
			} catch {
				//report error in provider... Probably to the EventLog
			}
			//Starts the ReceiveData callback loop
			if (connectionState._conn.Connected)
				connectionState._conn.BeginReceive(connectionState._buffer, 0, 0, SocketFlags.None,
					ReceivedDataReady, connectionState);
		}


		private void ReceivedDataReady_Handler(IAsyncResult ar) {
			var connectionState = ar.AsyncState as ConnectionState;
			connectionState._conn.EndReceive(ar);
			//Im considering the following condition as a signal that the
			//remote host droped the connection.
			if (connectionState._conn.Available == 0)
				DropConnection(connectionState);
			else {
				try {
					connectionState._provider.OnReceiveData(connectionState);
				} catch {
					//report error in the provider
				}
				//Resume ReceivedData callback loop
				if (connectionState._conn.Connected)
					connectionState._conn.BeginReceive(connectionState._buffer, 0, 0, SocketFlags.None,
						ReceivedDataReady, connectionState);
			}
		}


		public void Stop() {
			lock (this) {
				_listener.Close();
				_listener = null;
				//Close all active connections
				foreach (object obj in _connections) {
					var connectionState = obj as ConnectionState;
					try {
						connectionState._provider.OnDropConnection(connectionState);
					} catch {
						//some error in the provider
					}
					connectionState._conn.Shutdown(SocketShutdown.Both);
					connectionState._conn.Close();
				}
				_connections.Clear();
			}
		}


		internal void DropConnection(ConnectionState connectionState)
		{
			lock (this) {
				connectionState._conn.Shutdown(SocketShutdown.Both);
				connectionState._conn.Close();
				if (_connections.Contains(connectionState))
					_connections.Remove(connectionState);
			}
		}


		public int MaxConnections {
			get {
				return _maxConnections;
			}
			set {
				_maxConnections = value;
			}
		}


		public int CurrentConnections {
			get {
				lock (this) {
					return _connections.Count;
				}
			}
		}
	}
}

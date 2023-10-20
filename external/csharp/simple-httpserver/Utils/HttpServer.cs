using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MiniHttpd
{
	public class HttpServer : MarshalByRefObject, IDisposable
	{

		#region Constructors

		public HttpServer()
			: this(80)
		{
		}

		public HttpServer(int port)
			: this(IPAddress.Any, port)
		{
		}

		public HttpServer(IPAddress localAddress, int port)
		{
			this.port = port;
			this.localAddress = localAddress;

			ServerUri = new Uri("http://" +
			Dns.GetHostName() +
			(port != 80 ? ":" + port.ToString(System.Globalization.CultureInfo.InvariantCulture) : "")
			);
			
			System.Reflection.AssemblyName name = System.Reflection.Assembly.GetExecutingAssembly().GetName();
			this.serverName = name.Name + "/" + name.Version.ToString();

			idleTimer = new Timer(new TimerCallback(TimerCallback), null, 0, 1000);

			try {
				this.authenticator = new BasicAuthenticator();
			} catch (NotImplementedException) {
				//TODO: make an even simpler authenticator for .net implementations without md5
			} catch (MemberAccessException) {
			}
		}

		~HttpServer()
		{
			Dispose();
		}

		#endregion

		#region IDisposable Members

		public event EventHandler Disposed;

		bool isDisposed;

		public virtual void Dispose()
		{
			Stop();

			if (isDisposed)
				return;
			isDisposed = true;

			idleTimer.Dispose();

			if (Disposed != null)
				Disposed(this, null);
		}

		#endregion

		#region Server Settings

		int port;
		public int Port {
			get {
				return port;
			}
			set {
				if (isRunning)
					throw new InvalidOperationException("Port cannot be changed while the server is running.");
				port = value;

				UriBuilder uri = new UriBuilder(ServerUri);
				uri.Port = port;
				ServerUri = uri.Uri;
			}
		}

		public string HostName {
			get {
				return ServerUri.Host;
			}
			set {
				UriBuilder uri = new UriBuilder(ServerUri);
				uri.Host = value;
				ServerUri = uri.Uri;
			}
		}

		IPAddress localAddress;
		public IPAddress LocalAddress {
			get {
				return localAddress;
			}
			set {
				if (isRunning)
					return;
				localAddress = value;
			}
		}

		public static string HttpVersion {
			get {
				return "1.1";
			}
		}

		string serverName;
		public string ServerName {
			get {
				return serverName;
			}
			set {
				serverName = value;
			}
		}

		Thread listenerThread;
		public Thread ListenerThread {
			get {
				return listenerThread;
			}
		}

		public event EventHandler ServerUriChanged;
		Uri serverUri;

		public Uri ServerUri {
			get {
				return serverUri;
			}
			set {
				serverUri = value;
				this.relUriCache.Clear();
				if (ServerUriChanged != null)
					ServerUriChanged(this, null);
			}
		}

		double timeout = 100000;
		public double Timeout {
			get {
				return timeout;
			}
			set {
				timeout = value;
			}
		}

		int uriCacheMax = 1000;
		public int UriCacheMax {
			get {
				return uriCacheMax;
			}
			set {
				uriCacheMax = value;
				if (absUriCache.Count > value)
					absUriCache.Clear();
				if (relUriCache.Count > value)
					relUriCache.Clear();
				if (uriHostsCount > value) {
					uriHostsCount = 0;
					uriHosts.Clear();
				}
			}
		}

		bool logRequests;
		public bool LogRequests {
			get {
				return logRequests;
			}
			set {
				logRequests = value;
			}
		}

		bool logConnections;
		public bool LogConnections {
			get {
				return logConnections;
			}
			set {
				logConnections = value;
			}
		}

		bool requireAuthentication;

		public bool RequireAuthentication {
			get {
				return requireAuthentication;
			}
			set {
				requireAuthentication = value;
			}
		}

		string authenticateRealm;
		public string AuthenticateRealm {
			get {
				return authenticateRealm;
			}
			set {
				authenticateRealm = value;
			}
		}

		long maxPostLength = 4 * 1024 * 1024;
		public long MaxPostLength {
			get {
				return maxPostLength;
			}
			set {
				maxPostLength = value;
			}
		}

		#endregion

		#region Caches

		Hashtable absUriCache = new Hashtable();
		Hashtable relUriCache = new Hashtable();

		internal Uri GetAbsUri(string uri)
		{
			Uri ret;
			lock (absUriCache) {
				ret = absUriCache[uri] as Uri;
				if (ret == null) {
					if (absUriCache.Count > uriCacheMax)
						absUriCache.Clear();
					ret = new Uri(uri, true);
					absUriCache[uri] = ret;
				}
			}
			return ret;
//			return new Uri(uri);
		}

		internal Uri GetRelUri(string uri)
		{
			Uri ret;
			lock (relUriCache) {
				ret = relUriCache[uri] as Uri;
				if (ret == null) {
					if (relUriCache.Count > uriCacheMax)
						relUriCache.Clear();
					ret = new Uri(serverUri, uri, true);
					relUriCache[uri] = ret;
				}
			}
			return ret;
//			return new Uri(serverUri, uri);
		}

		Hashtable uriHosts = new Hashtable();
		int uriHostsCount;

		internal Uri GetHostUri(string host, string uri)
		{
			Uri ret;

			lock (uriHosts) {
				Hashtable uris = uriHosts[host] as Hashtable;
				if (uris == null) {
					uris = new Hashtable();
					uriHosts.Add(host, uris);
				}
				ret = uris[uri] as Uri;
				if (ret == null) {
					if (uriHostsCount > uriCacheMax) {
						uriHosts.Clear();
						uriHostsCount = 0;
					}
					//BUG: UriBuilder .ctor needs bool dontEscape parameter
//					UriBuilder ub = new UriBuilder(uri);
//					string[] hostSplit = host.Split(':');
//					if(hostSplit.Length == 1)
//					{
//						ub.Host = hostSplit[0];
//					}
//					else if(hostSplit.Length == 2)
//					{
//						ub.Host = hostSplit[0];
//						ub.Port = int.Parse(hostSplit[1], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
//					}
//					else
//						throw new FormatException();
//					ret = ub.Uri;
					ret = new Uri(new Uri("http://" + host), uri, true);
					uris[uri] = ret;
				}
			}

			return ret;
		}

		#endregion

		#region Listener

		TcpListener listener;

		bool isRunning;
		bool stop;

		public bool IsRunning {
			get {
				return isRunning;
			}
		}

		public event EventHandler Started;

		public event EventHandler Stopping;

		public event EventHandler Stopped;

		public void Start()
		{
			if (isRunning)
				return;

			Log.WriteLine("Server: " + ServerName);
			Log.WriteLine("CLR: " + Environment.Version);

			listenerThread = new Thread(new ThreadStart(DoListen));
			
			listener = new TcpListener(localAddress, port);
			listener.Start();

			Port = ((IPEndPoint)listener.LocalEndpoint).Port;

			isRunning = true;

			if (Started != null)
				Started(this, null);

			Log.WriteLine("Server running at " + ServerUri);

			listenerThread.Start();
		}

		public void Stop()
		{
			if (!isRunning)
				return;
			Log.WriteLine("Server stopping");
			stop = true;
			if (listener != null)
				listener.Stop();

			if (Stopping != null)
				Stopping(this, null);
			
			try {
				JoinListener();
			} catch (MemberAccessException) {
			} catch (NotImplementedException) {
			}

			Log.WriteLine("Server stopped");

			if (Stopped != null)
				Stopped(this, null);
		}

		void JoinListener()
		{
			listenerThread.Join();
		}

		void DoListen()
		{
			try {

				while (!stop) {
					HttpClient client;
					try {
						client = new HttpClient(listener.AcceptSocket(), this);
					} catch (IOException) {
						continue;
					} catch (SocketException) {
						continue;
					}
					client.Disconnected += new EventHandler(client_Disconnected);
					if (ClientConnected != null)
						ClientConnected(this, new ClientEventArgs(client));
					if (logConnections)
						Log.WriteLine("Connected: " + client.RemoteAddress);
				}
			}
#if !DEBUG
			catch (SocketException e) {
				Log.WriteLine("Error: " + e.ToString());
			}
#endif
			finally {
				stop = false;
				listener.Stop();
				listener = null;
				isRunning = false;
			}
		}

		#endregion

		#region Client Events

		Timer idleTimer;

		internal event EventHandler OneHertzTick;

		void TimerCallback(object state)
		{
			if (OneHertzTick != null)
				OneHertzTick(this, null);
			
		}

		public delegate void ClientEventHandler(object sender, ClientEventArgs e);

		public event ClientEventHandler ClientConnected;
		public event ClientEventHandler ClientDisconnected;

		private void client_Disconnected(object sender, EventArgs e)
		{
			HttpClient client = sender as HttpClient;
			if (logConnections)
				Log.WriteLine("Disconnected: " + client.RemoteAddress);
			if (ClientDisconnected != null)
				ClientDisconnected(this, new ClientEventArgs(client));
		}

		public delegate void RequestEventHandler(object sender, RequestEventArgs e);

		public event RequestEventHandler RequestReceived;
		public event RequestEventHandler ValidRequestReceived;
		public event RequestEventHandler InvalidRequestReceived;

		IAuthenticator authenticator;

		public IAuthenticator Authenticator {
			get {
				return authenticator;
			}
			set {
				authenticator = value;
			}
		}

		internal void OnRequestReceived(HttpClient client, HttpRequest request)
		{
			try {
				RequestEventArgs args = new RequestEventArgs(client, request);

				if (RequestReceived != null)
					RequestReceived(this, args);
				if (request.IsValidRequest) {
					if (logRequests)
						//BUG: Uri.ToString() decodes a url encoded string for a second time; % disappears
						Log.WriteLine("Request: " + client.RemoteAddress + " " + request.Uri);
					if (ValidRequestReceived != null)
						ValidRequestReceived(this, args);
				} else {
					if (InvalidRequestReceived != null)
						InvalidRequestReceived(this, args);
				}
			} finally {
			}
		}

		#endregion

		#region Logging

		TextWriter log = InitializeLog();

		static TextWriter InitializeLog()
		{
			TextWriter log;
			// Initialize the log to output to the console if it is available on the platform, otherwise initialize to null stream writer.
			try {
				log = GetConsoleLog();
			} catch (MemberAccessException) {
				log = TextWriter.Null;
			} catch (NotImplementedException) {
				log = TextWriter.Null;
			}

			return log;
		}

		static TextWriter GetConsoleLog()
		{
			return Console.Out;
		}

		public TextWriter Log {
			get {
				return log;
			}
			set {
				log = value;
			}
		}

		#endregion
	}
}

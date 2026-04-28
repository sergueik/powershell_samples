using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;

namespace MiniHttpd
{
	public class HttpRequest : MarshalByRefObject, IDisposable
	{
		internal HttpRequest(HttpClient client)
		{
			dataMode = DataMode.Text;
			state = ProcessingState.RequestLine;
			connMode = ConnectionMode.KeepAlive;
			this.client = client;
			response = new HttpResponse(this);
		}

		#region State

		internal enum DataMode
		{
			Text,
			Binary
		}

		DataMode dataMode;
		internal DataMode Mode {
			get {
				return dataMode;
			}
		}

		bool isRequestFinished;
		internal bool IsRequestFinished {
			get {
				return isRequestFinished;
			}
		}

		string statusCode = "200";
		string errorMessage;
		bool isRequestError;
		HttpClient client;

		public HttpClient Client {
			get {
				return client;
			}
		}
		public HttpServer Server {
			get {
				if (client == null)
					return null;
				return client.server;
			}
		}

		public bool IsValidRequest {
			get {
				return !isRequestError;
			}
		}

		public string StatusCode {
			get {
				return statusCode;
			}
		}

		public string ErrorMessage {
			get {
				return errorMessage;
			}
			set {
				errorMessage = value;
			}
		}

		internal void RequestError(string statusCode, string message)
		{
			connMode = ConnectionMode.Close;
			isRequestFinished = true;
			this.statusCode = statusCode;
			errorMessage = message; 
			isRequestError = true;
		}

		#endregion

		#region Processing

		static string[] httpDateTimeFormats = new string[] {
			"ddd, d MMM yyyy H:m:s GMT",
			"dddd, d-MMM-yy H:m:s GMT",
			"ddd MMM d H:mm:s yy"
		};

		static DateTime ParseHttpTime(string str)
		{
			DateTime dt;
			try {
				dt = DateTime.ParseExact(str, httpDateTimeFormats, System.Globalization.DateTimeFormatInfo.InvariantInfo,
					System.Globalization.DateTimeStyles.AllowWhiteSpaces | System.Globalization.DateTimeStyles.AdjustToUniversal);
			} catch (FormatException) {
				dt = DateTime.Parse(str, CultureInfo.InvariantCulture);
			}
			return dt;
		}

		enum ProcessingState
		{
			RequestLine = 0,
			Headers,
		}

		ProcessingState state;

		string requestUri;

		void PostProcessHeaders()
		{
			if (httpVersion == "1.1" && host == null) {
				RequestError("400", "HTTP/1.1 requests must include Host header");
				return;
			}

			if (client.server.RequireAuthentication && Server.Authenticator.Authenticate(username, password) == false) {
				this.Response.SetHeader("WWW-Authenticate", "Basic realm=\"" + client.server.AuthenticateRealm + "\"");
				RequestError("401", StatusCodes.GetDescription("401"));
				return;
			}

			try {
				// Try parsing a relative URI
				//uri = new Uri(client.server.ServerUri, requestUri);
				uri = client.server.GetRelUri(requestUri);
			} catch {
				try {
					// Try parsing an absolute URI
					//uri = new Uri(requestUri);
					uri = client.server.GetAbsUri(requestUri);
				} catch (UriFormatException) {
					RequestError("400", "Invalid URI");
					return;
				} catch (IndexOutOfRangeException) {	// System.Uri in .NET 1.1 throws this exception in certain cases
					RequestError("400", "Invalid URI");
					return;
				}
			}

			if (host != null) {
				uri = client.server.GetHostUri(host, requestUri);
			}

			// Try to determine the time difference between the client and this computer; adjust ifModifiedSince and ifUnmodifiedSince accordingly
			if (date != DateTime.MinValue) {
				if (ifModifiedSince != DateTime.MinValue)
					ifModifiedSince.Add(DateTime.UtcNow.Subtract(date));
				if (ifUnmodifiedSince != DateTime.MinValue)
					ifUnmodifiedSince.Add(DateTime.UtcNow.Subtract(date));
			}

			if (method == "POST") {
				if (contentLength == long.MinValue) {
					RequestError("411", StatusCodes.GetDescription("411"));
					return;
				}
				dataMode = DataMode.Binary;
			} else
				isRequestFinished = true;
		}

		internal void ProcessLine(string line)
		{

			switch (state) {
				case ProcessingState.RequestLine:
					{
						string[] protocol = line.Split(' ');
						if (protocol.Length != 3) {
							RequestError("400", "Invalid protocol string");
							return;
						}

						switch (protocol[0]) {
							case "GET":
							case "POST":
							case "HEAD":
								method = protocol[0];
								break;
							case "PUT":
							case "DELETE":
							case "OPTIONS":
							case "TRACE":
							default:
								RequestError("501", StatusCodes.GetDescription("501"));
								return;
						}

						if (protocol[1].Length > 2500) {
							RequestError("414", StatusCodes.GetDescription("414"));
							return;
						}
						requestUri = protocol[1];

						if (!protocol[2].StartsWith("HTTP/") || !(protocol[2].Length > "HTTP/".Length)) {
							RequestError("400", "Invalid protocol string");
							return;
						}

						httpVersion = protocol[2].Substring("HTTP/".Length);

						date = DateTime.Now;

						connMode = httpVersion == "1.0" ? ConnectionMode.Close : ConnectionMode.KeepAlive;

						state = ProcessingState.Headers;
						break;
					}
				case ProcessingState.Headers:
					{
						if (headers.Count > maxHeaderLines) {
							RequestError("400", "Maximum header line count exceeded");
							return;
						}

						if (line.Length == 0) {
							PostProcessHeaders();
							return;
						}

						int colonIndex = line.IndexOf(":");
						if (colonIndex <= 1)
							return;
						string val = line.Substring(colonIndex + 1).Trim();
						string name = line.Substring(0, colonIndex);

						try {
							headers.Add(name, val);
						} catch {
						}

						switch (name.ToLower(CultureInfo.InvariantCulture)) {
							case "host":
								host = val;
								break;
							case "authorization":
								{
									if (val.Length < 6)
										break;

									string encoded = val.Substring(6, val.Length - 6);
									byte[] byteAuth;
									try {
										byteAuth = Convert.FromBase64String(encoded);
									} catch (FormatException) {
										break;
									}

									string[] strings = Encoding.UTF8.GetString(byteAuth).Split(':');
									if (strings.Length != 2)
										break;

									username = strings[0];
									password = strings[1];

									break;
								}
							case "content-type":
								contentType = val;
								break;
							case "content-length":
								try {
									contentLength = long.Parse(val, NumberStyles.Integer, CultureInfo.InvariantCulture);
								} catch (FormatException) {
								}
								if (contentLength > client.server.MaxPostLength) {
									RequestError("413", StatusCodes.GetDescription("413"));
									return;
								} else if (contentLength < 0) {
									RequestError("400", StatusCodes.GetDescription("400"));
									return;
								}
								break;
							case "accept":
								accept = val;
								break;
							case "accept-language":
								acceptLanguage = val;
								break;
							case "user-agent":
								userAgent = val;
								break;
							case "connection":
								if (string.Compare(val, "close", true, CultureInfo.InvariantCulture) == 0)
									connMode = ConnectionMode.Close;
								else
									connMode = ConnectionMode.KeepAlive;
								break;
							case "if-modified-since":
								try {
									ifModifiedSince = ParseHttpTime(val);
								} catch (FormatException) {
								}
								break;
							case "if-unmodified-since":
								try {
									ifUnmodifiedSince = ParseHttpTime(val);
								} catch (FormatException) {
								}
								break;
							case "range":
								try {
									string[] rangeStrings = val.Split(',');
									this.ranges = new ByteRange[rangeStrings.Length];
									for (int i = 0; i < rangeStrings.Length; i++)
										ranges[i] = new ByteRange(rangeStrings[i]);
								} catch (FormatException) {
									this.ranges = null;
								}
								break;
							default:
								break;
						}
						break;
					}
			}
		}

		#endregion

		#region POST data processing

		long dataRemaining = -1;
		MemoryStream postData = new MemoryStream();
		
		internal void ProcessData(byte[] buffer, int offset, int length)
		{
			if (dataRemaining == -1) {
				dataRemaining = contentLength;

				// Trim the leading LF.
				offset++;
				length--;
			}
			if (dataRemaining == 0) {
				isRequestFinished = true;
				postData.Seek(0, SeekOrigin.Begin);
				return;
			}
			
			length = (int)(dataRemaining < length ? dataRemaining : length);
			if (postData.Length + length >= Server.MaxPostLength) {
				isRequestFinished = true;
				length = (int)(Server.MaxPostLength - postData.Length);
			}

			postData.Write(buffer, offset, length);
			dataRemaining -= length;
			if (dataRemaining <= 0) {
				isRequestFinished = true;
				postData.Seek(0, SeekOrigin.Begin);
			}
		}

		public MemoryStream PostData {
			get {
				return postData;
			}
		}

		#endregion

		#region Response

		NameValueCollection headers = new NameValueCollection(new CaseInsensitiveHashCodeProvider(CultureInfo.InvariantCulture), new CaseInsensitiveComparer(CultureInfo.InvariantCulture));

		public NameValueCollection Headers {
			get {
				return headers;
			}
		}

		internal void SendResponse()
		{
			if (response.ResponseContent == null) {
				//Default page
				MemoryStream stream = new MemoryStream(512);
				StreamWriter writer = new StreamWriter(stream);

				//string message = response.ResponseCode + " " + StatusCodes.GetDescription(response.ResponseCode);
				string message = response.ResponseCode + " " + (errorMessage != null ? errorMessage : StatusCodes.GetDescription(response.ResponseCode));
				writer.WriteLine("<html><head><title>" + message + "</title></head>");
				writer.WriteLine("<body><h2>" + message + "</h2>");
				if (errorMessage != null)
					writer.WriteLine(errorMessage);
				writer.WriteLine("<hr>" + this.client.server.ServerName);
				writer.WriteLine("</body></html>");

				writer.Flush();
				response.ContentType = ContentTypes.GetExtensionType(".html");
				response.ResponseContent = stream;
			}

			response.WriteOutput();
		}

		HttpResponse response;

		public HttpResponse Response {
			get {
				return response;
			}
		}

		#endregion

		#region Headers

		static int maxHeaderLines = 30;
		public static int MaxHeaderLines {
			get {
				return maxHeaderLines;
			}
			set {
				maxHeaderLines = value;
			}
		}

		ConnectionMode connMode;

		public ConnectionMode ConnectionMode {
			get {
				return connMode;
			}
		}

		string method;
		public string Method {
			get {
				return method;
			}
		}

		Uri uri;
		public Uri Uri {
			get {
				return uri;
			}
		}

		NameValueCollection query;
		public NameValueCollection Query {
			get {
				if (query == null)
					query = new UriQuery(this.uri);

				return query;
			}
		}

		string httpVersion = "1.1";
		public string HttpVersion {
			get {
				return httpVersion;
			}
		}

		DateTime date = DateTime.MinValue;
		public DateTime Date {
			get {
				return date;
			}
		}

		string host;
		public string Host {
			get {
				return host;
			}
		}

		string contentType;
		public string ContentType {
			get {
				return contentType;
			}
		}

		long contentLength = 0;
		public long ContentLength {
			get {
				return contentLength;
			}
		}

		string accept;
		public string Accept {
			get {
				return accept;
			}
		}

		string acceptLanguage;
		public string AcceptLanguage {
			get {
				return acceptLanguage;
			}
		}

		string userAgent;
		public string UserAgent {
			get {
				return userAgent;
			}
		}

		DateTime ifModifiedSince = DateTime.MinValue;
		public DateTime IfModifiedSince {
			get {
				return ifModifiedSince;
			}
		}

		DateTime ifUnmodifiedSince = DateTime.MinValue;
		public DateTime IfUnmodifiedSince {
			get {
				return ifUnmodifiedSince;
			}
		}

		ByteRange[] ranges;
		public ByteRange[] Ranges {
			get {
				return ranges;
			}
		}

		HttpProtocol protocol = HttpProtocol.Http;
		public HttpProtocol Protocol {
			get {
				return protocol;
			}
		}

		string username;
		public string Username {
			get {
				return username;
			}
		}

		string password;
		
		public string Password {
			get {
				return password;
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			postData.Close();
		}

		#endregion
	}

	public enum ConnectionMode
	{
		KeepAlive,
		Close
	}

	public enum HttpProtocol
	{
		Http,
		Https
	}
}
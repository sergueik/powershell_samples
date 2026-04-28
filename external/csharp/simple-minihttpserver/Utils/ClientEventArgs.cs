using System;

namespace MiniHttpd
{
	public class ClientEventArgs : EventArgs
	{
		public ClientEventArgs(HttpClient client)
		{
			this.client = client;
		}

		HttpClient client;

		public HttpClient HttpClient {
			get {
				return client;
			}
		}
	}

	public class RequestEventArgs : ClientEventArgs
	{
		internal RequestEventArgs(HttpClient client, HttpRequest request)
			: base(client)
		{
			this.request = request;
		}

		HttpRequest request;

		public HttpRequest Request {
			get {
				return request;
			}
		}

		bool isAuthenticated = true;
		public bool IsAuthenticated {
			get {
				return isAuthenticated;
			}
			set {
				isAuthenticated = value;
			}
		}
	}

	public class ResponseEventArgs : ClientEventArgs
	{
		internal ResponseEventArgs(HttpClient client, HttpResponse response)
			: this(client, response, -1)
		{
		}

		internal ResponseEventArgs(HttpClient client, HttpResponse response, long contentLength)
			: base(client)
		{
			this.response = response;
			this.contentLength = contentLength;
		}

		HttpResponse response;

		public HttpResponse Response {
			get {
				return response;
			}
		}

		long contentLength;

		public long ContentLength {
			get {
				return contentLength;
			}
		}
	}
}

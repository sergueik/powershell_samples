using System;

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading;

// based on:
// https://gist.github.com/aksakalli/9191056
// MIT License - Copyright (c) 2016 Can Güney Aksakalli
// https://aksakalli.github.io/2014/02/24/simple-http-server-with-csparp.html
// see also:
// https://github.com/unosquare/embedio
// https://github.com/bonesoul/uhttpsharp
namespace Utils {
	public class SimpleHTTPServer {

		private NameValueCollection queryString = new NameValueCollection();
		private String hash = null;
		private	string fileName = null;

		private Thread serverThread;
		private string documentRoot;
		private HttpListener listener;
		private int port;
		private int code = 0;

		public int Port {
			get { return port; }
		}

		public SimpleHTTPServer(string documentRoot, int port) {
			this.Initialize(documentRoot, port);
		}

		public SimpleHTTPServer(string documentRoot) {
			// find an unused port
			var tcpListener = new TcpListener(IPAddress.Loopback, 0);
			tcpListener.Start();
			int unusedPort = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
			tcpListener.Stop();
			this.Initialize(documentRoot, unusedPort);
		}

		public void Stop() {
			serverThread.Abort();
			listener.Stop();
			Console.Error.WriteLine("Stopped");
		}

		private void Listen() {
			listener = new HttpListener();
			listener.Prefixes.Add("http://*:" + port.ToString() + "/");
			listener.Start();
			while (true) {
				try {
					HttpListenerContext context = listener.GetContext();
					Process(context);
				}	catch(ThreadAbortException e) {
					Console.Error.WriteLine(String.Format("Exception (ignored): {0}", e.ToString()));
					return;
				} catch (Exception e) {
					Console.Error.WriteLine(String.Format("Exception: {0}", e.ToString()));
				}
			}
		}

		// origin: http://www.java2s.com/Code/CSharp/Security/GetMD5Hash.htm
		public static string getMD5Hash(byte[] inputBytes) {
			MD5 md5 = MD5.Create();

			byte[] hash = md5.ComputeHash(inputBytes);
			var stringBuilder = new StringBuilder();
			for (int i = 0; i < hash.Length; i++) {
				stringBuilder.Append(hash[i].ToString("X2"));
			}
			return stringBuilder.ToString();
		}

		private void Process(HttpListenerContext context) {

			if (String.Compare(context.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase) != 0) {
				Console.Error.WriteLine(String.Format("not implemented: {0}", context.Request.HttpMethod));
				context.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
				context.Response.OutputStream.Flush();
				context.Response.OutputStream.Close();
				return;
			}
			queryString = context.Request.QueryString;
			Console.Error.WriteLine(String.Format("Query String: {0}", queryString.AllKeys.ToArray()));
			if (queryString.AllKeys.Contains("filename")) {
				fileName = queryString["filename"];
				Console.Error.WriteLine(String.Format("filename: {0}", fileName));
			}
			if (queryString.AllKeys.Contains("code")) {
				code = int.Parse(queryString["code"]);
				Console.Error.WriteLine(String.Format("code: {0}", code));
			}
			if (queryString.AllKeys.Contains("hash")) {
				hash = queryString["hash"];
				Console.Error.WriteLine(String.Format("hash: {0}", hash));
			} else {
				hash = "";
			}
			if (fileName != null) {
				String filePath = Path.Combine(documentRoot, fileName);
				Console.Error.WriteLine(String.Format("inspect file: {0}", filePath));
				if (File.Exists(filePath)) {
					try {
						byte[] fileBytes = File.ReadAllBytes(filePath);
						String fileHash = getMD5Hash(fileBytes);
						if (string.IsNullOrEmpty(hash) || String.Compare(hash, fileHash, StringComparison.OrdinalIgnoreCase) != 0) {
							Console.Error.WriteLine(String.Format("Return {0}", filePath));
							string mime;
							context.Response.ContentType = mimeTypes.TryGetValue(Path.GetExtension(filePath), out mime) ? mime : "application/octet-stream";
							// https://stackoverflow.com/questions/32537219/error-httpwebrequest-bytes-to-be-written-to-the-stream-exceed-the-content-len
							context.Response.ContentLength64 = fileBytes.Length;
							context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
							context.Response.AddHeader("Last-Modified", System.IO.File.GetLastWriteTime(filePath).ToString("r"));
							context.Response.AddHeader("Hash", fileHash);
							Console.Error.WriteLine(String.Format("Send {0} bytes", fileBytes.Length));

							context.Response.OutputStream.Write(fileBytes, 0, fileBytes.Length);
							context.Response.OutputStream.Flush();

							context.Response.StatusCode = (int)HttpStatusCode.OK;
						} else {
							Console.Error.WriteLine(String.Format("Unmodified: {0}", fileName));
							context.Response.StatusCode = (int)HttpStatusCode.NotModified;
							context.Response.OutputStream.Flush();
						}
					} catch (Exception e) {
						Console.Error.WriteLine(String.Format("Exception: {0}", e.ToString()));
						context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
					}
				} else {
					Console.Error.WriteLine(String.Format("Processing hash error: {0}", hash));
					context.Response.StatusCode = (int)HttpStatusCode.NotFound;
				}
			}
			if (code != 0) {
				// https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistenerresponse.statuscode?view=netframework-4.5
				// https://learn.microsoft.com/en-us/dotnet/api/system.net.httpstatuscode?view=netframework-4.5
				context.Response.StatusCode = (int)((HttpStatusCode)code);
			}
			context.Response.OutputStream.Close();
		}

		private void Initialize(string documentRoot, int port) {
			this.documentRoot = documentRoot;
			this.port = port;
			serverThread = new Thread(this.Listen);
			serverThread.Start();
		}

		private static IDictionary<string, string> mimeTypes = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {
        #region extension to MIME type list
			{ ".htm", "text/html" },
			{ ".html", "text/html" },
			{ ".js", "application/x-javascript" },
			{ ".json", "application/json" },
			{ ".txt", "text/plain" },
         #endregion
		};
	}

}

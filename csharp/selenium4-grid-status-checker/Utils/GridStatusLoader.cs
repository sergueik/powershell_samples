using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;

using Utils;
using Utils.Support;


namespace Utils {
	public class GridStatusLoader {
		private static readonly ClientConfiguration clientConfiguration = GenericProxies.defaultConfiguration;
		private static Regex regex;
		private static MatchCollection matches;
		
		public GridStatusLoader()
		{
		}

		public static T GetMock<T>(FileStream fileStream) {
			// TODO: catch exception when path is invalid, e.g. when passing appPath instead of filePath 
			// System.UnauthorizedAccessException: Access to the path is denied.
			//   at System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)
			var filePath = fileStream.Name;
			fileStream.Dispose();
			string payload = File.ReadAllText(filePath);
			return GetMock<T>(payload);
		}

		public static T GetMock<T>(string payload) {
			T result = clientConfiguration.InBoundSerializerAdapter.Deserialize<T>(payload);
			return result;
		}

		public static T SynchronousGetServiceUrl<T>(String serviceUrl) {
			// Make a synchronous GET Rest service call
			HttpWebRequest httpWebRequest = GenericProxies.CreateRequest(serviceUrl, GenericProxies.defaultConfiguration);
			httpWebRequest.Accept = "application/json";
			httpWebRequest.Method = "GET";
			T result = GenericProxies.ReceiveData<T>(httpWebRequest, GenericProxies.defaultConfiguration);
			return result;
		}


		public string Selenium4Detected(Uri uri) {
			string payload = null;
			try {
				// Make a synchronous GET Rest service call
				HttpWebRequest httpWebRequest = GenericProxies.CreateRequest(uri.ToString(), GenericProxies.defaultConfiguration);
				httpWebRequest.Accept = "application/json";
				httpWebRequest.Method = "GET";
				payload = GenericProxies.ReceiveData(httpWebRequest, GenericProxies.defaultConfiguration);
			} catch (MissingMethodException) {
				// System.MissingMethodException: No parameterless constructor defined for type of 'System.String'
			}
			return (payload != null) ? Selenium4Detected(payload) : null; 
		}

		public string Selenium4Detected(FileStream fileStream) {
			var filePath = fileStream.Name;
			fileStream.Dispose();
			string payload = File.ReadAllText(filePath);
			return  Selenium4Detected(payload);
		}
		
		public string Selenium4Detected(String payload) {
			string version = "";

			// try to read Selenium 3.x Grid Status from payload JSON
			try {
				var root3 = GetMock<Grid3>(payload);
				var value = root3.value;
				
				var build = value.build;
				if (build != null)
					version = build.version;
			} catch (FormatException e) {
				Console.Error.WriteLine("Exception: " + e.ToString());
				// ignore, let fall through
			} catch (ArgumentException e) {
				// System.ArgumentException: Invalid JSON primitive: .
				// when the hub status page is not JSON
				Console.Error.WriteLine("Exception: " + e.ToString());
				// ignore, let fall through
			}
			//  NOTE: Error CS0136: A local variable named 'root' cannot be declared in this scope because it would give a different meaning to 'root', which is already used in a 'parent or current' scope to denote something else
			
			// try to read Selenium 4.x Grid Status from payload JSON
			try {
				var root4 = GetMock<Grid4>(payload);
				var value = root4.value;
				var nodes = value.nodes;
				if (nodes != null)
					// only the node version is shown, assume 4.0.0 
					version = "4.0.0"; 
			} catch (FormatException e) {
				Console.Error.WriteLine("Exception: " + e.ToString());
				// ignore, let fall through
			} catch (ArgumentException e) {
				// System.ArgumentException: Invalid JSON primitive: .
				// when the hub status page is not JSON
				Console.Error.WriteLine("Exception: " + e.ToString());
				// ignore, let fall through
			}
			return version;
		}

		// based on: https://github.com/sergueik/powershell_selenium/blob/master/csharp/protractor-net/Extensions/Extensions.cs
		private static string FindMatch(string text, string matchPattern, string matchTag) {
			string result = null;
			regex = new Regex(matchPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
			matches = regex.Matches(text);
			foreach (Match match in matches) {
				if (match.Length != 0) {
					foreach (Capture capture in match.Groups[matchTag].Captures) {
						if (result == null) {
							result = capture.ToString();
						}
					}
				}
			}
			return result;
		}

		public List<String> processDocument(Grid4 root) {
			List<Node> nodes = root.value.nodes;
			var nodeNames = new List<string>();
			foreach (var node in nodes) {
				if (node.availability != null && node.availability.CompareTo("UP") == 0) {
					String text = node.uri;
					if (text != null) {
						var hostname = FindMatch(text, @"^http://(?<hostname>[A-Z0-9-._]+):\d+$", "hostname");
						nodeNames.Add((hostname == null) ? text : hostname);
					}
				}
			}
			return nodeNames;
		}
	}
}

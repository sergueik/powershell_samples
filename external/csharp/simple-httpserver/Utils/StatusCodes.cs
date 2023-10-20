using System.Collections;

namespace MiniHttpd {
	public class StatusCodes {
		private StatusCodes() { }
		static Hashtable descriptions = InitDescriptions();

		static Hashtable InitDescriptions() {
			var d = new Hashtable();
			d.Add("100", "Continue");
			d.Add("101", "Switching Protocols");
			d.Add("200", "OK");
			d.Add("201", "Created");
			d.Add("202", "Accepted");
			d.Add("203", "Non-Authoritative Information");
			d.Add("204", "No Content");
			d.Add("205", "Reset Content");
			d.Add("206", "Partial Content");
			d.Add("300", "Multiple Choices");
			d.Add("301", "Moved Permanently");
			d.Add("302", "Found");
			d.Add("303", "See Other");
			d.Add("304", "Not Modified");
			d.Add("305", "Use Proxy");
			d.Add("307", "Temporary Redirect");
			d.Add("400", "Bad Request");
			d.Add("401", "Unauthorized");
			d.Add("402", "Payment Required");
			d.Add("403", "Forbidden");
			d.Add("404", "Not Found");
			d.Add("405", "Method Not Allowed");
			d.Add("406", "Not Acceptable");
			d.Add("407", "Proxy Authentication Required");
			d.Add("408", "Request Time-out");
			d.Add("409", "Conflict");
			d.Add("410", "Gone");
			d.Add("411", "Length Required");
			d.Add("412", "Precondition Failed");
			d.Add("413", "Request Entity Too Large");
			d.Add("414", "Request-URI Too Large");
			d.Add("415", "Unsupported Media Type");
			d.Add("416", "Requested range not satisfiable");
			d.Add("417", "Expectation Failed");
			d.Add("500", "Internal Server Error");
			d.Add("501", "Not Implemented");
			d.Add("502", "Bad Gateway");
			d.Add("503", "Service Unavailable");
			d.Add("504", "Gateway Time-out");
			d.Add("505", "HTTP Version not supported");
			return d;
		}

		static public string GetDescription(string codeNum) {
			return descriptions[codeNum] as string;
		}
	}
}


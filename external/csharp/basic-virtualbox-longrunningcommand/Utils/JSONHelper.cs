using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;

namespace Utils {
	public class JSONHelper {
		private static JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
		public static Dictionary<string, object> deserialize(string json) {
			Dictionary<string, object> data =	
				javaScriptSerializer.Deserialize<Dictionary<string, object>>(json);
			return data;
		}
		
		public static T deserialize<T>(string json) {
			return javaScriptSerializer.Deserialize<T>(json);
		}
	}
}


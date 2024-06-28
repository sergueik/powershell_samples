using System.Security.Principal;

using System;
using System.IO;
using System.Linq;
using System.Net;

namespace Utils {

	public class Common {
		
		private static int port;
		public static int Port {
			get { return port; }
			set {
				port = value;
			}
		}

		public static string CreateTempFile(string content) {
			var testFile = new FileInfo("webdriver.tmp");
			if (testFile.Exists) {
				testFile.Delete();
			}
			StreamWriter testFileWriter = testFile.CreateText();
			testFileWriter.WriteLine(content);
			testFileWriter.Close();
			return testFile.FullName;
		}

		public static void GetLocalHostPageContent(string filename) {
			//  https://stackoverflow.com/questions/4510212/how-i-can-get-web-pages-content-and-save-it-into-the-string-variable
			
			using (WebClient client = new WebClient()) {
				string downloadString = client.DownloadString(String.Format("http://127.0.0.1:{0}/{1}?filename={1}&name={2}", port, "index.html", filename));
				Console.Error.WriteLine("Data: " + downloadString);
				
				Console.Error.WriteLine("Headers: " + client.Headers.ToString());
			}
		}

	}
}

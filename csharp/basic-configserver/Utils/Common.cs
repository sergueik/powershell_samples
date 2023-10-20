using System.Security.Principal;

using System;
using System.IO;
using System.Linq;


namespace Utils {

	public class Common {
		
		private static int port;
		public static int Port {
			get { return port; }
			set {
				port = value;
			}
		}

		public static string CreateTempFile(string content){
			FileInfo testFile = new FileInfo("webdriver.tmp");
			if (testFile.Exists) {
				testFile.Delete();
			}
			StreamWriter testFileWriter = testFile.CreateText();
			testFileWriter.WriteLine(content);
			testFileWriter.Close();
			return testFile.FullName;
		}

		public static void GetLocalHostPageContent(string filename) {
			//  driver.Navigate().GoToUrl(String.Format("http://127.0.0.1:{0}/{1}{2}", port, "resources", filename));
		}

	}
}

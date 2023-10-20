using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Custom;

namespace Custom {
	public class Test {
		private static Credential data;
		private static Regex regex = new Regex("."); 
		public static void Main(string[] args) {
		
			data = CredentialsManagerHelper.ReadCredential("Demo");
			Console.WriteLine("Reading credential: " + data.UserName);
			String applicationName = "Demo"; 
			String userName = "Meziantou";
			String secret = "Passw0rd";
			Console.WriteLine(String.Format("Writing new credential entry: {0} {1} {2}" ,  applicationName, userName, secret));
			CredentialsManagerHelper.WriteCredential(applicationName, userName, secret);
			// CredentialsManagerHelper.WriteCredential("http://www.softfluent.com", "Meziantou", "Passw0rd");
			data = CredentialsManagerHelper.ReadCredential("Demo");
			Console.WriteLine("Reading credential: " + data.ToString());

			Console.WriteLine("Reading All Available credentials: ");
			foreach (var credential in CredentialsManagerHelper.EnumerateCrendentials()) {

				if (credential.UserName == null) { 
					continue;
				}
				Console.WriteLine("Type: " + credential.CredentialType);
				Console.WriteLine("ApplicationName: " + credential.ApplicationName);				
				Console.WriteLine("UserName: " + credential.UserName);				 
				Console.WriteLine("Password: " +  ((credential.Password != null) ? regex.Replace (credential.Password,"?") : ""));
			}	
		}
	}
}
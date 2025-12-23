using System;
using System.Linq;
using CredentialManagement;

namespace Utils {
	public class CredentialManagementHelper {

		private String password = null;
		private String userName = null;

		public string UserName {
			get { return userName; }
			set { userName = value; }
		}
		public string Password {
			set { password = value; }
		}

		public void SavePassword() {
			try {
				using (var cred = new Credential()) {
					cred.Password = password;
					cred.Target = userName;
					// better: cred.Target = "MyCompany.MyApp.A	piBootstrap.v1";
					cred.Type = CredentialType.Generic;
					cred.PersistanceType = PersistanceType.LocalComputer;
					// Stored in user profile
					// Roams only if the OS profile roams
					// Not shared across machines
					// To enable AD roaming (rare), Enterprise exists — but LocalComputer is correct for secrets.
					cred.Save();
				}
			} catch (Exception ex) {
				Console.Error.WriteLine("Exception (ignored) " + ex.ToString());
			}
		}

		public string GetPassword() {
			try {
				using (var cred = new Credential()) {
					cred.Target = userName;
					cred.Load();
					return cred.Password;
				}
			} catch (Exception ex) {
				Console.Error.WriteLine("Exception (ignored) " + ex.ToString());
			}
			return null;
			// TODO: distinguish
			// "credential missing" vs
			// "credential exists but empty"
		}
	}
}


using System;
using System.Text;
using System.DirectoryServices;
using System.Collections.Specialized;

namespace MiniHttpd
{
	[Serializable]
	public class LdapAuthenticator : IAuthenticator
	{
		public LdapAuthenticator(string basePath)
		{
			this.basePath = basePath;
		}

		[NonSerialized]
		NameValueCollection cache = new NameValueCollection();
		[NonSerialized]
		DateTime lastTimeout = DateTime.Now;

		string basePath;

		public string BasePath {
			get {
				return basePath;
			}
		}

		[NonSerialized]
		TimeSpan maxCacheAge = new TimeSpan(0, 1, 0, 0, 0);
		
		TimeSpan MaxCacheAge {
			get {
				return maxCacheAge;
			}
			set {
				maxCacheAge = value;
			}
		}
		
		public void ResetCache()
		{
			cache.Clear();
			lastTimeout = DateTime.Now;
		}

		public bool Authenticate(string username, string password)
		{
				
			if (lastTimeout.Add(maxCacheAge) < DateTime.Now)
				ResetCache();

			if (string.Compare(cache[username], password, true) == 0)
				return true;

			try {
				new DirectoryEntry(basePath, username, password);
				cache.Add(username, password);
			} catch {
				return false;
			}
			return true;
		}

	}
}

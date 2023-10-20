using System.Configuration;

namespace ExampleApplication {
	// https://docs.microsoft.com/en-us/dotnet/api/system.configuration.configurationsection?view=netframework-4.5
	public class UrlsSection : ConfigurationSection {

		[ConfigurationProperty("name", DefaultValue = "MyFavorites", IsRequired = true, IsKey = false)]
		[StringValidator(InvalidCharacters = " ~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
		public string Name {
			get { return (string)this["name"]; }
			set { this["name"] = value; }
		}

		// Declare a collection element represented
		// in the configuration file by the sub-section
		// <urls> <add .../> </urls>
		// Note: the "IsDefaultCollection = false"
		// instructs the .NET Framework to build a nested
		// section like <urls> ...</urls>.
       
		[ConfigurationProperty("urls", IsDefaultCollection = false)]
		// public UrlsCollection Urls => (UrlsCollection)base["urls"];		
	        public UrlsCollection Urls { 
			get {
				return (UrlsCollection)base["urls"];
			}
		}
	}
}

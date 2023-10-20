using System.Configuration;

namespace ExampleApplication {
	// https://docs.microsoft.com/en-us/dotnet/api/system.configuration.configurationelementcollection?view=netframework-4.5
    public class UrlsCollection : ConfigurationElementCollection {
		// https://docs.microsoft.com/en-us/dotnet/api/system.configuration.configurationelementcollection.createnewelement?view=netframework-4.5
        protected override ConfigurationElement CreateNewElement() {
            return new UrlConfigElement();
        }
		// https://docs.microsoft.com/en-us/dotnet/api/system.configuration.configurationelementcollection.getelementkey?view=netframework-4.5
        protected override object GetElementKey(ConfigurationElement element) {
            return ((UrlConfigElement)element).Name;
        }
    }
}

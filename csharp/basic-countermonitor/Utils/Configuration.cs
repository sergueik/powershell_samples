using System.Configuration;
using System.Xml;

namespace Utils {

	public class CustomSettingElement : System.Configuration.ConfigurationElement {
		[ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        public string Text { get; private set; }

        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey) {
            Name = reader["name"];
            Text = reader.ReadElementContentAsString();
        }
    }

    [ConfigurationCollection(typeof(CustomSettingElement), AddItemName = "add")]
	 public class CustomSettingCollection : System.Configuration.ConfigurationElementCollection {
        protected override ConfigurationElement CreateNewElement() {
            return new CustomSettingElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return ((CustomSettingElement)element).Name;
        }

    	public CustomSettingElement this[int index]{ 
    		get { return (CustomSettingElement) BaseGet(index);}
    	}
    }
    public class CustomSettingsSection : System.Configuration.ConfigurationSection {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public CustomSettingCollection Settings {
            get { return (CustomSettingCollection)this[""]; }
        }
    }
}
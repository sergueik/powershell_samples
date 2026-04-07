using System.Configuration;
using System.Xml;

namespace Utils
{
    public class CustomSettingElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        public string Text { get; private set; }

        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            Name = reader["name"];
            Text = reader.ReadElementContentAsString();
        }
    }
    
        public class CustomSettingsSection : ConfigurationSection
    {
        [ConfigurationProperty("add", IsRequired = true)]
        public CustomSettingElement Add
        {
            get { return (CustomSettingElement)this["add"]; }
        }
    }
}
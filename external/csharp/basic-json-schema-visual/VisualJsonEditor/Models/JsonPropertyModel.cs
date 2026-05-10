using System;
using System.Text;
using MyToolkit.Model;
using NJsonSchema;

namespace VisualJsonEditor.Models
{
    public class JsonPropertyModel : ObservableObject
    {
        public JsonPropertyModel(string name, JsonObjectModel parent, JsonSchemaProperty schema)
        {
            Name = name;
            Parent = parent;
            Schema = schema;

            // Parent.PropertyChanged += (sender, args) => RaisePropertyChanged(() => Value);
            Parent.PropertyChanged += (sender, args) => RaisePropertyChanged("Value");
        }

        public string Name { get; private set; }

        public JsonObjectModel Parent { get; private set; }

        public JsonSchemaProperty Schema { get; private set; }

        public bool IsRequired
        {
            get { return Schema.IsRequired; }
        }

        /// <summary>Gets the contentEncoding value. </summary>
        public string GetContentEncoding
        {
            get
            {
                object value = null;

                if (Schema.ExtensionData!= null)
                	Schema.ExtensionData.TryGetValue("contentEncoding", out value);
                return value!= null?value.ToString():"";
            }
        }

        /// <summary>Indicates if this value is base64 encoded. </summary>
        public bool IsBase64String
        {
            get
            {
                return Schema.Type == JsonObjectType.String && 
                    (GetContentEncoding == "base64"
                     || Schema.Format == JsonFormatStrings.Byte);
            }
        }

        /// <summary>Gets or sets the value of the property. </summary>
        public object Value
        {
            get
            {
                if (Parent.ContainsKey(Name))
                {
                    return IsBase64String
                        ? Base64Decode(Parent[Name].ToString())
                        : Parent[Name];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                Parent[Name] = IsBase64String 
                    ? Base64Encode(value.ToString()) 
                    : value;

                // RaisePropertyChanged(() => Value);
                // RaisePropertyChanged(() => HasValue);
                RaisePropertyChanged("Value");
                RaisePropertyChanged("HasValue");
            }
        }

        /// <summary>Gets a value indicating whether the property has a value. </summary>
        public bool HasValue
        {
            get { return Value != null; }
        }

        /// <summary>Encode a string in Base64. </summary>
        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>Decode a string in Base64. </summary>
        private static string Base64Decode(string base64EncodedData)
        {
            try
            {
                var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
                return Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch(Exception)
            {
                // Hide any decode error
                return "";
            }
        }
    }
}
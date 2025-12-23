using System.Configuration;
using System;
using System.Text;

namespace Utils
{

	// semi-extension  method - not fully masking - avoid "silent monkey‑patching"
	// string apiKey = SecureAppSettings.Get("Key");
	// with the .config file
	// <appSettings>
	//   <add key="Key" value="enc:BASE64_PAYLOAD_HERE" />
	// </appSettings>
	// alternatively use Lazy<String> with a lambda function in constructor
	// https://learn.microsoft.com/en-us/dotnet/api/system.lazy-1?view=netframework-4.5
	// private static Lazy<string> _apiKey = new Lazy<string>(() => SecureAppSettings.Get("SomeKey"));
	// public static string ApiKey => _apiKey.Value;

	public class AppSettingsHelper {

		public static string Get(string key, string passwordString) {
			string value = ConfigurationManager.AppSettings[key];
			return string.IsNullOrEmpty(value) ?
				null
				:
				value.StartsWith("enc:") ?
				Helper.Decrypt(Encoding.UTF8.GetString(Convertor.HexStringToByteArray(value.Substring(4))), passwordString)
				:
				value;
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;

using System.Threading;
using System.Reflection;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

using Utils;

namespace Test
{

	[TestFixture]
	public class ExtractInfoTest
	{
		private string result = null;
		// private NameValueCollection appSettings;
		private KeyValueConfigurationCollection appSettings;
		private string directory;
		private Regex regex = new Regex("%(?<token>[A-Z0-9_]+)%");
		private HashSet<string> resolved = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		private Dictionary<string, string> values = new Dictionary<string, string>();


		[SetUp]
		public void SetUp()
		{
			// NOTE: the "ConfigurationManager.AppSettings" is somewhat useless:
			// it is Hard wired to fallback to assembly
			// and immutable during execution
			// Solution: access directly
			directory = Path.GetDirectoryName(Path.GetDirectoryName(
				typeof(ExtractInfoTest).Assembly.Location));
			directory = AppDomain.CurrentDomain.BaseDirectory;

			directory = Path.GetFullPath(Path.Combine(directory, ".."));
			directory = Path.GetFullPath(Path.Combine(directory, ".."));
			directory = Path.GetFullPath(Path.Combine(directory, ".."));

			var configPath = Path.Combine(Path.Combine(
				                 Path.Combine(Path.Combine(directory, "Program"), "bin"), "Debug"),
				                 "VboxManageSystemTrayApp.exe.config");

			var map = new ExeConfigurationFileMap {
				ExeConfigFilename = configPath
			};

			var config = ConfigurationManager.OpenMappedExeConfiguration(
				             map,
				             ConfigurationUserLevel.None);

			appSettings = config.AppSettings.Settings;
		}

		[Test]
		public void test1() {
			var vars = new List<string> {
				@"""XPSP3"" {91047a20-5df0-4b68-b11d-1abd36738105}",
				@"""Xubuntu 22.04"" {7e261a39-d356-4eb1-a8ed-75675b149241}",
				@"""default"" {59c3df8a-e359-4211-8e7c-74ec5dd3e51d}",
				@"""Windows 7"" {55d01a4a-4656-480f-bccb-e6838f5df285}",
				@"""Windows 10 x64 ru"" {184f37d0-8529-474c-962d-6fd6781d9757}",
				@"""Xubuntu VS Code"" {0b64d785-4228-4357-83bc-2b6a436f81bf}"

			};
			// https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2?view=netframework-4.5
			var extractors = new Dictionary<string, string>() {
				{ "\\\"(?<name>[^\"]+)\\\"", "name" },
				{ "{(?<guid>[0-9a-zA-Z-]+)}", "guid" }
			};
			vars.ForEach((string input) => {
				foreach (KeyValuePair<string, string> entry in extractors) {
					Console.WriteLine(String.Format("Key: {0}, Value: {1}", entry.Key, entry.Value));
					String pattern = entry.Key;
					String groupName = entry.Value;
					result = input.FindMatch(pattern, groupName);
					Assert.IsNotNull(result);
					Console.WriteLine(String.Format("input: {0}: result: {1}", input, result));
					//StringAssert.Contains(value, result);
				}
			});
		}

		[Test]
		public void test2() {
			var variables = new List<string> { "UserName", "Debug", "Password" };

			variables.ForEach((string variable) => {

				result = null;
				var setting = appSettings[variable];

				if (setting != null) {
					result = setting.Value.ToString();
				}
//				if (appSettings.AllKeys.Contains(variable)) {
//					result = appSettings[variable].Value.ToString();
//				}
				Assert.IsNotNull(result);
				Console.WriteLine(String.Format("variable: {0}: result: {1}", variable, result));
			});
		}


		[Test]
		public void test3() {
			regex = new Regex("%(?<token>[A-Z0-9_]+)%");

			// Arrange:
			foreach (KeyValueConfigurationElement setting in appSettings) {
				values[Canonical(setting.Key)] = setting.Value;
				if (!regex.IsMatch(setting.Value ?? String.Empty))
					resolved.Add(Canonical(setting.Key));
			}

			// Seed some known vars:
			values["PROGRAMFILES"] = Environment.GetEnvironmentVariable("PROGRAMFILES");
			resolved.Add("PROGRAMFILES");
			resolved.Add("TEMP");
			values["TEMP"] = "/tmp"; // filler
			// Act:
			bool progress;

			do {
				progress = false;

				foreach (var key in values.Keys.ToList()) {
					if (resolved.Contains(key))
						continue;
					var value = values[key];
					var matches = regex.Matches(value);
					bool ready = true;

					foreach (Match m in matches) {
						var token =
							m.Groups["token"].Value;

						if (!resolved.Contains(token)) {
							ready = false;
							break;
						}
					}

					if (!ready)
						continue;

					foreach (Match m in matches) {
						var token =
							m.Groups["token"].Value;

						value = value.Replace(
							"%" + token + "%",
							values[token]);
					}

					values[Canonical(key)] = value;
					resolved.Add(Canonical(key));
					progress = true;
				}

			} while (progress);

			Console.Error.WriteLine(String.Format("values: {0}\n({1} items)\nresolved: {2}\n({3} items)", values.PrettyPrint(), values.Count, string.Join(",", resolved), resolved.Count));
			var unresolved = values.Keys.Except(resolved, StringComparer.OrdinalIgnoreCase).ToList();
			Console.WriteLine(   "Unresolved: {0}",   unresolved.Count == 0 ? "none" :String.Join(",", unresolved));
			Assert.IsTrue(resolved.Count == values.Count, "some are not resolved");
		}

		[Test]
		public void test4() {

		    var optional = new HashSet<string>( StringComparer.OrdinalIgnoreCase) {
		        "Password",
		        "VM"
		    };

		    foreach (string key in appSettings.AllKeys) {
		        if (optional.Contains(key))
		            continue;

		        string value = appSettings[key].Value;

		        Assert.IsFalse( string.IsNullOrWhiteSpace(value),
		            string.Format( @"AppSetting ""{0}"" expected to have a value.", key));
		    }
		}

		private static string Canonical(string name) {
    			return name.ToUpperInvariant();
		}
	}
}

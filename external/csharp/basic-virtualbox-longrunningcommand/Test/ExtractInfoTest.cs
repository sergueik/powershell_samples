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
using NUnit.Framework;

using Utils;
// using Program;

namespace Test
{

	[TestFixture]
	public class ExtractInfoTest
	{
		private string result = null;
		private NameValueCollection appSettings;
	
		[Test]
		public void test1()
		{
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
		public void test2()
		{
			var variables = new List<string> { "UserName", "Debug", "Password" };
			
			
			
			// NOTE: Hard wired to fallback to assembly
			// appSettings = ConfigurationManager.AppSettings;
			// Solution: access directly
			var sutDir = Path.GetDirectoryName(Path.GetDirectoryName(
				             typeof(ExtractInfoTest).Assembly.Location));
			// immutable during execution
			var configPath = Path.Combine(
				                 sutDir, "Program",
				                 "VboxManageSystemTrayApp.exe.config");

			var map = new ExeConfigurationFileMap {
				ExeConfigFilename = configPath
			};

			var config = ConfigurationManager.OpenMappedExeConfiguration(
				             map,
				             ConfigurationUserLevel.None);

			var appSettings = config.AppSettings.Settings;
			variables.ForEach((string variable) => {
				result = null;
				if (appSettings.AllKeys.Contains(variable)) {
					result = appSettings[variable].Value.ToString();
				}
				Assert.IsNotNull(result);
				Console.WriteLine(String.Format("variable: {0}: result: {1}", variable, result));
			});
		}
	
	}
}

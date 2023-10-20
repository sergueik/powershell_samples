using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using Microsoft.Win32;
using Utils;

namespace Tests {
	[TestFixture]
	class Test {

		private RegistryHelper reg;

		const String taskName = "example_task_service";			
		const string id = "{29EB5B50-CA43-4CEE-9CF2-AEBA108AFE70}";

		[TestFixtureSetUp]
		public void SetUp() {
			reg = new RegistryHelper();
		}
		[Test]
		[Ignore("Task Scheduler implemetation details -  only available in Windows 10, missing on Windows 7 ")]
		// NOTE: Task Scheduler implemetation details
		// only available in Windows 10, missing on Windows 7
		public void test3() {
			String key = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Schedule\\TaskCache\\Tasks\\" + id;

			byte[] nnValue = reg.GetBinaryValue(Registry.LocalMachine, key, "Actions");
			if (reg.strRegError == null) {
				for (int i = 0; i < nnValue.Length; i++)
					Console.Write("{0}({1:X}) ", (char)nnValue[i], nnValue[i]);
				Console.WriteLine("");
			} else {
				Console.WriteLine("Error: " + reg.strRegError);
			}
		}
		

		
		[Test]
		public void test1() {
 
			String key = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Schedule\\TaskCache\\Tasks\\" + id;

			string value = reg.GetStringValue(Registry.LocalMachine, key, "Path");
			if (reg.strRegError == null) {				
				Console.Write(value);
			} else {
				Console.WriteLine("Error: " + reg.strRegError);
			}
			Assert.IsNullOrEmpty(reg.strRegError);
			StringAssert.Contains(@"\" + taskName, value);
		}

		[Test]
		public void test2() {
			const String key = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Schedule\TaskCache\Tree\" + taskName;
			string value = reg.GetStringValue(Registry.LocalMachine, key, "Id");
			if (reg.strRegError == null) {				
				Console.Write(value);
			} else {
				Console.WriteLine("Error: " + reg.strRegError);
			}
			Assert.IsNullOrEmpty(reg.strRegError);
			StringAssert.IsMatch("[0-9A-D]+", value);
		}
	}
}


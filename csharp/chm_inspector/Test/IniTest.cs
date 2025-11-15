using System;
using System.Text;
using NUnit.Framework;
using Utils;
using System.IO;

namespace Test {

	[TestFixture]
	public class IniTest {
		private StringBuilder verificationErrors = new StringBuilder();
		private const string data = @"
[Environments]
values=Prod,Dev,Test,Placeholder 1,Placeholder 2

[Prod]
hub=127.0.0.1
nodes=host1,host2,host3,host4,host-1-23-45
name=host1

[Placeholder 1]
hub=127.0.0.1
nodes=host11,host12,host13
name=host11

[Placeholder 2]
hub=127.0.0.1
nodes=host21,host22,host23,host24
name=host22
";

		string line  = null;
		StringReader stringReader = null;
		IniFile iniFile;
		IniFileReader iniFileReader = null;
		
		[SetUp]
		public void SetUp() {
			stringReader = new StringReader(data);
			iniFileReader = new IniFileReader(new MemoryStream(Encoding.UTF8.GetBytes(data)), Encoding.UTF8);
			iniFile = IniFile.FromStream(iniFileReader);
			// NOTE: fragile with respect to encoding etc.:
			// System.ArgumentException : String passed to the ParseLine method cannot contain more than one line
		}

		[TearDown]
		public void TearDown() {
			Assert.AreEqual("", verificationErrors.ToString());
		}

		[Ignore]
		[Test] 
		// http://www.java2s.com/Tutorials/CSharp/System.IO/StringReader/C_StringReader_StringReader.htm
		public void test1() {
			line = null;
			stringReader = new StringReader(data);
			while (true) {
				line = stringReader.ReadLine();
				if (line == null) {
					break;
				}
				Console.WriteLine(line);
			}
		}

		[Ignore]
		[Test] 
		public void test2() {
			var sections = iniFile.GetSectionNames();
			// NOTE: does not handle whitespace after the comma as section separator
			foreach (var section in sections) {			
				Console.Error.WriteLine(String.Format("Section: {0}", section));
			}
			var environments = iniFile["Environments"]["values"];
			if (environments != null) {
				foreach (var environment in environments.Split(new char[] {','})) {
					Console.Error.WriteLine(String.Format("Environment: {0}", environment));
				}
			}
		}
	}
}


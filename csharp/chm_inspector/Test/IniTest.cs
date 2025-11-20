using System;
using System.Text;
using System.IO;
using System.Linq;

using NUnit.Framework;

using Utils;
using TestUtils;

namespace Tests {

	[TestFixture]
	public class IniTest {

		private const string data = @"
[Operations]
values=List,Title

[List]
grfMode=0x00000010
wait=10000

[Title]
grfMode=STGM_SHARE_EXCLUSIVE | STGM_READ

[CHM]
fileName=PowerCollections.chm
lastBrowseDir=
";

		string line = null;
		StringReader stringReader = null;
		IniFile iniFile;
		IniFileReader iniFileReader = null;

		[SetUp]
		public void SetUp() {
			stringReader = new StringReader(data);
			iniFileReader = new IniFileReader(new MemoryStream(Encoding.UTF8.GetBytes(data)), Encoding.UTF8);
			iniFile = IniFile.FromStream(iniFileReader);

		}

		// [Ignore]
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

		// [Ignore]
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
		[Test]
		public void test4() {
			string expr = iniFile.readValue("List", "grfMode", "STGM_READ | STGM_SHARE_DENY_NONE");

			// Sample: convert to enum flags (adjust to your STGM enum)
			uint flags = IniExpressionParser.ParseEnumFlags<STGM>(expr);
			Assert.IsTrue(flags.hasFlag(Utils.STGM.STGM_READ));
			Assert.IsTrue(flags.hasFlag(Utils.STGM.STGM_SHARE_EXCLUSIVE));
		}
		[Test]
		public void test5() {
			string hex = "0x10";
			uint val = IniExpressionParser.ParseEnumFlags<STGM>(hex);
			Assert.AreEqual(16, val);
		}

		[Test]
		public void test6() {
			string hex = "0xINVALID_HEX";
			Assert.Throws<FormatException>(() => IniExpressionParser.ParseEnumFlags<STGM>(hex));
		}

		[Test]
		public void test7() {
			string hex = "0x10";
			uint val = IniExpressionParser.ParseEnumFlags<STGM>(hex);
			Assert.AreEqual(16, val);
		}


		[Test]
		public void test8()
		{
			string expr = "STGM_READ | INVALID_FLAG";
			Assert.Throws<ArgumentException>(() => IniExpressionParser.ParseEnumFlags<STGM>(expr));
		}

		[Test]
		public void test9() {
			string ops = iniFile.readValue("Operations", "values", "List,Title");
			var values = ops.Split(',').Select(s => s.Trim()).ToArray();
			Assert.Contains("List", values);
			Assert.Contains("Title", values);
		}

		[Test]
		public void test10() {
			// NOTE: fragile with respect to encoding line ending etc.
			// System.ArgumentException : String passed to the ParseLine method cannot contain more than one line

			iniFile = IniFile.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini"));
			Assert.NotNull(iniFile);
			Assert.GreaterOrEqual(iniFile.GetSectionNames().Length, 1, "Eepect at least one section");
		}
	}

	[Flags]
	public enum STGM {
		STGM_READ = 0x00000000,
		STGM_WRITE = 0x00000001,
		STGM_SHARE_EXCLUSIVE = 0x00000010,
		STGM_SHARE_DENY_READ = 0x00000030,
	    STGM_SHARE_DENY_WRITE = 0x00000020,
		STGM_SHARE_DENY_NONE = 0x00000040,
	}

}

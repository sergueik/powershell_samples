using System;
using System.Linq;
using System.IO;

using NUnit.Framework;

using Utils;

namespace Test {
	[TestFixture]
	public class RenamerTest {
		private const string extension = "flac";
		private string oldNamePattern = null;
		private const string newNamePattern = "<id> - <title> - <artist>";
		private string directoryName = null;

		[Description("Rename files")]
		[Test]
		public void test1() {
			oldNamePattern = "^(?<id>[0-9]+)\\. (?<artist>[^-]+) - (?<title>.+)$";
			directoryName =
			@"C:\Users\kouzm\Desktop\Music\Cal Tjader\Cal Tjader - Both Sides Of The Coin - flac";

			// https://learn.microsoft.com/en-us/dotnet/api/system.io.directory.exists?view=netframework-4.5
			if (Directory.Exists(directoryName)) {
				var renamer = new Renamer();
				renamer.Extension = extension;
				renamer.DirectoryName = directoryName;
				renamer.NewNamePattern = newNamePattern;
				renamer.OldNamePattern = oldNamePattern;
				renamer.Rename();
			} else {
				Console.WriteLine(String.Format("skipped test - non existing directory {0}", directoryName));
				// TODO: add test GetFiles: directoryName with invalid character in the path, e.g. <tab>
				// Exception: System.ArgumentException: Illegal characters in path.
				// at System.IO.Path.LegacyNormalizePath(String path, Boolean fullCh eck, Int32 maxPathLength, Boolean expandShortPaths)
			}
		}
		[Description("Rename files 2")]
		[Test]
		public void test2() {
			oldNamePattern = "^\\((?<id>[0-9]+)\\)  *\\[(?<artist>[^]]+)\\]  *(?<title>.+)$";
			directoryName = @"C:\Users\kouzm\rrr";

			// https://learn.microsoft.com/en-us/dotnet/api/system.io.directory.exists?view=netframework-4.5
			if (Directory.Exists(directoryName)) {
				Console.WriteLine(String.Format("Process directory {0}", directoryName));
				var renamer = new Renamer();
				renamer.Extension = extension;
				renamer.DirectoryName = directoryName;
				renamer.NewNamePattern = newNamePattern;
				renamer.OldNamePattern = oldNamePattern;
				renamer.Rename();
				// NOTE: 
				// arg1: The first value, expected to be greater
				// arg2: The second value, expected to be less
				Assert.Greater(renamer.Count, 0, "expect to rename at least one file" );
			} else {
				Console.WriteLine(String.Format("skipped test - non existing directory {0}", directoryName));
			}
		}
	}
}
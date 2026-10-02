using System;
using System.Linq;
using System.IO;

using NUnit.Framework;

using Utils;

namespace Test {
	[TestFixture]
	public class RenamerTest {
		const string extension = "flac";
		const string oldNamePattern = "^(?<id>[0-9]+)\\. (?<artist>[^-]+) - (?<title>.+)$";
		const string newNamePattern = "<id> - <title> - <artist>";
		private const string directoryName =
			@"C:\Users\kouzm\Desktop\Music\Cal Tjader\Cal Tjader - Both Sides Of The Coin - flac";

		[Description("Rename files")]
		[Test]
		public void test() {
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
	}
}
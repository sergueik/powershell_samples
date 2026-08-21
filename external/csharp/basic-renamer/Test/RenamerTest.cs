using System;
using System.Linq;

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
		public void test()
		{
			var renamer = new Renamer();
			renamer.Extension = extension;
			renamer.DirectoryName = directoryName;
			renamer.NewNamePattern = newNamePattern;
			renamer.OldNamePattern = oldNamePattern;
			renamer.Rename();
		}
	}
	// TODO: add test GetFiles: C:\Users\kouzm\Desktop\Musi	c\Russian\Fantastique - Fantastique (2006) *.wav
	// Exception: System.ArgumentException: Illegal characters in path.
	// at System.IO.Path.LegacyNormalizePath(String path, Boolean fullCheck, Int32 maxPathLength, Boolean expandShortPaths)
}
using System;
using System.IO;
using System.Diagnostics;

using NUnit.Framework;

using Utils;

namespace Test {
	[TestFixture]
	public class FileMoveTest {
		private const string directoryName =
			@"C:\Users\kouzm\Desktop\Music\Russian\Fantastique - Fantastique (2006)";

		[Test]
		public void test() {
			// https://learn.microsoft.com/en-us/dotnet/api/system.io.directory.getfiles?view=netframework-4.5
			var files = Directory.GetFiles(directoryName, "*.wav");

			foreach (var file in files) {
				var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
				var extension = Path.GetExtension(file);
				var fileDirectoryName = Path.GetDirectoryName(file);

				var newFilePath = Path.Combine(directoryName, fileNameWithoutExtension + "_new" + extension);

				Console.WriteLine("filename: \"{0}\" extension: \"{1}\" directory: \"{2}\" new file path: \"{3}\"", fileNameWithoutExtension, extension, fileDirectoryName, newFilePath);

				if (!File.Exists(file)) {
					Console.WriteLine("Source file missing.");
					continue;
				}

				if (File.Exists(newFilePath)) {
					Console.WriteLine("Target file already exists.");
					continue;
				}
				File.Move(file, newFilePath);
			}
		}
	}
}
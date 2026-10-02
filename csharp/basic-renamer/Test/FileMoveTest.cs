using System;
using System.IO;

using NUnit.Framework;

using Utils;

namespace Test {
	[TestFixture]
	public class FileMoveTest {
		private const string directoryName =
			@"C:\Users\kouzm\Desktop\Music\Russian\Fantastique - Fantastique (2006)";

		[Description("Rename file with basic appending string to filename")]
		[Test]
		public void test1() {
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

		[Description("Rename file with mask")]
		[Test]
		public void test2() {
			// https://learn.microsoft.com/en-us/dotnet/api/system.io.directory.getfiles?view=netframework-4.5
			var files = Directory.GetFiles(directoryName, "*.wav");

			foreach (var file in files) {
				var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
				var extension = Path.GetExtension(file);
				var fileDirectoryName = Path.GetDirectoryName(file);
				var newName = FileMover.GetNewName(fileNameWithoutExtension,"^(?<id>[0-9]+)_(?<artist>[^-]+) - (?<title>.+)$","<id> - <title> - <artist>");
				if (String.IsNullOrEmpty(newName)) {
					Console.WriteLine("Not renaming {0}", file);
					continue;
				}
				var newFilePath = Path.Combine(directoryName, newName + extension);

				Console.WriteLine("filename: \"{0}\" extension: \"{1}\" directory: \"{2}\" new name: \"{3}\" new file path: \"{4}\"", fileNameWithoutExtension, extension, fileDirectoryName, newName, newFilePath);

				if (!File.Exists(file)) {
					Console.WriteLine("Source file missing");
					continue;
				}

				if (File.Exists(newFilePath)) {
					Console.WriteLine("Target file \"{0}\" already exists", newFilePath);
					continue;
				}
				File.Move(file, newFilePath);
			}
		}
	}
}
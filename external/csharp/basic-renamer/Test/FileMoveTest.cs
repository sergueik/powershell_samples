using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

using NUnit.Framework;

using Utils;

namespace Test {
	[TestFixture]
	public class FileMoveTest {
		[Test]
		public void test() {
			// https://learn.microsoft.com/en-us/dotnet/api/system.io.directory.getfiles?view=netframework-4.5
			try {
				var directoryName = @"C:\Users\kouzm\Desktop\Music\Russian\Fantastique - Fantastique (2006)";
				// Only get files that match the mask
				string[] files = Directory.GetFiles(directoryName, "*.wav");
				foreach (string file in files) {
					var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
					var extension = Path.GetExtension(file);
					var fileDirectoryName = Path.GetDirectoryName(file);
					var newFilePath = directoryName + "\\" + fileNameWithoutExtension + "_new" + extension;
					Console.WriteLine(String.Format("filename: \"{0}\" extension: \"{1}\" directory: \"{2}\" new file path: \"{3}\"", fileNameWithoutExtension, extension, fileDirectoryName, newFilePath));
					try {
						if (File.Exists(file) && !File.Exists(newFilePath)) {
							File.Move(file, newFilePath);
							// TODO: new files does not have extention
							Console.WriteLine("File renamed successfully.");
						} else {
							Console.WriteLine("Source file missing or target file already exists.");
						}
					} catch (IOException e) {
						Console.WriteLine(String.Format("An error occurred: {0}", e.Message));
					}
				}
			} catch (Exception e) {
				Console.WriteLine("The process failed: {0}", e.ToString());
			}
		}
	}
}

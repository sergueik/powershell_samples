using System;
using System.IO;
using System.Linq;

// based on: https://github.com/Legato-Dev/AlbumArtExtraction/tree/master/AlbumArtExtraction

namespace Utils {
	class SimplifiedParseArgs {
		static void ParseArgs(string[] args) {
			var options =
				from token in args
				where token.StartsWith("-")
				select token.Substring(1);

			var positionalArguments =
				from token in args
				where !token.StartsWith("-")
				select token;

			// a value indicating whether to overwrite
			var force = options.FirstOrDefault(i => i == "y") != null;

			string first, second;

			// first
			if (positionalArguments.Count() == 1) {
				first = positionalArguments.ElementAt(0);
				second = Path.Combine(Path.GetDirectoryName(first), Path.GetFileNameWithoutExtension(first));
			}
			// first, second
			else if (positionalArguments.Count() == 2) {
				first = positionalArguments.ElementAt(0);
				second = positionalArguments.ElementAt(1);
			} else {
				Usage();
				return;
			}

		}


		static void Usage() {
			Console.WriteLine("\nUsage:" + "\n" +
			"[-y] first [ second ]" + "\n" +
			"\t" + "-y : Overwrite existing file");
		}
	}
}



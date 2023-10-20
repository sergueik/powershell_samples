using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using System.Security;
using Utils;

namespace Program {
	class Program {
		private static void Usage() {
			Console.WriteLine("Usage: grep [/h|/H]" + "\n" + "       grep [/c] [/i] [/l] [/n] [/r] /E:reg_exp /F:files");
		}

		[STAThread]
		static void Main(string[] args) {
			var CommandLine = new Arguments(args);
			if (CommandLine["h"] != null || CommandLine["H"] != null) {
				Usage();
				return;
			}
			var grep = new ConsoleGrep();

			if (CommandLine["E"] != null)
				grep.RegEx = (string)CommandLine["E"];
			else {
				Console.WriteLine("Error: No Regular Expression specified!");
				Console.WriteLine();
				Usage();
				return;
			}
			if (CommandLine["F"] != null)
				grep.Files = (string)CommandLine["F"];
			else {
				Console.WriteLine("Error: No Search Files specified!");
				Console.WriteLine();
				Usage();
				return;
			}
			grep.Recursive = (CommandLine["r"] != null);
			grep.IgnoreCase = (CommandLine["i"] != null);
			grep.JustFilenames = (CommandLine["l"] != null);
			if (grep.JustFilenames)
				grep.LineNumbers = false;
			else
				grep.LineNumbers = (CommandLine["n"] != null);
			if (grep.JustFilenames)
				grep.CountLines = false;
			else
				grep.CountLines = (CommandLine["c"] != null);	
			grep.Verbose = true; // unused
			grep.Search();
		}
	}
}

using System;
using System.IO;
using System.Text;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Utils {
	public class FontResource {

		public static void Main(string[] args) {
			if (args.Length == 0) {
				Console.Error.WriteLine("Usage: FontResourceTool --file=<filename> [--noop] [--format=bytes|base64] [--debug]");
				return;
			}

			var opts = NewParseArgs.Parse(args);

			bool noop = opts.ContainsKey("noop") || opts.ContainsKey("n");
			string format = opts.ContainsKey("format") ? opts["format"] : "base64";

			string path = opts.ContainsKey("file") ?
                  opts["file"] :
                  (opts.ContainsKey("f") ? opts["f"] : null);

			if (path == null) {
				Console.WriteLine("Error: --file=<file.ttf>");
				return;
			}
			if (!File.Exists(path)) {
				Console.Error.WriteLine(String.Format("ERROR: File not found: {0}", path));
				return;
			}


			// do conversion
			// 1) Try loading the font (validation)
			try {
				var privateFontCollection = new PrivateFontCollection();
				privateFontCollection.AddFontFile(path);
				privateFontCollection.Dispose();
				Console.WriteLine(String.Format("OK: Font '{0}' is loadable.", Path.GetFileName(path)));

			} catch (Exception e) {
				Console.Error.WriteLine(String.Format("ERROR loading font: {0} {1}", e.GetType().Name, e.Message));
				return;
			}

			if (noop)
				return;

			// 2) Dump the raw data
			var bytes = File.ReadAllBytes(path);

			if (format == "base64") {
				var payload = Convert.ToBase64String(bytes);
				Console.WriteLine(String.Format(@"/* Base64 string */
string data = @""\n{0}"";\n", payload));
			} else { // bytes
				Console.WriteLine(@"/* Byte array literal */
byte[] data = new byte[] {");
				for (int i = 0; i < bytes.Length; i++) {
					Console.Write("  0x" + bytes[i].ToString("X2"));
					if (i < bytes.Length - 1)
						Console.Write(",");
					if (i % 16 == 0)
						Console.WriteLine();
				}
				Console.WriteLine("};");
			}
			return;
		}
	}
}

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public class Util {

	private string pathSource = @"data.txt";
	public string PathSource { set { pathSource = value ; } }
	private int offset = 1000;
	public int Offset { set { offset = value ; } }
	private string text;
	public string Text { get{ return text; } }
	private byte[] bytes;
	public byte[] Bytes { get{ return bytes; } }
	
	public static void Main(string[] args) {
		// Instantiate to hold result
		var util = new Util();
		if (args.Length > 1) {
			Console.WriteLine("{0}", args[0]);
			util.PathSource = args[0];
			util.Offset = Int32.Parse(args[1]);
			util.Tail();
			Console.WriteLine(String.Format("Length: {0} / {1}", util.Text.Length, util.Bytes.Length));
			// Console.WriteLine(String.Format("\"{0}\"", util.Text));
			//  NOTE: systematic one off
			Console.WriteLine(String.Format("Rpm: {0} / {1}", util.Rpm(),util.Rpm(false)));
			if (args.Length > 2) {
				String fragment = args[2];
				Console.WriteLine(String.Format("Rpm({0}): {1}", fragment, util.Rpm(fragment)));
			}
		}
	}

	public int Rpm() {
		int result = 0;
		for (int cnt=0; cnt != bytes.Length; cnt ++)
			//  NOTE: \r may not be present at all
			if (bytes[cnt] == '\n' )
				result ++;
		return result;
	}

	public int Rpm(String fragment) {
		int result = 0;
		String[]lines = text.Split('\n');
		var regex = new Regex(".*" + Regex.Escape(fragment) + ".*",RegexOptions.Compiled | RegexOptions.IgnoreCase);
		
		for (int cnt=0; cnt != lines.Length; cnt ++) {
			if (regex.IsMatch(lines[cnt]))
				result ++;
		}
		return result;
	}
	public int Rpm(bool flag) {
		int result = text.Split('\n').Length;
		// Console.WriteLine(String.Format("last line: \"{0}\"", text.Split('\n')[result - 1] ));
		return result - 1;
	}
	
	public void Tail() {
		try {

			using (var fileStream = new FileStream(pathSource,
				                               FileMode.Open, FileAccess.Read)) {

				fileStream.Seek(offset, SeekOrigin.Begin);
				int numBytesToRead = (int)fileStream.Length - offset;
				if (numBytesToRead > 0) {
					bytes = new byte[numBytesToRead];
					int numBytesRead = 0;
					while (numBytesToRead > 0) {
						// Console.Error.WriteLine(String.Format("{0} bytes to read", numBytesToRead));
						int n = fileStream.Read(bytes, numBytesRead, numBytesToRead);

						if (n == 0)
							break;

						numBytesRead += n;
						numBytesToRead -= n;
					}
					numBytesToRead = bytes.Length;
					text = Encoding.ASCII.GetString(bytes);
				} else {
					text = "";
				}
			}
		} catch (FileNotFoundException e) {
			Console.Error.WriteLine(e.Message);
		}
	}
}
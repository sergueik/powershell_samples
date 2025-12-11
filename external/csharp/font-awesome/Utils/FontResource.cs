using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;



namespace Utils
{
	public class FontResource
	{

		[StructLayout(LayoutKind.Sequential)]
		public struct GLYPHSET
		{
			public uint cbThis;
			public uint flAccel;
			public uint cGlyphsSupported;
			public uint cRanges;
			// followed by WCRANGE[]
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct WCRANGE
		{
			public ushort wcLow;
			public ushort cGlyphs;
		}

		[DllImport("gdi32.dll")]
		public static extern int GetFontUnicodeRanges(IntPtr hdc, IntPtr lpgs);

		[DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
		public static extern uint GetGlyphIndicesW(
			IntPtr hdc, char[] lpstr, int c, ushort[] pgi, uint fl);

		[DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr CreateFont(
			int nHeight, int nWidth, int nEscapement, int nOrientation,
			int fnWeight, uint fdwItalic, uint fdwUnderline, uint fdwStrikeOut,
			uint fdwCharSet, uint fdwOutputPrecision, uint fdwClipPrecision,
			uint fdwQuality, uint fdwPitchAndFamily, string lpszFace);

		[DllImport("gdi32.dll")]
		public static extern IntPtr SelectObject(IntPtr hdc, IntPtr h);

		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr h);

		public struct GlyphInfo
		{
			public int CodePoint;
			public string Name;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct LOGFONT
		{
			public int lfHeight;
			public int lfWidth;
			public int lfEscapement;
			public int lfOrientation;
			public int lfWeight;
			public byte lfItalic;
			public byte lfUnderline;
			public byte lfStrikeOut;
			public byte lfCharSet;
			public byte lfOutPrecision;
			public byte lfClipPrecision;
			public byte lfQuality;
			public byte lfPitchAndFamily;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string lfFaceName;
		}

		public const int GDI_ERROR = 0;
		public const uint FR_PRIVATE = 0x10;
		[DllImport("gdi32.dll")]
		private static extern int AddFontResourceEx(string lpFileName, uint fl, IntPtr pdv);
		[DllImport("gdi32.dll")]
		public static extern bool RemoveFontResourceEx(string lpFileName, uint fl, IntPtr pdv);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateFontIndirect([In] ref LOGFONT lplf);

		public static IEnumerable<Tuple<int, int>> EnumerateGlyphRanges(string fontPath)
		{
			// Load font PRIVATE (not installed)
			AddFontResourceEx(fontPath, FR_PRIVATE, IntPtr.Zero);

			// Prepare DC
			IntPtr hdc = Graphics.FromHwnd(IntPtr.Zero).GetHdc();

			// Create LOGFONT using the font's family name
			PrivateFontCollection pfc = new PrivateFontCollection();
			pfc.AddFontFile(fontPath);
			string face = pfc.Families[0].Name;
			pfc.Dispose();

			LOGFONT lf = new LOGFONT();
			lf.lfHeight = -16;
			lf.lfFaceName = face;

			IntPtr hFont = CreateFontIndirect(ref lf);
			SelectObject(hdc, hFont);

			// First call to get size
			uint size = (uint)(uint)GetFontUnicodeRanges(hdc, IntPtr.Zero);
			if (size == 0) {
				yield break;
			}

			IntPtr mem = Marshal.AllocHGlobal((int)size);
			GetFontUnicodeRanges(hdc, mem);

			GLYPHSET gs = (GLYPHSET)Marshal.PtrToStructure(mem, typeof(GLYPHSET));
			IntPtr pr = IntPtr.Add(mem, Marshal.SizeOf(typeof(GLYPHSET)));

			for (int i = 0; i < gs.cRanges; i++) {
				WCRANGE rc = (WCRANGE)Marshal.PtrToStructure(pr, typeof(WCRANGE));
				int low = rc.wcLow;
				int high = rc.wcLow + rc.cGlyphs - 1;
				yield return Tuple.Create(low, high);

				pr = IntPtr.Add(pr, Marshal.SizeOf(typeof(WCRANGE)));
			}

			Marshal.FreeHGlobal(mem);
			DeleteObject(hFont);
			RemoveFontResourceEx(fontPath, FR_PRIVATE, IntPtr.Zero);
		}

		public static IEnumerable<GlyphInfo> EnumerateGlyphs(string fontName)
		{
			var bmp = new Bitmap(1, 1);
			Graphics g = Graphics.FromImage(bmp);
			IntPtr hdc = g.GetHdc();

			// Create the font
			IntPtr hFont = CreateFont(0, 0, 0, 0, 400, 0, 0, 0, 1, 0, 0, 0, 0, fontName);

			IntPtr oldFont = SelectObject(hdc, hFont);

			try {
				// Get unicode ranges
				List<Tuple<int, int>> ranges = GetUnicodeRanges(hdc);

				// Iterate every codepoint in every supported range
				foreach (Tuple<int, int> r in ranges) {
					int start = r.Item1;
					int end = r.Item2;

					for (int cp = start; cp <= end; cp++) {
						char[] chars;

						// BMP vs surrogate pair
						if (cp <= 0xFFFF) {
							chars = new char[] { (char)cp };
						} else {
							chars = Char.ConvertFromUtf32(cp).ToCharArray();
						}

						ushort[] glyphIndex = new ushort[chars.Length];
						uint result = GetGlyphIndicesW(hdc, chars, chars.Length, glyphIndex, 0);

						if (glyphIndex[0] == 0xFFFF)
							continue; // Unsupported code point

						// Synthesize glyph name
						string name = "glyph_" + cp.ToString("X4");

						GlyphInfo info = new GlyphInfo();
						info.CodePoint = cp;
						info.Name = name;

						yield return info;
					}
				}
			} finally {
				SelectObject(hdc, oldFont);
				DeleteObject(hFont);
				g.ReleaseHdc(hdc);

				g.Dispose();
				bmp.Dispose();
			}
		}

		public static void GenerateEnum(string font, string enumName = "data", string outFile = null) {
			TextWriter textWriter = (outFile == null) ? Console.Out : new StreamWriter(outFile);
			var stringBuilder = new System.Text.StringBuilder();
			stringBuilder.AppendLine("public enum " + enumName);
			stringBuilder.AppendLine("{");

			foreach (var glyph in EnumerateGlyphs(font)) {
				stringBuilder.AppendLine(String.Format("    {0} = 0x{1:X4},", glyph.Name, glyph.CodePoint));
			}

			stringBuilder.AppendLine("}");

			textWriter.Write(stringBuilder.ToString());
			textWriter.Flush();
			if (outFile != null)
				textWriter.Close();
		}


		private static List<Tuple<int, int>> GetUnicodeRanges(IntPtr hdc) {
			var ranges = new List<Tuple<int, int>>();

			int size = GetFontUnicodeRanges(hdc, IntPtr.Zero);
			IntPtr buffer = Marshal.AllocHGlobal(size);

			try {
				GetFontUnicodeRanges(hdc, buffer);

				var gs = (GLYPHSET)Marshal.PtrToStructure(buffer, typeof(GLYPHSET));

				// Offset to first WCRANGE:
				int baseOffset = Marshal.SizeOf(typeof(GLYPHSET));
				IntPtr p = new IntPtr(buffer.ToInt64() + baseOffset);

				for (uint i = 0; i < gs.cRanges; i++) {
					WCRANGE range = (WCRANGE)Marshal.PtrToStructure(p, typeof(WCRANGE));

					int start = range.wcLow;
					int end = range.wcLow + range.cGlyphs - 1;

					ranges.Add(Tuple.Create(start, end));

					p = new IntPtr(p.ToInt64() + Marshal.SizeOf(typeof(WCRANGE)));
				}
			} finally {
				Marshal.FreeHGlobal(buffer);
			}

			return ranges;
		}

		public static void Main(string[] args)
		{
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

			if (opts.ContainsKey("list")) {
				foreach (var r in EnumerateGlyphRanges(path)) {
					Console.WriteLine("U+{0:X4}–U+{1:X4}", r.Item1, r.Item2);
				}
				return;
			}

			if (opts.ContainsKey("enum")) {
			    string outFile = opts.ContainsKey("out") ? opts["out"] : null;
			    GenerateEnum(path, opts["enum"], outFile);
			    return;
			}
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

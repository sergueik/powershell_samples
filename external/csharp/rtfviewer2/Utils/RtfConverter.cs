using System;
using System.Globalization;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Generic;

namespace Utils {
	public class RtfConverter {
		// IMPORTANT NOTE REGARDING IMAGES IN MARKDOWN
		// The RichTextBox Readonly value must be False, otherwise images will not load. This is a bug, since at least 2018

		// https://manpages.ubuntu.com/manpages/jammy/man3/RTF::Cookbook.3pm.html   RTF cookbook
		// https://www.oreilly.com/library/view/rtf-pocket-guide/9781449302047/   RTF Pocket Guide  // scroll down for table of contents
		// https://www.oreilly.com/library/view/rtf-pocket-guide/9781449302047/ch04.html   ASCII-RTF Character Chart / RTF escaped characters
		// https://www.biblioscape.com/rtf15_spec.htm   Rich Text Format (RTF) Version 1.5 Specification
		// https://latex2rtf.sourceforge.net/rtfspec.html   Rich Text Format (RTF) Specification,version 1.6

		// To set table column width, add a line before the table like this, each column value enclosed by : on both sides
		// <!---CW:2000:4000:1000:-->
		// Where the widths are listed in Twips, 1/20th of a point or 1/1440th of an inch.

		//public Color TextColor = Color.Black;
		//public Color HeadingColor = Color.SteelBlue;
		//public Color CodeFontColor = Color.DarkSlateGray;
		//public Color CodeBackgroundColor = Color.Lavender;
		//public Color ListPrefixColor = Color.Blue;
		//public Color LinkColor = Color.Blue;

		private RtfColorInfo rtfTextColor = new RtfColorInfo(Color.Black, 1);
		private RtfColorInfo rtfHeadingColor = new RtfColorInfo(Color.SteelBlue, 2);
		private RtfColorInfo rtfCodeFontColor = new RtfColorInfo(Color.DarkSlateGray, 3);
		private RtfColorInfo rtfCodeBackgroundColor = new RtfColorInfo(Color.Lavender, 4);
		private RtfColorInfo rtfListPrefixColor = new RtfColorInfo(Color.Blue, 5);
		private RtfColorInfo rtfLinkColor = new RtfColorInfo(Color.CornflowerBlue, 6);

		private RtfWriter rtfWriter = new RtfWriter();
		public Color TextColor {
			get { return rtfTextColor.color; }
			set { rtfTextColor = new RtfColorInfo(value, 1); }
		}
		public Color HeadingColor {
			get { return rtfHeadingColor.color; }
			set { rtfHeadingColor = new RtfColorInfo(value, 2); }
		}
		public Color CodeFontColor {
			get { return rtfCodeFontColor.color; }
			set { rtfCodeFontColor = new RtfColorInfo(value, 3); }
		}
		public Color CodeBackgroundColor {
			get { return rtfCodeBackgroundColor.color; }
			set { rtfCodeBackgroundColor = new RtfColorInfo(value, 4); }
		}
		public Color ListPrefixColor {
			get { return rtfListPrefixColor.color; }
			set { rtfListPrefixColor = new RtfColorInfo(value, 5); }
		}
		public Color LinkColor {
			get { return rtfLinkColor.color; }
			set { rtfLinkColor = new RtfColorInfo(value, 6); }
		}

		public string Font = "fswiss Segoe UI";
		//"fswiss Tahoma"; // "fswiss Calibri"; //"fswiss Segoe UI";
		public string CodeFont = "fmodern Courier New";
		public int DefaultPointSize = 10;
		public int H1PointSize = 24;
		public int H2PointSize = 18;
		public int H3PointSize = 15;
		public int H4PointSize = 13;
		public int H5PointSize = 11;
		public int H6PointSize = 10;
		private int CodeBlockPaddingWidth = 50;
		public ParseErrorOutput parseErrorOutput = ParseErrorOutput.ErrorTextAndRawText;
		public List<string> Errors = new List<string>();
		public bool AllowUnderscoreBold = true;
		public bool AllowUnderscoreItalic = true;
		public bool AllowOrderedList = true;
		public bool AllowUnOrderedList = true;
		public int tabLength = 5;
		// some systems use 8, some use 5 spaces as a tab character. Output in Winforms RTF box is 5

		private int currentPaddingWidth;
		private RtfColorInfo currentFontColor;
		private RtfColorInfo previousFontColor;
		private string FileName;

		public enum ParseErrorOutput {
			NoOutput,
			RawText,
			ErrorText,
			ErrorTextAndRawText
		}

		private bool codeBlockActive = false;

		public enum CodeBlockType {
			Indented,
			Fenced,
			Inline,
			None
		}

		private CodeBlockType CurrentCodeBlockType = CodeBlockType.None;

		public RtfConverter(string fileName) {
			currentFontColor = rtfTextColor;
			previousFontColor = rtfTextColor;
			FileName = fileName;
		}

		public string stringReader(string text) {
			var lines = new List<string>();
			lines.Clear();
			using (var stringReader = new StringReader(text)) {
				string line;
				while ((line = stringReader.ReadLine()) != null) {
					if (line != null) {
						lines.Add(line);
					}
				}
			}
			return ConvertText(lines);
		}

		private void CreateCodeBlock(List<string> lines, StringBuilder text, ref int currentLineNum, CodeBlockType blockType)//ref string line, bool blockStartedPreviously)
		{
			bool fencedBlockHasEnd = false;
			//text.Append("---Code block check---");

			Debug.WriteLine("Code block check start");
			List<string> blockLines = new List<string>();
			int MaxLineLength = 0;
			for (int i = currentLineNum; i < lines.Count; i++) {
				string line = lines[i];
				System.Globalization.StringInfo stringInfo = new StringInfo(line);
				MaxLineLength = Math.Max(MaxLineLength, stringInfo.LengthInTextElements);
				Debug.WriteLine(String.Format("line {0}, length: {1}, max {2}", i, stringInfo.LengthInTextElements, MaxLineLength));

				if (blockType == CodeBlockType.Indented) {
					if (line.StartsWith("\t") || line.StartsWith("    ")) {
						//continue
						blockLines.Add(line);
					} else {
						//block ended
						break;
					}
				}

				if (blockType == CodeBlockType.Fenced) {
					if (i == currentLineNum) {
						blockLines.Add("");
					} else if (i > currentLineNum && line.StartsWith("```")) {
						fencedBlockHasEnd = true;
						blockLines.Add("");
						break;
					} else {
						blockLines.Add(line);
					}
				}
			}
			if (blockType == CodeBlockType.Fenced && fencedBlockHasEnd == false) {
				return;
			}
			currentLineNum += blockLines.Count;
			//text.Append($"\\par CodeBlock start: {currentLineNum} lines: {blockLines.Count}, type: {blockType}, widest: {MaxLineLength}\\par");

			text.Append(CodeblockLine("", Math.Max(CodeBlockPaddingWidth, MaxLineLength + 3)));
			foreach (string l in blockLines) {
				text.Append(CodeblockLine(l, Math.Max(CodeBlockPaddingWidth, MaxLineLength + 3)));
				//text.Append($" raw {l.Length} / uu {unicodeLine.Length} / te {realLength}");
			}
			text.Append(CodeblockLine("", Math.Max(CodeBlockPaddingWidth, MaxLineLength + 3)));
			text.AppendLine("\\par ");
		}

		private string CodeblockLine(string line, int padding) {
			var stringInfo = new StringInfo(line);
			int elements = stringInfo.LengthInTextElements;
			int lineDiff = line.Length - elements;
			int padDiff = Math.Max(padding - elements, 0);
			int tabCount = line.AllIndexesOf("\t").Count();

			var sb = new StringBuilder();
			sb.Append(UseFontColor(rtfCodeFontColor, "Code block"));
			sb.Append(@"\f1 ");
			sb.Append(rtfCodeBackgroundColor.asBackgroundColor());
			//sb.Append(line);//.PadRight(line.Length - lineDiff + padDiff));
			string unicodeLine = GetRtfUnicodeEscapedString(line);
			sb.Append(unicodeLine);
			for (int i = 0; i < padDiff; i++) {
				sb.Append(' ');
			}
			sb.Append("\\highlight0 ");
			sb.Append(@"\f0 ");
			sb.Append(UseFontColor(rtfTextColor, "Code Block"));
			Debug.WriteLine(String.Format(" ln: {0} pd:{1} / pad {2}", elements, padDiff, padding));
			sb.AppendLine("\\par ");
			return sb.ToString();
		}

		private string CodeSegment(string line) {
			var sb = new StringBuilder();
			sb.Append(UseFontColor(rtfCodeFontColor, "Code block"));
			sb.Append(@"\f1 ");
			sb.Append(rtfCodeBackgroundColor.asBackgroundColor());
			sb.Append(line);
			sb.Append("\\highlight0 ");
			sb.Append(@"\f0 ");
			sb.Append(UseFontColor(rtfTextColor, "Code Block"));
			return sb.ToString();
		}

		public string ConvertText(string text){
			var lines = new List<string>();
			lines.Clear();
			using (var sr = new StringReader(text)) {
				string line;
				while ((line = sr.ReadLine()) != null) {
					if (line != null) {
						lines.Add(line);
					}
				}
			}
			return ConvertText(lines);
		}

		public string ConvertText(List<string> lines)
		{
			Errors = new List<string>();
			var textSizes = new int[7] {
				DefaultPointSize * 2,
				H1PointSize * 2,
				H2PointSize * 2,
				H3PointSize * 2,
				H4PointSize * 2,
				H5PointSize * 2,
				H6PointSize * 2
			};
			var columnSizes = new List<int>();
			var text = new StringBuilder();

			string colorTable = @"{\colortbl;" + ColorToTableDef(TextColor) + ColorToTableDef(HeadingColor) + ColorToTableDef(CodeFontColor) + ColorToTableDef(CodeBackgroundColor) + ColorToTableDef(ListPrefixColor) + ColorToTableDef(LinkColor) + "}";
			string fontTable = "{\\fonttbl{\\f0\\" + Font + "; }{\\f1\\" + CodeFont + "; }}";
			text.AppendLine("{\\rtf1\\ansi\\deff0 " + fontTable + colorTable + "\\pard");
			rtfWriter.AppendLine("{\\rtf1\\ansi\\deff0 " + fontTable + colorTable + "\\pard");
			//string fontTable = @"\deff0{\fonttbl{\f0\fnil Default Sans Serif;}{\f1\froman Times New Roman;}{\f2\fswiss Arial;}{\f3\fmodern Courier New;}{\f4\fscript Script MT Bold;}{\f5\fdecor Old English Text MT;}}";
			text.Append(UseFontColor(rtfTextColor, "Start convert"));
			rtfWriter.Append(UseFontColor(rtfTextColor, "Start convert"));
			for (int i = 0; i < lines.Count; i++) {
				string line = lines[i];
				string nextLine = string.Empty;
				if (lines.Count > i + 1) {
					nextLine = lines[i + 1];
				}

				// Debug.WriteLine($"¤{line}¤");

				try {
					// Code block - skip all other formatting
					// https://www.markdownguide.org/basic-syntax/#code-blocks
					if (line.StartsWith("\t") || line.StartsWith("    ")) {
						CurrentCodeBlockType = CodeBlockType.Indented;
						CreateCodeBlock(lines, text, ref i, CurrentCodeBlockType);
						CurrentCodeBlockType = CodeBlockType.None;
						// https://www.markdownguide.org/extended-syntax/#fenced-code-blocks
					} else if (line.StartsWith("```") && CurrentCodeBlockType == CodeBlockType.None) {
						CurrentCodeBlockType = CodeBlockType.Fenced;
						CreateCodeBlock(lines, text, ref i, CurrentCodeBlockType);
						CurrentCodeBlockType = CodeBlockType.None;
					} else {
						// normal processing
						CurrentCodeBlockType = CodeBlockType.None;
						line = SetEscapeCharacters(line, true).Text;

						line = SetHeading(textSizes, line);

						line = EscapeNonStyleTags(line, new char[] { '*', '_' });

						// Font style, * _ ** __ used for bold and italic
						line = SetStyle(line, "**", "b"); // bold
						line = SetStyle(line, "*", "i"); // italic
						// Option: in cases where unescaped underlines cause problems mid text, disable underscore as font style
						if (AllowUnderscoreBold) {
							line = SetStyle(line, "__", "b"); // bold
						}
						if (AllowUnderscoreItalic) {
							line = SetStyle(line, "_", "i"); // italic
						}

						// Images. Currently images are removed, TODO: inline images
						// https://www.markdownguide.org/basic-syntax/#images-1
						line = SetImage(line);

						// Comment tag. Remove or implement special behavior in the comment
						if (line.Contains("<!--")) {
							if (line.Contains("<!---CW:")) { // Commen Widths instruction, set the Twip width of the following tables, until a new CW is defined
								var newColumnSizes = SetColumnWidths(line);
								if (newColumnSizes.Count > 0) {
									columnSizes = newColumnSizes;
								}
							}
							line = RemoveComment(line);
							if (line.Length == 0) {
								continue; // skip this line, it's a "<!--" comment, with no other text on the line
							}
						}

						// lists, ordered and unordered
						// https://www.markdownguide.org/basic-syntax/#lists-1
						line = SetListSymbols(line, nextLine);

						// Table. Create table if at least one line followin also start with |.
						// Using the format | one | two | three |   // headings
						//                  |-----|-----|-------|   // this line is skipped
						//                  | a   | b   | c     |   // content
						// https://www.markdownguide.org/extended-syntax/#tables
						if (line.TrimStart().StartsWith("|")) {
							var result = CreateTable(i, lines, columnSizes);
							// append a hiden mark
							line = insertHiddenMark(result.Text);			
							// line = result.Text;
							i = result.Num;
							//  (line, i) = CreateTable(i, lines, columnSizes);
						}

						// insert links if there's a [example title](http://example.com) in text. Actual link click handling is handled by the rich text box in your application
						line = SetLink(line);

						// add the finished line and insert line break
						text.AppendLine(line);
						rtfWriter.AppendLine(line);
						//text.Append(RevertFontColor());
						//rtfWriter.Append(RevertFontColor());
						text.AppendLine("\\par ");
						rtfWriter.AppendLine("\\par ");
					}
				} catch (Exception e){
					Debug.WriteLine(String.Format("Error parsing line {0}: {1}: {2}", i, line,e.ToString()));
					bool outputError = false;
					bool outputRawText = false;

					if (parseErrorOutput == ParseErrorOutput.ErrorText || parseErrorOutput == ParseErrorOutput.ErrorTextAndRawText) {
						outputError = true;
					}
					if (parseErrorOutput == ParseErrorOutput.RawText || parseErrorOutput == ParseErrorOutput.ErrorTextAndRawText) {
						outputRawText = true;
					}

					if (outputError) {
						text.Append("PARSE ERROR");
						rtfWriter.Append("PARSE ERROR");
            
					}
					if (outputError && outputRawText) {
						text.Append(": ");
						rtfWriter.Append(": ");
					}
					if (outputRawText) {
						text.Append(line);
						rtfWriter.Append(line);
					}

					Errors.Add(String.Format("Parse error on line {0}: {1}", i.ToString().PadLeft(3), line));

					text.AppendLine("\\par ");
					rtfWriter.AppendLine("\\par ");
				}
			}

			// end the rtf file
			text.AppendLine("}");
			rtfWriter.AppendLine("}");
			return rtfWriter.ToString();      
		}

		// RTF encoding of the invisible space "Zero Width Space" (ZWSP)character U+200B is \u8203 
		// decimal value of 0x200B with a trailing ? - ANSI fallback required by RTF spec
		// an invisible character that indicates a line break opportunity without creating a visible spa
			
		private string  insertHiddenMark(string line, string mark = @"\u8203?\u8203?\u8203?"){
			var text = new StringBuilder(line);
			text.AppendLine(mark);
			return text.ToString();
		}

		private string UseFontColor(RtfColorInfo newColor, string debugHint = "???") {
			//Debug.WriteLine($"Use font color, hint: {debugHint}. new:{newColor.Color}, previous:{previousFontColor.Color}, current:{currentFontColor.Color}");
			previousFontColor = currentFontColor;
			currentFontColor = newColor;
			return newColor.asFontColor();
		}

		private string SetLink(string line) {
			string linkTitle = "";
			string linkUrl = "";
			int endLinkUrl;
			int startLinkTitle = line.IndexOf("[");
			if (startLinkTitle == -1) {
				return line;
			} else {

				int endLinkTitle = line.IndexOf("]", startLinkTitle);
				if (endLinkTitle > -1) {
					int startLinkUrl = line.IndexOf("(", endLinkTitle);
					if (startLinkUrl > -1 && startLinkUrl < endLinkTitle + 2) {  // the ( must immediately follow the closing ]
						endLinkUrl = line.IndexOf(")", startLinkUrl);
						if (endLinkUrl > -1) {
							linkTitle = line.Substring(startLinkTitle + 1, endLinkTitle - startLinkTitle - 1);
							linkUrl = line.Substring(startLinkUrl + 1, endLinkUrl - startLinkUrl - 1);
							StringBuilder sb = new StringBuilder();
							// sb.Append(line.AsSpan(0, startLinkTitle));
							// MemoryExtensions.AsSpan
							// https://learn.microsoft.com/en-us/dotnet/api/system.memoryextensions.asspan?view=net-10.0&viewFallbackFrom=netframework-4.5
							sb.Append(line.Substring(0, startLinkTitle));
							sb.Append(UseFontColor(rtfLinkColor, "Link"));

							sb.Append(CreateLinkCode(linkTitle, linkUrl));
							if (LineIsHeading) {
								sb.Append(rtfHeadingColor.asFontColor());
							} else {
								sb.Append(rtfTextColor.asFontColor());
							}
							sb.Append(line.Substring(endLinkUrl + 1));
							return sb.ToString();
						}
					}
				}
			}
			return line;
		}

		private string CreateLinkCode(string linkTitle, string linkURL) {
			string result = "{\\field{\\*\\fldinst HYPERLINK \"" + linkURL + "\"}{\\fldrslt " + linkTitle + "}}";
			return result;
		}

		private string SetImage(string line) {

			// IMPORTANT
			// The RichTextBox Readonly value must be False,
			// otherwise images will not load. This is a bug, since at least 2018
			// https://stackoverflow.com/questions/34843931/readonly-content-of-richtextbox-doesnt-show-images
			// https://developercommunity.visualstudio.com/t/richtextbox-fails-to-display-image/383903
			// https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.richtextbox?view=netframework-4.5
			string imageTitle = "";
			string imageUrl = "";
			// https://www.markdownguide.org/basic-syntax/#images
			int startImageTitle = line.IndexOf("![");
			if (startImageTitle == -1) {
				return line;
			} else {
				int endImageTitle = line.IndexOf("]", startImageTitle);
				if (endImageTitle > -1) {
					int startImageUrl = line.IndexOf("(", endImageTitle);
					if (startImageUrl > -1 && startImageUrl < endImageTitle + 2) { // the ( must immediately follow the closing ]
						int endImageUrl = line.IndexOf(")", startImageUrl);
						if (endImageUrl > -1) {
							imageTitle = line.Substring(startImageTitle + 2, endImageTitle - startImageTitle - 2); // text inside []
							imageUrl = line.Substring(startImageUrl + 1, endImageUrl - startImageUrl - 1); // text inside ()

							var  sb = new StringBuilder();
							sb.Append(line.Substring(0, startImageTitle)); // text before the image
							sb.Append(CreateImageCode(imageTitle, imageUrl)); // the pict code
							sb.Append(line.Substring(endImageUrl + 1)); // text after the image
							return sb.ToString();
						}
					}
				}
			}
			return line;
		}

		private string CreateImageCode(string imageTitle, string imageUrl) {
			// IMPORTANT
			// The RichTextBox Readonly value must be set to False, otherwise images will not load. This is a bug, since at least 2017

			// https://www.codeproject.com/Articles/4544/Insert-Plain-Text-and-Images-into-RichTextBox-at-R
			//    {\pict\wmetafile8\picw[N]\pich[N]\picwgoal[N]\pichgoal[N] [BYTES]}
			// OR {\pict\pngblip\picw[N]\pich[N]\picwgoal[N]\pichgoal[N] [BYTES]}
			// \pict - The starting picture or image tag
			// \wmetafile[N] - Indicates that the image type is a Windows Metafile. [N] = 8 specifies that the metafile's axes can be sized independently.
			// \picw[N] and \pich[N] - Define the size of the image, where[N] is in units of hundreths of millimeters(0.01)mm.
			// \picwgoal[N] and \pichgoal[N] - Define the target size of the image, where[N] is in units of twips.
			// [BYTES] - The HEX representation of the image.

			// \emfblip      Source of the picture is an EMF (enhanced metafile).
			// \pngblip      Source of the picture is a PNG.
			// \jpegblip     Source of the picture is a JPEG.
			// \shppict      Specifies a Word 97-2000 picture. This is a destination control word.
			// \nonshppict   Specifies that Word 97-2000 has written a {\pict destination that it will not read on input. This keyword is for compatibility with other readers.
			// \macpict      Source of the picture is QuickDraw.
			// \pmmetafileN  Source of the picture is an OS/2 metafile. The N argument identifies the metafile type. The N values are described in the \pmmetafile table below.
			// \wmetafileN   Source of the picture is a Windows metafile. The N argument identifies the metafile type (the default is 1).
			// \dibitmapN    Source of the picture is a Windows device-independent bitmap. The N argument identifies the bitmap type (must equal 0).The information to be included in RTF from a Windows device-independent bitmap is the concatenation of the BITMAPINFO structure followed by the actual pixel data.
			// \wbitmapN     Source of the picture is a Windows device-dependent bitmap. The N argument identifies the bitmap type (must equal 0).The information to be included in RTF from a Windows device-dependent bitmap is the result of the GetBitmapBits function.

			// couldn't get metafile to work, using png
			// https://www.codeproject.com/Articles/177394/Working-with-Metafile-Images-in-NET

			// This worked using pngblip, along with turning readonly off
			// https://itecnote.com/tecnote/c-programmatically-adding-images-to-rtf-document/

			#pragma warning disable CA1416 // Validate platform compatibility

			string docPath;
			string imagePath;
			Image img = null;
			byte[] bytes = null;
			var stream = new MemoryStream();
			int imageWidth = 100;
			int imageHeight = 100;
			int imageTwipsWidth = 0;
			int imageTwipsHeight = 0;
			const double targetWidthTwips = 8000; 
			double scale = 1;

			if (imageUrl.StartsWith("http") || imageUrl.StartsWith("ftp")) {
				// load file from web
				//imageUrlIsWebAddress = true;
				imagePath = imageUrl;
				using (WebClient client = new WebClient()) {
					bytes = client.DownloadData(translateImageUrl(imageUrl));
					dumpBytes(bytes);
					using (var ms = new MemoryStream(bytes)) {
						img = Image.FromStream(ms);
					}
					imageWidth = img.Width;
					imageHeight = img.Height;
					imageTwipsWidth = (int)(img.Width * 1440 / img.HorizontalResolution);
					imageTwipsHeight = (int)(img.Height * 1440 / img.VerticalResolution);

					img.Dispose();
					img = null;

				}
			} else {
				// load file from disk
				//imageUrlIsWebAddress = false;
				docPath = Path.GetDirectoryName(FileName) + "";
				imagePath = Path.Combine(docPath, imageUrl);
				if (File.Exists(imagePath)) {
					Debug.WriteLine("Loading file from disk: " + imagePath);

					img = Image.FromFile(imagePath);
					img.Save(stream, ImageFormat.Png);
					bytes = stream.ToArray();
					imageWidth = img.Width;
					imageHeight = img.Height;
					imageTwipsWidth = (int)(img.Width * 1440 / img.HorizontalResolution);
					imageTwipsHeight = (int)(img.Height * 1440 / img.VerticalResolution);
					img.Dispose();
					img = null;
				} else {
					Debug.WriteLine("File not found: " + imagePath);
				}
			}

			if (bytes != null) {
				#pragma warning disable CA1416 // Validate platform compatibility
				Debug.WriteLine(String.Format("Load Image {0} URL {1}", imageTitle, imagePath));

				//Debug.WriteLine("Image width: " + img.Width);

				StringBuilder sb = new StringBuilder();
				sb.Append(@"{\pict\pngblip");
				sb.Append("\\picw" + imageWidth); //width source
				sb.Append("\\pich" + imageHeight); //height source
				
				scale = targetWidthTwips / imageTwipsWidth;

				// int imageTwipsWidth = imageWidth * 15;
				// int imageTwipsHeight = imageHeight * 15;

				int scaledWidthTwips = (int)(imageTwipsWidth * scale);
				int scaledHeightTwips = (int)(imageTwipsHeight * scale);				
				
				// sb.Append("\\picwgoal" + imageTwipsWidth); //width in twips
				// sb.Append("\\pichgoal" + imageTwipsHeight); //height in twips
				sb.Append("\\picwgoal" + scaledWidthTwips); // scaled width in twips
				sb.Append("\\pichgoal" + scaledHeightTwips); // scaled height in twips
				sb.Append("\\hex ");

				//MemoryStream stream = new MemoryStream();
				//img.Save(stream, ImageFormat.Png);

				//byte[] bytes = stream.ToArray();
				string str = BitConverter.ToString(bytes, 0).Replace("-", string.Empty);

				sb.Append(str);

				sb.Append("}");
				// NOTE: do not print image fully - may be heavy
				string imageCodeString = sb.ToString();
				Debug.WriteLine(imageCodeString.Length > 100 ?
				                String.Format("{0}...{1}", imageCodeString.Substring(0, 90), imageCodeString.Substring(imageCodeString.Length - 10)) : imageCodeString
				);
				return imageCodeString; // sb.ToString();
			} else {

				Debug.WriteLine(String.Format("Image {0} could not be found from URL {1}", imageTitle, imagePath));
				//return String.Format("\\u128444? ({imagePath.Replace("\\", "\\\\")})"; // outputs an icon of a framed picture (fallback) 🖼
				return CreateLinkCode(String.Format("\\u128444? ({0}: {1})", imageTitle, imageUrl), imageUrl); //.Replace("\\","\\\\")}
			}

			#pragma warning restore CA1416 // Validate platform compatibility
		}

		private void dumpBytes(byte[] bytes, int probeLength = 64) {
			int take = 64;

			Debug.WriteLine(String.Format("Length: {0}", bytes.Length));

			Debug.WriteLine(String.Format("\n=== FIRST BYTES ===\n{0}", BitConverter.ToString(bytes.Take(take).ToArray())));
 
			Debug.WriteLine(String.Format("\n=== LAST BYTES ===\n{0}", bytes.Skip(Math.Max(0, bytes.Length - take)).ToArray()));
   
			// Attempt to print first probeLength bytes as string
			Debug.WriteLine("\n=== FIRST BYTES AS STRING ===");
			try {
				var probeBytes = bytes.Take(Math.Min(bytes.Length, probeLength)).ToArray();
				string probeText = Encoding.UTF8.GetString(probeBytes);
				Debug.WriteLine(probeText);
			} catch (Exception ex) {
				Debug.WriteLine(String.Format("Could not convert bytes to string: {0}", ex.Message));
			}
		}

		private string SetListSymbols(string line, string nextLine) {
			string updatedLine = line;
			if (AllowUnOrderedList) {
				updatedLine = UnorderedListSymbol(line, nextLine);
			}
			if (updatedLine != line) {
				return updatedLine;
			} else if (AllowOrderedList) {
				return OrderedListSymbol(line, nextLine);
			}
			return line;
		}

		int OrderedListCurrentNumber = -1;
		bool OrderedListActive = false;
		private string OrderedListSymbol(string line, string nextLine) {
			var sb = new StringBuilder();
			int prefixLenght = 1;

			if (line.Length == 0) {
				return line;
			}
			char firstChar = line[0];
			char firstCharNextLine = ' ';
			if (nextLine.Length > 0) {
				firstCharNextLine = nextLine[0];
			}


			bool lineHasNumber = (Char.IsNumber(firstChar));
			bool nextLineHasNumber = (Char.IsNumber(firstCharNextLine));

			if (lineHasNumber == false) {
				OrderedListActive = false;
				return line; // not a list, exit.
			}

			if (OrderedListActive == false && nextLineHasNumber == false) {
				OrderedListActive = false;
				return line; // not a list, exit.
			}

			// start making the list
			if (OrderedListActive == false) {
				OrderedListCurrentNumber = 1;
			}
			OrderedListActive = true;

			// if prefix is more than 1 digit
			bool listSymbolValid = false;
			while (line.Length > prefixLenght) {
				char nextChar = line[prefixLenght];
				if (Char.IsNumber(nextChar)) {
					prefixLenght++;
				} else {
					if (nextChar == '.' || nextChar == ')') {
						listSymbolValid = true;
						prefixLenght++;
					}
					break;
				}
			}

			if (!listSymbolValid) {
				return line;
			}

			sb.Append(UseFontColor(rtfListPrefixColor, "Ordered List"));
			sb.Append(OrderedListCurrentNumber.ToString().PadLeft(prefixLenght).PadRight(4));
			//sb.Append(RevertFontColor("Ordered List"));
			sb.Append(UseFontColor(rtfTextColor, "Ordered List"));
			OrderedListCurrentNumber++;
			sb.Append(line.Substring(prefixLenght));
			return sb.ToString();
		}

		bool UnOrderedListActive = false;

		private string UnorderedListSymbol(string line, string nextLine) {
			string asteriskEsc = @"\'2a ";
			string[] unOrderedListPrefixes = { "- ", "+ ", "* ", asteriskEsc };
			//bool unOrderedList = false;
			string currentPrefix = "";
			string unOrderedOutSymbol = " • ";


			bool thisLineHasPrefix = false;
			bool nextLineHasPrefix = false;
			foreach (string prefix in unOrderedListPrefixes) {
				thisLineHasPrefix = line.StartsWith(prefix);
				nextLineHasPrefix = nextLine.StartsWith(prefix);

				if (thisLineHasPrefix && (nextLineHasPrefix || UnOrderedListActive)) {
					currentPrefix = prefix;
					UnOrderedListActive = true;
					break;
				}
			}

			if (thisLineHasPrefix == false || (nextLineHasPrefix == false && UnOrderedListActive == false)) {
				UnOrderedListActive = false;
				return line;
			}

			if (UnOrderedListActive) {
				StringBuilder sb = new StringBuilder();
				sb.Append(UseFontColor(rtfListPrefixColor, "UnOrdered List"));
				sb.Append(unOrderedOutSymbol.PadRight(4));
				sb.Append(UseFontColor(rtfTextColor, "UnOrdered List"));
				sb.Append(line.Substring(currentPrefix.Length));
				line = sb.ToString();
			}
			return line;
		}

		private void OldCreateCodeBlock(List<string> lines, StringBuilder text, int i, ref string line, bool blockStartedPreviously) {
			int numReplaced;
			var result = SetEscapeCharacters(line, false);
			line = result.Text;
			numReplaced = result.Num;
			//  (line, numReplaced) = SetEscapeCharacters(line, false);

			bool codeBlockStarting = false;
			if (blockStartedPreviously == false) {
				// the whole code block has a text background color, and must be padded for the lines to end evenly
				int longestLine = CheckMaxLineLength(lines, i);
				currentPaddingWidth = Math.Max(longestLine, CodeBlockPaddingWidth) + 3;
				codeBlockStarting = true;
			}

			if (codeBlockStarting) {
				//insert a blank line if it's the start of a block
				text.Append(CodeblockLine("\t", currentPaddingWidth));
			}

			// count TABs in line as more characters than normal
			int tabCount = line.AllIndexesOf("\t").Count() - 1;
			// instert the actual text
			line = CodeblockLine(line, currentPaddingWidth + numReplaced - (tabCount * tabLength));
			text.Append(line);
		}

		private string EscapeNonStyleTags(string line, char[] tagChars){
			foreach (char tagChar in tagChars) {
				if (!line.Contains(tagChar))
					continue;
				//Debug.WriteLine("EscapleNonStylTags, line:" + line);
				string nonTag = String.Concat(Enumerable.Repeat(tagChar, 3));
				string esc = ToUnicode(tagChar);
				//Debug.WriteLine("Esc: " + esc);

				//line = line.Replace(nonTag, escNonTag);
				int loopCount = 0;
				while (loopCount < 10) {
					// get first 3*nonTag (*** or ___)
					int match = line.IndexOf(nonTag);

					// count sequence length
					int sequenceLength = 0;
					if (match == -1)
						break; // stop looping, no match found
					//Debug.WriteLine("match:" + match);
					for (int i = match; i < line.Length; i++) {
						if (line[i] == tagChar) {
							sequenceLength++;
						} else {
							break;
						}
					}
					string escNonTag = String.Concat(Enumerable.Repeat(esc, sequenceLength));//sequenceLength));
					if (match >= 0) {
						//Debug.WriteLine("Before: " + line);
						line = line.Substring(0, match) + escNonTag + line.Substring(match + sequenceLength);
						//Debug.WriteLine("After: " + line);
						loopCount++;
					} else {
						break;
					}
				}
			}
			return line;
		}

		private int CheckMaxLineLength(List<string> lines, int startLine) {
			int longestLine = 0;

			for (int i = startLine; i < lines.Count; i++) {
				if (lines[i].StartsWith("\t") == false && lines[i].StartsWith("    ") == false)
					break;
				longestLine = Math.Max(longestLine, lines[i].Length);
			}
			return longestLine;
		}

		private string RemoveComment(string line) {
			const string startTag = "<!--";
			const string endTag = "-->";
			int commentStart = line.IndexOf(startTag);
			int commentEnd = line.IndexOf(endTag);
			var stringBuilder = new StringBuilder();
			if (commentStart > 0)
				stringBuilder.Append(line.Substring(0, commentStart));
			if (commentEnd < line.Length)
				stringBuilder.Append(line.Substring(commentEnd + endTag.Length));
			return stringBuilder.ToString();
		}

		private static string ColorToTableDef(Color color) {
			return @"\red" + color.R + @"\green" + color.G + @"\blue" + color.B + ";";
		}

		private static Result SetEscapeCharacters(string line, bool doubleToSingleBackslash = true) {
			// https://www.oreilly.com/library/view/rtf-pocket-guide/9781449302047/ch04.html
			string result = line;
			int numReplaced = 0;
			// IMPORTANT: \’7d is not the same as \'7d, the ' character matters

			// Escaped markdown characters
			if (doubleToSingleBackslash) {
				result = result.ReplaceAndCount(@"\\", @"\'5c", out numReplaced, numReplaced); // right curly brace
			} else {
				result = result.ReplaceAndCount(@"\\", @"\'5c\'5c", out numReplaced, numReplaced);
			}
			result = result.ReplaceAndCount(@"\#", @"\'23", out numReplaced, numReplaced); // number / hash, to prevent deliberate # from being used as heading
			result = result.ReplaceAndCount(@"\*", @"\'2a", out numReplaced, numReplaced); // asterisk, not font style
			result = result.ReplaceAndCount(@"\_", @"\'5f", out numReplaced, numReplaced); // underscore, not font style
			result = result.ReplaceAndCount(@"\[", @"\'5b", out numReplaced, numReplaced); // left square brace
			result = result.ReplaceAndCount(@"\]", @"\'5d", out numReplaced, numReplaced); // right square brace
			result = result.ReplaceAndCount(@"\{", @"\'7b", out numReplaced, numReplaced); // left curly brace
			result = result.ReplaceAndCount(@"\}", @"\'7d", out numReplaced, numReplaced); // right curly brace
			result = result.ReplaceAndCount(@"\`", @"\'60", out numReplaced, numReplaced); // grave
			result = result.ReplaceAndCount(@"\(", @"\'28", out numReplaced, numReplaced); // left parenthesis
			result = result.ReplaceAndCount(@"\)", @"\'29", out numReplaced, numReplaced); // right parenthesis
			result = result.ReplaceAndCount(@"\+", @"\'2b", out numReplaced, numReplaced); // plus
			result = result.ReplaceAndCount(@"\-", @"\'2d", out numReplaced, numReplaced); // minus
			result = result.ReplaceAndCount(@"\.", @"\'2e", out numReplaced, numReplaced); // period
			result = result.ReplaceAndCount(@"\!", @"\'21", out numReplaced, numReplaced); // exclamation
			result = result.ReplaceAndCount(@"\|", @"\'7c", out numReplaced, numReplaced); // pipe / vertical bar

			// Escape RTF special characters (what remains after escaping the above)
			// replace backslashes not followed by a '
			string regMatchBS = @"\\+(?!')";
			var reg = new Regex(regMatchBS);
			result = ReplaceAndCountRegEx(result, reg, @"\'5c", out numReplaced, numReplaced);

			// replace curly braces
			result = result.ReplaceAndCount(@"{", @"\'7b", out numReplaced, numReplaced); // left curly brace
			result = result.ReplaceAndCount(@"}", @"\'7d", out numReplaced, numReplaced); // right curly brace

			result = GetRtfUnicodeEscapedString(result);

			return new Result {
				Text = result,
				Num = numReplaced
			};
		}

		private static string ReplaceAndCountRegEx(string text, Regex reg, string newValue, out int count, int addToValue) {
			int countBefore = text.Length;
			string result = reg.Replace(text, newValue);
			int countAfter = result.Length;
			int change = countAfter - countBefore;
			count = change + addToValue;
			return result;
		}


		public static string GetRtfUnicodeEscapedString(string s) {
			// https://stackoverflow.com/questions/1368020/how-to-output-unicode-string-to-rtf-using-c
			var sb = new StringBuilder();
			foreach (var c in s) {
				if (c <= 0x7f) // if ((int)c <= 127)
                    sb.Append(c);
				else {
					//string converted = "\\u" + Convert.ToUInt32(c) + "?";
					string converted = "\\u" + (int)c + "?";
					sb.Append(converted);
					//sb.Append("\\'" + ((int)c).ToString("X"));
				}
			}
			return sb.ToString();
		}

		public static string ToUnicode(char c) {
			return ("\\u" + Convert.ToUInt32(c) + "?");
		}

		private static List<int> SetColumnWidths(string line) {
			var result = new List<int>();
			// line is e.g.: <!---CW:2000:4000:1000:-->
			if (line.Contains("<!---CW:")) {
				string[] columnWidths = line.Split(':');
				foreach (string cw in columnWidths) {
					int width;
					if (int.TryParse(cw, out width)) {
						result.Add(width);
					}
				}
			}
			return result;
		}

		private Result CreateTable(int i, List<string> lines, List<int> columSizes) {
			StringBuilder result = new StringBuilder();
			int tableRows = 0;
			bool foundRow = true;
			int columns = lines[i].AllIndexesOf("|").Count() - 1;

			for (int j = i; foundRow && j < lines.Count; j++) {
				if (lines[j].TrimStart().StartsWith("|")) {
					tableRows++;
					foundRow = true;
				} else {
					foundRow = false;
				}
			}

			if (tableRows > 2) { // if there are at least two lines starting with |, it's treated as a table
				for (int r = i; r < i + tableRows; r++) {// string line in lines)
					int lastColumnWidth = 0;
					if (r == i + 1)
						continue; // skip row with dashes that separates headings from rows
					result.AppendLine("\\trowd\\trgaph150");
					for (int c = 0; c < columns; c++) {
						if (columSizes.Count >= columns) {
							int newWidth = lastColumnWidth + columSizes[c];
							result.AppendLine(String.Format("\\cellx{0}", newWidth));
							lastColumnWidth = newWidth;
						} else {
							int newWidth = (c + 1) * 2000;
							result.AppendLine(String.Format("\\cellx{0}", newWidth));
						}
					}

					string[] split = lines[r].Trim().Split('|');
					for (int c = 1; c < split.Length - 1; c++) { // string column in split)
						string colWord = split[c].Trim();

						colWord = SetStyle(colWord, "**", "b"); // bold
						colWord = SetStyle(colWord, "*", "i"); // italic
						if (AllowUnderscoreBold) {
							colWord = SetStyle(colWord, "__", "b"); // bold
						}
						if (AllowUnderscoreItalic) {
							colWord = SetStyle(colWord, "_", "i"); // italic
						}

						result.Append(colWord);
						result.AppendLine("\\intbl\\cell");
					}
					result.AppendLine("\\row ");
				}
				result.AppendLine("\\pard");

				return new Result {
					Text = result.ToString(),
					Num = i + tableRows
				};
			} else {
				return new Result {
					Text = lines[i],
					Num = i
				};
			}
		}

		// TODO: the underscore symbol (_), a common underline 0x5F(95) which is valid in url
		// gets replaced with \'5f
		private static string SetStyle(string line, string tag, string rtfTag) {
			if (line.Contains(tag)) {
				StringBuilder sb = new StringBuilder();
				List<int> matches = line.AllIndexesOf(tag).ToList();
				if (matches.Count > 0) {
					//Debug.WriteLine(String.formt("SetStyle start, tag: {0} to {1}",tag,rtfTag));
					sb.Append(line.Substring(0, matches[0])); // add first chunk before a tag

					int lastTagIndex = 0;

					for (int i = 0; i < matches.Count; i++) {
						if (i + 1 < matches.Count) { // is there a second closing tag?
							//TEST DEBUG
							//Debug.WriteLine("line: " + line);
							//Debug.WriteLine("sb  : " + sb.ToString());
							//Debug.WriteLine($"i: {i}, mC: {matches.Count}, lineL: {line.Length}");
							//Debug.Write("AllIndexes : ");
							//foreach (int m in matches)
							//{
							//    Debug.Write(m + ", ");
							//}
							//Debug.WriteLine("");
							// TEST DEBUG END

							sb.Append(String.Format("\\{0} ", rtfTag)); // start the styled text
							if (matches[i] < line.Length && matches[i + 1] < line.Length) {
								try {
									string words = line.Substring(matches[i], matches[i + 1] - matches[i]); // get the styled text inside the tags
									string add = words.Replace(tag, "");
									sb.Append(words.Replace(tag, "")); // remove the tags from the text
									//Debug.WriteLine(String.Format("1 Appending {0}",add));
								} catch {
									Debug.WriteLine(String.Format("SetStyle, match index {0} and {1}, line length is {2} from:\n{3}", matches[i], matches[i + 1], line.Length, line));
								}
							}

							sb.Append(String.Format("\\{0}0 ", rtfTag)); // end the styled text
							if (matches.Count > i + 2) {

								int startChunkIndex = matches[i + 1] + tag.Length;
								int chunkLength = matches[i + 2] - matches[i + 1] - tag.Length;
								string chunk = line.Substring(startChunkIndex, chunkLength);
								sb.Append(chunk);
								//Debug.WriteLine($"Appending chunk from {startChunkIndex}, length {chunkLength}:{chunk}");
								lastTagIndex = startChunkIndex + chunkLength;
							}

							//Debug.WriteLine($"ending? i: {i}, mC: {matches.Count}");
							if (i + 2 == matches.Count) {
								int endChunkIndex = matches[i + 1] + tag.Length;
								string endChunk1 = line.Substring(endChunkIndex);
								sb.Append(endChunk1);
								//Debug.WriteLine($"End chunk from {endChunkIndex}:" + endChunk1);
								lastTagIndex = endChunkIndex;
							}

							i++;
						} else { // there is no closing tag, output the tag as text
								// without escaping characters
								/*
							string escapedTag = "";
							foreach (char c in tag.ToCharArray()) {
								escapedTag += SetEscapeCharacters("\\" + c.ToString()).Text;
							}
							//Debug.WriteLine($"Escaped tag: {escapedTag}");
							sb.Append(escapedTag);
							*/
							sb.Append(tag);
							//int endChunkIndex = matches[0] + tag.Length;
							int endChunkIndex = Math.Max(lastTagIndex + tag.Length, matches[0] + tag.Length);
							string endChunk2 = "";
							if (endChunkIndex < line.Length)
								endChunk2 = line.Substring(endChunkIndex).ToString();
							//Debug.WriteLine($"Unclosed End Span from {endChunkIndex}:{endChunk2}");
							//sb.Append(line.AsSpan(matches[0] + tag.Length));
							sb.Append(endChunk2);
						}
					}
					//Debug.WriteLine("Done: " + sb.ToString() + "\n");
					line = sb.ToString();
				}

			}
			return line;
		}

		bool LineIsHeading = false;
		private string SetHeading(int[] textSizes, string line) {
			if (line.TrimStart().StartsWith("#")) {
				StringBuilder sb = new StringBuilder();

				//string lineStart = line.Substring(0, 6);
				//string lineEnd = line.Substring(6);
				//int headingSize = lineStart.Split('#').Length - 1; // smaller numbers are bigger text
				line = line.TrimStart();

				int headingSize = 0;

				foreach (char c in line) {
					if (c == '#') {
						headingSize++;
					} else {
						break;
					}
				}

				if (headingSize >= textSizes.Length) {
					//not a valid heading, too many #
					LineIsHeading = false;
					return line;
				} else {
					sb.Append(UseFontColor(rtfHeadingColor, "Heading")); // set heading color
					string headingSizeText = String.Format("\\fs{0} ", textSizes[headingSize]);
					sb.Append(headingSizeText); // set heading size
					int trimStart = headingSize;
					if (line.Substring(headingSize, Math.Min(1, line.Length - headingSize)) == " ") {
						// remove the first space after heading indicator
						trimStart++;
					}
					sb.Append(line.Substring(trimStart));
					//sb.Append(lineEnd);

					sb.Append(String.Format("\\fs{0}", textSizes[0])); // set normal size
					sb.Append(UseFontColor(rtfTextColor, "Heading")); // set normal color
					line = sb.ToString();
					LineIsHeading = true;
					return line;
				}
			}
			LineIsHeading = false;
			return line;
		}
		public static string translateImageUrl(string url){
			if (string.IsNullOrEmpty(url))
				return url;

			var pattern = new Regex(
				              @"https?://github\.com/(?<user>[^/]+)/(?<repo>[^/]+)/(?:blob|tree)/(?<branch>[^/]+)/(?<path>.+)",
				              RegexOptions.IgnoreCase
			              );

			var match = pattern.Match(url);
			if (match.Success) {
				string user = match.Groups["user"].Value;
				string repo = match.Groups["repo"].Value;
				string branch = match.Groups["branch"].Value;
				string path = match.Groups["path"].Value;

				// Normalize path
				if (path.StartsWith("/"))
					path = path.Substring(1);

				string translated = String.Format("https://raw.githubusercontent.com/{0}/{1}/{2}/{3}", user, repo, branch, path);

				Debug.WriteLine(String.Format("TranslateImageUrl: {0} -> {1}", url, translated));

				return translated;
			}

			// Return original URL if no match
			return url;
		}
	}

	public static class ExtensionMethods {
		public static IEnumerable<int> AllIndexesOf(this string str, string searchstring)
		{
			int minIndex = str.IndexOf(searchstring);
			while (minIndex != -1) {
				yield return minIndex;
				minIndex = str.IndexOf(searchstring, minIndex + searchstring.Length);
			}
		}

		public static string ReplaceAndCount(this string text, string oldValue, string newValue, out int count, int addToCount = 0)
		{
			int lenghtDiff = newValue.Length - oldValue.Length;
			count = addToCount + ((text.Split(oldValue.ToCharArray()).Length - 1) * lenghtDiff);
			return text.Replace(oldValue, newValue);
		}
		
	}

	public sealed  class Result {
		private string text;
		private int num;
		public string Text { set; get; }
		public int Num { set; get; }

	}

}

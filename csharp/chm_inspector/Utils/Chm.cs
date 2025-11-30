using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;
//  using STATSTG = System.Runtime.InteropServices.ComTypes.STATSTG;
using Serilog;
using Serilog.Core;
// using Serilog.Sinks.Console;
using Serilog.Debugging;
using Serilog.Sinks.Elasticsearch;
using Serilog.Formatting.Json;
using Serilog.Sinks.File;

using Elasticsearch;


/**
 * Copyright 2025 Serguei Kouzmine
 */


namespace Utils {

	// based on: https://learn.microsoft.com/en-us/answers/questions/1358539/get-chm-title

	public class Chm {

		// Microsoft InfoTech IStorage System (MSITFS) COM server
		public static Guid CLSID_ITStorage = new Guid("5d02926a-212e-11d0-9df9-00a0c922e6ec");

		private static string tocFilename = "toc.hhc"; // "api.hhc"
		// default seems to be "toc.hhc"
		private	const int CHUNK_SIZE = 4096;

		public static string title(string filePath) {
		    object obj = null;
		    IITStorage iit = null;
		    IStorage storage = null;
		    IEnumSTATSTG enumStat = null;
		    IStream stream = null;
		    string result = null;

		    try {
		        obj = Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_ITStorage, true));
		        iit = (IITStorage)obj;

		        HRESULT hresult = iit.StgOpenStorage(filePath, null, (uint)(STGM.STGM_SHARE_EXCLUSIVE | STGM.STGM_READ), IntPtr.Zero, 0, out storage);
		        if (hresult != HRESULT.S_OK || storage == null)
							throw new Exception(String.Format("Failed to open CHM: {0}\nError: 0x{1}\n{2}", filePath, hresult.ToString("X"), MessageHelper.Msg(hresult)));

		        hresult = storage.EnumElements(0, IntPtr.Zero, 0, out enumStat);
		        if (hresult != HRESULT.S_OK || enumStat == null)
		 					throw new Exception(String.Format("Failed to enumerate CHM elements\nError: 0x{0}\n{1}", hresult.ToString("X"), MessageHelper.Msg(hresult)));

		        var stat = new System.Runtime.InteropServices.ComTypes.STATSTG[1];
		        uint fetched;
		        while (enumStat.Next(1, stat, out fetched) == HRESULT.S_OK && fetched == 1) {
		            string name = stat[0].pwcsName;
		            if (string.Equals(name, "#SYSTEM", StringComparison.OrdinalIgnoreCase)) {
		                hresult = storage.OpenStream("#SYSTEM", IntPtr.Zero, (uint)(STGM.STGM_SHARE_EXCLUSIVE | STGM.STGM_READ), 0, out stream);
		                if (hresult != HRESULT.S_OK || stream == null)
		                	throw new Exception(String.Format("Failed to open #SYSTEM stream: 0x{0}\n{1}", hresult.ToString("X"), MessageHelper.Msg(hresult)));


		                // Read entire #SYSTEM into bytes
		                byte[] systemData = devour(stream);

		                // Parse #SYSTEM: the layout is a sequence of records; we look for entries with type STGTY_ILOCKBYTES (value 3)
		                // The exact layout depends on the CHM; we'll scan bytes for (type, length, data) pairs.
		                // This code assumes the pattern you used: one-byte type, one-byte reserved, 2-byte length (or similar).
		                // We'll implement a conservative parser that looks for ASCII NUL-terminated strings in the data blocks.

		                using (var ms = new MemoryStream(systemData)) {
		                    var br = new BinaryReader(ms);
		                    // Skip header (you previously skipped 4 bytes)
		                    if (ms.Length >= 4) br.ReadBytes(4);

		                    while (ms.Position < ms.Length) {
		                        // read type (1 byte)
		                        int typeCode = -1;
		                        try { typeCode = br.ReadByte(); } catch { break; }
		                        // read next byte (often reserved)
		                        if (ms.Position >= ms.Length) break;
		                        br.ReadByte();

		                        // Read 2-byte length (unsigned short little-endian)
		                        if (ms.Position + 2 > ms.Length) break;
		                        ushort len = br.ReadUInt16();

		                        if (len == 0) continue;
		                        if (ms.Position + len > ms.Length) break;

		                        byte[] data = br.ReadBytes(len);

		                        if (typeCode == (int)STGTY.STGTY_ILOCKBYTES) {
		                            // the payload often contains a zero-terminated ANSI string path/name
		                            // Use ASCII/Default encoding
		                            int z = Array.IndexOf<byte>(data, 0);
		                            if (z < 0) z = data.Length;
		                            result = Encoding.Default.GetString(data, 0, z);
		                            break;
		                        }
		                    }
		                }

		                break; // done after #SYSTEM
		            }
		        }
		    } finally {
		        if (stream != null) Marshal.ReleaseComObject(stream);
		        if (enumStat != null) Marshal.ReleaseComObject(enumStat);
		        if (storage != null) Marshal.ReleaseComObject(storage);
		        if (iit != null) Marshal.ReleaseComObject(iit);
		        if (obj != null) Marshal.ReleaseComObject(obj);
		    }

		    return result;
		}

		public static List<string> urls_structured(string filePath) {

			// check MOTW alternative stream (ATS)
			Nullable<int> zone = Security.PeekMotwZone(filePath);
			if (zone.HasValue) {
				Console.WriteLine("File is blocked, ZoneId=" + zone.Value);
				Security.RemoveMotw(filePath);
			} else
				Console.WriteLine("File is safe");


			var urls = new List<string>();

			object obj = null;
			IITStorage iit = null;
			IStorage storage = null;
			IEnumSTATSTG enumStat = null;

			try {
				obj = Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_ITStorage, true));
				iit = (IITStorage)obj;

				HRESULT hresult = iit.StgOpenStorage(filePath, null, (uint)(STGM.STGM_SHARE_EXCLUSIVE | STGM.STGM_READ), IntPtr.Zero, 0, out storage);

				if (hresult != HRESULT.S_OK || storage == null) {
					throw new Exception(String.Format("Failed to open CHM: {0}\nError: 0x{1}\n{2}", filePath, hresult.ToString("X"), MessageHelper.Msg(hresult)));
				}

				hresult = storage.EnumElements(0, IntPtr.Zero, 0, out enumStat);
				if (hresult != HRESULT.S_OK || enumStat == null) {
					throw new Exception(String.Format("Failed to enumerate CHM elements\nError: 0x{0}\n{1}", hresult.ToString("X"), MessageHelper.Msg(hresult)));
				}

				var stat = new System.Runtime.InteropServices.ComTypes.STATSTG[1];
				uint fetched = 0;

				while (enumStat.Next(1, stat, out fetched) == HRESULT.S_OK && fetched == 1) {
					if (stat[0].type == (int)STGTY.STGTY_STREAM) {
						string name = stat[0].pwcsName;
						if (name != null) {
							string lower = name.ToLowerInvariant();
							if (lower.EndsWith(".html") || lower.EndsWith(".htm")) {
								urls.Add(name.Replace("\\", "/"));
							}
						}
					}
				}

			} finally {
				if (enumStat != null)
					Marshal.ReleaseComObject(enumStat);
				if (storage != null)
					Marshal.ReleaseComObject(storage);
				if (iit != null)
					Marshal.ReleaseComObject(iit);
				if (obj != null)
					Marshal.ReleaseComObject(obj);
			}

			return urls;
		}


		// #URLSTR always contains exactly one table of contents (TOC) file name
		// CHM may contain additional .hhc files
		// but only the one referenced from #URLSTR is the real TOC.
		public static string tocfilename_structured(string filePath) {

		    object obj = null;
		    IITStorage iit = null;
		    IStorage storage = null;
		    IStream stream = null;

		    try {
		        obj = Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_ITStorage, true));
		        iit = (IITStorage)obj;

		        HRESULT hresult = iit.StgOpenStorage(filePath, null,
		            (uint)(STGM.STGM_SHARE_EXCLUSIVE | STGM.STGM_READ),
		            IntPtr.Zero, 0, out storage);

		        if (hresult != HRESULT.S_OK || storage == null)
		            return null;

		        hresult = storage.OpenStream("#URLSTR", IntPtr.Zero,
		            (uint)(STGM.STGM_SHARE_EXCLUSIVE | STGM.STGM_READ),
		            0, out stream);

		        if (hresult != HRESULT.S_OK || stream == null)
		            return null;

		        var data = devour(stream);

		        var text = Encoding.Default.GetString(data);
		        var parts = text.Split('\0');

		        foreach (var p in parts) {
		            if (p.EndsWith(".hhc", StringComparison.OrdinalIgnoreCase))
		                return p.Replace("\\", "/");
		        }
		    }
		    finally {
		        if (stream != null) Marshal.ReleaseComObject(stream);
		        if (storage != null) Marshal.ReleaseComObject(storage);
		        if (iit != null) Marshal.ReleaseComObject(iit);
		        if (obj != null) Marshal.ReleaseComObject(obj);
		    }
		    return null;
		}

		private static IEnumerable<string> split_on_nulls(byte[] data) {
		    string text = Encoding.Default.GetString(data);
		    string[] parts = text.Split('\0');

		    var result = new List<string>();
		    foreach (string s in parts) {
		        if (!string.IsNullOrWhiteSpace(s))
		            result.Add(s.Trim());
		    }
		    return result;
		}
		private static byte[] devour(IStream stream) {
			Log.Information("Starting devour");
		    var ms = new MemoryStream();
		    IntPtr bytesReadPtr = IntPtr.Zero;
		    try {
		        bytesReadPtr = Marshal.AllocCoTaskMem(sizeof(int));
		        var buffer = new byte[CHUNK_SIZE];
				// NOTE: do not be logging every iteration of the read loop
				int loopCounter = 0;

		        while (true) {
		            // Read returns HRESULT; number of bytes read is returned via bytesReadPtr
		            stream.Read(buffer, buffer.Length, bytesReadPtr);
		            int bytesRead = Marshal.ReadInt32(bytesReadPtr);
		            if (bytesRead <= 0) break;
		            ms.Write(buffer, 0, bytesRead);

                    loopCounter++;
					if (loopCounter % 20 == 0){
						long mem = GC.GetTotalMemory(false);
						var doc = new {
							timestamp = DateTime.UtcNow,
							message = "OOM imminent",
							mem
						};
						var resp = Telemetry.sendEvent("oom-events", doc);
						Log.Information(String.Format("OOM telemetry sent: status {0}", resp.HttpStatusCode));
                    }
		        }
		        return ms.ToArray();
		    } finally {
		        if (bytesReadPtr != IntPtr.Zero) Marshal.FreeCoTaskMem(bytesReadPtr);
		    }
		}

		public static String tocfilename_7zip(string filePath) {
			string result = null;
			string resourceFilename = "#URLSTR";
			string tempDir = Path.Combine(Path.GetTempPath(), "chm_" + Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(tempDir);

			try {
				// NOTE: whitespace sensitive
				string arguments = string.Format("x \"{0}\" {1} -o\"{2}\"", filePath, resourceFilename, tempDir);

				var processStartInfo = new ProcessStartInfo {
					FileName = @"c:\Program Files\7-zip\7z.exe",
					UseShellExecute = false,
					Arguments = arguments,
					WorkingDirectory = tempDir,
					CreateNoWindow = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true
				};

				using (var process = Process.Start(processStartInfo)) {
					const int waitForExit = 10000;
					if (!process.WaitForExit(waitForExit)) {
						process.Kill();
						throw new Exception("7-Zip process timed out");
					}

					if (process.ExitCode != 0) {
						string error = process.StandardError.ReadToEnd();
						throw new Exception(String.Format("7-Zip failed with exit code {0}: {1}", process.ExitCode , error));
					}
				}

				// Read the extracted ""
				string resourceFilePath = Path.Combine(tempDir, resourceFilename);
				if (!File.Exists(resourceFilePath))
					throw new FileNotFoundException(String.Format("file {0} not found after extraction: {1}\n{2} {3}", resourceFilename, resourceFilePath, processStartInfo.FileName, processStartInfo.Arguments));
				string payload = File.ReadAllText(resourceFilePath, Encoding.UTF8);

		        var parts = payload.Split('\0');

		        foreach (var p in parts) {
		            if (p.EndsWith(".hhc", StringComparison.OrdinalIgnoreCase))
		                result = p.Replace("\\", "/");
		        }


			} finally {
				// Clean up temp directory
				try { Directory.Delete(tempDir, true); } catch { /* ignore */ }
			}

			return result;
		}

		public static List<string> urls_7zip(string filePath) {
			var urls = new List<string>();

			var processStartInfo = new ProcessStartInfo {
				FileName = @"c:\Program Files\7-zip\7z.exe",
				Arguments = String.Format("l -slt \"{0}\"", filePath),
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};

			using (var process = Process.Start(processStartInfo)) {
				var output = process.StandardOutput;
				string line;
				string currentPath;

				while ((line = output.ReadLine()) != null) {
					if (line.StartsWith("Path = ")) {
						currentPath = line.Substring("Path = ".Length);

						string lower = currentPath.ToLowerInvariant();
						if (lower.EndsWith(".html") || lower.EndsWith(".htm")) {
							urls.Add(currentPath.Replace("\\", "/"));
						}
					}
				}
				process.WaitForExit(10000);
				if (process.ExitCode != 0)
					throw new Exception("7-Zip failed with exit code " + process.ExitCode);
			}

			return urls;
		}

		public static List<string> urls_7zip_alt(string filePath) {
			var urls = new List<string>();
			string tempDir = Path.Combine(Path.GetTempPath(), "chm_" + Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(tempDir);
			try {
				// NOTE: whitespace sensitive
				String arguments = String.Format("x \"{0}\" -o\"{1}\"", filePath,  tempDir);
				// Console.Error.WriteLine("{0} {1}", "\"c:\\Program Files\\7-zip\\7z.exe\"", arguments);
				var processStartInfo = new ProcessStartInfo /*(@"c:\Program Files\7-zip\7z.exe", arguments) */ {
					FileName = @"c:\Program Files\7-zip\7z.exe",
					UseShellExecute = false,
					Arguments = arguments,
					WorkingDirectory = tempDir,
					CreateNoWindow = true
				};
				const int waitForExit = 10000;
				var process = Process.Start(processStartInfo);
				process.WaitForExit(waitForExit);
				if (process.ExitCode != 0)
					throw new Exception("7-Zip failed with exit code " + process.ExitCode);
				string[] searchPatterns = { "*.html", "*.htm" };
				var allMatchingFiles = new List<string>();
				foreach (string pattern in searchPatterns) {
					var filesForPattern = Directory.GetFiles(tempDir, pattern, SearchOption.AllDirectories);
					allMatchingFiles.AddRange(filesForPattern);
				}
				foreach (var file in allMatchingFiles)
					urls.Add(file.Substring(tempDir.Length + 1).Replace("\\", "/"));
			} finally {
				// optionally clean up
				Directory.Delete(tempDir, true);
			}
			return urls;
		}

		public static List<TocEntry> toc_structured(string filePath) {
		    var result = new List<TocEntry>();

		    object obj = null;
		    IITStorage iit = null;
		    IStorage storage = null;
		    IEnumSTATSTG enumStat = null;
		    IStream stream = null;

		    try {
		        obj = Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_ITStorage, true));
		        iit = (IITStorage)obj;

		        HRESULT hresult = iit.StgOpenStorage(
		            filePath,
		            null,
		            (uint)(STGM.STGM_SHARE_EXCLUSIVE | STGM.STGM_READ),
		            IntPtr.Zero,
		            0,
		            out storage
		        );

		        if (hresult != HRESULT.S_OK || storage == null)
		            throw new Exception(String.Format("Failed to open CHM file {0}\nError: 0x{1}\n{2}", filePath, hresult.ToString("X"), MessageHelper.Msg(hresult)));

		        // Enumerate CHM root directory
		        hresult = storage.EnumElements(0, IntPtr.Zero, 0, out enumStat);
		        if (hresult != HRESULT.S_OK)
		          throw new Exception(String.Format("Failed to enumerate CHM elements\nError: 0x{0}\n{1}", hresult.ToString("X"), MessageHelper.Msg(hresult)));


		        var stat = new System.Runtime.InteropServices.ComTypes.STATSTG[1];
		        uint fetched = 0;

		        while (enumStat.Next(1, stat, out fetched) == HRESULT.S_OK && fetched == 1) {
		            if (String.Compare(stat[0].pwcsName, tocFilename, StringComparison.OrdinalIgnoreCase) == 0) {
		                // Open table of contents as stream
		                HRESULT hresult2 = storage.OpenStream(
		                    tocFilename,
		                    IntPtr.Zero,
		                    (uint)(STGM.STGM_READ | STGM.STGM_SHARE_EXCLUSIVE),
		                    0,
		                    out stream
		                );

		                if (hresult2 != HRESULT.S_OK || stream == null)
			                throw new Exception(String.Format("Failed to open stream {0},\nError: 0x{1}\n{2}", tocFilename, hresult2.ToString("X"), MessageHelper.Msg(hresult2)));


		                // Memory-conservative read loop
		                using (var ms = new MemoryStream()) {
		                    var buffer = new byte[CHUNK_SIZE];
		                    IntPtr bytesReadPtr = Marshal.AllocCoTaskMem(sizeof(int));

		                    try {
		                        while (true) {
		                            stream.Read(buffer, buffer.Length, bytesReadPtr);
		                            int bytesRead = Marshal.ReadInt32(bytesReadPtr);

		                            if (bytesRead == 0)
		                                break;

		                            ms.Write(buffer, 0, bytesRead);

									long mem = GC.GetTotalMemory(false);
									var doc = new {
										timestamp = DateTime.UtcNow,
										message = "OOM imminent",
										mem
									};
									// NOTE: no need to flush timing with Serilog when the process may OOM at any moment
									// Telemetry through Telemetry.sendEvent() is already flushed instantly and is safe for OOM scenarios
									var resp = Telemetry.sendEvent("oom-events", doc);
									Log.Information(String.Format("OOM telemetry sent: status {0}", resp.HttpStatusCode));
		                        }
		                    } finally {
		                        Marshal.FreeCoTaskMem(bytesReadPtr);
		                    }
		                    string payload = Encoding.UTF8.GetString(ms.ToArray());
							// Extract OBJECT PARAM Name/Local
							result = parseToc(payload);
		                }
		                break;
		            }
		        }
		    } finally {
		        if (stream != null) Marshal.ReleaseComObject(stream);
		        if (enumStat != null) Marshal.ReleaseComObject(enumStat);
		        if (storage != null) Marshal.ReleaseComObject(storage);
		        if (iit != null) Marshal.ReleaseComObject(iit);
		        if (obj != null) Marshal.ReleaseComObject(obj);
		    }
		    return result;
		}


		public static List<TocEntry> parseToc(String payload) {
			var result = new List<TocEntry>();
			// Extract OBJECT PARAM Name/Local
			var matches = Regex.Matches(
				payload,
				@"<OBJECT[^>]*>.*?<param name=""Name"" value=""(.*?)"">" +
				@".*? " +
				@"<param name=""Local"" value=""(.*?)"">.*?</OBJECT>",
				RegexOptions.Singleline
			);
			foreach (Match match in matches) {
				result.Add(new TocEntry {
				           	Name = match.Groups[1].Value,
				           	Local = match.Groups[2].Value
				           });
			}
			return result;
		}

		public static List<TocEntry> toc_7zip(string filePath) {
			var result = new List<TocEntry>();

			string tempDir = Path.Combine(Path.GetTempPath(), "chm_" + Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(tempDir);

			try {
				// NOTE: whitespace sensitive
				string arguments = string.Format("x \"{0}\" {1} -o\"{2}\"", filePath, tocFilename, tempDir);

				var processStartInfo = new ProcessStartInfo {
					FileName = @"c:\Program Files\7-zip\7z.exe",
					UseShellExecute = false,
					Arguments = arguments,
					WorkingDirectory = tempDir,
					CreateNoWindow = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true
				};

				using (var process = Process.Start(processStartInfo)) {
					const int waitForExit = 10000;
					if (!process.WaitForExit(waitForExit)) {
						process.Kill();
						throw new Exception("7-Zip process timed out");
					}

					if (process.ExitCode != 0) {
						string error = process.StandardError.ReadToEnd();
						throw new Exception(String.Format("7-Zip failed with exit code {0}: {1}", process.ExitCode , error));
					}
				}

				// Read the extracted toc.hhc
				string tocFilePath = Path.Combine(tempDir, tocFilename);
				// NOTE: acidentally removing the following line
				// leads to compiler error:
				// Embedded statement cannot be a declaration or labeled statement (CS1023)

				if (!File.Exists(tocFilePath))
                   throw new FileNotFoundException(String.Format("table of contents {0} not found after extraction: {1}\n{2} {3}", tocFilename, tocFilePath, processStartInfo.FileName , processStartInfo.Arguments));
				string payload = File.ReadAllText(tocFilePath, Encoding.UTF8);
				result = parseToc(payload);

			} finally {
				// Clean up temp directory
				try { Directory.Delete(tempDir, true); } catch { /* ignore */ }
			}

			return result;
		}

		public static List<string> extract_7zip(string filePath, List<string> files) {
			var filesArg = buildArgument(files);
			var urls = new List<string>();
			string tempDir = Path.Combine(Path.GetTempPath(), "chm_" + Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(tempDir);
			try {
				// NOTE: whitespace sensitive
				String arguments = String.Format("x \"{0}\" {1} -o\"{2}\"", filePath, filesArg,  tempDir);
				Console.Error.WriteLine("{0} {1}", "\"c:\\Program Files\\7-zip\\7z.exe\"", arguments);
				var processStartInfo = new ProcessStartInfo /*(@"c:\Program Files\7-zip\7z.exe", arguments) */ {
					FileName = @"c:\Program Files\7-zip\7z.exe",
					UseShellExecute = false,
					Arguments = arguments,
					WorkingDirectory = tempDir,
					CreateNoWindow = true
				};
				const int waitForExit = 10000;
				var process = Process.Start(processStartInfo);
				process.WaitForExit(waitForExit);
				if (process.ExitCode != 0)
					throw new Exception("7-Zip failed with exit code " + process.ExitCode);
				var allMatchingFiles = new List<string>();
				foreach (string pattern in files) {
					var filesForPattern = Directory.GetFiles(tempDir, pattern, SearchOption.AllDirectories);
					allMatchingFiles.AddRange(filesForPattern);
				}
				foreach (var file in allMatchingFiles)
					urls.Add(file.Substring(tempDir.Length + 1).Replace("\\", "/"));
			} finally {
				// optionally clean up
				Directory.Delete(tempDir, true);
			}
			return urls;
		}


		public static string buildArgument(List<string> files) {
			string arg = string.Join(" ", files.Select(path => String.Format("\"{0}\"",path)));

			if (arg.Length > 28000) {
				string listFile = Path.GetTempFileName();
				File.WriteAllLines(listFile, files);
				return String.Format("@{0}",listFile);
			} else
				return arg;
		}
	}

	public class TocEntry {
		public string Name { get; set; }
		public string Local { get; set; }
	}
	public enum HRESULT : int {
		S_OK = 0,
		S_FALSE = 1,
		E_NOTIMPL = unchecked((int)0x80004001),
		E_NOINTERFACE = unchecked((int)0x80004002),
		E_POINTER = unchecked((int)0x80004003),
		E_FAIL = unchecked((int)0x80004005),
		E_UNEXPECTED = unchecked((int)0x8000FFFF),
		E_OUTOFMEMORY = unchecked((int)0x8007000E),
	};

	[StructLayout(LayoutKind.Sequential)]
	public struct ITS_Control_Data {
		public uint cdwControlData;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
		public uint[] adwControlData;
	}

	public enum ECompactionLev {
		COMPACT_DATA = 0,
		COMPACT_DATA_AND_PATH
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct LARGE_INTEGER {
		[FieldOffset(0)]
		public uint LowPart;
		[FieldOffset(4)]
		public uint HighPart;
		[FieldOffset(0)]
		public long QuadPart;
	}

	// https://www.pinvoke.net/default.aspx/Enums.STGty
	public enum STGTY : int {
		STGTY_STORAGE = 1,
		STGTY_STREAM = 2,
		STGTY_ILOCKBYTES = 3,
		STGTY_ROOT = 4
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct FILETIME {
		public uint DateTimeLow;
		public uint DateTimeHigh;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct STATSTG {
		public string pwcsName;
		public uint type;
		// STGTY_ enum
		public ulong cbSize;
		public FILETIME mtime;
		public FILETIME ctime;
		public FILETIME atime;
		public uint grfMode;
		public uint grfLocksSupported;
		public Guid clsid;
		public uint grfStateBits;
		public uint reserved;
	}

	[ComImport]
	[Guid("0000000d-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IEnumSTATSTG {
		// The user needs to allocate an STATSTG array whose size is celt.
		[PreserveSig]
		HRESULT Next(uint celt, [MarshalAs(UnmanagedType.LPArray), Out] System.Runtime.InteropServices.ComTypes.STATSTG[] rgelt, out uint pceltFetched);
		HRESULT Skip(uint celt);
		HRESULT Reset();
		[return: MarshalAs(UnmanagedType.Interface)]
		IEnumSTATSTG Clone();
	}

	[ComImport]
	[Guid("0000000b-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IStorage {
		HRESULT CreateStream(string pwcsName, uint grfMode, uint reserved1, uint reserved2, out IStream ppstm);
		HRESULT OpenStream(string pwcsName, IntPtr reserved1, uint grfMode, uint reserved2, out IStream ppstm);
		HRESULT CreateStorage(string pwcsName, uint grfMode, uint reserved1, uint reserved2, out IStorage ppstg);
		HRESULT OpenStorage(string pwcsName, IStorage pstgPriority, uint grfMode, IntPtr snbExclude, uint reserved, out IStorage ppstg);
		HRESULT CopyTo(uint ciidExclude, Guid rgiidExclude, IntPtr snbExclude, IStorage pstgDest);
		HRESULT MoveElementTo(string pwcsName, IStorage pstgDest, string pwcsNewName, uint grfFlags);
		HRESULT Commit(uint grfCommitFlags);
		HRESULT Revert();
		HRESULT EnumElements(uint reserved1, IntPtr reserved2, uint reserved3, out IEnumSTATSTG ppenum);
		HRESULT DestroyElement(string pwcsName);
		HRESULT RenameElement(string pwcsOldName, string pwcsNewName);
		HRESULT SetElementTimes(string pwcsName, System.Runtime.InteropServices.ComTypes.FILETIME pctime, System.Runtime.InteropServices.ComTypes.FILETIME patime,
		                        System.Runtime.InteropServices.ComTypes.FILETIME pmtime);

		HRESULT SetClass(Guid clsid);
		HRESULT SetStateBits(uint grfStateBits, uint grfMask);
		HRESULT Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, uint grfStatFlag);
	}

	[ComImport]
	[Guid("88CC31DE-27AB-11D0-9DF9-00A0C922E6EC")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IITStorage {
		HRESULT StgCreateDocfile(string pwcsName, uint grfMode, uint reserved, out IStorage ppstgOpen);
		HRESULT StgCreateDocfileOnILockBytes(IntPtr/*ILockBytes*/ plkbyt, uint grfMode, uint reserved, out IStorage ppstgOpen);
		HRESULT StgIsStorageFile(string pwcsName);
		HRESULT StgIsStorageILockBytes(IntPtr/*ILockBytes*/ plkbyt);
		HRESULT StgOpenStorage(string pwcsName, IStorage pstgPriority, uint grfMode, IntPtr snbExclude, uint reserved, out IStorage ppstgOpen);
		HRESULT StgOpenStorageOnILockBytes(IntPtr/*ILockBytes*/ plkbyt, IStorage pStgPriority, uint grfMode,
		                                   IntPtr snbExclude, uint reserved, out IStorage ppstgOpen);
		HRESULT StgSetTimes(string lpszName, System.Runtime.InteropServices.ComTypes.FILETIME pctime, System.Runtime.InteropServices.ComTypes.FILETIME patime, System.Runtime.InteropServices.ComTypes.FILETIME pmtime);
		HRESULT SetControlData(ITS_Control_Data pControlData);
		HRESULT DefaultControlData(out ITS_Control_Data ppControlData);
		HRESULT Compact(string pwcsName, ECompactionLev iLev);
	}

	[Flags]
	public enum STGM : uint {
		STGM_READ = 0x00000000,
		STGM_WRITE = 0x00000001,
		STGM_READWRITE = 0x00000002,
		STGM_SHARE_DENY_NONE = 0x00000040,
		STGM_SHARE_DENY_READ = 0x00000030,
		STGM_SHARE_DENY_WRITE = 0x00000020,
		STGM_SHARE_EXCLUSIVE = 0x00000010,
		STGM_PRIORITY = 0x00040000,
		STGM_DELETEONRELEASE = 0x04000000,
		STGM_NOSCRATCH = 0x00100000
	}
}

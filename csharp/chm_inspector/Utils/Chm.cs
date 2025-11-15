using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;
//  using STATSTG = System.Runtime.InteropServices.ComTypes.STATSTG;

/**
 * Copyright 2025 Serguei Kouzmine
 */


namespace Utils {

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

	public enum ECompactionLev  {
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

// based on: https://learn.microsoft.com/en-us/answers/questions/1358539/get-chm-title

public class Chm {

		// Microsoft InfoTech IStorage System (MSITFS) COM server
		public static Guid CLSID_ITStorage = new Guid("5d02926a-212e-11d0-9df9-00a0c922e6ec");
		
		public static string title(string filePath) {
		
		    object obj = null;
		    IITStorage iit = null;
		    IStorage storage = null;
		    IEnumSTATSTG enumStat = null;
		    IStream stream = null;
		
		    string result = null;
		
		    try {
		        obj = Activator.CreateInstance( Type.GetTypeFromCLSID(CLSID_ITStorage, true) );
		        iit = (IITStorage)obj;
		
		        HRESULT hresult = iit.StgOpenStorage( filePath, null, (uint)(STGM.STGM_SHARE_EXCLUSIVE | STGM.STGM_READ), IntPtr.Zero, 0, out storage );
		
		        if (hresult != HRESULT.S_OK || storage == null) {
		            throw new Exception(String.Format( "Failed to open CHM: {0}\nError: 0x{1}\n{2}", filePath, hresult.ToString("X"), MessageHelper.Msg(hresult) ));
		        }
				
		        hresult = storage.EnumElements(0, IntPtr.Zero, 0, out enumStat);
		        if (hresult != HRESULT.S_OK || enumStat == null) {
		            throw new Exception(String.Format( "Failed to enumerate CHM elements\nError: 0x{0}\n{1}", hresult.ToString("X"), MessageHelper.Msg(hresult) ));
		        }
		
		        var stat = new System.Runtime.InteropServices.ComTypes.STATSTG[1];
		        uint fetched = 0;
		
		        while (enumStat.Next(1, stat, out fetched) == HRESULT.S_OK && fetched == 1) {
		
		            if (stat[0].pwcsName == "#SYSTEM") {
		
		                HRESULT hresult2 = storage.OpenStream( "#SYSTEM", IntPtr.Zero, (uint)(STGM.STGM_SHARE_EXCLUSIVE | STGM.STGM_READ), 0, out stream);
		
		                if (hresult2 != HRESULT.S_OK || stream == null) {
		                    throw new Exception(String.Format("Failed to open #SYSTEM stream: 0x{0}\n{1}", hresult2.ToString("X"), MessageHelper.Msg(hresult2)));
		                }
		
		                // first skip 4-byte header
		                byte[] buf = new byte[4];
		                IntPtr pcb = Marshal.AllocCoTaskMem(4);
		                stream.Read(buf, 4, pcb);
		                Marshal.FreeCoTaskMem(pcb);
		
		                // now read segments until we find STGTY_ILOCKBYTES
		                while (true) {
		                    buf = new byte[2];
		                    pcb = Marshal.AllocCoTaskMem(4);
		                    stream.Read(buf, 2, pcb);
		                    int nRead = Marshal.ReadInt32(pcb);
		                    Marshal.FreeCoTaskMem(pcb);
		
		                    if (nRead == 0)
		                        break;
		
		                    int typeCode = buf[0];
		
		                    // length prefix
		                    buf = new byte[2];
		                    stream.Read(buf, 2, IntPtr.Zero);
		
		                    int len = buf[0];
		                    if (len <= 0) continue;
		
		                    byte[] data = new byte[len];
		                    stream.Read(data, len, IntPtr.Zero);
		
		                    if (typeCode == (int)STGTY.STGTY_ILOCKBYTES) {
		                        IntPtr ptr = Marshal.AllocHGlobal(len);
		                        Marshal.Copy(data, 0, ptr, len);
		                        result = Marshal.PtrToStringAnsi(ptr);
		                        Marshal.FreeHGlobal(ptr);
		                        break;
		                    }
		                }
		                break; // we are done with #SYSTEM
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
		        obj = Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_ITStorage, true) );
		        iit = (IITStorage)obj;
		
		        HRESULT hresult = iit.StgOpenStorage( filePath, null, (uint)(STGM.STGM_SHARE_EXCLUSIVE | STGM.STGM_READ), IntPtr.Zero, 0, out storage);
		
		        if (hresult != HRESULT.S_OK || storage == null) {
		            throw new Exception(String.Format( "Failed to open CHM: {0}\nError: 0x{1}\n{2}", filePath, hresult.ToString("X"), MessageHelper.Msg(hresult) ));
		        }
		
		        hresult = storage.EnumElements(0, IntPtr.Zero, 0, out enumStat);
		        if (hresult != HRESULT.S_OK || enumStat == null) {
		            throw new Exception(String.Format( "Failed to enumerate CHM elements\nError: 0x{0}\n{1}", hresult.ToString("X"), MessageHelper.Msg(hresult) ));
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
		        if (enumStat != null) Marshal.ReleaseComObject(enumStat);
		        if (storage != null) Marshal.ReleaseComObject(storage);
		        if (iit != null) Marshal.ReleaseComObject(iit);
		        if (obj != null) Marshal.ReleaseComObject(obj);
		    }

		    return urls;
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


	}
}

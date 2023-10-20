# origin: https://www.codeproject.com/Articles/5336372/Windows-11-Version-Detection
# check the default way
$version = [System.Environment]::OSVersion.Version
if ($version.Build -lt 9600) { 
  write-host 'skip RtlGetVersion'
  $releases = @{
    6002 = 'Windows_Vista';
    7601 = 'Windows_7' ;
    9200 = 'Windows_8' ;
  }
  $result = @{
    Major = $version.Major;
    Minor = $version.Minor;
    BuildNum = $version.Build;
    Release = $releases[$version.Build];
  }
  write-output $result |format-list
  exit
}
# NOTE: can not compile the code on Windows 7
# Unhandled exception at 0x77913bd3 in powershell.exe: 0xC0000374: A heap has been corrupted.
# the try-catch around add-Type do not help

add-type -typedefinition  @'

using System;
using System.Runtime.InteropServices;

namespace win_version_csharp {
	public struct VersionInfo {
		public uint Major;
		public uint Minor;
		public uint BuildNum;
		public Release Release;
	}

	public enum Release {
		Windows_Vista = 6002,
		Windows_7 = 7601,
		Windows_8 = 9200,
		Windows_8_1 = 9600,
		Windows_10_1507 = 10240,
		Windows_10_1511 = 10586,
		Windows_10_1607 = 14393,
		Windows_10_1703 = 15063,
		Windows_10_1709 = 16299,
		Windows_10_1803 = 17134,
		Windows_10_1809 = 17763,
		Windows_10_1903 = 18362,
		Windows_10_1909 = 18363,
		Windows_10_2004 = 19041,
		Windows_10_20H2 = 19042,
		Windows_10_21H1 = 19043,
		Windows_10_21H2 = 19044,
		Windows_10_22H2 = 19045,
		Windows_11_21H2 = 22000,
		Windows_11_22H2 = 22621,
		Unknown =  0,
	};

	public class WinVersion {
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct OSVERSIONINFOEXW {
			public uint dwOSVersionInfoSize;
			public uint dwMajorVersion;
			public uint dwMinorVersion;
			public uint dwBuildNumber;
			public uint dwPlatformId;
			[MarshalAs(UnmanagedType.LPWStr, SizeConst = 128)]
			public string szCSDVersion;
			public UInt16 wServicePackMajor;
			public UInt16 wServicePackMinor;
			public UInt16 wSuiteMask;
			public byte wProductType;
			public byte wReserved;
		}
		[DllImport("ntdll.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern int RtlGetVersion(out OSVERSIONINFOEXW osv);

		public static bool GetVersion(out VersionInfo info) {
			info.Major = 0;
			info.Minor = 0;
			info.BuildNum = 0;
			info.Release= (Release) 0;

			OSVERSIONINFOEXW osv = new OSVERSIONINFOEXW();
			osv.dwOSVersionInfoSize = 284;
			if (RtlGetVersion(out osv) == 0) {
				info.Major = osv.dwMajorVersion;
				info.Minor = osv.dwMinorVersion;

				info.BuildNum = osv.dwBuildNumber;
				info.Release = (Release) osv.dwBuildNumber;
				if (osv.dwBuildNumber >= 22000){
					info.Major = 11;
				}
				Console.Error.WriteLine(info.Release.ToString());
				return true;
			}
			return false;
		}
	/*		
        public static bool IsBuildNumGreaterOrEqual(uint buildNumber) {
	// ; expected
            if (GetVersion(out var info)) {
                return info.BuildNum >= buildNumber;
            }
            return false;
        }
	
	*/
	}
}

'@

[win_version_csharp.VersionInfo] $result = @{
  Major = 0
  Minor = 0
  BuildNum = 0
  Release = 0
}
# NOTE: Cannot convert null to type "win_version_csharp.VersionInfo"
[win_version_csharp.WinVersion]::GetVersion([ref]$result)
write-output $result |format-list

using Microsoft.Win32;

namespace Utils {
	public class WindowsVersionManager {
		public WindowsVersion CurrentVersion;

		public bool runningDistroCheckSupported;

		public WindowsVersionManager() {
			int releaseId = int.Parse(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", "").ToString());

			CurrentVersion = new WindowsVersion(releaseId);

			runningDistroCheckSupported = (CurrentVersion.Version >= WindowsVersion.V1903.Version) ? true:  false;
		}
	}

	public class WindowsVersion   {
        public WindowsVersion(int version) { Version = version; }

        public int Version { get; set; }

        public static readonly WindowsVersion V1803 = new WindowsVersion(1803);
        public static readonly WindowsVersion V1809 = new WindowsVersion(1809);
        public static readonly WindowsVersion V1903 = new WindowsVersion(1903);
        public static readonly WindowsVersion V1909 = new WindowsVersion(1909);
        public static readonly WindowsVersion V2004 = new WindowsVersion(2004);
        public static readonly WindowsVersion V2009 = new WindowsVersion(2009);
    }
}

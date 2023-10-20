using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;

[StructLayout(LayoutKind.Sequential)]
public struct DEVMODE1 {
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string dmDeviceName;
	public short dmSpecVersion;
	public short dmDriverVersion;
	public short dmSize;
	public short dmDriverExtra;
	public int dmFields;

	public short dmOrientation;
	public short dmPaperSize;
	public short dmPaperLength;
	public short dmPaperWidth;

	public short dmScale;
	public short dmCopies;
	public short dmDefaultSource;
	public short dmPrintQuality;
	public short dmColor;
	public short dmDuplex;
	public short dmYResolution;
	public short dmTTOption;
	public short dmCollate;
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string dmFormName;
	public short dmLogPixels;
	public short dmBitsPerPel;
	public int dmPelsWidth;
	public int dmPelsHeight;

	public int dmDisplayFlags;
	public int dmDisplayFrequency;

	public int dmICMMethod;
	public int dmICMIntent;
	public int dmMediaType;
	public int dmDitherType;
	public int dmReserved1;
	public int dmReserved2;

	public int dmPanningWidth;
	public int dmPanningHeight;
};



class User_32 {
	// https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.dllimportattribute?view=netframework-4.5
	[DllImport("user32.dll")]
	public static extern int EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE1 devMode);
	[DllImport("user32.dll")]
	public static extern int ChangeDisplaySettings(ref DEVMODE1 devMode, int flags);

	public const int ENUM_CURRENT_SETTINGS = -1;
	public const int CDS_UPDATEREGISTRY = 0x01;
	public const int CDS_TEST = 0x02;
	public const int DISP_CHANGE_SUCCESSFUL = 0;
	public const int DISP_CHANGE_RESTART = 1;
	public const int DISP_CHANGE_FAILED = -1;
}


namespace Resolution {
	
	public class CResolution {
		public CResolution(int a, int b) {
			Screen screen = Screen.PrimaryScreen;
			int iWidth = a;
			int iHeight = b;

			DEVMODE1 dm = new DEVMODE1();
			dm.dmDeviceName = new String(new char[32]);
			dm.dmFormName = new String(new char[32]);
			dm.dmSize = (short)Marshal.SizeOf(dm);

			if (0 != User_32.EnumDisplaySettings(null, User_32.ENUM_CURRENT_SETTINGS, ref dm)) {				
				dm.dmPelsWidth = iWidth;
				dm.dmPelsHeight = iHeight;

				int iRet = User_32.ChangeDisplaySettings(ref dm, User_32.CDS_TEST);

				if (iRet == User_32.DISP_CHANGE_FAILED) {
					// TODO: throw exception or pass to the caller in non GUI way
					MessageBox.Show("Unable to process your request");
					MessageBox.Show("Description: Unable To Process Your Request. Sorry For This Inconvenience.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
				} else {
					iRet = User_32.ChangeDisplaySettings(ref dm, User_32.CDS_UPDATEREGISTRY);

					switch (iRet) {
						case User_32.DISP_CHANGE_SUCCESSFUL: {
								break;
							}
						case User_32.DISP_CHANGE_RESTART: {
							
								MessageBox.Show("Description: You Need To Reboot For The Change To Happen.\n If You Feel Any Problem After Rebooting Your Machine\nThen Try To Change Resolution In Safe Mode.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
								break;
								//windows 9x series you have to restart
							}
						default: {
								MessageBox.Show("Description: Failed To Change The Resolution.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
								break;
								//failed to change
							}
					}
				}
				
			}
		}
	}
	
		public class AvilableResolutions
	{
		public AvilableResolutions()
		{
			int dMode = -1;
			AvailScrRes = new List<DEVMODE1>();

			DEVMODE1 DM = new DEVMODE1();
			DM.dmSize = (short)Marshal.SizeOf(DM);
			DM.dmFields = (int)(DM_PELSWIDTH | DM_PELSHEIGHT | DM_BITSPERPEL);
			while (User_32.EnumDisplaySettings(null, dMode, ref DM) > 0) {
				AvailScrRes.Add(DM);
				dMode++;
			}
			piAvailableDisplayModes = AvailScrRes.Count;
			
		}
		
		private const long DM_BITSPERPEL = 0x04;
		// &H40000;
		private const long DM_PELSWIDTH = 0x08;
		// & H80000;
		private const long DM_PELSHEIGHT = 0x01;
		// & H100000;

		public List<DEVMODE1> AvailScrRes = null;
		public int piAvailableDisplayModes;

		public long MaxHRes {
			get {
				int iAns = 0;
				int iTest = 0;
				int iCtr = 0;
				for (iCtr = 0; iCtr < AvailScrRes.Count; iCtr++) {
					iTest = AvailScrRes[iCtr].dmPelsWidth;
					if (iTest > iAns) {
						iAns = iTest;
					}
				}
				return iAns;
			}
		}

		public long MaxVRes {
			get {
				int iAns = 0;
				int iTest = 0;
				int iCtr = 0;
				for (iCtr = 0; iCtr < AvailScrRes.Count; iCtr++) {
					iTest = AvailScrRes[iCtr].dmPelsHeight;
					if (iTest > iAns) {
						iAns = iTest;
					}
				}
				return iAns;
			}
		}

		public int AvailableDisplayModes {
			get {
				piAvailableDisplayModes = AvailScrRes.Count;
				return piAvailableDisplayModes;
			}
		}
	}

}

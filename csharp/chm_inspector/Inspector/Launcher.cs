using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Utils;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Drawing;

namespace Program {

	// origin: https://learn.microsoft.com/en-us/answers/questions/1358539/get-chm-title
	public partial class DialogMessage : Form
	{
		// TODO: move into Utils
		public enum HRESULT : int
		{
			S_OK = 0,
			S_FALSE = 1,
			E_NOTIMPL = unchecked((int)0x80004001),
			E_NOINTERFACE = unchecked((int)0x80004002),
			E_POINTER = unchecked((int)0x80004003),
			E_FAIL = unchecked((int)0x80004005),
			E_UNEXPECTED = unchecked((int)0x8000FFFF),
			E_OUTOFMEMORY = unchecked((int)0x8007000E),
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ITS_Control_Data
		{
			public uint cdwControlData;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
			public uint[] adwControlData;
		};

		public enum ECompactionLev
		{
			COMPACT_DATA = 0,
			COMPACT_DATA_AND_PATH
		}

		Guid CLSID_ITStorage = new Guid("5d02926a-212e-11d0-9df9-00a0c922e6ec");

		[ComImport]
		[Guid("88CC31DE-27AB-11D0-9DF9-00A0C922E6EC")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IITStorage
		{
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

		/* Storage instantiation modes */
		public const int STGM_DIRECT = 0x00000000;
		public const int STGM_TRANSACTED = 0x00010000;
		public const int STGM_SIMPLE = 0x08000000;

		public const int STGM_READ = 0x00000000;
		public const int STGM_WRITE = 0x00000001;
		public const int STGM_READWRITE = 0x00000002;

		public const int STGM_SHARE_DENY_NONE = 0x00000040;
		public const int STGM_SHARE_DENY_READ = 0x00000030;
		public const int STGM_SHARE_DENY_WRITE = 0x00000020;
		public const int STGM_SHARE_EXCLUSIVE = 0x00000010;

		public const int STGM_PRIORITY = 0x00040000;
		public const int STGM_DELETEONRELEASE = 0x04000000;

		public const int STGM_NOSCRATCH = 0x00100000;

		public const int STGM_CREATE = 0x00001000;
		public const int STGM_CONVERT = 0x00020000;
		public const int STGM_FAILIFTHERE = 0x00000000;

		public const int STGM_NOSNAPSHOT = 0x00200000;

		public const int STGM_DIRECT_SWMR = 0x00400000;

		[ComImport]
		[Guid("0000000b-0000-0000-C000-000000000046")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IStorage
		{
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
		[Guid("0000000d-0000-0000-C000-000000000046")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IEnumSTATSTG
		{
			// The user needs to allocate an STATSTG array whose size is celt.
			[PreserveSig]
			HRESULT Next(uint celt, [MarshalAs(UnmanagedType.LPArray), Out] System.Runtime.InteropServices.ComTypes.STATSTG[] rgelt, out uint pceltFetched);
			HRESULT Skip(uint celt);
			HRESULT Reset();
			[return: MarshalAs(UnmanagedType.Interface)]
			IEnumSTATSTG Clone();
		}

		public enum STGTY : int
		{
			STGTY_STORAGE = 1,
			STGTY_STREAM = 2,
			STGTY_LOCKBYTES = 3,
			STGTY_PROPERTY = 4
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct LARGE_INTEGER
		{
			[FieldOffset(0)]
			public uint LowPart;
			[FieldOffset(4)]
			public uint HighPart;
			[FieldOffset(0)]
			public long QuadPart;
		}

		private Button button1;
		private Button button2;
		private	String sCHMFile = @"c:\Windows\Help\mui\0409\sqlsodbc.chm";
		private OpenFileDialog openFileDialog1;
		private String initialDirectory = @"C:\";
		private TextBox textBox1;

		[STAThread]
		public static void Main()
		{
			// use GDI
			Application.SetCompatibleTextRenderingDefault(false);

			Application.EnableVisualStyles();
			Application.Run(new DialogMessage());
		}

		public DialogMessage()
		{
			InitializeComponent();
		}

		private void InitializeComponent() {
			this.SuspendLayout();

			openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			openFileDialog1.InitialDirectory = initialDirectory;
			openFileDialog1.RestoreDirectory = true;
			openFileDialog1.Title = "Browse Chm Files";
			openFileDialog1.DefaultExt = "chm";
			openFileDialog1.Filter = "chm files (*.chm)|*.chm|All files (*.*)|*.*";
			openFileDialog1.FilterIndex = 0;
			openFileDialog1.CheckFileExists = true;
			openFileDialog1.CheckPathExists = true;	
			openFileDialog1.Multiselect = false;

			
			button1 = new Button();
			button1.Location = new Point(30, 64);
			button1.Name = "button1";
			button1.Size = new Size(90, 23);
			button1.TabIndex = 1;
			button1.Text = "Open";
			button1.Click += button1_Click;
			Controls.Add(button1);

			button2 = new Button();
			button2.Location = new Point(130, 64);
			button2.Name = "button2";
			button2.Size = new Size(90, 23);
			button2.TabIndex = 2;
			button2.Text = "Inspect";
			button2.Click += button2_Click;
			Controls.Add(button2);

			textBox1 = new TextBox();
			textBox1.Location = new Point(30, 7);
			textBox1.Name = "textBox1";
			textBox1.Top = 30;
			textBox1.Left = 30;
			textBox1.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			Controls.Add(textBox1);
	
			textBox1.Size = new Size(200, 23);
			textBox1.TabIndex = 3;
			textBox1.Text = "";

			this.ResumeLayout(false);
			this.PerformLayout();
		}
	
		private void button1_Click(object sender, EventArgs e)
		{
			var dr = this.openFileDialog1.ShowDialog();
			if (dr == System.Windows.Forms.DialogResult.OK) {
				foreach (String fileName in openFileDialog1.FileNames)
					textBox1.Text = fileName;

			}
		}

	
		private void MakeDataSet() {
			dataSet = new DataSet("DataSet");

			var dataTable = new DataTable("Hosts");

			// Create two columns, and add them to the first table.
			var cHostId = new DataColumn("HostId", typeof(int));
			var chostname = new DataColumn("hostname");
			dataTable.Columns.Add(cHostId);
			dataTable.Columns.Add(chostname);

			// Add the tables to the DataSet.
			dataSet.Tables.Add(dataTable);

			DataRow newRow1;

			for (int i = 1; i < 5; i++) {
				newRow1 = dataTable.NewRow();
				newRow1["HostId"] = i;
				// Add the row to the Hosts table.
				dataTable.Rows.Add(newRow1);
			}
			dataTable.Rows[0]["hostname"] = "host1";
		}

		private void button2_Click(object sender, EventArgs e)
		{        
		
			object oIITStorage = Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_ITStorage, true));
			IITStorage pITStorage = (IITStorage)oIITStorage;
			if (pITStorage != null) {
				IStorage pStorage;
				HRESULT hr = pITStorage.StgOpenStorage(sCHMFile, null, STGM_SHARE_EXCLUSIVE | STGM_READ, IntPtr.Zero, 0, out pStorage);
				if (hr == HRESULT.S_OK) {
					IEnumSTATSTG pEnum;
					hr = pStorage.EnumElements(0, IntPtr.Zero, 0, out pEnum);
					System.Runtime.InteropServices.ComTypes.STATSTG[] ss = new System.Runtime.InteropServices.ComTypes.STATSTG[1];
					uint c;
					while (HRESULT.S_OK == pEnum.Next(1, ss, out /* uint */ c)) {
						if (ss[0].pwcsName == "#SYSTEM") {
							string sTitle = null;
							IStream pStream = null;
							hr = pStorage.OpenStream(ss[0].pwcsName, IntPtr.Zero, STGM_SHARE_EXCLUSIVE | STGM_READ, 0, out pStream);
							if (hr == HRESULT.S_OK) {
								uint nSize = 4;
								byte[] pBuffer = new byte[nSize];
								IntPtr pcbRead = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(uint)));
								pStream.Read(pBuffer, (int)nSize, pcbRead);
								int nRead = Marshal.ReadInt32(pcbRead);
								Marshal.FreeCoTaskMem(pcbRead);
								while (true) {
									nSize = 2;
									pBuffer = new byte[nSize];
									pcbRead = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(uint)));    
									pStream.Read(pBuffer, (int)nSize, pcbRead);
									nRead = Marshal.ReadInt32(pcbRead);
									Marshal.FreeCoTaskMem(pcbRead);
									if (nRead == 0)
										break;
									int nCode = pBuffer[0];

									pBuffer = new byte[nSize];
									pcbRead = IntPtr.Zero;
									pStream.Read(pBuffer, (int)nSize, pcbRead);

									nSize = pBuffer[0];
									pBuffer = new byte[nSize];
									pcbRead = IntPtr.Zero;
									pStream.Read(pBuffer, (int)nSize, pcbRead);
									if (nCode == 3) {
										IntPtr pBytesPtr = Marshal.AllocHGlobal(pBuffer.Length);
										Marshal.Copy(pBuffer, 0, pBytesPtr, pBuffer.Length);
										sTitle = Marshal.PtrToStringAnsi(pBytesPtr);
										Marshal.FreeHGlobal(pBytesPtr);
										break;
									}
								}
								if (sTitle != null)
									System.Windows.Forms.MessageBox.Show("Title = " + sTitle, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

								Marshal.ReleaseComObject(pStream);
							}
						}
					}
					Marshal.ReleaseComObject(pStorage);
				}
				Marshal.ReleaseComObject(pITStorage);
			}
		}
	}

}


using System;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Program
{

	public class Program : Form
	{
		
		[STAThread]
		public static void Main()
		{
			Application.EnableVisualStyles();
			// https://learn.microsoft.com/en-us/dotnet/api/application.setcompatibletextrenderingdefault?view=netframework-4.5
			// NOTE: can only call this method before
			// the first window is created by Windows Forms application
			try {
				Application.SetCompatibleTextRenderingDefault(false);
			} catch (InvalidOperationException) {
			}
			Application.Run(new Program());
		}

		public Program()
		{
			InitializeComponent();
		}
		private IContainer components = null;
 
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		// https://pbdd.org/wp-content/uploads/2015/06/WordPractice2007.docx
		private void InitializeComponent()
		{
			this.SuspendLayout();

			this.Text = "title";


			this.Size = new System.Drawing.Size(700, 450);

			panel = new System.Windows.Forms.Panel();


			p1 = new System.Windows.Forms.Panel();
			b2 = new System.Windows.Forms.Button();
			l1 = new System.Windows.Forms.Label();
			t1 = new System.Windows.Forms.TextBox();
			t = new System.Windows.Forms.Panel();
			p1.SuspendLayout();

			p1.Controls.Add(b2);
			p1.Controls.Add(l1);
			p1.Controls.Add(t1);
			p1.Dock = System.Windows.Forms.DockStyle.Top;
			p1.Location = new System.Drawing.Point(0, 0);
			p1.Name = "panel1";
			p1.Size = new System.Drawing.Size(681, 57);
			p1.TabIndex = 0;

			cb1 = new System.Windows.Forms.CheckBox();
			cb1.Location = new System.Drawing.Point(515, 27);
			cb1.Size = new System.Drawing.Size(120, 20);
			cb1.Text = "Files";

			p1.Controls.Add(cb1);

			b2.Location = new System.Drawing.Point(560, 27);
			b2.Name = "btnDirectory";
			b2.Size = new System.Drawing.Size(60, 21);
			b2.TabIndex = 2;
			b2.Text = "Select";
			// b2.add_click({ if (caller.Data -ne null) { f.Close(); } })

			l1.Location = new System.Drawing.Point(9, 9);
			l1.Name = "label1";
			l1.Size = new System.Drawing.Size(102, 18);
			l1.TabIndex = 1;
			l1.Text = "Selection:";

			t1.Location = new System.Drawing.Point(9, 27);
			t1.Name = "txtDirectory";
			t1.Size = new System.Drawing.Size(503, 20);
			t1.TabIndex = 0;
			t1.Text = "";

			t.Dock = System.Windows.Forms.DockStyle.Fill;
			t.Location = new System.Drawing.Point(0, 57);
			t.Name = "treePanel";
			t.Size = new System.Drawing.Size(621, 130);
			t.TabIndex = 1;

			c = new FileSystemTreeView();

			c.ShowFiles = true;
			c.Dock = System.Windows.Forms.DockStyle.Fill;
			
			c.AfterSelect += new System.Windows.Forms.TreeViewEventHandler((object sender, System.Windows.Forms.TreeViewEventArgs  e) => { /* if (c.Debug) { Write-Host c.Data; } t1.Text = caller.Data = c.Data } */
			});
			c.Load(AppDomain.CurrentDomain.BaseDirectory);
			t.Controls.Add(c);

			cb1.Click +=  new EventHandler((object sender, EventArgs e) => { if (cb1.Checked ) { c.ShowFiles = true; } else { c.ShowFiles = false; } });

			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(621, 427);
			this.Controls.Add(t);
			this.Controls.Add(p1);
			this.Name = "Form1";
			this.Text = "Demo Chooser";
			p1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
			this.Shown += new System.EventHandler((object sender, EventArgs e) =>{ this.Activate(); });
			this.KeyPreview = true;
			this.KeyDown += new System.Windows.Forms.KeyEventHandler((object sender, KeyEventArgs e) => {  
				if ("Escape".Equals(e.KeyCode)) { /* caller.Data = null */
				} else
					return;
				this.Close();
			});
		}
		private CheckBox cb1;
		private Button b2;
		private Panel panel;
		private Panel p1;
		private Panel t;
		private Label l1;
		private TextBox t1;
		private FileSystemTreeView c;
	}
	
	// origin: https://www.codeproject.com/Articles/10834/Filesystem-TreeView
	// https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.treeview?view=netframework-4.5
	// https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.treeview?view=netframework-4.5
	// https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.treenode?view=netframework-4.5
	// https://docs.microsoft.com/en-us/dotnet/api/system.io.directory?view=netframework-4.5
	// see also:
	// https://stackoverflow.com/questions/44477583/c-sharp-access-to-treenode-parameter
	// https://docs.microsoft.com/en-us/dotnet/api/system.io.directoryinfo?view=netframework-4.5
	public class FileSystemTreeView : TreeView
	{
		private TreeNode _selectedNode;
		private bool _showFiles = true;
		public bool ShowFiles {
			get { return this._showFiles; }
			set { this._showFiles = value; }
		}

		private ImageList _imageList = new ImageList();
		private Hashtable _systemIcons = new Hashtable();

		private string _data;
		public String Data {
			get { return this._data; }
			set { this._data = value; }
		}
        private string _iconPath = Path.Combine( Directory.GetCurrentDirectory(), "folder.ico" );

		public String IconPath {
			get { return this._iconPath; }
			set { this._iconPath = value; }
		}

		private bool _debug = false;
		public bool Debug {
			get { return this._debug; }
			set { this._debug = value; }
		}

		public static readonly int Folder = 0;

		public FileSystemTreeView()
		{
			this.ImageList = _imageList;
			this.MouseDown += new MouseEventHandler(FileSystemTreeView_MouseDown);
			this.BeforeExpand += new TreeViewCancelEventHandler(FileSystemTreeView_BeforeExpand);
			this.AfterSelect += new TreeViewEventHandler(FileSystemTreeView_AfterSelect);
		}

		void FileSystemTreeView_MouseDown(object sender, MouseEventArgs e)
		{
			TreeNode node = this.GetNodeAt(e.X, e.Y);
			_selectedNode = node;
			if (node == null)
				return;
			this.SelectedNode = node; //selected the node under the mouse
		}

		void FileSystemTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			if (e.Node is FileNode)
				return;

			DirectoryNode node = (DirectoryNode)e.Node;

			if (!node.Loaded) {
				node.Nodes[0].Remove(); //remove the fake child node used for virtualization
				node.LoadDirectory();
				if (this._showFiles == true)
					node.LoadFiles();
			}
		}
		private void FileSystemTreeView_AfterSelect(System.Object sender, System.Windows.Forms.TreeViewEventArgs e)
		{

			// MessageBox.Show(this._debug.ToString());
			// e.Action = Unknown
			if (_selectedNode != null && _selectedNode.Parent != null) {
				if (_debug)
					MessageBox.Show(String.Format("AfterSelect: {0}", _selectedNode.FullPath.ToString()));
				this._data = _selectedNode.FullPath.ToString();
			}
		}

		public void Load(string directoryPath)
		{
			if (Directory.Exists(directoryPath) == false)
				throw new DirectoryNotFoundException(String.Format("Directory Not Found: {0}", directoryPath));

			_systemIcons.Clear();
			_imageList.Images.Clear();
			Nodes.Clear();

			const string iconBase64 = @"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
AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACGF/wAdgf8AHYH/AB2B/wAegf8AHoH/
AB6B/wAegf8AHoH/AB+B/wAfgf8AH4H/AB+B/wAfgf8AH4H/AB+B/wAfgf8AH4H/AB+B/wAfgf8A
IIH/ACSF/wAAAAAAJIn/O2u4/0Ob6f8vkOb/MI7l/y6M5f8qi+X/KIjl/yeH5f8lhuX/I4Tl/yGD
5f8ef+T/HX7k/xp85f8Ye+T/F3jk/xd45P8XeOT/F3jk/xd45P8Ye+f/DU+2/wAnif8AIYj/fLfr
/1q9//9Ar///P67//z2r//87qf3/N6f9/zWl/f8zov3/MKD9/y2e/f8rm/3/KJn8/yaX/P8kk/3/
IJH8/x+P/P8ejvz/Ho/8/x6P/P8ekv//GHrn/wAmiP8AIo3/fLTo/1q+//9CsPz/Qq78/z+s/P89
qvz/Oqf6/zel+v81o/r/M6D6/zCe+v8tnPr/K5n6/yiX+f8mlfn/I5P6/yCQ+f8ejvn/HYz5/x2M
+f8ejvz/F3rl/wAnjv8AJo//fbbs/1zA//9Hs/z/RbL8/0Ov/P9Brfz/Pav8/zun+v85pfr/NqT6
/zSg+v8wn/r/Lp36/yyb+v8ql/n/Jpb5/ySU+f8hkfn/H4/5/x2N+f8ejvv/F3nm/wAsj/8AKJT/
f7js/2DE//9Jtv3/SbT8/0ay/P9Er/z/Qa38/z6r/P88qPz/Oqb6/zek+v80ovr/Mp/6/y+d+v8t
m/r/Kpj6/yeW+f8llPn/I5H6/yCP+f8fj/v/GHrn/wEulf8AKpj/gLvs/2TG//9Muf3/TLf9/0q1
/P9Is/z/RLH8/0Ku/P8/rPz/Par8/zun+v83pfr/NaP6/zOg+v8wnvr/LZz6/yuZ+v8ol/n/JpX5
/yST+v8hkvv/GXzn/wEwmP8ALZz/g73t/2jJ//9SvP3/ULv9/025/f9Ltv3/SbT8/0Wy/P9Dr/z/
Qa38/z6r/P87qPr/Oab6/zak+v80ovr/MJ/6/y6d+v8sm/r/Kpj5/yaW+f8llvv/HH/o/wMznf8A
MKH/g7/t/2rM//9WwP3/U779/1G8/f9Ouv3/TLf9/0m0/P9Gsvz/RLH8/0Kt/P8+q/z/PKj8/zqm
+v83pPr/NKL6/zKg+v8vnvr/LZv6/yuY+v8nl/v/HIHo/wI2of8AMqX/icPu/2nN//9Xw/3/VsD9
/1TA/f9SvP3/ULr9/0y5/f9Ktfz/SLP8/0Wx/P9Crvz/P6z8/z2q/P87p/r/N6X6/zWj+v8yn/r/
LZ36/yma+v8vnPz/PpPr/wA2pf8ANan/n8vv/53f//9vzv3/YMf9/1PA/f9Sv/3/ULz9/0+6/f9M
uP3/SrX9/0iz/P9Esfz/Qq78/0Cs/P87qvz/Oaf6/zSk+v85pfr/Sav8/2O2/f94wf//W6Ps/wA6
qf8AN63/nsvx/6Hj//+S2v7/k9r+/4/Z/v980f7/cMr9/2DD/f9dwP3/Wb/9/1e8/f9Uuv3/Ubj9
/061/f9guv3/ZL39/2+//f98xf3/eMP9/3O//f90wP//WaTt/wA7rf8AObD/oM3w/6Pl//+T3v7/
kdv+/5Ha/v+P2f7/kNj+/43V/v+M1P7/i9P+/4nR/v+I0f7/hc3+/4TN/v+Byv7/f8n+/3vG/v94
xP7/d8P9/3XB/f92w///XKbu/wA9sP8APrf/oc7x/6bn//+V3v7/lN7+/5Pc/v+Q2v7/jtn+/4zX
/v+L1P7/iNL+/4fR/v+F0P7/g8/+/4HM/v9/y/7/fsn+/3zH/v96xf7/eMT+/3fC/v93xP7/Xanw
/wBBt/8AQLv/otHy/6fp//+X4P7/luD+/5Xf/v+T3P7/k9r+/53h//+b3v//md3//5jc//+X2///
ldj//5PX//+R1v//kNX//47T//+L0v//is7//4jN//+L0P//a7Ly/wBDvP8ARL7/o9Lz/6nr//+a
4v7/meL+/5fg/v+U4P7/pef//6rV9/+by/P/nMr0/5vL9P+byfT/msr0/5nJ9P+XxvT/lcX0/5bF
8/+VxPP/lcXz/5TE8/+WxvP/Zp7k/wBKwP8ARsb/pNT0/6vt//+c5v7/nOT+/5ni/v+b5P//c7/x
/wA8wf8AR8X/AEfF/wBHxf8AR8X/AEfF/wBHxf8ASMX/AEjF/wBIxf8ASMX/AEjF/wBIxf8ASMb/
AEzH/wAAAAAAScn/pNT1/63w//+f5/7/nef+/5vj/v+e5v//e8Xz/wBLyf8AAAAAAAAAAAAAAAAA
AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAATc7/rNz2/8X5
//+u8P//re7//6vt//+s7///is71/wBPzv8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABVdT/U5fm/6zd+P+l2Pf/pdf3/6TV9/+o2fj/
UZXm/wFV1P8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
AAAAAAAAAAAAAAAAAAAAAVnZ/wBV1/8AVNf/AFTX/wBV1/8AVdf/AVrZ/wAAAAAAAAAAAAAAAAAA
AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
AAAAAAAAAAAAAAAAAAD///8AgAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQAAf/8AAH//AAB//wCA//8A////AP///wA=";
			byte[] iconBytes = Convert.FromBase64String(iconBase64);
			var iconStream = new MemoryStream(iconBytes, 0, iconBytes.Length);
			iconStream.Write(iconBytes, 0, iconBytes.Length);
			var iconImage = Image.FromStream(iconStream, true);
			var iconBitmap = new Bitmap(iconStream);
			IntPtr hicon = iconBitmap.GetHicon();
			Icon icon = Icon.FromHandle(hicon);

			Icon folderIcon = icon;


			
			_imageList.Images.Add(folderIcon);
			_systemIcons.Add(FileSystemTreeView.Folder, 0);

			DirectoryNode node = new DirectoryNode(this, new DirectoryInfo(directoryPath));
			node.Expand();
		}

		public int GetIconImageIndex(string path)
		{
			string extension = Path.GetExtension(path);

			if (_systemIcons.ContainsKey(extension) == false) {
				Icon icon = ShellIcon.GetSmallIcon(path);
				_imageList.Images.Add(icon);
				_systemIcons.Add(extension, _imageList.Images.Count - 1);
			}

			return (int)_systemIcons[Path.GetExtension(path)];
		}

	}

	public class DirectoryNode : TreeNode
	{
		private DirectoryInfo _directoryInfo;

		public DirectoryNode(DirectoryNode parent, DirectoryInfo directoryInfo)
			: base(directoryInfo.Name)
		{
			this._directoryInfo = directoryInfo;

			this.ImageIndex = FileSystemTreeView.Folder;
			this.SelectedImageIndex = this.ImageIndex;

			parent.Nodes.Add(this);

			Virtualize();
		}

		public DirectoryNode(FileSystemTreeView treeView, DirectoryInfo directoryInfo)
			: base(directoryInfo.Name)
		{
			this._directoryInfo = directoryInfo;

			this.ImageIndex = FileSystemTreeView.Folder;
			this.SelectedImageIndex = this.ImageIndex;

			treeView.Nodes.Add(this);

			Virtualize();

		}

		void Virtualize()
		{
			int fileCount = 0;

			try {
				if (this.TreeView.ShowFiles == true)
					fileCount = this._directoryInfo.GetFiles().Length;

				if ((fileCount + this._directoryInfo.GetDirectories().Length) > 0)
					new FakeChildNode(this);
			} catch {
			}
		}

		public void LoadDirectory()
		{
			foreach (DirectoryInfo directoryInfo in _directoryInfo.GetDirectories()) {
				new DirectoryNode(this, directoryInfo);
			}
		}

		public void LoadFiles()
		{
			foreach (FileInfo file in _directoryInfo.GetFiles()) {
				new FileNode(this, file);
			}
		}

		public bool Loaded {
			get {
				if (this.Nodes.Count != 0) {
					if (this.Nodes[0] is FakeChildNode)
						return false;
				}
				return true;
			}
		}

		public new FileSystemTreeView TreeView {
			get { return (FileSystemTreeView)base.TreeView; }
		}
	}

	public class FileNode : TreeNode
	{
		private FileInfo _fileInfo;
		private DirectoryNode _directoryNode;

		public FileNode(DirectoryNode directoryNode, FileInfo fileInfo)
			: base(fileInfo.Name)
		{
			this._directoryNode = directoryNode;
			this._fileInfo = fileInfo;

			this.ImageIndex = ((FileSystemTreeView)_directoryNode.TreeView).GetIconImageIndex(_fileInfo.FullName);
			this.SelectedImageIndex = this.ImageIndex;

			_directoryNode.Nodes.Add(this);
		}
	}

	public class FakeChildNode : TreeNode
	{
		public FakeChildNode(TreeNode parent)
			: base()
		{
			parent.Nodes.Add(this);
		}
	}

	public class ShellIcon
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct SHFILEINFO
		{
			public IntPtr hIcon;
			public IntPtr iIcon;
			public uint dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		};

		class Win32 {
			public const uint SHGFI_ICON = 0x100;
			public const uint SHGFI_LARGEICON = 0x0;
			// 'Large icon
			public const uint SHGFI_SMALLICON = 0x1;
			// 'Small icon

			// https://www.pinvoke.net/default.aspx/shell32.shgetfileinfo
			[DllImport("shell32.dll")]
			public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
		}

		public ShellIcon() {
			// TODO: Add constructor logic here
		}

       public static Icon GetSmallIcon(string fileName) {
            IntPtr hImgSmall; //the handle to the system image list
            SHFILEINFO shinfo = new SHFILEINFO();

            //Use this to get the small Icon
            hImgSmall = Win32.SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON);

            //The icon is returned in the hIcon member of the shinfo struct
            // return System.Drawing.Icon.FromHandle(shinfo.hIcon);
			// cannot pass instance property into static methos
			// Icon folderIcon = new Icon(this._iconPath);
			// WOW Exception
			// System.ArgumentException: Argument 'picture' must be a picture that can be used as a Icon.
            // string _iconPath = Path.Combine(Directory.GetCurrentDirectory(), "slide.png");//
			// Icon folderIcon = new Icon(_iconPath);
			// return folderIcon;
			const string iconBase64 = @"iVBORw0KGgoAAAANSUhEUgAAADQAAAAwCAMAAABpN6nPAAAAAXNSR0IArs4c6QAAAARnQU1BAACx
jwv8YQUAAAKOUExURf///x5atR1atfD1/+/0/+vy/e3z/u70/+ju/env/eLr+uDq+ePs+97o+Nvl
+LvP7Nzn+Nrk+NTg9tbi9tXh9s3b8tHf9dHf9MfX8cva8c3b8y1lu0h6xGiQz6e/5ebt/MXW8MfX
8CRguHqf1r/Q7cPU7x1atMDR7nme1kx9xR5btSxluqnA5rrO7L/Q7h1atrXK6qO95aG745244pq2
4Zaz4JWx33uf1lSCyDNqvSZht9Xh9ePs+p+54lSCyYSm2uft/N3o+aK846rC53ae1o+u3Zi04J65
46K85BxbtdLf9WCLzbfL697o+dvl93qd1SBdtoan2q3D59Tg9dvm+LjM7OXu+9He9enw/MjX8cLT
7+vx/b/R7sza8ujv/LjN67PJ6iljuSZguGGLzLTJ6rbK6p254yBcttbh9VqHy67D6LzO7Zez36K7
43GY07vN7U19xXug1py34cjY8SVguJCu3qW+5cnZ8eXu/NPg9bvP7enw/Ul5xWmS0ISl2erw/ZKx
3kR3w5254kd5xLPI6Zq34S9ou7fL6hxatCRfuCJet77Q7cva8uju/LnN7Nbh9i9nu+Hq+t/p+drl
987d873P7cbW8LLI6jJpvEl7xJSx38TU8LPH6kl6xYWm2WiRz2GLzYen2oOl2crZ8idhuKrD5q/E
6SZgt32h11uIy+Dp+r3O7leFyWOOznme1dPf9ShjuWWQz3CY0ytkulyIy6rB5yFdtrbL6yljumeR
z5Kv3sLT7mKNzR5btrHG6KC645Wx3o+u3Hid1VKAxzNqviZhuCJdtjBqvUd5xYGi2L3Q7dTh9s/e
9MbW8cHS7rzO7LfM67LH6Yur3JCv3Zm14J+64xxatcTV7+Ts+wAAAIgFnWMAAADadFJOU///////
////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////8AgwWs3gAAAAlwSFlzAAAO
wwAADsMBx2+oZAAAABl0RVh0U29mdHdhcmUAUGFpbnQuTkVUIDUuMS4xMhMBR3QAAAC4ZVhJZklJ
KgAIAAAABQAaAQUAAQAAAEoAAAAbAQUAAQAAAFIAAAAoAQMAAQAAAAIAAAAxAQIAEQAAAFoAAABp
hwQAAQAAAGwAAAAAAAAAYAAAAAEAAABgAAAAAQAAAFBhaW50Lk5FVCA1LjEuMTIAAAMAAJAHAAQA
AAAwMjMwAaADAAEAAAABAAAABaAEAAEAAACWAAAAAAAAAAIAAQACAAQAAABSOTgAAgAHAAQAAAAw
MTAwAAAAANmnmpXJtwtfAAACz0lEQVRIS52WZ1cTQRSGk/daY0HsgoqxgK4gIDYM9ogIwYZASCJd
AXvBXrBX7F1RUbGDDSv23nvNv/HsTGImAywnvPtl7jP7ZHbvbM4Znc7HOJ1Op8zqjM+SKjBJD9+j
g++WXgc91ZoGMuAEOkCe8KShDDiRpEbCmBpTE7FkYUSSvO5qSgaxZGFEkpoJY2pOLcSShRFJakl+
nqsV+Yslu/xrkFoLY2pDbcWShRFJaieMqT11EEsWRiSpozCmThQgliyMSBICO3fpGuQqupFRmOJh
RJbUdO/BW92TeglTPIzIUnBI7z4K+oaqRRj1E6Z4GIFOHx4R2T9qwMBBg4dEDwXCTDHDhgMjRhpG
jR5jrp6xsePixrtXik/gX7ApxpI4YeIkTCaaQuakGEvi1OSUVD9rms3umJaeYeYrcSkzC9k5udMB
1bGkzMhTQqkrmWUnX5Qy81Aw02q0AcxJnTUbgYY5vC/e8UjxWZg7z2q02QHuWI3zwxcs1JYSUMAc
B+BybIuUwsW0ROgbDyNMWqpkq89mdwQAycuWr1iZZrOHrMJqipMdTpi0BjncSQf4OkWOgLVYR+tl
hxMmbcBG7mQAbid9E6JJ3iSz2dO9zdjCnXyEI5i/8NZgbNPcp+3YYQ8p3rlrt0HqlNY+7UE+W2ev
Zd/+5KADB4sOHcYRNls9/6WjOObtHC/Bibqkkzjl7RSV4rQ6e8bTNlcYYdJZpeycl3P+glKszl6U
HU74F1GKS5cF50o5KtgtV2WHEy5du44biR7nJipN7BZ5k8R9IsrNw63b7mcrx527/He19omIIu6h
7H6V6UFJSWkhKl2O5j6pSXqouJqqVPBn02z5o8dPomKfVj17/uLlq9dv3r577//hY/Gnz1++fvsu
GwB+/PwV+du9Uo3R+j/VGve7SURb+iMDTrSlJBlwoi39lQEn2lItqafk+9mjvicW9yHJl/wDNsMw
p67yspwAAAAASUVORK5CYII=";

			byte[] iconBytes = Convert.FromBase64String(iconBase64);
			var iconStream = new MemoryStream(iconBytes, 0, iconBytes.Length);
			iconStream.Write(iconBytes, 0, iconBytes.Length);
			var iconImage = Image.FromStream(iconStream, true);
			var iconBitmap = new Bitmap(iconStream);
			IntPtr hicon = iconBitmap.GetHicon();
			Icon icon = Icon.FromHandle(hicon);
			return icon;
			

        }

		public static Icon GetLargeIcon(string fileName)
		{
			IntPtr hImgLarge; //the handle to the system image list
			SHFILEINFO shinfo = new SHFILEINFO();

			//Use this to get the large Icon
			hImgLarge = Win32.SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_LARGEICON);

			//The icon is returned in the hIcon member of the shinfo struct
			return System.Drawing.Icon.FromHandle(shinfo.hIcon);
		}
	}

}


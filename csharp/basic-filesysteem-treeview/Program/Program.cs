using System;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace Program {

	public class Program : Form {
		
		[STAThread]
		public static void Main() {
			Application.EnableVisualStyles();
			// https://learn.microsoft.com/en-us/dotnet/api/application.setcompatibletextrenderingdefault?view=netframework-4.5
			try {
				Application.SetCompatibleTextRenderingDefault(false);
			} catch (InvalidOperationException) { }
			Application.Run(new Program());
		}

		public Program() {
			InitializeComponent();
		}
		private IContainer components = null;
 
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		// https://pbdd.org/wp-content/uploads/2015/06/WordPractice2007.docx
		private void InitializeComponent() {
			this.SuspendLayout();

			this.Text = "title";


			this.Size = new Size(700, 450);

			panel = new Panel();


			p1 = new Panel();
			b2 = new Button();
			l1 = new Label();
			t1 = new TextBox();
			t = new Panel();
			p1.SuspendLayout();

			p1.Controls.Add(b2);
			p1.Controls.Add(l1);
			p1.Controls.Add(t1);
			p1.Dock = DockStyle.Top;
			p1.Location = new Point(0, 0);
			p1.Name = "panel1";
			p1.Size = new Size(681, 57);
			p1.TabIndex = 0;

			cb1 = new CheckBox();
			cb1.Location = new Point(515, 27);
			cb1.Size = new Size(120, 20);
			cb1.Text = "Files";

			p1.Controls.Add(cb1);

			b2.Location = new Point(560, 27);
			b2.Name = "btnDirectory";
			b2.Size = new Size(60, 21);
			b2.TabIndex = 2;
			b2.Text = "Select";
			// b2.add_click({ if (caller.Data -ne null) { f.Close(); } })

			l1.Location = new Point(9, 9);
			l1.Name = "label1";
			l1.Size = new Size(102, 18);
			l1.TabIndex = 1;
			l1.Text = "Selection:";

			t1.Location = new Point(9, 27);
			t1.Name = "txtDirectory";
			t1.Size = new Size(503, 20);
			t1.TabIndex = 0;
			t1.Text = "";

			t.Dock = DockStyle.Fill;
			t.Location = new Point(0, 57);
			t.Name = "treePanel";
			t.Size = new Size(621, 130);
			t.TabIndex = 1;

			c = new FileSystemTreeView();

			c.ShowFiles = true;
			c.Dock = DockStyle.Fill;
			
			c.AfterSelect += new TreeViewEventHandler((object sender, TreeViewEventArgs  e) => { /* if (c.Debug) { Write-Host c.Data; } t1.Text = caller.Data = c.Data } */
			});
			c.Load(AppDomain.CurrentDomain.BaseDirectory);
			t.Controls.Add(c);

			cb1.Click +=  new EventHandler((object sender, EventArgs e) => { if (cb1.Checked ) { c.ShowFiles = true; } else { c.ShowFiles = false; } });

			this.AutoScaleBaseSize = new Size(5, 13);
			this.ClientSize = new Size(621, 427);
			this.Controls.Add(t);
			this.Controls.Add(p1);
			this.Name = "Form1";
			this.Text = "Demo Chooser";
			p1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
			this.Shown += new EventHandler((object sender, EventArgs e) =>{ this.Activate(); });
			this.KeyPreview = true;
			// https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.keyeventargs?view=netframework-4.5
			// https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.keys?view=netframework-4.5
			this.KeyDown += new KeyEventHandler((object sender, KeyEventArgs e) => {  
			                                                         	if ("Escape".Equals(e.KeyCode.ToString())) {
			                                                         		// there is no caller
			                                                         		// caller.Data = null;
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
	public class FileSystemTreeView : TreeView {
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

		public FileSystemTreeView() {
			this.ImageList = _imageList;
			this.MouseDown += new MouseEventHandler(FileSystemTreeView_MouseDown);
			this.BeforeExpand += new TreeViewCancelEventHandler(FileSystemTreeView_BeforeExpand);
			this.AfterSelect += new TreeViewEventHandler(FileSystemTreeView_AfterSelect);
		}

		void FileSystemTreeView_MouseDown(object sender, MouseEventArgs e) {
			TreeNode node = this.GetNodeAt(e.X, e.Y);
			_selectedNode = node;
			if (node == null)
				return;
			this.SelectedNode = node; //selected the node under the mouse
		}

		void FileSystemTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e) {
			if (e.Node is FileNode)
				return;

			var node = (DirectoryNode)e.Node;

			if (!node.Loaded) {
				node.Nodes[0].Remove(); //remove the fake child node used for virtualization
				node.LoadDirectory();
				if (this._showFiles == true)
					node.LoadFiles();
			}
		}
		private void FileSystemTreeView_AfterSelect(System.Object sender, TreeViewEventArgs e) {

			// MessageBox.Show(this._debug.ToString());
			// e.Action = Unknown
			if (_selectedNode != null && _selectedNode.Parent != null) {
				if (_debug)
					MessageBox.Show(String.Format("AfterSelect: {0}", _selectedNode.FullPath.ToString()));
				this._data = _selectedNode.FullPath.ToString();
			}
		}

		public void Load(string directoryPath) {
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

			var node = new DirectoryNode(this, new DirectoryInfo(directoryPath));
			node.Expand();
		}

		public int GetIconImageIndex(string path) {
			string extension = Path.GetExtension(path);

			if (_systemIcons.ContainsKey(extension) == false) {
				Icon icon = ShellIcon.GetSmallIcon(path);
				_imageList.Images.Add(icon);
				_systemIcons.Add(extension, _imageList.Images.Count - 1);
			}

			return (int)_systemIcons[Path.GetExtension(path)];
		}

	}

	public class DirectoryNode : TreeNode {
		private DirectoryInfo _directoryInfo;

		public DirectoryNode(DirectoryNode parent, DirectoryInfo directoryInfo)	: base(directoryInfo.Name) {
			this._directoryInfo = directoryInfo;

			this.ImageIndex = FileSystemTreeView.Folder;
			this.SelectedImageIndex = this.ImageIndex;

			parent.Nodes.Add(this);

			Virtualize();
		}

		public DirectoryNode(FileSystemTreeView treeView, DirectoryInfo directoryInfo) : base(directoryInfo.Name) {
			this._directoryInfo = directoryInfo;

			this.ImageIndex = FileSystemTreeView.Folder;
			this.SelectedImageIndex = this.ImageIndex;

			treeView.Nodes.Add(this);

			Virtualize();

		}

		void Virtualize() {
			int fileCount = 0;

			try {
				if (this.TreeView.ShowFiles == true)
					fileCount = this._directoryInfo.GetFiles().Length;

				if ((fileCount + this._directoryInfo.GetDirectories().Length) > 0)
					new FakeChildNode(this);
			} catch {
			}
		}

		public void LoadDirectory() {
			foreach (DirectoryInfo directoryInfo in _directoryInfo.GetDirectories()) {
				new DirectoryNode(this, directoryInfo);
			}
		}

		public void LoadFiles() {
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

	public class FileNode : TreeNode {
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

	public class FakeChildNode : TreeNode {
		public FakeChildNode(TreeNode parent)
			: base()
		{
			parent.Nodes.Add(this);
		}
	}

	public class ShellIcon {
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

	
		private static readonly Dictionary<string, string> dictionary = new Dictionary<string, string> { {  "slide", @"iVBORw0KGgoAAAANSUhEUgAAADQAAAAwCAMAAABpN6nPAAAAAXNSR0IArs4c6QAAAARnQU1BAACx
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
p67yspwAAAAASUVORK5CYII="
			}, {"printersettings",
				@"iVBORw0KGgoAAAANSUhEUgAAADAAAAAwCAYAAABXAvmHAAAACXBIWXMAAAsTAAALEwEAmpwYAAAB
OklEQVR4nO3XQWrCQBQG4DmFIugug/vkAOG9Rc7xJjdw03PkEi4UwVu0Jcueo5UqdOFqJDWCUnRG
TedN7fvh37gw/5eMBJWSSCT/J1W9tSGqBHAmAqgFcF8EUDMBENE2DQXA9noC+LMAIhoR0YKINsYY
y1nab1iWZTm+ZvwH93Dzs6tmmw9gEcFYe6ZzHwD7sTFte68T23+ZHH+2dgK4R5ujNuP7zycA9w+b
e7RxVABGAEYAF3N4jcda5Qr3QBQAPiAgTVOrtbZJkgSp1tpmWdYd4DAep19BmrSIzgDN3fj+wkAA
3fUT4PpHhjcCNrECAODTCQCA5TWAwdPb3a38n8DMBzBGxFVsAAB4z/N8qHxSFMUIEecAsOY+QrDf
MPMefymhAOq30tUREsCtecgjVAlgGw4gkUhUdNkBE3fM1TNxpSEAAAAASUVORK5CYII="
			}, { "chart",
				@"iVBORw0KGgoAAAANSUhEUgAAADAAAAAwCAYAAABXAvmHAAAACXBIWXMAAAsTAAALEwEAmpwYAAAC
TElEQVR4nO2WP2/TQBiHjwUx0ImlWcsEUuI/pXM6tmQgnwCaxLbYurKxdEMqtQ0DilTUMiAlFR+A
5JwYx3GaLgj4AnHis+MtFUgtinToTFxFISlIFbGj3iP9Ng+/5+59bQNAoVBiSUvyMy3R61mS27VE
tAkWDavg+S2pj1uSi5sSssGioK5V7ii8tm88cXCkAmQELNF1/nUEMMA3FBY+VnnNV/kaPlw/OW8U
nEFTdLtNCW2AedMSPBSe4Ke8/UNltW2Z11ZJ0cln5VV4V+G0j6Q4icLX9Ndc/d7cS18UStU5Y8sZ
hgJ6zsYKpwWRWegqLHz/IfP1lSWgfqPQGxyst89VjpTXfHIL0yTnV56rbSgsPD1Mn2Aj55yZguOV
H35WSemg/EhkfMZ1Ishq+2T+QZSoDMzJDPypsOSktdLbdO3W5DO7Dyor5JTNHPoeCjSEnh/pe3i0
gDuk+O/Anb+NQVv0Ny3J61qia/+XJT0W+93xJdxLVrfl5J9L+Px+6abMwnfBqTPaUOWqT0EcOB4T
qG91sMzAIHtM1ZWZcAm9vpHrnR2k2+TUT8n8g7jQHl1xU0DoKPPlhZyqvpFTsBOIsHBiCTtD8ooE
i8BusrLyMlhC52IJTclBYNFohzckomi+lHEli7CZRRhPyyOEDRB3sjPKhwFxJ0sFrrvActE2E0Ub
T8ty0TZiL5CYUT4MFYhaYE0fXJrsVQVu50vmUr6MZ8SIvcDS7PJBqIBOBa4o8OzbpaECiAqUqUCC
CmAqgKlAkQrgSH6nKRQK5XrxC/UtL6QP8cQIAAAAAElFTkSuQmCC"
			}, { "table",
				@"iVBORw0KGgoAAAANSUhEUgAAADAAAAAwCAYAAABXAvmHAAAACXBIWXMAAAsTAAALEwEAmpwYAAAE
YElEQVR4nO2U6VbbRhxHeZaAwawm4RGaPmmbYEgwqw00oU/QpmHzvmu1JC9gwAvLv2dmNNIILUjD
oacfPOfcL/aX+7sz9tTU5EzO5EzO//Ksfe/9unbSLa392QPMCeGDRdfNd3/es3xDdIL5g5A47jRW
vxkfIw94f9Jr+IqeRBENJ7saQOK4XYw8IFLVCKIvya4et10kjtqD6AMY0dH4HkajsS+sJPv50ANb
tg3D4SiQBJGHxCHHALZqkDyCLeglzUqxZYfmZwMfsDz/AI6qjJQXA7bqUdtXnLJyaFCGkQe8edUj
w/39YIi5M3ndgOP2U+iqR1xVHbJerGQMQlrnGHBkPOF/AHPAgKeqWdSnqqf07R1igFnO6LCc1mH5
gGNA4tB44ql6F7ZqRrdEWW5uEXcYLI/RRpwDjGhVM35VB+6qad0SZenfIG4xSwe6SSv6gJVD/ZEt
6yLjBsuxpMOxdMCiefCaAR6iK28mqsHSvpvFfZ4BGf3RJRpWNoyor6zmZE+DhT11HHnAclp79Hur
7Dt1vlUNrvs3Dq6uKX0MK9q76kPv6hrT7T3nChb3WhiuAUtp7YEOYGUR1/1bhyRblIoiiGDfIUqr
IrGuKYrodJ/Tg4XdFmZ+l2PA4oH2QMW8i/pVvfapeuWoiuiYou2OF11mgMIzoPUQuqpZlK3qVZSt
imibokbbjW50YGFHxcS3lfvoA/ZbD/SHFPROo1RFYvYA1ZTtYFkWTW9j5ndUmN9WIZ7iGLCw27q3
q/q/UyQWtSoW21EtUUQLoRkYFdEysDwZIPMMUK0B9Pq93qldlVw3FXUU9aiKUC1ZHRSKqoOsaph4
SsHMbXEMmN9R7+2q3kXp9UevqmAUUxSjaCApLQtRblkD4in5gXMAqcoy78V2EEQ2jkiFYMvJHEbi
GaCMo8kqtmwquuwcRnbzFcExIL6tjN9CNB4oKvvAPYCIOt+p5ninoqw6RAVJgSZCJDREmSDIUBec
srWmiKk2KAJU6jazX2QT8THygLmUPKZiRJTICghJdYiy109F64KEqTUpRJItWzFFy/UmlGuEEqYB
pWoDZr9IJlwDlFFgVYHKSg4pZ1ERKkzVslWViJVM0aJFHQoVm9lNCWIYjgGzW8rIsypTNFLV6vOq
ki1brkHeJFdCVDFYfkOE2IbAMeCrPApXtRlclSmKIEUJOVM0W6xgLhGFMlwg8mVTXoRYUnh61QB6
/V7vtMhW3bSr5h1VqShbVSSyeUQJzhG5IpwhskU4zRZgJikS1nkGbEpDq2o1oGq5Fly1wFYt2VU3
RCx6li1g2dPLAvy8zMPPizz8c5HDkAECzKw3XzHgxaoViG3YVakorpoLqJoUbdnzHPw4z8KPsyz8
jbmEv04vYXpdIHzmGBDbkIZsWQum4HNmKMmXEMyyAmY6iM+E6AOSYulF2YiiMyFEp100YfpTs8Yz
4GMsKdQjVQ0j6ivbxLyjfDL5vVF491v9l8gDJmdyJmdypv6L8y9mzzo9QpytlQAAAABJRU5ErkJg
gg=="},
			{ "layout" , @"iVBORw0KGgoAAAANSUhEUgAAADAAAAAwCAYAAABXAvmHAAAACXBIWXMAAAsTAAALEwEAmpwYAAAE
CUlEQVR4nO3YXWxTZRzH8QfxLRgTlWBfKNvO0wmTzUCC7kKZE7SnJ23ZWEd7uq2sdNghrF3nurbn
rOsbwxcQgiTeoAQJcEGIIAk3OkkggqlksJ4zXtzGqEajVxqNxKiZ42+egySG7JxKDMmzpN/k3G0X
n/ye02crQqVKlZqxcCE95M1F/sKD9pNG4eX5aLYVLmSAPL7hKFRkbZdMr7NPoNlUzzdZBUAe75cR
KEvZRssE++NottT73SDcRnRNJKF6lxNMCW5sfrThUTQb6vg2AbcR3lwEXjzkg9YvemFR0pbTRSyP
INpbORaAfyOU43Q9DbXvt4Kx3yqXp+sfRjS39HIb3IkgL3TN7mZoPhUC04BtqDLEPYRorXLUAzMh
uq/ferFXHfGDUbCeR50rHkA0ZpZ5UEMERkVgBh3AnQiAqZ89htL19yPawjIPWogt40llCe7jTjCK
ltPI5ZqLaAPgIogNF2PKEi8cWA8LResBlE7fh2gDYBVE10QK8FYHOD8LKUvYTm4CvWg9jhCag2gD
YBVE4FK/skToWkr5dFq2xwX6OLsH0QjARY6T63S3ck9wJzrBILB7qQTgIggCqDvYDovfbgSDYMlS
CcAEIakgvs4oC5BPqNVHOsAosikqAab9a7SXKGSgaSgIeNABhphFpA5gEFhgZHdWC1F32Af+izGo
O9QOeoHtow6AlJ9x7yq2xIbhKJi3OUAXt3ZRB0CA5mDZvU8N4bsQu3VXnAqSy+6mQWQDdAFIR11z
scQfnQnx/IdecJ8JK0tsvpqAxW813DQKrB9RBUDkT2/Xg1jmP9E6TisPtkP9YR/UftAybYxZPFQB
SIYLjnmMzJ9TQ4QmU8oSwWspqNqxdnqhuHodoglAKs83PoZlPq+1hOWjjfDMu+tg+Xv8lC5utVMF
UH5P8j6JZfe4GmLzVwMQnEhBcCIJT+9smtJFWQdVANJTo60Yy+7vtZZw/nPR1ex2/qmLW1+iCkAy
5z3VjMT/qIbovNIPGyUBusaTUL2z+Q+9yNVTBSAxeVctI/G/ai2x/nwfVGTssGT72t/0AvccVQCS
Oe9ehSX37zMhfMMxYLJ2cJ0Jw5axAah6x3nDILArqAKQKmRPAyPzU3cieDkCbbmIssRrVxNQtaMJ
zG+suaETLTVUAUhM3uXFknta7Tj5R+JgOfYqbLqSgCXbG3/Si9zSewb4v4/WO9E9Sb79a4GKjOOX
BX2cmUqAFsI/Eodn97ZAhxSHyjcbftAlLAyVgGJLhAsZsB4PQFnG9rM+YSunEoA1EOSeIP9bt5zt
AbzNUVgUfcVIJQBrILoLaWWJjpE4MFvtkwv6bHoqAfg/HKe2XC/5zunc3QDO0oTwfN5DAJ/e1TEq
VapUKXSv+xuIwacaMjC3VAAAAABJRU5ErkJggg=="}
		};
private static readonly string questionmark = @"iVBORw0KGgoAAAANSUhEUgAAADAAAAAwCAYAAABXAvmHAAAACXBIWXMAAAsTAAALEwEAmpwYAAAC
GUlEQVR4nO2ZzUsbQRiHR6FCT568WXBM0OJNRC/Fk/QmHnqtJ69e8gcI0l7Ug4eiu02KSDSHQEqL
H0QQ9KDYU6vVqJUoStqmbWqjMTvZzdeGt8wWs26qokayMzAP/C5ZsrzP7PvODglCAoFAwCQtAajB
MhnBkvITywTsifIDy2SY1nJrAfpF+wonpRm+vYCtK09Kn8SvuzwBYClICMg3T19Qg/fhPGwf6/BN
KcCXuA4LhzlwLaXB6WZYwPGawMRWDr4rhSszf5CDx28YFXAtpS3Ffk0WYCOmQ+TMKvFyLcOmwHIk
XywyfKJDl181Pu+YSsHnmF689iGaZ1NgI6Yb/U4z9jFruTa+ni0KHCUKbArga+INmbOx+VvnS6Dn
rQaHCXMGxj9l+RF46ldhL272P52F1skUHwKdPhVCx2bxu3FzsLkQmAmbu9LOn7sXj+0QaPemjLfw
uUDvnFbW/VClBZ69Uy2rT9/SXAk8mVZhYCVjpH8xXXY7okoL3HdQpQWaPcQ480yGcvB8VuNP4MVa
pjgD9DBHW4orAenC2YemO8DZLtTpU43T6f6pbsg08tZC+J6DhIBsYyQlyrVAg0SGOBVQyvpp8bob
R/BYsgmxDL6yH8meQ9IeIdbBl6/8uvOVUod4AP8/SKuNntNaxAvYUrwSrB+Fh4gnsLkH+9s88ADx
Bv43sG40CNWIRzD9VwSgyu46BAKBADHNXw+6OLRdIRsuAAAAAElFTkSuQmCC";
		public static Icon GetSmallIcon(string fileName) {
			IntPtr hImgSmall; //the handle to the system image list
			SHFILEINFO shinfo = new SHFILEINFO();

			//Use this to get the small Icon
			hImgSmall = Win32.SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON);

			// The icon is returned in the hIcon member of the shinfo struct
            
			// return Icon.FromHandle(shinfo.hIcon);
			// cannot pass instance property into static methos
			// Icon folderIcon = new Icon(this._iconPath);
			// WOW Exception
			// System.ArgumentException: Argument 'picture' must be a picture that can be used as a Icon.
			// string _iconPath = Path.Combine(Directory.GetCurrentDirectory(), "slide.png");//
			// Icon folderIcon = new Icon(_iconPath);
			// return folderIcon;
			var check = "(layout|chart|slide|printerSettings)(?:\\d)+";
			var resultRegex = new Regex(check, RegexOptions.IgnoreCase | RegexOptions.Compiled);
			Match match = resultRegex.Match(Path.GetFileNameWithoutExtension(fileName));
			if (match != null) {
				string key = match.Groups[1].Value.ToLower();
				if (!String.IsNullOrEmpty(key) ) {
				Debug.WriteLine(String.Format("Will use icon {0} for {1}", key, fileName));
				string value = null;
				string iconBase64 =	dictionary.TryGetValue(key, out value) ? value : questionmark;
				byte[] iconBytes = Convert.FromBase64String(iconBase64);
				var iconStream = new MemoryStream(iconBytes, 0, iconBytes.Length);
				iconStream.Write(iconBytes, 0, iconBytes.Length);
				var iconImage = Image.FromStream(iconStream, true);
				var iconBitmap = new Bitmap(iconStream);
				IntPtr hicon = iconBitmap.GetHicon();
				return Icon.FromHandle(hicon);
			} else { 
				Debug.WriteLine(String.Format("Use Shell icon for {0}", fileName));
				return Icon.FromHandle(shinfo.hIcon);
				}
			} else {
			Debug.WriteLine(String.Format("Use Shell icon for {0}", fileName));
				return Icon.FromHandle(shinfo.hIcon);
			}

		}

		public static Icon GetLargeIcon(string fileName) {
			IntPtr hImgLarge; //the handle to the system image list
			SHFILEINFO shinfo = new SHFILEINFO();

			//Use this to get the large Icon
			hImgLarge = Win32.SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_LARGEICON);

			//The icon is returned in the hIcon member of the shinfo struct
			return Icon.FromHandle(shinfo.hIcon);
		}
	}

}


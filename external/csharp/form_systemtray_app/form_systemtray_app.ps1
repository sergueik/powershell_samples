#Copyright (c) 2024 Serguei Kouzmine
#
#Permission is hereby granted, free of charge, to any person obtaining a copy
#of this software and associated documentation files (the "Software"), to deal
#in the Software without restriction, including without limitation the rights
#to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
#copies of the Software, and to permit persons to whom the Software is
#furnished to do so, subject to the following conditions:
#
#The above copyright notice and this permission notice shall be included in
#all copies or substantial portions of the Software.
#
#THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
#IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
#FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
#AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
#LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
#OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
#THE SOFTWARE.


Add-Type -TypeDefinition @"

// "
// intend to call from a Form or from a timer 

using System;
using System.Windows.Forms;
public class Win32Window : IWin32Window {
    private IntPtr _hWnd;
    public IntPtr Handle {
        get { return _hWnd; }
    }

    public Win32Window(IntPtr handle) {
        _hWnd = handle;
    }
}

"@ -ReferencedAssemblies 'System.Windows.Forms.dll','System.Drawing.dll','System.Data.dll'


# http://www.codeproject.com/Articles/290013/Formless-System-Tray-Application

Add-Type -TypeDefinition @"

using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Reflection;


namespace SystemTrayApp
{

	public class ProcessIcon : Form
	{
  
		public ProcessIcon()
		{
			notifyIcon = new NotifyIcon();
		}

		public void Display()
		{
			notifyIcon.MouseClick += new MouseEventHandler(ni_MouseClick);
			notifyIcon.Icon = idle_icon;
			notifyIcon.Text = "text";

			notifyIcon.Visible = true;
			
			notifyIcon.BalloonTipText = "Balloon Tip Text";
			notifyIcon.BalloonTipTitle = "System Tray App";

			notifyIcon.ContextMenuStrip = new ContextMenus().Create();
			myTimer.Tick += new EventHandler(TimerEventProcessor);
			myTimer.Interval = 5000;
			myTimer.Start();
		}

		void ni_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left) {
				Process.Start("explorer", null);
			}
		}

		private void TimerEventProcessor(Object myObject,
			EventArgs myEventArgs)
		{
			myTimer.Stop();
			nScanCounter++;
			Console.Write("{0}\r", nScanCounter.ToString());
			is_busy = !is_busy;
			notifyIcon.Visible = false;

			notifyIcon.Icon = (is_busy) ? busy_icon : idle_icon;
			notifyIcon.Visible = true;

			Thread.Sleep(1000);
			is_busy = !is_busy;
			notifyIcon.Visible = false;
			notifyIcon.Icon = (is_busy) ? busy_icon : idle_icon;
			notifyIcon.Visible = true;
			// restart Timer.
			myTimer.Start();
		}


        public new void Dispose() {
            notifyIcon.Dispose();
        }
		public void DisplayBallonMessage(string message, int delay)
		{
			if (!string.IsNullOrEmpty(message)) {
				notifyIcon.BalloonTipText = message;
			} else {
				notifyIcon.BalloonTipText = "Balloon Tip Text";
				notifyIcon.BalloonTipTitle = "Balloon Tip Title";
			}
			notifyIcon.ShowBalloonTip(delay);
		}
		
		// NOTE: A field initializer cannot reference the non-static field, method, or property 'SystemTrayApp.ProcessIcon.drawIcon(string)' (CS0236) -
		public static Icon drawIcon(string iconBase64)
		{
			var iconBytes = Convert.FromBase64String(iconBase64);
			var iconStream = new MemoryStream(iconBytes, 0, iconBytes.Length);
			iconStream.Write(iconBytes, 0, iconBytes.Length);
			var iconImage = Image.FromStream(iconStream, true);
			var iconBitmap = new Bitmap(iconStream);			
			IntPtr hicon = iconBitmap.GetHicon();
			Icon icon = Icon.FromHandle(hicon);
			return icon;
		}

		NotifyIcon notifyIcon;
		// 'Timer' is an ambiguous reference between
		// 'System.Windows.Forms.Timer' and 'System.Threading.Timer'
		System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
		bool is_busy = false;
		int nScanCounter = 1;
		// static bool exitFlag = false;
		// NOTE:
		// Add-Type : ...\fidmokvi.0.cs(92) : Warning as
		// Error: The field 'SystemTrayApp.ProcessIcon.exitFlag' is assigned but its
		// value is never used
		// https://base64.guru/converter/encode/image/ico
		 
		private Icon idle_icon = drawIcon(@"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//8AAP//AAD//wAA//8AAP//AgL//wIC//+rq///Gxv//wAA//8BAf//AAD//wAA//9P5P//ANb//wPY//8A2P//Adj//wfY//+x8///Gdz//wLX//8C2P//ANj//wDY//8A2P//ANj//wDX//8A3f//ACn//wAA//8AAv//AAD//wAA//8AAP//AAD//wMD//8AAP//SEj//5qa//8AAP//AwP//wAA//8AAP//AAD//0/k//8A1v//A9j//wDY//8D2f//ANX//17m//+R7v//ANX//wXZ//8A2P//ANj//wDY//8A2P//ANf//wDd//8AKf//AAD//wAC//8AAP//AAD//wAA//8BAf//BAT//wIC//+zs///Li7//wEB//8CAv//AAD//wAA//8AAP//T+T//wDW//8D2P//ANj//wDY//8D2P//ANb//6Tx//9V5f//ANT//wXZ//8A2P//ANj//wDY//8A1///AN3//wAp//8AAP//AAL//wAA//8AAP//AQH//wUF//8AAP//h4f//3l5//8AAP//AwP//wAA//8AAP//AAD//wAA//9P5P//ANb//wPY//8A2P//ANj//wHY//8B2P//Etr//7j0//9K4v//ANT//wLY//8D2f//Adj//wDX//8A3f//ACn//wAA//8AAv//AQH//wQE//8AAP//AAD//3h4//+goP//AAD//wMD//8AAP//AAD//wAA//8AAP//AAD//0/k//8A1v//A9j//wDY//8A2P//ANj//wLY//8A1///F9v//7f0//906f//ANf//wDV//8A1///Atf//wTd//8ELP//BAT//wED//8AAP//AAD//xAQ//+Wlv//n5///wQE//8BAf//AQH//wAA//8AAP//AAD//wAA//8AAP//T+T//wDW//8D2P//ANj//wDY//8A2P//ANj//wLY//8A1///B9j//4vt//+w8v//WeX//xHa//8A1f//ANv//wAa//8AAP//AAD//x4e//9zc///t7f//21t//8AAP//AAD//wEB//8AAP//AAD//wAA//8AAP//AAD//wAA//9P5P//ANb//wPY//8A2P//ANj//wDY//8A2P//ANj//wLY//8A2P//ANX//y3e//+K7f//qfL//5vv//+J8P//g5f//4yL//+dnv//qan//3l5//8YGP//AAD//wIC//8BAf//AAD//wAA//8AAP//AAD//wAA//8AAP//AAD//0/k//8A1v//A9j//wDY//8A2P//ANj//wDY//8A2P//ANj//wHY//8D2f//ANb//wDV//8G2f//Ld7//0Tm//9IZf//QD7//yUm//8AAP//AAD//wAA//8DA///AAD//wAA//8AAP//AAD//wAA//8AAP//AAD//wAA//8AAP//T+T//wDW//8D2P//ANj//wDY//8A2P//ANj//wDY//8A2P//ANj//wDY//8C2P//A9n//wDY//8A1f//ANv//wAd//8AAP//AAD//wAA//8EBP//AQH//wAA//8AAP//AAD//wAA//8AAP//AAD//wAA//8AAP//AAD//wAA//9P5P//ANb//wPY//8A2P//ANj//wDY//8A2P//ANj//wDY//8A2P//ANj//wDY//8A2P//ANj//wLX//8C3f//Aiv//wIC//8BA///AAD//wAA//8AAP//AAD//wAA//8AAP//AAD//wAA//8AAP//AAD//wAA//8AAP//AAD//0/k//8A1v//A9j//wDY//8A2P//ANj//wDY//8A2P//ANj//wDY//8A2P//ANj//wDY//8A2P//ANf//wDd//8CK///AwP//wMF//8DA///AwP//wMD//8DA///AwP//wMD//8DA///AwP//wMD//8DA///AwP//wMD//8DA///T+T//wDW//8D2P//ANj//wDY//8A2P//ANj//wDY//8A2P//ANj//wDY//8A2P//ANj//wDY//8A1///AN3//wAf//8AAP//AAD//wAA//8AAP//AAD//wAA//8AAP//AAD//wAA//8AAP//AAD//wAA//8AAP//AAD//wAA//9P5P//ANb//wPY//8A2P//ANj//wDY//8A2P//ANj//wDY//8A2P//ANj//wDY//8A2P//ANj//wDX//8A2///P2j//01N//9NT///TU3//01N//9NTf//TU3//01N//9NTf//TU3//01N//9NTf//TU3//01N//9NTf//TU3//wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=");
		private Icon busy_icon = drawIcon(@"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//8A4///AGX//wAA//8AA///AAH//wAA//8AAP//AAD//wAA//8CAv/8AAD//6ys/7EA2P//ANj//wDY//8A2P//ANf7/wDe//8RrH3/EHgA/w2AA/8OgAH/Dn8B/w58AP8Gs5X/AN3//wDX/P8A2P7/ANj//wDY//8A2P//ANf//wDX//8A4///AGj//wAA//8AAv//AAH//wAA//8AAP//AAD//wIC//wAAP//rKz/sQDY//8A2P//ANj//wDY//8B2P3/ANr+/xLS4v8dihj/DH8D/w5/Af8PfAD/BbSY/wDd//8A1/z/ANj//wDY//8A2P//ANj//wDY//8A2P//ANf//wDX//8A5P//AG3//wAA//8AAv//AAH//wAA//8AAP//AgL//AAA//+srP+xANj//wDY//8A2P//ANj//wDY//8A1vz/AN///yC1nP8UdQD/DYAE/wW2nv8A3v//ANf7/wDY//8A2P//ANj//wDY//8A2P//ANj//wDY//8A2P//ANf//wDX//8A5P//AHH//wAA//8AAP//AAD//wAA//8CAv/8AAD//6ys/7EA2P//ANj//wDY//8A2P//ANj//wDX/v8B2v3/ANHx/wmERv8Fs5b/AOD//wHW+/8A2P//ANj//wDY//8A2P//ANj//wDY//8A2P//ANj//wDY//8A2P//ANf//wDX//8A4v//AHz//wAH//8AAf//AAD//wIC//wAAP//rKz/sQDY//8A2P//ANj//wDY//8A2P//ANj//wDX/v8A3P//ANX+/wC94/8A1ff/ANj//wHY//8A2P//ANj//wDY//8A2P//ANj//wDY//8A2P//ANj//wDY//8A1v//ANP//wDp//8At///AA7//wAB//8AAf//AgL//AAA//+srP+xANj//wDY//8A2P//ANj//wDY//8A2P//ANj//wDX/v8B2///AND0/x3C4f8d3f3/ANf+/wDX//8A2P//ANf//wDX//8A1///ANf//wDV//8A1P//ANb//wDg//8A4///AKT//wAP//8AAP//AAH//wAA//8CAv/8AAD//6ys/7EA2P//ANj//wDY//8A2P//ANj//wDY//8A2P//ANj//wDX/v8A2v//B9j//xvb//8s3v//D9r//wbY//8A2v//AN///wDf//8A3v//AOP//wDp//8A3v//ALj//wBE//8AAP//AAD//wAB//8AAP//AAD//wIC//wAAP//rKz/sQDY//8A2P//ANj//wDY//8A2P//ANj//wDY//8A2P//ANj//wDY//8A2P//ANj//wjZ//8a3P//I9///wXP//8Ivf//BLz//wDB//8BmP//AH3//wA9//8AAP//AAD//wAB//8AAf//AAD//wAA//8AAP//AgL//AAA//+srP+xANj//wDY//8A2P//ANj//wDY//8A2P//ANj//wDY//8A2P//ANj//wDY//8B2P//ANj//wDU//8A5f//AY7//wIC//8BBP//AAP//wAA//8AAP//AAD//wAA//8AAv//AAD//wAA//8AAP//AAD//wAA//8CAv/8AAD//6ys/7EA2P//ANj//wDY//8A2P//ANj//wDY//8A2P//ANj//wDY//8A2P//ANj//wDY//8A2P//AdX//wHm//8Ajf//AAD//wAC//8AAP//AAL//wAE//8AAv//AAD//wAA//8AAP//AAD//wAA//8AAP//AAD//wIC//wAAP//rKz/sQPY//8D2P//A9j//wPY//8D2P//A9j//wPY//8D2P//A9j//wPY//8D2P//A9j//wPY//8D1f//A+b//wOO//8DA///Awb//wMD//8DA///AwP//wMD//8DA///AwP//wMD//8DA///AwP//wMD//8DA///BQX//AMD//+trf+xANb//wDW//8A1v//ANb//wDW//8A1v//ANb//wDW//8A1v//ANb//wDW//8A1v//ANb//wDT//8A5f//AIj//wAA//8AAP//AAD//wAA//8AAP//AAD//wAA//8AAP//AAD//wAA//8AAP//AAD//wAA//8AAP/8AAD//6io/7FL5P//S+T//0vk//9L5P//S+T//0vk//9L5P//S+T//0vk//9L5P//S+T//0vk//9L5P//TeP//0vn//9rz///qKj//6ep//+oqP//qKj//6io//+oqP//qKj//6io//+oqP//qKj//6io//+oqP//qKj//6mp//yoqP//4+P/sQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=");

		
	}

	class ContextMenus
	{
		bool isAboutLoaded = false;

		public ContextMenuStrip Create()
		{
			var menu = new ContextMenuStrip();
			ToolStripMenuItem item;
			ToolStripSeparator sep;

			item = new ToolStripMenuItem();
			item.Text = "Explorer";
			item.Click += new EventHandler(Explorer_Click);
			// TODO: The name 'Resources' does not exist in the current context
			// item.Image = Resources.Explorer;
			menu.Items.Add(item);

			item = new ToolStripMenuItem();
			item.Text = "About";
			item.Click += new EventHandler(About_Click);
			// item.Image = Resources.About;
			menu.Items.Add(item);

			sep = new ToolStripSeparator();
			menu.Items.Add(sep);

			item = new ToolStripMenuItem();
			item.Text = "Exit";
			item.Click += new System.EventHandler(Exit_Click);
			// item.Image = Resources.Exit;
			menu.Items.Add(item);

			return menu;
		}

		void Explorer_Click(object sender, EventArgs e)
		{
			Process.Start("explorer", null);
		}

		void About_Click(object sender, EventArgs e)
		{
			if (!isAboutLoaded) {
				isAboutLoaded = true;
				new AboutBox().ShowDialog();
				isAboutLoaded = false;
			}
		}

		void Exit_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}
	}

	public class AboutBox: Form
	{

		public AboutBox()
		{
			InitializeComponent();
			this.Text = String.Format("About {0}", AssemblyTitle);
			this.labelProductName.Text = AssemblyProduct;
			this.labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
			this.labelCopyright.Text = AssemblyCopyright;
			this.labelCompanyName.Text = AssemblyCompany;
			this.textBoxDescription.Text = AssemblyDescription;
		}

		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
			this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.logoPictureBox = new System.Windows.Forms.PictureBox();
			this.labelProductName = new System.Windows.Forms.Label();
			this.labelVersion = new System.Windows.Forms.Label();
			this.labelCopyright = new System.Windows.Forms.Label();
			this.labelCompanyName = new System.Windows.Forms.Label();
			this.textBoxDescription = new System.Windows.Forms.TextBox();
			this.okButton = new System.Windows.Forms.Button();
			this.tableLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// tableLayoutPanel
			// 
			this.tableLayoutPanel.ColumnCount = 2;
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67F));
			this.tableLayoutPanel.Controls.Add(this.logoPictureBox, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.labelProductName, 1, 0);
			this.tableLayoutPanel.Controls.Add(this.labelVersion, 1, 1);
			this.tableLayoutPanel.Controls.Add(this.labelCopyright, 1, 2);
			this.tableLayoutPanel.Controls.Add(this.labelCompanyName, 1, 3);
			this.tableLayoutPanel.Controls.Add(this.textBoxDescription, 1, 4);
			this.tableLayoutPanel.Controls.Add(this.okButton, 1, 5);
			this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel.Location = new System.Drawing.Point(9, 9);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 6;
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel.Size = new System.Drawing.Size(417, 265);
			this.tableLayoutPanel.TabIndex = 0;
			// 
			// logoPictureBox
			// 
			this.logoPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
			// this.logoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("logoPictureBox.Image")));
			this.logoPictureBox.Location = new System.Drawing.Point(3, 3);
			this.logoPictureBox.Name = "logoPictureBox";
			this.tableLayoutPanel.SetRowSpan(this.logoPictureBox, 6);
			this.logoPictureBox.Size = new System.Drawing.Size(131, 259);
			this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.logoPictureBox.TabIndex = 12;
			this.logoPictureBox.TabStop = false;
			// 
			// labelProductName
			// 
			this.labelProductName.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelProductName.Location = new System.Drawing.Point(143, 0);
			this.labelProductName.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
			this.labelProductName.MaximumSize = new System.Drawing.Size(0, 17);
			this.labelProductName.Name = "labelProductName";
			this.labelProductName.Size = new System.Drawing.Size(271, 17);
			this.labelProductName.TabIndex = 19;
			this.labelProductName.Text = "Product Name";
			this.labelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelVersion
			// 
			this.labelVersion.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelVersion.Location = new System.Drawing.Point(143, 26);
			this.labelVersion.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
			this.labelVersion.MaximumSize = new System.Drawing.Size(0, 17);
			this.labelVersion.Name = "labelVersion";
			this.labelVersion.Size = new System.Drawing.Size(271, 17);
			this.labelVersion.TabIndex = 0;
			this.labelVersion.Text = "Version";
			this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelCopyright
			// 
			this.labelCopyright.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelCopyright.Location = new System.Drawing.Point(143, 52);
			this.labelCopyright.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
			this.labelCopyright.MaximumSize = new System.Drawing.Size(0, 17);
			this.labelCopyright.Name = "labelCopyright";
			this.labelCopyright.Size = new System.Drawing.Size(271, 17);
			this.labelCopyright.TabIndex = 21;
			this.labelCopyright.Text = "Copyright";
			this.labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelCompanyName
			// 
			this.labelCompanyName.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelCompanyName.Location = new System.Drawing.Point(143, 78);
			this.labelCompanyName.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
			this.labelCompanyName.MaximumSize = new System.Drawing.Size(0, 17);
			this.labelCompanyName.Name = "labelCompanyName";
			this.labelCompanyName.Size = new System.Drawing.Size(271, 17);
			this.labelCompanyName.TabIndex = 22;
			this.labelCompanyName.Text = "Company Name";
			this.labelCompanyName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBoxDescription
			// 
			this.textBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBoxDescription.Location = new System.Drawing.Point(143, 107);
			this.textBoxDescription.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
			this.textBoxDescription.Multiline = true;
			this.textBoxDescription.Name = "textBoxDescription";
			this.textBoxDescription.ReadOnly = true;
			this.textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxDescription.Size = new System.Drawing.Size(271, 126);
			this.textBoxDescription.TabIndex = 23;
			this.textBoxDescription.TabStop = false;
			this.textBoxDescription.Text = "Description";
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.okButton.Location = new System.Drawing.Point(339, 239);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 24;
			this.okButton.Text = "&OK";
			// 
			// AboutBox
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(435, 283);
			this.Controls.Add(this.tableLayoutPanel);
			this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			// this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutBox";
			this.Padding = new System.Windows.Forms.Padding(9);
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "About SystemTrayApp";
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
			this.ResumeLayout(false);

		}


		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
		private System.Windows.Forms.PictureBox logoPictureBox;
		private System.Windows.Forms.Label labelProductName;
		private System.Windows.Forms.Label labelVersion;
		private System.Windows.Forms.Label labelCopyright;
		private System.Windows.Forms.Label labelCompanyName;
		private System.Windows.Forms.TextBox textBoxDescription;
		private System.Windows.Forms.Button okButton;

		public string AssemblyTitle {
			get {
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
				if (attributes.Length > 0) {
					AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
					if (titleAttribute.Title != "") {
						return titleAttribute.Title;
					}
				}
				return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
			}
		}

		public string AssemblyVersion {
			get {
				return Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
		}

		public string AssemblyDescription {
			get {
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
				if (attributes.Length == 0) {
					return "";
				}
				return ((AssemblyDescriptionAttribute)attributes[0]).Description;
			}
		}

		public string AssemblyProduct {
			get {
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
				if (attributes.Length == 0) {
					return "";
				}
				return ((AssemblyProductAttribute)attributes[0]).Product;
			}
		}

		public string AssemblyCopyright {
			get {
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
				if (attributes.Length == 0) {
					return "";
				}
				return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
			}
		}

		public string AssemblyCompany {
			get {
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
				if (attributes.Length == 0) {
					return "";
				}
				return ((AssemblyCompanyAttribute)attributes[0]).Company;
			}
		}
	}
	public static class Program
	{
		[STAThread]
		public static void Main()
		{
			using (var processIcon = new ProcessIcon()) {
				processIcon.Visible = false;
				processIcon.Display();
				processIcon.DisplayBallonMessage(null, 3000);
				var context = new ApplicationContext();
				Application.Run(context);
			}
		}
	}
}
"@ -ReferencedAssemblies 'System.Windows.Forms.dll', 'System.Drawing.dll', 'Microsoft.CSharp.dll' , 'System.Xml.Linq.dll', 'System.Xml.dll'


$owner = New-Object Win32Window -ArgumentList ([System.Diagnostics.Process]::GetCurrentProcess().MainWindowHandle)

# $i = new-object -typeName 'SystemTrayApp.ProcessIcon'
# $i.Display()

$p = [SystemTrayApp.Program]::Main()



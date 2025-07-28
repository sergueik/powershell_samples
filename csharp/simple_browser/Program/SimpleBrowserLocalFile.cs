using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Utils;

public class SimpleBrowserLocalFile : Form {
	private String localFile = @"file://c:\developer\sergueik\powershell_ui_samples\test.html";
	// TODO: suppress warning CS0414:
	// because add-Type :  Warning as Error
	private StatusStrip statusStrip1;
	private ToolStripProgressBar toolStripProgressBar1;
	private WebBrowser webBrowser1;

	[STAThread]
	public static void Main() {
		Application.EnableVisualStyles();
		Application.Run(new SimpleBrowserLocalFile());
	}

		
	private void webBrowser1_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e) {
		toolStripProgressBar1.Maximum = (int)e.MaximumProgress;
		toolStripProgressBar1.Value = (int)e.CurrentProgress;
	}

	private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
		toolStripProgressBar1.Value = toolStripProgressBar1.Maximum;     
	}
	
	public SimpleBrowserLocalFile() {
		statusStrip1 = new StatusStrip();
		toolStripProgressBar1 = new ToolStripProgressBar();
		webBrowser1 = new WebBrowser();
		statusStrip1.SuspendLayout();
		SuspendLayout();

		statusStrip1.Items.AddRange(new ToolStripItem[] {
			toolStripProgressBar1
		});
		statusStrip1.LayoutStyle = ToolStripLayoutStyle.Table;
		statusStrip1.Location = new System.Drawing.Point(0, 488);
		statusStrip1.Name = "statusStrip1";
		statusStrip1.Size = new System.Drawing.Size(695, 22);
		statusStrip1.TabIndex = 0;
		statusStrip1.Text = "statusStrip1";

		toolStripProgressBar1.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
		toolStripProgressBar1.Name = "toolStripProgressBar1";
		toolStripProgressBar1.Size = new System.Drawing.Size(100, 15);
		toolStripProgressBar1.Text = "toolStripProgressBar1";


		webBrowser1.Dock = DockStyle.Fill;
		webBrowser1.Location = new System.Drawing.Point(0, 0);
		webBrowser1.Name = "webBrowser1";
		webBrowser1.Size = new System.Drawing.Size(695, 488);
 
		Console.Error.WriteLine("Loading uri: " + localFile);
		try {
			webBrowser1.Url = new System.Uri(localFile, System.UriKind.Absolute);
			// https://stackoverflow.com/questions/17926197/open-local-file-in-system-windows-forms-webbrowser-control
			// webBrowser1.DocumentText = pageContent;
			String html = "<h1>test</h1>";
			string fullHtml = String.Format(@"<html><head><meta charset='utf-8'></head><body>{0}</body></html>",html);
			Helper helper = new Helper();
			html = helper.convert();
			webBrowser1.DocumentText = html;
		} catch (UriFormatException e) {
			Console.Error.WriteLine(e.ToString());
			return;
		} catch (NullReferenceException e) {
			Console.Error.WriteLine(e.ToString());
			return;
		}
		webBrowser1.ProgressChanged += new WebBrowserProgressChangedEventHandler(webBrowser1_ProgressChanged);
		webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_DocumentCompleted);

		AutoScaleDimensions = new SizeF(6F, 13F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(695, 510);
		Controls.Add(webBrowser1);
		Controls.Add(statusStrip1);
		Name = "Form1";
		Text = "Form1";
		statusStrip1.ResumeLayout(false);
		ResumeLayout(false);
		PerformLayout();
	}

	// https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.webbrowser?view=netframework-4.0
	// Navigates to the given URL if it is valid.
	private void Navigate(String address) {
		// TODO: better handle "relative URL"
		var prefix = "file://";
		if (String.IsNullOrEmpty(address))
			return;
		if (address.Equals("about:blank"))
			return;
		if (!address.StartsWith(prefix)) {
			address = prefix + address;
		}
		try {
			webBrowser1.Navigate(new Uri(address));
		} catch (System.UriFormatException) {
			return;
		}
	}

}


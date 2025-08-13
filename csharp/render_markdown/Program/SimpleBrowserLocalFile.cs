using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Utils;

public class SimpleBrowserLocalFile : Form
{
	private String localFile = @"file://c:\developer\sergueik\powershell_ui_samples\test.html";
	// TODO: suppress warning CS0414:
	// because add-Type :  Warning as Error
	private StatusStrip statusStrip1;
	private ToolStripProgressBar toolStripProgressBar1;
	private IMarkdownConverter markdownConverter;
	private WebBrowser webBrowser;

	[STAThread]
	public static void Main()
	{
		Application.EnableVisualStyles();
		Application.Run(new SimpleBrowserLocalFile());
	}

		
	private void progressChanged(object sender, WebBrowserProgressChangedEventArgs e)
	{
		toolStripProgressBar1.Maximum = (int)e.MaximumProgress;
		toolStripProgressBar1.Value = (int)e.CurrentProgress;
	}

	private void documentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
	{
		toolStripProgressBar1.Value = toolStripProgressBar1.Maximum;     
	}
	FileSystemWatcher watcher;
	public SimpleBrowserLocalFile()
	{
		statusStrip1 = new StatusStrip();
		toolStripProgressBar1 = new ToolStripProgressBar();
		// https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/webbrowser-control-overview
		webBrowser = new WebBrowser();
		// webBrowser.DocumentText = convertMarkdownToHtml(filePath);
		
		statusStrip1.SuspendLayout();
		SuspendLayout();

		statusStrip1.Items.AddRange(new ToolStripItem[] {
			toolStripProgressBar1
		});
		statusStrip1.LayoutStyle = ToolStripLayoutStyle.Table;
		statusStrip1.Location = new Point(0, 488);
		statusStrip1.Name = "statusStrip1";
		statusStrip1.Size = new Size(695, 22);
		statusStrip1.TabIndex = 0;
		statusStrip1.Text = "statusStrip1";

		toolStripProgressBar1.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
		toolStripProgressBar1.Name = "toolStripProgressBar1";
		toolStripProgressBar1.Size = new Size(100, 15);
		toolStripProgressBar1.Text = "toolStripProgressBar1";

		webBrowser.Dock = DockStyle.Fill;
		webBrowser.Location = new Point(0, 0);
		webBrowser.Name = "webBrowser1";
		webBrowser.Size = new Size(695, 488);
 

string localFilePath = new Uri(localFile).LocalPath; // -> C:\developer\sergueik\powershell_ui_samples\test.html

// Then use FileSystemWatcher safely
var directory = Path.GetDirectoryName(localFilePath);
var fileName = Path.GetFileName(localFilePath);

if (Directory.Exists(directory))
{		
	watcher = new FileSystemWatcher(directory, fileName);
		watcher.NotifyFilter = NotifyFilters.LastWrite;
		watcher.Changed += (s, e) => {
			// Small delay to allow file write completion
			System.Threading.Thread.Sleep(200);
			webBrowser.Invoke(new Action(() => {
				webBrowser.DocumentText = convertMarkdownToHtml(localFile);
			}));
		};
		watcher.EnableRaisingEvents = true;
		Console.Error.WriteLine("Monitoring dir: " + directory);
		
}
		Console.Error.WriteLine("Loading uri: " + localFile);
		try {
			// TODO: about:blank
			webBrowser.Url = new Uri(localFile, System.UriKind.Absolute);
			// https://stackoverflow.com/questions/17926197/open-local-file-in-system-windows-forms-webbrowser-control
			// webBrowser.DocumentText = pageContent;
			// var markdownConverter = new MarkdownConverter();
			markdownConverter = new MarkdownConverter();
			var html = markdownConverter.convert();
			webBrowser.DocumentText = html;
		} catch (UriFormatException e) {
			Console.Error.WriteLine(e.ToString());
			return;
		} catch (NullReferenceException e) {
			Console.Error.WriteLine(e.ToString());
			return;
		}
		webBrowser.ProgressChanged += new WebBrowserProgressChangedEventHandler(progressChanged);
		webBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(documentCompleted);

		AutoScaleDimensions = new SizeF(6F, 13F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(695, 510);
		Controls.Add(webBrowser);
		Controls.Add(statusStrip1);
		Name = "Form1";
		Text = "Form1";
		statusStrip1.ResumeLayout(false);
		ResumeLayout(false);
		PerformLayout();
	}

	
	public string convertMarkdownToHtml()
	{
		return markdownConverter.convert();
	}

	public string convertMarkdownToHtml(string payload)
	{
		return markdownConverter.convert(payload);
	}

	public string ConvertMarkdownFileToHtml(string filePath)
	{
		return markdownConverter.convertFile(filePath);
	}



	private void ReloadMarkdown()
	{
		string content = File.ReadAllText(localFile);
		webBrowser.DocumentText = markdownConverter.convert(content);
	}
	
	// https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.webbrowser?view=netframework-4.0
	// Navigates to the given URL if it is valid.
	private void Navigate(String address)
	{
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
			webBrowser.Navigate(new Uri(address));
		} catch (UriFormatException) {
			return;
		}
	}

}


using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace HowToRunPowerShell {
	public partial class FormPowerShellSample : Form {
		// Represents the kind of drag drop formats we want to receive
		private const string dragDropFormat = "FileDrop";

		public FormPowerShellSample() {
			InitializeComponent();
		}

		private void buttonRunScript_Click(object sender, EventArgs e) {
			try {
				textBoxOutput.Clear();
				textBoxOutput.Text = RunScript(textBoxScript.Text);
			} catch (Exception error) {
				textBoxOutput.Text += String.Format("\r\nError in script : {0}\r\n", error.Message);
			}
		}

		private string RunScript(string scriptText) {
			// https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.runspaces.runspace?view=powershellsdk-1.1.0
			var runspace = RunspaceFactory.CreateRunspace();

			// https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.runspaces.runspace.open?view=powershellsdk-1.1.0
			runspace.Open();

			// https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.runspaces.pipeline?view=powershellsdk-1.1.0
            
			var pipeline = runspace.CreatePipeline();
			// https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.runspaces.commandcollection?view=powershellsdk-7.4.0
			pipeline.Commands.AddScript(scriptText);

			// https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.runspaces.command?view=powershellsdk-1.1.0
			// add an extra command to transform the script output objects into nicely formatted strings
			// remove this line to get the actual objects that the script returns. For example, the script
			// "Get-Process" returns a collection of System.Diagnostics.Process instances.
			pipeline.Commands.Add("Out-String");

			// https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.psobject?view=powershellsdk-1.1.0
			// https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.runspaces.pipeline.invoke?view=powershellsdk-1.1.0
			var results = pipeline.Invoke();

			// https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.runspaces.runspace.close?view=powershellsdk-1.1.0
			runspace.Close();

			// convert the script result into a single string
			StringBuilder stringBuilder = new StringBuilder();
			foreach (var PSObject in results) {
				stringBuilder.AppendLine(PSObject.ToString());
			}

			return stringBuilder.ToString();
		}

		#region Drag-drop handling events
		private void FormPowerShellSample_DragDrop(object sender, DragEventArgs e)
		{
			// is it the correct type of data?
			if (e.Data.GetDataPresent(dragDropFormat)) {
				// dragging files onto the window yields an array of pathnames
				string[] files = (string[])e.Data.GetData(dragDropFormat);

				if (files.Length > 0) {
					// just read the first file
					using (StreamReader sr = new StreamReader(files[0])) {
						// and plunk the contents in the textbox
						textBoxScript.Text = sr.ReadToEnd();
					}
				}
			}
		}

		private void FormPowerShellSample_DragEnter(object sender, DragEventArgs e)
		{
			// only accept the dropped data if it has the correct format
			e.Effect = e.Data.GetDataPresent(dragDropFormat) ? DragDropEffects.Link : DragDropEffects.None;
		}
		#endregion
	}
}
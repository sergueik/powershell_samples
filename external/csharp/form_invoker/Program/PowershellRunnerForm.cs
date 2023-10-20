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

namespace PowershellRunner {
    public partial class PowershellRunnerForm : Form {

		// Represents the kind of drag drop formats we want to receive
        private const string dragDropFormat = "FileDrop";

        public PowershellRunnerForm() {
            InitializeComponent();
        }

        private void buttonRunScript_Click(object sender, EventArgs eventArgs) {
            try {
                textBoxOutput.Clear();
                textBoxOutput.Text = RunScript(textBoxScript.Text);
            } catch (Exception e) {
                textBoxOutput.Text += String.Format("\r\nError in script : {0}\r\n", e.Message);
            }
        }

        private string RunScript(string scriptText) {
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
            Pipeline pipeline = runspace.CreatePipeline();
            pipeline.Commands.AddScript(scriptText);

            // add an extra command to transform the script output objects into nicely formatted strings
            // remove this line to get the actual objects that the script returns. For example, the script
            // "Get-Process" returns a collection of System.Diagnostics.Process instances.
            pipeline.Commands.Add("Out-String");

            Collection<PSObject> results = pipeline.Invoke();

            runspace.Close();

            var stringBuilder = new StringBuilder();
            foreach (PSObject obj in results) {
                stringBuilder.AppendLine(obj.ToString());
            }
            return stringBuilder.ToString();
        }

        #region Drag-drop handling events
        private void FormPowerShellSample_DragDrop(object sender, DragEventArgs e)  {
            // is it the correct type of data?
            if (e.Data.GetDataPresent(dragDropFormat)) {
                // dragging files onto the window yields an array of pathnames
                var files = (string[])e.Data.GetData(dragDropFormat);

                if (files.Length > 0) {
                    // just read the first file
                    using (StreamReader sr = new StreamReader(files[0])) {
                        // and plunk the contents in the textbox
                        textBoxScript.Text = sr.ReadToEnd();
                    }
                }
            }
        }

        private void FormPowerShellSample_DragEnter(object sender, DragEventArgs e) {
            // only accept the dropped data if it has the correct format
            e.Effect = e.Data.GetDataPresent(dragDropFormat) ? DragDropEffects.Link : DragDropEffects.None;
        }
        #endregion
    }
}
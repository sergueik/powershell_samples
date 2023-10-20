using System.Configuration;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using Utils;

namespace PowershellRunner {
    partial class PowershellRunnerForm  {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent() {
            splitContainer1 = new SplitContainer();
            groupBox1 = new GroupBox();
            textBoxScript = new TextBox();
            panel1 = new Panel();
            buttonRunScript = new Button();
            groupBox2 = new GroupBox();
            textBoxOutput = new TextBox();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            groupBox1.SuspendLayout();
            panel1.SuspendLayout();
            groupBox2.SuspendLayout();
            this.SuspendLayout();

         	splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;

            splitContainer1.Panel1.Controls.Add(groupBox1);

            splitContainer1.Panel2.Controls.Add(groupBox2);
            splitContainer1.Size = new Size(557, 369);
            splitContainer1.SplitterDistance = 209;
            splitContainer1.TabIndex = 0;

            groupBox1.Controls.Add(textBoxScript);
            groupBox1.Controls.Add(panel1);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new Point(0, 0);
            groupBox1.Margin = new Padding(8, 8, 8, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(8);
            groupBox1.Size = new Size(557, 209);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Script";

            textBoxScript.Dock = DockStyle.Fill;
            textBoxScript.Location = new Point(8, 21);
            textBoxScript.MaxLength = 0;
            textBoxScript.Multiline = true;
            textBoxScript.Name = "textBoxScript";
            this.textBoxScript.ScrollBars = ScrollBars.Both;
            textBoxScript.Size = new Size(541, 143);
            textBoxScript.TabIndex = 0;
            
            var s = ConfigurationManager.GetSection("script") as Utils.ScriptElement;

            textBoxScript.Text = s.Source.Value; 
            textBoxScript.WordWrap = false;

            panel1.Controls.Add(buttonRunScript);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(8, 164);
            panel1.Name = "panel1";
            panel1.Size = new Size(541, 37);
            panel1.TabIndex = 2;

            buttonRunScript.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Bottom)
                        | AnchorStyles.Right)));
            buttonRunScript.Location = new Point(446, 6);
            buttonRunScript.Name = "buttonRunScript";
            buttonRunScript.Size = new Size(95, 31);
            buttonRunScript.TabIndex = 1;
            buttonRunScript.Text = "&Run Script";
            buttonRunScript.UseVisualStyleBackColor = true;
            buttonRunScript.Click += new System.EventHandler(buttonRunScript_Click);

            groupBox2.Controls.Add(this.textBoxOutput);
            groupBox2.Dock = DockStyle.Fill;
            groupBox2.Location = new Point(0, 0);
            groupBox2.Margin = new Padding(8);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(8);
            groupBox2.Size = new Size(557, 156);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Output";

            textBoxOutput.Dock = DockStyle.Fill;
            textBoxOutput.Font = new Font("Lucida Console", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            textBoxOutput.ForeColor = SystemColors.WindowText;
            textBoxOutput.Location = new Point(8, 21);
            textBoxOutput.MaxLength = 0;
            textBoxOutput.Multiline = true;
            textBoxOutput.Name = "textBoxOutput";
            textBoxOutput.ScrollBars = ScrollBars.Both;
            textBoxOutput.Size = new Size(541, 127);
            textBoxOutput.TabIndex = 0;
            textBoxOutput.WordWrap = false;

            this.AllowDrop = true;
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(557, 369);
            this.Controls.Add(splitContainer1);
            this.Name = "FormPowerShellSample";
            this.Text = "PowerShellScripting";
            this.DragDrop += new DragEventHandler(this.FormPowerShellSample_DragDrop);
            this.DragEnter += new DragEventHandler(this.FormPowerShellSample_DragEnter);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            panel1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private SplitContainer splitContainer1;
        private GroupBox groupBox1;
        private TextBox textBoxScript;
        private Panel panel1;
        private Button buttonRunScript;
        private GroupBox groupBox2;
        private TextBox textBoxOutput;
        
    }
}


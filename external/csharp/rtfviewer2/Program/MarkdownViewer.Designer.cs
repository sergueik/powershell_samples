using System;
using System.Drawing;
using System.Windows.Forms;


namespace Program {
    partial class MarkdownViewer {
        private System.ComponentModel.IContainer components = null;

        private Button buttonLoad;
        private Button buttonSave;
        private RichTextBox richTextBoxRtfView;
        private Button button1;
        private RichTextBox richTextBoxRtfCode;
        private Panel panel1;
        private CheckBox checkBoxShowRtfCode;
        private CheckBox checkBoxShowSourceMd;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private TextBox textBoxSourceMd;
        private CheckBox checkBoxLiveUpdate;
        private Timer timerUpdate;
        private Button buttonSaveMd;
        private Button button6;
        private Button button5;

        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent() {
        	components = new System.ComponentModel.Container();
        	System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MarkdownViewer));
        	buttonLoad = new Button();
        	buttonSave = new Button();
        	richTextBoxRtfView = new RichTextBox();
        	button1 = new Button();
        	richTextBoxRtfCode = new RichTextBox();
        	panel1 = new Panel();
        	button6 = new Button();
        	button5 = new Button();
        	buttonSaveMd = new Button();
        	checkBoxLiveUpdate = new CheckBox();
        	checkBoxShowRtfCode = new CheckBox();
        	checkBoxShowSourceMd = new CheckBox();
        	splitContainer1 = new SplitContainer();
        	textBoxSourceMd = new TextBox();
        	splitContainer2 = new SplitContainer();
        	timerUpdate = new Timer(components);
        	panel1.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(splitContainer1)).BeginInit();
        	splitContainer1.Panel1.SuspendLayout();
        	splitContainer1.Panel2.SuspendLayout();
        	splitContainer1.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(splitContainer2)).BeginInit();
        	splitContainer2.Panel1.SuspendLayout();
        	splitContainer2.Panel2.SuspendLayout();
        	splitContainer2.SuspendLayout();
        	SuspendLayout();
        	//
        	// buttonLoad
        	//
        	buttonLoad.Location = new Point(5, 5);
        	buttonLoad.Margin = new Padding(5);
        	buttonLoad.Name = "buttonLoad";
        	buttonLoad.Size = new Size(118, 37);
        	buttonLoad.TabIndex = 0;
        	buttonLoad.Text = "Load";
        	buttonLoad.UseVisualStyleBackColor = true;
        	//
        	// buttonSave
        	//
        	buttonSave.Location = new Point(449, 5);
        	buttonSave.Margin = new Padding(5);
        	buttonSave.Name = "buttonSave";
        	buttonSave.Size = new Size(118, 37);
        	buttonSave.TabIndex = 1;
        	buttonSave.Text = "Save RTF";
        	buttonSave.UseVisualStyleBackColor = true;
        	//
        	// richTextBoxRtfView
        	//
        	richTextBoxRtfView.AcceptsTab = true;
        	richTextBoxRtfView.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right)));
        	richTextBoxRtfView.BackColor = Color.White;
        	richTextBoxRtfView.ImeMode = ImeMode.NoControl;
        	richTextBoxRtfView.Location = new Point(0, 8);
        	richTextBoxRtfView.Margin = new Padding(5);
        	richTextBoxRtfView.Name = "richTextBoxRtfView";
        	richTextBoxRtfView.ScrollBars = RichTextBoxScrollBars.Both;

        	richTextBoxRtfView.Dock = DockStyle.Fill;
        	richTextBoxRtfView.Multiline = true;
        	richTextBoxRtfView.WordWrap = false;

        	richTextBoxRtfView.Size = new Size(843, 1134);
        	richTextBoxRtfView.TabIndex = 3;
        	richTextBoxRtfView.Text = "";
        	//
        	// button1
        	//
        	button1.Location = new Point(132, 5);
        	button1.Margin = new Padding(5);
        	button1.Name = "button1";
        	button1.Size = new Size(118, 37);
        	button1.TabIndex = 4;
        	button1.Text = "Update";
        	button1.UseVisualStyleBackColor = true;
        	//
        	// richTextBoxRtfCode
        	//
        	richTextBoxRtfCode.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right)));
        	richTextBoxRtfCode.Location = new Point(0, 8);
        	richTextBoxRtfCode.Margin = new Padding(5);
        	richTextBoxRtfCode.Name = "richTextBoxRtfCode";
        	richTextBoxRtfCode.ReadOnly = true;
        	richTextBoxRtfCode.Size = new Size(582, 1134);
        	richTextBoxRtfCode.TabIndex = 6;
        	richTextBoxRtfCode.Text = "";
        	//
        	// panel1
        	//
        	panel1.Controls.Add(button6);
        	panel1.Controls.Add(button5);
        	panel1.Controls.Add(buttonSaveMd);
        	panel1.Controls.Add(checkBoxLiveUpdate);
        	panel1.Controls.Add(checkBoxShowRtfCode);
        	panel1.Controls.Add(checkBoxShowSourceMd);
        	panel1.Controls.Add(buttonLoad);
        	panel1.Controls.Add(buttonSave);
        	panel1.Controls.Add(button1);
        	panel1.Dock = DockStyle.Top;
        	panel1.Location = new Point(0, 0);
        	panel1.Margin = new Padding(5);
        	panel1.Name = "panel1";
        	panel1.Size = new Size(1955, 48);
        	panel1.TabIndex = 8;
        	//
        	// button6
        	//
        	button6.Location = new Point(1155, 6);
        	button6.Margin = new Padding(5);
        	button6.Name = "button6";
        	button6.Size = new Size(68, 37);
        	button6.TabIndex = 10;
        	button6.Text = ">";
        	button6.UseVisualStyleBackColor = true;
        	button6.Click += new System.EventHandler(button6_Click);
        	//
        	// button5
        	//
        	button5.Location = new Point(1077, 6);
        	button5.Margin = new Padding(5);
        	button5.Name = "button5";
        	button5.Size = new Size(68, 37);
        	button5.TabIndex = 9;
        	button5.Text = "<";
        	button5.UseVisualStyleBackColor = true;
        	button5.Click += new System.EventHandler(button5_Click);
        	//
        	// buttonSaveMd
        	//
        	buttonSaveMd.Location = new Point(259, 5);
        	buttonSaveMd.Margin = new Padding(5);
        	buttonSaveMd.Name = "buttonSaveMd";
        	buttonSaveMd.Size = new Size(181, 37);
        	buttonSaveMd.TabIndex = 8;
        	buttonSaveMd.Text = "Save Markdown";
        	buttonSaveMd.UseVisualStyleBackColor = true;
        	//
        	// checkBoxLiveUpdate
        	//
        	checkBoxLiveUpdate.AutoSize = true;
        	checkBoxLiveUpdate.Checked = true;
        	checkBoxLiveUpdate.CheckState = CheckState.Checked;
        	checkBoxLiveUpdate.Location = new Point(941, 10);
        	checkBoxLiveUpdate.Margin = new Padding(5);
        	checkBoxLiveUpdate.Name = "checkBoxLiveUpdate";
        	checkBoxLiveUpdate.Size = new Size(139, 29);
        	checkBoxLiveUpdate.TabIndex = 7;
        	checkBoxLiveUpdate.Text = "Live update";
        	checkBoxLiveUpdate.UseVisualStyleBackColor = true;
        	//
        	// checkBoxShowRtfCode
        	//
        	checkBoxShowRtfCode.AutoSize = true;
        	checkBoxShowRtfCode.Checked = true;
        	checkBoxShowRtfCode.CheckState = CheckState.Checked;
        	checkBoxShowRtfCode.Location = new Point(767, 10);
        	checkBoxShowRtfCode.Margin = new Padding(5);
        	checkBoxShowRtfCode.Name = "checkBoxShowRtfCode";
        	checkBoxShowRtfCode.Size = new Size(179, 29);
        	checkBoxShowRtfCode.TabIndex = 6;
        	checkBoxShowRtfCode.Text = "Show RTF code";
        	checkBoxShowRtfCode.UseVisualStyleBackColor = true;
        	//
        	// checkBoxShowSourceMd
        	//
        	checkBoxShowSourceMd.AutoSize = true;
        	checkBoxShowSourceMd.Checked = true;
        	checkBoxShowSourceMd.CheckState = CheckState.Checked;
        	checkBoxShowSourceMd.Location = new Point(577, 10);
        	checkBoxShowSourceMd.Margin = new Padding(5);
        	checkBoxShowSourceMd.Name = "checkBoxShowSourceMd";
        	checkBoxShowSourceMd.Size = new Size(188, 29);
        	checkBoxShowSourceMd.TabIndex = 5;
        	checkBoxShowSourceMd.Text = "Show source MD";
        	checkBoxShowSourceMd.UseVisualStyleBackColor = true;
        	//
        	// splitContainer1
        	//
        	splitContainer1.Dock = DockStyle.Fill;
        	splitContainer1.Location = new Point(0, 0);
        	splitContainer1.Margin = new Padding(5);
        	splitContainer1.Name = "splitContainer1";
        	//
        	// splitContainer1.Panel1
        	//
        	splitContainer1.Panel1.Controls.Add(textBoxSourceMd);
        	splitContainer1.Panel1MinSize = 0;
        	//
        	// splitContainer1.Panel2
        	//
        	splitContainer1.Panel2.Controls.Add(richTextBoxRtfView);
        	splitContainer1.Panel2MinSize = 100;
        	splitContainer1.Size = new Size(1354, 1144);
        	splitContainer1.SplitterDistance = 496;
        	splitContainer1.SplitterWidth = 6;
        	splitContainer1.TabIndex = 10;
        	//
        	// textBoxSourceMd
        	//
        	textBoxSourceMd.AcceptsTab = true;
        	textBoxSourceMd.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left)  | AnchorStyles.Right)));
        	textBoxSourceMd.Font = new Font("Courier New", 9F);
        	textBoxSourceMd.Location = new Point(5, 8);
        	textBoxSourceMd.Margin = new Padding(5);
        	textBoxSourceMd.Multiline = true;
        	textBoxSourceMd.Name = "textBoxSourceMd";
        	textBoxSourceMd.ScrollBars = ScrollBars.Both;
        	textBoxSourceMd.Size = new Size(484, 1134);
        	textBoxSourceMd.TabIndex = 6;
        	textBoxSourceMd.Dock = DockStyle.Fill;
        	textBoxSourceMd.WordWrap = false;

        	//
        	// splitContainer2
        	//
        	splitContainer2.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right)));
        	splitContainer2.Location = new Point(5, 50);
        	splitContainer2.Margin = new Padding(5);
        	splitContainer2.Name = "splitContainer2";
        	//
        	// splitContainer2.Panel1
        	//
        	splitContainer2.Panel1.Controls.Add(splitContainer1);
        	splitContainer2.Panel1MinSize = 100;
        	//
        	// splitContainer2.Panel2
        	//
        	splitContainer2.Panel2.Controls.Add(richTextBoxRtfCode);
        	splitContainer2.Panel2MinSize = 0;
        	splitContainer2.Size = new Size(1950, 1144);
        	splitContainer2.SplitterDistance = 1354;
        	splitContainer2.SplitterWidth = 6;
        	splitContainer2.TabIndex = 7;
        	//
        	// timerUpdate
        	//
        	timerUpdate.Interval = 1000;
        	//
        	// MarkdownViewer
        	//
        	AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
        	AutoScaleMode = AutoScaleMode.Font;
        	ClientSize = new Size(1955, 1195);
        	Controls.Add(splitContainer2);
        	Controls.Add(panel1);
        	Icon = ((Icon)(resources.GetObject("$this.Icon")));
        	Margin = new Padding(5);
        	Name = "MarkdownViewer";
        	Text = "Markdown Viewer";
        	panel1.ResumeLayout(false);
        	panel1.PerformLayout();
        	splitContainer1.Panel1.ResumeLayout(false);
        	splitContainer1.Panel1.PerformLayout();
        	splitContainer1.Panel2.ResumeLayout(false);
        	((System.ComponentModel.ISupportInitialize)(splitContainer1)).EndInit();
        	splitContainer1.ResumeLayout(false);
        	splitContainer2.Panel1.ResumeLayout(false);
        	splitContainer2.Panel2.ResumeLayout(false);
        	((System.ComponentModel.ISupportInitialize)(splitContainer2)).EndInit();
        	splitContainer2.ResumeLayout(false);
        	ResumeLayout(false);

        }

    }
}

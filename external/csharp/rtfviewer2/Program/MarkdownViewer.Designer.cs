using System;
using System.Drawing;
using System.Windows.Forms;


namespace Program
{
	partial class MarkdownViewer
	{

		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Panel panelTop;
		private System.Windows.Forms.Panel panelBottom;
		private System.Windows.Forms.Label labelVersion;
		private System.Windows.Forms.Button btnRender;
		private System.Windows.Forms.CheckBox chkDebugMarkers;
		private System.Windows.Forms.Button btnScrollTop;
		private System.Windows.Forms.Button btnScrollUp;
		private System.Windows.Forms.Button btnScrollDown;
		private System.Windows.Forms.Button btnScrollEnd;

		private System.Windows.Forms.SplitContainer splitContainerMain;
		private System.Windows.Forms.TextBox textBoxSourceMd;
		private System.Windows.Forms.RichTextBox richTextBoxRtfView;

		private System.Windows.Forms.Timer timerUpdate;

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.timerUpdate = new System.Windows.Forms.Timer(this.components);
			this.panelTop = new System.Windows.Forms.Panel();
			this.btnRender = new System.Windows.Forms.Button();
			this.chkDebugMarkers = new System.Windows.Forms.CheckBox();
			this.btnScrollTop = new System.Windows.Forms.Button();
			this.btnScrollUp = new System.Windows.Forms.Button();
			this.btnScrollDown = new System.Windows.Forms.Button();
			this.btnScrollEnd = new System.Windows.Forms.Button();
			this.panelBottom = new System.Windows.Forms.Panel();
			this.labelVersion = new System.Windows.Forms.Label();
			this.splitContainerMain = new System.Windows.Forms.SplitContainer();
			this.textBoxSourceMd = new System.Windows.Forms.TextBox();
			this.richTextBoxRtfView = new System.Windows.Forms.RichTextBox();
			this.panelTop.SuspendLayout();
			this.panelBottom.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
			this.splitContainerMain.Panel1.SuspendLayout();
			this.splitContainerMain.Panel2.SuspendLayout();
			this.splitContainerMain.SuspendLayout();
			this.SuspendLayout();
			//
			// timerUpdate
			//
			this.timerUpdate.Interval = 1000;
			this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);

			//
			// panelTop
			//
			this.panelTop.Controls.Add(this.btnRender);
			this.panelTop.Controls.Add(this.chkDebugMarkers);
			this.panelTop.Controls.Add(this.btnScrollTop);
			this.panelTop.Controls.Add(this.btnScrollUp);
			this.panelTop.Controls.Add(this.btnScrollDown);
			this.panelTop.Controls.Add(this.btnScrollEnd);
			this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelTop.Location = new System.Drawing.Point(0, 0);
			this.panelTop.Name = "panelTop";
			this.panelTop.Size = new System.Drawing.Size(1076, 53);
			this.panelTop.TabIndex = 2;
			//
			// btnRender
			//
			this.btnRender.Location = new System.Drawing.Point(5, 8);
			this.btnRender.Name = "btnRender";
			this.btnRender.Size = new System.Drawing.Size(97, 37);
			this.btnRender.TabIndex = 0;
			this.btnRender.Text = "Render";
			this.btnRender.Click += new System.EventHandler(this.btnRenderClick);
			//
			// chkDebugMarkers
			//
			this.chkDebugMarkers.Location = new System.Drawing.Point(108, 14);
			this.chkDebugMarkers.Name = "chkDebugMarkers";
			this.chkDebugMarkers.Size = new System.Drawing.Size(104, 24);
			this.chkDebugMarkers.TabIndex = 1;
			this.chkDebugMarkers.Text = "Debug markers";
			//
			// btnScrollTop
			//
			this.btnScrollTop.DialogResult = System.Windows.Forms.DialogResult.No;
			this.btnScrollTop.Location = new System.Drawing.Point(230, 8);
			this.btnScrollTop.Name = "btnScrollTop";
			this.btnScrollTop.Size = new System.Drawing.Size(75, 37);
			this.btnScrollTop.TabIndex = 2;
			this.btnScrollTop.Text = "<<";
			//
			// btnScrollUp
			//
			this.btnScrollUp.Location = new System.Drawing.Point(310, 8);
			this.btnScrollUp.Name = "btnScrollUp";
			this.btnScrollUp.Size = new System.Drawing.Size(75, 37);
			this.btnScrollUp.TabIndex = 3;
			this.btnScrollUp.Text = "<";
			this.btnScrollUp.Click += new System.EventHandler(this.btnScrollUp_Click);
			//
			// btnScrollDown
			//
			this.btnScrollDown.Location = new System.Drawing.Point(391, 8);
			this.btnScrollDown.Name = "btnScrollDown";
			this.btnScrollDown.Size = new System.Drawing.Size(75, 37);
			this.btnScrollDown.TabIndex = 4;
			this.btnScrollDown.Text = ">";
			this.btnScrollDown.Click += new System.EventHandler(this.btnScrollDown_Click);
			//
			// btnScrollEnd
			//
			this.btnScrollEnd.Location = new System.Drawing.Point(472, 8);
			this.btnScrollEnd.Name = "btnScrollEnd";
			this.btnScrollEnd.Size = new System.Drawing.Size(75, 37);
			this.btnScrollEnd.TabIndex = 5;
			this.btnScrollEnd.Text = ">>";
			//
			// panelBottom
			//
			this.panelBottom.Controls.Add(this.labelVersion);
			this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelBottom.Location = new System.Drawing.Point(0, 601);
			this.panelBottom.Name = "panelBottom";
			this.panelBottom.Size = new System.Drawing.Size(1076, 35);
			this.panelBottom.TabIndex = 1;
			//
			// labelVersion
			//
			this.labelVersion.Dock = System.Windows.Forms.DockStyle.Right;
			this.labelVersion.Location = new System.Drawing.Point(937, 0);
			this.labelVersion.Margin = new System.Windows.Forms.Padding(3, 8, 3, 0);
			this.labelVersion.Name = "labelVersion";
			this.labelVersion.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
			this.labelVersion.Size = new System.Drawing.Size(139, 35);
			this.labelVersion.TabIndex = 0;
			this.labelVersion.Text = "version 0.3.0";
			//
			// splitContainerMain
			//
			this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerMain.Location = new System.Drawing.Point(0, 53);
			this.splitContainerMain.Name = "splitContainerMain";
			//
			// splitContainerMain.Panel1
			//
			this.splitContainerMain.Panel1.Controls.Add(this.textBoxSourceMd);
			//
			// splitContainerMain.Panel2
			//
			this.splitContainerMain.Panel2.Controls.Add(this.richTextBoxRtfView);
			this.splitContainerMain.Size = new System.Drawing.Size(1076, 548);
			this.splitContainerMain.SplitterDistance = 358;
			this.splitContainerMain.TabIndex = 0;
			//
			// textBoxSourceMd
			//
			this.textBoxSourceMd.AcceptsTab = true;
			this.textBoxSourceMd.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBoxSourceMd.Location = new System.Drawing.Point(0, 0);
			this.textBoxSourceMd.Multiline = true;
			this.textBoxSourceMd.Name = "textBoxSourceMd";
			this.textBoxSourceMd.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxSourceMd.Size = new System.Drawing.Size(358, 548);
			this.textBoxSourceMd.TabIndex = 0;
			this.textBoxSourceMd.TextChanged += new System.EventHandler(this.textBoxSourceMd_TextChanged);
			//
			// richTextBoxRtfView
			//
			this.richTextBoxRtfView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBoxRtfView.Location = new System.Drawing.Point(0, 0);
			this.richTextBoxRtfView.Name = "richTextBoxRtfView";
			this.richTextBoxRtfView.Size = new System.Drawing.Size(714, 548);
			this.richTextBoxRtfView.TabIndex = 0;
			this.richTextBoxRtfView.Text = "";
			//
			// MarkdownViewer
			//
			this.ClientSize = new System.Drawing.Size(1076, 636);
			this.Controls.Add(this.splitContainerMain);
			this.Controls.Add(this.panelBottom);
			this.Controls.Add(this.panelTop);
			this.Name = "MarkdownViewer";
			this.Text = "Markdown → RTF Renderer";
			this.panelTop.ResumeLayout(false);
			this.panelBottom.ResumeLayout(false);
			this.splitContainerMain.Panel1.ResumeLayout(false);
			this.splitContainerMain.Panel1.PerformLayout();
			this.splitContainerMain.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
			this.splitContainerMain.ResumeLayout(false);
			this.ResumeLayout(false);

		}
	}
}

using System.Windows.Forms;
using System.ComponentModel;

namespace RealTimeEventLogReader {
	partial class MainForm {
		private IContainer components = null;

		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}


		private void InitializeComponent() {
			menuStrip1 = new MenuStrip();
			optionsToolStripMenuItem = new ToolStripMenuItem();
			startToolStripMenuItem = new ToolStripMenuItem();
			exportToXMLToolStripMenuItem = new ToolStripMenuItem();
			stopToolStripMenuItem = new ToolStripMenuItem();
			gvLogs = new DataGridView();
			statusStrip1 = new StatusStrip();
			toolStripStatusLabelCount = new ToolStripStatusLabel();
			toolStripStatusLabelNumCount = new ToolStripStatusLabel();
			toolStripStatusLabelStatusString = new ToolStripStatusLabel();
			menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(gvLogs)).BeginInit();
			statusStrip1.SuspendLayout();
			SuspendLayout();
			// 
			// menuStrip1
			// 
			menuStrip1.Items.AddRange(new ToolStripItem[] {
				optionsToolStripMenuItem
			});
			menuStrip1.Location = new System.Drawing.Point(0, 0);
			menuStrip1.Name = "menuStrip1";
			menuStrip1.Size = new System.Drawing.Size(721, 24);
			menuStrip1.TabIndex = 0;
			menuStrip1.Text = "menuStrip1";
			// 
			// optionsToolStripMenuItem
			// 
			optionsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
				startToolStripMenuItem,
				exportToXMLToolStripMenuItem,
				stopToolStripMenuItem
			});
			optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
			optionsToolStripMenuItem.Text = "Options";
			// 
			// startToolStripMenuItem
			// 
			startToolStripMenuItem.Name = "startToolStripMenuItem";
			startToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
			startToolStripMenuItem.Text = "Start";
			startToolStripMenuItem.Click += new System.EventHandler(startToolStripMenuItem_Click);
			// 
			// exportToXMLToolStripMenuItem
			// 
			exportToXMLToolStripMenuItem.Name = "exportToXMLToolStripMenuItem";
			exportToXMLToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
			exportToXMLToolStripMenuItem.Text = "Export to XML";
			// 
			// stopToolStripMenuItem
			// 
			stopToolStripMenuItem.Name = "stopToolStripMenuItem";
			stopToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
			stopToolStripMenuItem.Text = "Stop";
			stopToolStripMenuItem.Click += new System.EventHandler(stopToolStripMenuItem_Click);
			// 
			// gvLogs
			// 
			gvLogs.AllowUserToAddRows = false;
			gvLogs.AllowUserToDeleteRows = false;
			gvLogs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			gvLogs.Dock = DockStyle.Fill;
			gvLogs.Location = new System.Drawing.Point(0, 24);
			gvLogs.MultiSelect = false;
			gvLogs.Name = "gvLogs";
			gvLogs.ReadOnly = true;
			gvLogs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			gvLogs.Size = new System.Drawing.Size(721, 398);
			gvLogs.TabIndex = 1;
			gvLogs.DoubleClick += new System.EventHandler(gvLogs_DoubleClick);
			// 
			// statusStrip1
			// 
			statusStrip1.Items.AddRange(new ToolStripItem[] {
				toolStripStatusLabelCount,
				toolStripStatusLabelNumCount,
				toolStripStatusLabelStatusString
			});
			statusStrip1.Location = new System.Drawing.Point(0, 400);
			statusStrip1.Name = "statusStrip1";
			statusStrip1.Size = new System.Drawing.Size(721, 22);
			statusStrip1.TabIndex = 2;
			statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabelCount
			// 
			toolStripStatusLabelCount.Name = "toolStripStatusLabelCount";
			toolStripStatusLabelCount.Size = new System.Drawing.Size(40, 17);
			toolStripStatusLabelCount.Text = "Count";
			// 
			// toolStripStatusLabelNumCount
			// 
			toolStripStatusLabelNumCount.Name = "toolStripStatusLabelNumCount";
			toolStripStatusLabelNumCount.Size = new System.Drawing.Size(13, 17);
			toolStripStatusLabelNumCount.Text = "0";
			// 
			// toolStripStatusLabelStatusString
			// 
			toolStripStatusLabelStatusString.Name = "toolStripStatusLabelStatusString";
			toolStripStatusLabelStatusString.Size = new System.Drawing.Size(118, 17);
			toolStripStatusLabelStatusString.Text = "toolStripStatusLabel1";
			// 
			// MainForm
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(721, 422);
			Controls.Add(statusStrip1);
			Controls.Add(gvLogs);
			Controls.Add(menuStrip1);
			MainMenuStrip = menuStrip1;
			Name = "MainForm";
			Text = "Realtime Event Log Reader";
			FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
			menuStrip1.ResumeLayout(false);
			menuStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(gvLogs)).EndInit();
			statusStrip1.ResumeLayout(false);
			statusStrip1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();

		}

		private MenuStrip menuStrip1;
		private ToolStripMenuItem optionsToolStripMenuItem;
		private ToolStripMenuItem startToolStripMenuItem;
		private ToolStripMenuItem exportToXMLToolStripMenuItem;
		private ToolStripMenuItem stopToolStripMenuItem;
		private DataGridView gvLogs;
		private StatusStrip statusStrip1;
		private ToolStripStatusLabel toolStripStatusLabelCount;
		private ToolStripStatusLabel toolStripStatusLabelNumCount;
		private ToolStripStatusLabel toolStripStatusLabelStatusString;
	}
}


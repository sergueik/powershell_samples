namespace Temperature
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maxTempToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setIntervalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.secToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.secToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.secToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.resetWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.graphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pbLog = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLog)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.LightSlateGray;
            this.label1.Font = new System.Drawing.Font("DejaVu Sans", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Lime;
            this.label1.Location = new System.Drawing.Point(48, 14);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 28);
            this.label1.TabIndex = 0;
            this.label1.Text = "CPU";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // toolTip1
            // 
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip1.ToolTipTitle = "Temp Monitor";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.quitToolStripMenuItem,
            this.infoToolStripMenuItem,
            this.updateToolStripMenuItem,
            this.setIntervalToolStripMenuItem,
            this.resetWindowToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.graphToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(198, 214);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(197, 30);
            this.quitToolStripMenuItem.Text = "&Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // infoToolStripMenuItem
            // 
            this.infoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.maxTempToolStripMenuItem});
            this.infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            this.infoToolStripMenuItem.Size = new System.Drawing.Size(197, 30);
            this.infoToolStripMenuItem.Text = "&Info";
            this.infoToolStripMenuItem.Click += new System.EventHandler(this.infoToolStripMenuItem_Click);
            // 
            // maxTempToolStripMenuItem
            // 
            this.maxTempToolStripMenuItem.Name = "maxTempToolStripMenuItem";
            this.maxTempToolStripMenuItem.Size = new System.Drawing.Size(194, 30);
            this.maxTempToolStripMenuItem.Text = "Max Temp =";
            // 
            // updateToolStripMenuItem
            // 
            this.updateToolStripMenuItem.Name = "updateToolStripMenuItem";
            this.updateToolStripMenuItem.Size = new System.Drawing.Size(197, 30);
            this.updateToolStripMenuItem.Text = "&Update Now";
            this.updateToolStripMenuItem.Click += new System.EventHandler(this.updateToolStripMenuItem_Click);
            // 
            // setIntervalToolStripMenuItem
            // 
            this.setIntervalToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.secToolStripMenuItem,
            this.secToolStripMenuItem1,
            this.secToolStripMenuItem2});
            this.setIntervalToolStripMenuItem.Name = "setIntervalToolStripMenuItem";
            this.setIntervalToolStripMenuItem.Size = new System.Drawing.Size(197, 30);
            this.setIntervalToolStripMenuItem.Text = "&Set Interval";
            // 
            // secToolStripMenuItem
            // 
            this.secToolStripMenuItem.Checked = true;
            this.secToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.secToolStripMenuItem.Name = "secToolStripMenuItem";
            this.secToolStripMenuItem.Size = new System.Drawing.Size(148, 30);
            this.secToolStripMenuItem.Text = "60 Sec";
            this.secToolStripMenuItem.Click += new System.EventHandler(this.secToolStripMenuItem_Click);
            // 
            // secToolStripMenuItem1
            // 
            this.secToolStripMenuItem1.Name = "secToolStripMenuItem1";
            this.secToolStripMenuItem1.Size = new System.Drawing.Size(148, 30);
            this.secToolStripMenuItem1.Text = "30 Sec";
            this.secToolStripMenuItem1.Click += new System.EventHandler(this.secToolStripMenuItem1_Click);
            // 
            // secToolStripMenuItem2
            // 
            this.secToolStripMenuItem2.Name = "secToolStripMenuItem2";
            this.secToolStripMenuItem2.Size = new System.Drawing.Size(148, 30);
            this.secToolStripMenuItem2.Text = "1 Sec";
            this.secToolStripMenuItem2.Click += new System.EventHandler(this.secToolStripMenuItem2_Click);
            // 
            // resetWindowToolStripMenuItem
            // 
            this.resetWindowToolStripMenuItem.Name = "resetWindowToolStripMenuItem";
            this.resetWindowToolStripMenuItem.Size = new System.Drawing.Size(197, 30);
            this.resetWindowToolStripMenuItem.Text = "&Reset Window";
            this.resetWindowToolStripMenuItem.Click += new System.EventHandler(this.resetWindowToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(197, 30);
            this.optionsToolStripMenuItem.Text = "&Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // graphToolStripMenuItem
            // 
            this.graphToolStripMenuItem.Name = "graphToolStripMenuItem";
            this.graphToolStripMenuItem.Size = new System.Drawing.Size(197, 30);
            this.graphToolStripMenuItem.Text = "&Graph";
            this.graphToolStripMenuItem.Click += new System.EventHandler(this.graphToolStripMenuItem_Click);
            // 
            // pbLog
            // 
            this.pbLog.Image = global::Temperature.Properties.Resources.icons8_data_backup_16;
            this.pbLog.Location = new System.Drawing.Point(146, 28);
            this.pbLog.Name = "pbLog";
            this.pbLog.Size = new System.Drawing.Size(26, 28);
            this.pbLog.TabIndex = 1;
            this.pbLog.TabStop = false;
            this.toolTip1.SetToolTip(this.pbLog, "Temp Logging on");
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSlateGray;
            this.ClientSize = new System.Drawing.Size(170, 50);
            this.Controls.Add(this.pbLog);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(170, 50);
            this.Name = "MainForm";
            this.Opacity = 0.75D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "CPU Temperature";
            this.toolTip1.SetToolTip(this, "Right Click for Options");
            this.TopMost = true;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Click += new System.EventHandler(this.MainForm_Click);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbLog)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setIntervalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem secToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem secToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem secToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem maxTempToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem graphToolStripMenuItem;
        private System.Windows.Forms.PictureBox pbLog;
    }
}


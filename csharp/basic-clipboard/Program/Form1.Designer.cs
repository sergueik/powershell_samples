namespace clipboard_helper
{
    partial class Form1
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
            this.clip_entries = new System.Windows.Forms.ListBox();
            this.contextMenu1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.usunToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenu1.SuspendLayout();
            this.SuspendLayout();
            // 
            // clip_entries
            // 
            this.clip_entries.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.clip_entries.ContextMenuStrip = this.contextMenu1;
            this.clip_entries.FormattingEnabled = true;
            this.clip_entries.HorizontalScrollbar = true;
            this.clip_entries.Location = new System.Drawing.Point(0, 2);
            this.clip_entries.Name = "clip_entries";
            this.clip_entries.Size = new System.Drawing.Size(393, 160);
            this.clip_entries.TabIndex = 0;
            this.clip_entries.MouseHover += new System.EventHandler(this.clip_entries_MouseHover);
            this.clip_entries.MouseClick += new System.Windows.Forms.MouseEventHandler(this.clip_entries_MouseClick);
            this.clip_entries.SelectedIndexChanged += new System.EventHandler(this.clip_entries_SelectedIndexChanged);
            this.clip_entries.MouseDown += new System.Windows.Forms.MouseEventHandler(this.clip_entries_MouseDown);
            // 
            // contextMenu1
            // 
            this.contextMenu1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.contextMenu1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.usunToolStripMenuItem});
            this.contextMenu1.Name = "contextMenu1";
            this.contextMenu1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.contextMenu1.Size = new System.Drawing.Size(110, 26);
            // 
            // usunToolStripMenuItem
            // 
            this.usunToolStripMenuItem.Name = "usunToolStripMenuItem";
            this.usunToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.usunToolStripMenuItem.Text = "Usun";
            this.usunToolStripMenuItem.Click += new System.EventHandler(this.usunToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(390, 165);
            this.ControlBox = false;
            this.Controls.Add(this.clip_entries);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Clipboard Little Helper";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.WhiteSmoke;
            this.Deactivate += new System.EventHandler(this.Form1_Deactivate);
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.contextMenu1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox clip_entries;
        private System.Windows.Forms.ContextMenuStrip contextMenu1;
        private System.Windows.Forms.ToolStripMenuItem usunToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        
    }
}


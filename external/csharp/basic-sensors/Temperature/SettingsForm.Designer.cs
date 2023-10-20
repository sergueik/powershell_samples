namespace Temperature
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.btnClose = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.trbTransparancy = new System.Windows.Forms.TrackBar();
            this.gbTrackBar = new System.Windows.Forms.GroupBox();
            this.lblTrackBarCurrentValue = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.gbThresholds = new System.Windows.Forms.GroupBox();
            this.nudRedAlert = new System.Windows.Forms.NumericUpDown();
            this.nudYellowAlert = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblRed = new System.Windows.Forms.Label();
            this.lblYellow = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.gbRadioButtons = new System.Windows.Forms.GroupBox();
            this.cbLogTemps = new System.Windows.Forms.CheckBox();
            this.btnRestoreDef = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trbTransparancy)).BeginInit();
            this.gbTrackBar.SuspendLayout();
            this.gbThresholds.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRedAlert)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudYellowAlert)).BeginInit();
            this.gbRadioButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClose.Location = new System.Drawing.Point(595, 663);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(112, 35);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "&Close";
            this.toolTip1.SetToolTip(this.btnClose, "Exit without saving changes");
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnApply.Location = new System.Drawing.Point(445, 663);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(144, 35);
            this.btnApply.TabIndex = 1;
            this.btnApply.Text = "&Apply";
            this.toolTip1.SetToolTip(this.btnApply, "Saves user settings ");
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // trbTransparancy
            // 
            this.trbTransparancy.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trbTransparancy.LargeChange = 10;
            this.trbTransparancy.Location = new System.Drawing.Point(36, 28);
            this.trbTransparancy.Maximum = 100;
            this.trbTransparancy.Name = "trbTransparancy";
            this.trbTransparancy.Size = new System.Drawing.Size(624, 69);
            this.trbTransparancy.SmallChange = 5;
            this.trbTransparancy.TabIndex = 2;
            this.trbTransparancy.TickFrequency = 5;
            this.trbTransparancy.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trbTransparancy.Value = 50;
            this.trbTransparancy.Scroll += new System.EventHandler(this.trbTransparancy_Scroll);
            // 
            // gbTrackBar
            // 
            this.gbTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbTrackBar.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.gbTrackBar.Controls.Add(this.lblTrackBarCurrentValue);
            this.gbTrackBar.Controls.Add(this.label2);
            this.gbTrackBar.Controls.Add(this.label1);
            this.gbTrackBar.Controls.Add(this.trbTransparancy);
            this.gbTrackBar.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gbTrackBar.Location = new System.Drawing.Point(12, 296);
            this.gbTrackBar.Name = "gbTrackBar";
            this.gbTrackBar.Size = new System.Drawing.Size(695, 117);
            this.gbTrackBar.TabIndex = 3;
            this.gbTrackBar.TabStop = false;
            this.gbTrackBar.Text = "Window Opacity";
            this.toolTip1.SetToolTip(this.gbTrackBar, "Sets Opacity of small temperature window");
            // 
            // lblTrackBarCurrentValue
            // 
            this.lblTrackBarCurrentValue.AutoSize = true;
            this.lblTrackBarCurrentValue.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblTrackBarCurrentValue.Location = new System.Drawing.Point(334, 94);
            this.lblTrackBarCurrentValue.Name = "lblTrackBarCurrentValue";
            this.lblTrackBarCurrentValue.Size = new System.Drawing.Size(27, 20);
            this.lblTrackBarCurrentValue.TabIndex = 5;
            this.lblTrackBarCurrentValue.Text = "50";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(614, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "100%";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 94);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "0%";
            // 
            // gbThresholds
            // 
            this.gbThresholds.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.gbThresholds.Controls.Add(this.nudRedAlert);
            this.gbThresholds.Controls.Add(this.nudYellowAlert);
            this.gbThresholds.Controls.Add(this.label4);
            this.gbThresholds.Controls.Add(this.label3);
            this.gbThresholds.Controls.Add(this.lblRed);
            this.gbThresholds.Controls.Add(this.lblYellow);
            this.gbThresholds.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gbThresholds.Location = new System.Drawing.Point(12, 66);
            this.gbThresholds.Name = "gbThresholds";
            this.gbThresholds.Size = new System.Drawing.Size(695, 212);
            this.gbThresholds.TabIndex = 4;
            this.gbThresholds.TabStop = false;
            this.gbThresholds.Text = "Temperature Thresholds";
            // 
            // nudRedAlert
            // 
            this.nudRedAlert.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.nudRedAlert.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudRedAlert.Location = new System.Drawing.Point(219, 118);
            this.nudRedAlert.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.nudRedAlert.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudRedAlert.Name = "nudRedAlert";
            this.nudRedAlert.ReadOnly = true;
            this.nudRedAlert.Size = new System.Drawing.Size(162, 26);
            this.nudRedAlert.TabIndex = 11;
            this.nudRedAlert.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudRedAlert.Value = new decimal(new int[] {
            130,
            0,
            0,
            0});
            // 
            // nudYellowAlert
            // 
            this.nudYellowAlert.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.nudYellowAlert.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudYellowAlert.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudYellowAlert.Location = new System.Drawing.Point(219, 55);
            this.nudYellowAlert.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.nudYellowAlert.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudYellowAlert.Name = "nudYellowAlert";
            this.nudYellowAlert.ReadOnly = true;
            this.nudYellowAlert.Size = new System.Drawing.Size(162, 26);
            this.nudYellowAlert.TabIndex = 10;
            this.nudYellowAlert.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudYellowAlert.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(396, 124);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 20);
            this.label4.TabIndex = 9;
            this.label4.Text = "Deg F";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(396, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 20);
            this.label3.TabIndex = 8;
            this.label3.Text = "Deg F";
            // 
            // lblRed
            // 
            this.lblRed.BackColor = System.Drawing.Color.Red;
            this.lblRed.ForeColor = System.Drawing.SystemColors.Control;
            this.lblRed.Location = new System.Drawing.Point(32, 118);
            this.lblRed.Name = "lblRed";
            this.lblRed.Size = new System.Drawing.Size(162, 26);
            this.lblRed.TabIndex = 6;
            this.lblRed.Text = "Red Alert Temp";
            this.lblRed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.lblRed, "Temp at which display becomes red");
            // 
            // lblYellow
            // 
            this.lblYellow.BackColor = System.Drawing.Color.Yellow;
            this.lblYellow.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblYellow.Location = new System.Drawing.Point(32, 55);
            this.lblYellow.Name = "lblYellow";
            this.lblYellow.Size = new System.Drawing.Size(162, 26);
            this.lblYellow.TabIndex = 5;
            this.lblYellow.Text = "Yellow Alert Temp";
            this.lblYellow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.lblYellow, "Temp at which display becomes yellow");
            // 
            // gbRadioButtons
            // 
            this.gbRadioButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbRadioButtons.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.gbRadioButtons.Controls.Add(this.cbLogTemps);
            this.gbRadioButtons.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gbRadioButtons.Location = new System.Drawing.Point(12, 476);
            this.gbRadioButtons.Name = "gbRadioButtons";
            this.gbRadioButtons.Size = new System.Drawing.Size(695, 117);
            this.gbRadioButtons.TabIndex = 7;
            this.gbRadioButtons.TabStop = false;
            this.gbRadioButtons.Text = "Settings";
            this.toolTip1.SetToolTip(this.gbRadioButtons, "Turn Temperature Logging on and off");
            // 
            // cbLogTemps
            // 
            this.cbLogTemps.AutoSize = true;
            this.cbLogTemps.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.cbLogTemps.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.cbLogTemps.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbLogTemps.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cbLogTemps.Location = new System.Drawing.Point(15, 48);
            this.cbLogTemps.Name = "cbLogTemps";
            this.cbLogTemps.Size = new System.Drawing.Size(161, 24);
            this.cbLogTemps.TabIndex = 9;
            this.cbLogTemps.Text = "&Log Temperatures";
            this.toolTip1.SetToolTip(this.cbLogTemps, "Save Temp Readings for Graph display");
            this.cbLogTemps.UseVisualStyleBackColor = true;
            this.cbLogTemps.CheckedChanged += new System.EventHandler(this.cbLogTemps_CheckedChanged);
            // 
            // btnRestoreDef
            // 
            this.btnRestoreDef.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRestoreDef.AutoSize = true;
            this.btnRestoreDef.Location = new System.Drawing.Point(12, 663);
            this.btnRestoreDef.Name = "btnRestoreDef";
            this.btnRestoreDef.Size = new System.Drawing.Size(185, 35);
            this.btnRestoreDef.TabIndex = 6;
            this.btnRestoreDef.Text = "&Restore Default Values";
            this.toolTip1.SetToolTip(this.btnRestoreDef, "Restore program defaults");
            this.btnRestoreDef.UseVisualStyleBackColor = true;
            this.btnRestoreDef.Click += new System.EventHandler(this.btnRestoreDef_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.AutoSize = true;
            this.btnPreview.Location = new System.Drawing.Point(12, 419);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(185, 35);
            this.btnPreview.TabIndex = 5;
            this.btnPreview.Text = "&Preview";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(719, 754);
            this.Controls.Add(this.gbRadioButtons);
            this.Controls.Add(this.btnRestoreDef);
            this.Controls.Add(this.btnPreview);
            this.Controls.Add(this.gbThresholds);
            this.Controls.Add(this.gbTrackBar);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnClose);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(741, 810);
            this.MinimumSize = new System.Drawing.Size(741, 810);
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Temperature Options";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trbTransparancy)).EndInit();
            this.gbTrackBar.ResumeLayout(false);
            this.gbTrackBar.PerformLayout();
            this.gbThresholds.ResumeLayout(false);
            this.gbThresholds.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRedAlert)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudYellowAlert)).EndInit();
            this.gbRadioButtons.ResumeLayout(false);
            this.gbRadioButtons.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.TrackBar trbTransparancy;
        private System.Windows.Forms.GroupBox gbTrackBar;
        private System.Windows.Forms.GroupBox gbThresholds;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblRed;
        private System.Windows.Forms.Label lblYellow;
        private System.Windows.Forms.Label lblTrackBarCurrentValue;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudRedAlert;
        private System.Windows.Forms.NumericUpDown nudYellowAlert;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnRestoreDef;
        private System.Windows.Forms.GroupBox gbRadioButtons;
        private System.Windows.Forms.CheckBox cbLogTemps;
    }
}
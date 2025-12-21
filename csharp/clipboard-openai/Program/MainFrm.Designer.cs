namespace TalkingClipboard
{
  partial class MainFrm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainFrm));
      this._btnSpeak = new System.Windows.Forms.Button();
      this._btnSaveAsWavFile = new System.Windows.Forms.Button();
      this._editText = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this._btnStop = new System.Windows.Forms.Button();
      this._comboVoices = new System.Windows.Forms.ComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this._knobRate = new System.Windows.Forms.TrackBar();
      this._lblSpeed = new System.Windows.Forms.Label();
      this._btnPauseRestart = new System.Windows.Forms.Button();
      this._checkDontWatchClipboard = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this._knobRate)).BeginInit();
      this.SuspendLayout();
      // 
      // _btnSpeak
      // 
      this._btnSpeak.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this._btnSpeak.Location = new System.Drawing.Point(12, 172);
      this._btnSpeak.Name = "_btnSpeak";
      this._btnSpeak.Size = new System.Drawing.Size(56, 23);
      this._btnSpeak.TabIndex = 2;
      this._btnSpeak.Text = "Speak";
      this._btnSpeak.UseVisualStyleBackColor = true;
      this._btnSpeak.Click += new System.EventHandler(this._btnSpeak_Click);
      // 
      // _btnSaveAsWavFile
      // 
      this._btnSaveAsWavFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this._btnSaveAsWavFile.Location = new System.Drawing.Point(244, 172);
      this._btnSaveAsWavFile.Name = "_btnSaveAsWavFile";
      this._btnSaveAsWavFile.Size = new System.Drawing.Size(120, 23);
      this._btnSaveAsWavFile.TabIndex = 4;
      this._btnSaveAsWavFile.Text = "Save As .WAV File...";
      this._btnSaveAsWavFile.UseVisualStyleBackColor = true;
      this._btnSaveAsWavFile.Click += new System.EventHandler(this._btnSaveAsWavFile_Click);
      // 
      // _editText
      // 
      this._editText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this._editText.Location = new System.Drawing.Point(12, 28);
      this._editText.Multiline = true;
      this._editText.Name = "_editText";
      this._editText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this._editText.Size = new System.Drawing.Size(352, 134);
      this._editText.TabIndex = 1;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 8);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(83, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Speak this text:";
      // 
      // _btnStop
      // 
      this._btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this._btnStop.Location = new System.Drawing.Point(72, 172);
      this._btnStop.Name = "_btnStop";
      this._btnStop.Size = new System.Drawing.Size(56, 23);
      this._btnStop.TabIndex = 3;
      this._btnStop.Text = "Stop";
      this._btnStop.UseVisualStyleBackColor = true;
      this._btnStop.Click += new System.EventHandler(this._btnStop_Click);
      // 
      // _comboVoices
      // 
      this._comboVoices.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this._comboVoices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._comboVoices.FormattingEnabled = true;
      this._comboVoices.Location = new System.Drawing.Point(12, 224);
      this._comboVoices.Name = "_comboVoices";
      this._comboVoices.Size = new System.Drawing.Size(148, 21);
      this._comboVoices.TabIndex = 5;
      this._comboVoices.SelectionChangeCommitted += new System.EventHandler(this._comboVoices_SelectionChangeCommitted);
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 204);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(36, 13);
      this.label2.TabIndex = 6;
      this.label2.Text = "Voice:";
      // 
      // _knobRate
      // 
      this._knobRate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this._knobRate.Location = new System.Drawing.Point(172, 220);
      this._knobRate.Name = "_knobRate";
      this._knobRate.Size = new System.Drawing.Size(196, 45);
      this._knobRate.TabIndex = 7;
      this._knobRate.Value = 5;
      this._knobRate.ValueChanged += new System.EventHandler(this._knobRate_ValueChanged);
      // 
      // _lblSpeed
      // 
      this._lblSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this._lblSpeed.AutoSize = true;
      this._lblSpeed.Location = new System.Drawing.Point(180, 204);
      this._lblSpeed.Name = "_lblSpeed";
      this._lblSpeed.Size = new System.Drawing.Size(41, 13);
      this._lblSpeed.TabIndex = 8;
      this._lblSpeed.Text = "Speed:";
      // 
      // _btnPauseRestart
      // 
      this._btnPauseRestart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this._btnPauseRestart.Location = new System.Drawing.Point(132, 172);
      this._btnPauseRestart.Name = "_btnPauseRestart";
      this._btnPauseRestart.Size = new System.Drawing.Size(56, 23);
      this._btnPauseRestart.TabIndex = 9;
      this._btnPauseRestart.Text = "Pause";
      this._btnPauseRestart.UseVisualStyleBackColor = true;
      this._btnPauseRestart.Click += new System.EventHandler(this._btnPauseRestart_Click);
      // 
      // _checkDontWatchClipboard
      // 
      this._checkDontWatchClipboard.AutoSize = true;
      this._checkDontWatchClipboard.Location = new System.Drawing.Point(224, 8);
      this._checkDontWatchClipboard.Name = "_checkDontWatchClipboard";
      this._checkDontWatchClipboard.Size = new System.Drawing.Size(147, 17);
      this._checkDontWatchClipboard.TabIndex = 10;
      this._checkDontWatchClipboard.Text = "Stop monitoring clipboard";
      this._checkDontWatchClipboard.UseVisualStyleBackColor = true;
      // 
      // MainFrm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(376, 260);
      this.Controls.Add(this._checkDontWatchClipboard);
      this.Controls.Add(this._btnPauseRestart);
      this.Controls.Add(this._lblSpeed);
      this.Controls.Add(this._knobRate);
      this.Controls.Add(this.label2);
      this.Controls.Add(this._comboVoices);
      this.Controls.Add(this._btnStop);
      this.Controls.Add(this.label1);
      this.Controls.Add(this._editText);
      this.Controls.Add(this._btnSaveAsWavFile);
      this.Controls.Add(this._btnSpeak);
      this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MinimumSize = new System.Drawing.Size(384, 287);
      this.Name = "MainFrm";
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "The Talking Clipboard";
      this.Load += new System.EventHandler(this.MainFrm_Load);
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFrm_FormClosing);
      ((System.ComponentModel.ISupportInitialize)(this._knobRate)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button _btnSpeak;
    private System.Windows.Forms.Button _btnSaveAsWavFile;
    private System.Windows.Forms.TextBox _editText;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button _btnStop;
    private System.Windows.Forms.ComboBox _comboVoices;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TrackBar _knobRate;
    private System.Windows.Forms.Label _lblSpeed;
    private System.Windows.Forms.Button _btnPauseRestart;
    private System.Windows.Forms.CheckBox _checkDontWatchClipboard;
  }
}


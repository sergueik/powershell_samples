﻿namespace SerialisationTest {
  partial class Main {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.btnOpen = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // btnOpen
      // 
      this.btnOpen.Location = new System.Drawing.Point(12, 12);
      this.btnOpen.Name = "btnOpen";
      this.btnOpen.Size = new System.Drawing.Size(75, 23);
      this.btnOpen.TabIndex = 1;
      this.btnOpen.Text = "Open";
      this.btnOpen.UseVisualStyleBackColor = true;
      this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
      // 
      // Main
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(108, 48);
      this.Controls.Add(this.btnOpen);
      this.Name = "Main";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Serialisation test";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btnOpen;
  }
}


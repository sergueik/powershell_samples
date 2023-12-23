namespace UIThread35Desktop
{
    partial class FormAvoidingInvokeRequired
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
            this.labelMultiThreadWithParams = new System.Windows.Forms.Label();
            this.buttonMultiStandardParams = new System.Windows.Forms.Button();
            this.buttonMultiStandard = new System.Windows.Forms.Button();
            this.buttonSingleStandard = new System.Windows.Forms.Button();
            this.buttonMultiNone = new System.Windows.Forms.Button();
            this.buttonSingleNone = new System.Windows.Forms.Button();
            this.labelMultiThread = new System.Windows.Forms.Label();
            this.labelSingleThread = new System.Windows.Forms.Label();
            this.groupBoxUIThread = new System.Windows.Forms.GroupBox();
            this.buttonMultiUIThread = new System.Windows.Forms.Button();
            this.buttonMultiUIThreadParams = new System.Windows.Forms.Button();
            this.SingleUIThread = new System.Windows.Forms.Button();
            this.textBoxOut = new System.Windows.Forms.TextBox();
            this.buttonMultiNoneParams = new System.Windows.Forms.Button();
            this.groupBoxUIThread.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelMultiThreadWithParams
            // 
            this.labelMultiThreadWithParams.AutoSize = true;
            this.labelMultiThreadWithParams.Location = new System.Drawing.Point(301, 10);
            this.labelMultiThreadWithParams.Name = "labelMultiThreadWithParams";
            this.labelMultiThreadWithParams.Size = new System.Drawing.Size(126, 13);
            this.labelMultiThreadWithParams.TabIndex = 28;
            this.labelMultiThreadWithParams.Text = "MultiThread With Params";
            // 
            // buttonMultiStandardParams
            // 
            this.buttonMultiStandardParams.Location = new System.Drawing.Point(304, 70);
            this.buttonMultiStandardParams.Name = "buttonMultiStandardParams";
            this.buttonMultiStandardParams.Size = new System.Drawing.Size(130, 23);
            this.buttonMultiStandardParams.TabIndex = 27;
            this.buttonMultiStandardParams.Text = "MultiStandardParams";
            this.buttonMultiStandardParams.UseVisualStyleBackColor = true;
            this.buttonMultiStandardParams.Click += new System.EventHandler(this.buttonMultiStandardParams_Click);
            // 
            // buttonMultiStandard
            // 
            this.buttonMultiStandard.Location = new System.Drawing.Point(167, 70);
            this.buttonMultiStandard.Name = "buttonMultiStandard";
            this.buttonMultiStandard.Size = new System.Drawing.Size(111, 23);
            this.buttonMultiStandard.TabIndex = 25;
            this.buttonMultiStandard.Text = "MultiStandard";
            this.buttonMultiStandard.UseVisualStyleBackColor = true;
            this.buttonMultiStandard.Click += new System.EventHandler(this.buttonMultiStandard_Click);
            // 
            // buttonSingleStandard
            // 
            this.buttonSingleStandard.Location = new System.Drawing.Point(30, 70);
            this.buttonSingleStandard.Name = "buttonSingleStandard";
            this.buttonSingleStandard.Size = new System.Drawing.Size(111, 23);
            this.buttonSingleStandard.TabIndex = 24;
            this.buttonSingleStandard.Text = "SingleStandard";
            this.buttonSingleStandard.UseVisualStyleBackColor = true;
            this.buttonSingleStandard.Click += new System.EventHandler(this.buttonSingleStandard_Click);
            // 
            // buttonMultiNone
            // 
            this.buttonMultiNone.Location = new System.Drawing.Point(167, 34);
            this.buttonMultiNone.Name = "buttonMultiNone";
            this.buttonMultiNone.Size = new System.Drawing.Size(111, 23);
            this.buttonMultiNone.TabIndex = 23;
            this.buttonMultiNone.Text = "MultiNone";
            this.buttonMultiNone.UseVisualStyleBackColor = true;
            this.buttonMultiNone.Click += new System.EventHandler(this.buttonMultiNone_Click);
            // 
            // buttonSingleNone
            // 
            this.buttonSingleNone.Location = new System.Drawing.Point(30, 34);
            this.buttonSingleNone.Name = "buttonSingleNone";
            this.buttonSingleNone.Size = new System.Drawing.Size(111, 23);
            this.buttonSingleNone.TabIndex = 22;
            this.buttonSingleNone.Text = "SingleNone";
            this.buttonSingleNone.UseVisualStyleBackColor = true;
            this.buttonSingleNone.Click += new System.EventHandler(this.buttonSingleNone_Click);
            // 
            // labelMultiThread
            // 
            this.labelMultiThread.AutoSize = true;
            this.labelMultiThread.Location = new System.Drawing.Point(189, 10);
            this.labelMultiThread.Name = "labelMultiThread";
            this.labelMultiThread.Size = new System.Drawing.Size(63, 13);
            this.labelMultiThread.TabIndex = 21;
            this.labelMultiThread.Text = "MultiThread";
            // 
            // labelSingleThread
            // 
            this.labelSingleThread.AutoSize = true;
            this.labelSingleThread.Location = new System.Drawing.Point(52, 10);
            this.labelSingleThread.Name = "labelSingleThread";
            this.labelSingleThread.Size = new System.Drawing.Size(70, 13);
            this.labelSingleThread.TabIndex = 20;
            this.labelSingleThread.Text = "SingleThread";
            // 
            // groupBoxUIThread
            // 
            this.groupBoxUIThread.Controls.Add(this.buttonMultiUIThread);
            this.groupBoxUIThread.Controls.Add(this.buttonMultiUIThreadParams);
            this.groupBoxUIThread.Controls.Add(this.SingleUIThread);
            this.groupBoxUIThread.Location = new System.Drawing.Point(16, 99);
            this.groupBoxUIThread.Name = "groupBoxUIThread";
            this.groupBoxUIThread.Size = new System.Drawing.Size(434, 53);
            this.groupBoxUIThread.TabIndex = 29;
            this.groupBoxUIThread.TabStop = false;
            this.groupBoxUIThread.Text = "This is UIThread solution";
            // 
            // buttonMultiUIThread
            // 
            this.buttonMultiUIThread.Location = new System.Drawing.Point(151, 19);
            this.buttonMultiUIThread.Name = "buttonMultiUIThread";
            this.buttonMultiUIThread.Size = new System.Drawing.Size(111, 23);
            this.buttonMultiUIThread.TabIndex = 12;
            this.buttonMultiUIThread.Text = "MultiUIThread";
            this.buttonMultiUIThread.UseVisualStyleBackColor = true;
            this.buttonMultiUIThread.Click += new System.EventHandler(this.buttonMultiUIThread_Click);
            // 
            // buttonMultiUIThreadParams
            // 
            this.buttonMultiUIThreadParams.Location = new System.Drawing.Point(288, 19);
            this.buttonMultiUIThreadParams.Name = "buttonMultiUIThreadParams";
            this.buttonMultiUIThreadParams.Size = new System.Drawing.Size(130, 23);
            this.buttonMultiUIThreadParams.TabIndex = 15;
            this.buttonMultiUIThreadParams.Text = "MultiUIThreadParams";
            this.buttonMultiUIThreadParams.UseVisualStyleBackColor = true;
            this.buttonMultiUIThreadParams.Click += new System.EventHandler(this.buttonMultiUIThreadParams_Click);
            // 
            // SingleUIThread
            // 
            this.SingleUIThread.Location = new System.Drawing.Point(14, 19);
            this.SingleUIThread.Name = "SingleUIThread";
            this.SingleUIThread.Size = new System.Drawing.Size(111, 23);
            this.SingleUIThread.TabIndex = 11;
            this.SingleUIThread.Text = "SingleUIThread";
            this.SingleUIThread.UseVisualStyleBackColor = true;
            this.SingleUIThread.Click += new System.EventHandler(this.SingleUIThread_Click);
            // 
            // textBoxOut
            // 
            this.textBoxOut.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxOut.Location = new System.Drawing.Point(16, 348);
            this.textBoxOut.Multiline = true;
            this.textBoxOut.Name = "textBoxOut";
            this.textBoxOut.ReadOnly = true;
            this.textBoxOut.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxOut.Size = new System.Drawing.Size(434, 94);
            this.textBoxOut.TabIndex = 30;
            this.textBoxOut.WordWrap = false;
            // 
            // buttonMultiNoneParams
            // 
            this.buttonMultiNoneParams.Location = new System.Drawing.Point(304, 34);
            this.buttonMultiNoneParams.Name = "buttonMultiNoneParams";
            this.buttonMultiNoneParams.Size = new System.Drawing.Size(130, 23);
            this.buttonMultiNoneParams.TabIndex = 33;
            this.buttonMultiNoneParams.Text = "MultiNoneParams";
            this.buttonMultiNoneParams.UseVisualStyleBackColor = true;
            this.buttonMultiNoneParams.Click += new System.EventHandler(this.buttonMultiNoneParams_Click);
            // 
            // FormAvoidingInvokeRequired
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(465, 454);
            this.Controls.Add(this.buttonMultiNoneParams);
            this.Controls.Add(this.textBoxOut);
            this.Controls.Add(this.groupBoxUIThread);
            this.Controls.Add(this.labelMultiThreadWithParams);
            this.Controls.Add(this.buttonMultiStandardParams);
            this.Controls.Add(this.buttonMultiStandard);
            this.Controls.Add(this.buttonSingleStandard);
            this.Controls.Add(this.buttonMultiNone);
            this.Controls.Add(this.buttonSingleNone);
            this.Controls.Add(this.labelMultiThread);
            this.Controls.Add(this.labelSingleThread);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAvoidingInvokeRequired";
            this.Text = "AvoidingInvokeRequired";
            this.groupBoxUIThread.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelMultiThreadWithParams;
        private System.Windows.Forms.Button buttonMultiStandardParams;
        private System.Windows.Forms.Button buttonMultiStandard;
        private System.Windows.Forms.Button buttonSingleStandard;
        private System.Windows.Forms.Button buttonMultiNone;
        private System.Windows.Forms.Button buttonSingleNone;
        private System.Windows.Forms.Label labelMultiThread;
        private System.Windows.Forms.Label labelSingleThread;
        private System.Windows.Forms.GroupBox groupBoxUIThread;
        private System.Windows.Forms.Button buttonMultiUIThread;
        private System.Windows.Forms.Button buttonMultiUIThreadParams;
        private System.Windows.Forms.Button SingleUIThread;
        private System.Windows.Forms.TextBox textBoxOut;
        private System.Windows.Forms.Button buttonMultiNoneParams;
    }
}


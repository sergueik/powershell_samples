namespace ServiceMaster
{
    partial class Option
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.chbSaveLog = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbxDBAddr = new System.Windows.Forms.TextBox();
            this.btnDBAddr = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbxLogAddr = new System.Windows.Forms.TextBox();
            this.btnLogAddr = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chbSaveLog
            // 
            this.chbSaveLog.AutoSize = true;
            this.chbSaveLog.Location = new System.Drawing.Point(12, 26);
            this.chbSaveLog.Name = "chbSaveLog";
            this.chbSaveLog.Size = new System.Drawing.Size(96, 16);
            this.chbSaveLog.TabIndex = 0;
            this.chbSaveLog.Text = "保存优化日志";
            this.chbSaveLog.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "数据库位置";
            // 
            // tbxDBAddr
            // 
            this.tbxDBAddr.Location = new System.Drawing.Point(8, 60);
            this.tbxDBAddr.Name = "tbxDBAddr";
            this.tbxDBAddr.Size = new System.Drawing.Size(234, 21);
            this.tbxDBAddr.TabIndex = 2;
            // 
            // btnDBAddr
            // 
            this.btnDBAddr.Location = new System.Drawing.Point(248, 58);
            this.btnDBAddr.Name = "btnDBAddr";
            this.btnDBAddr.Size = new System.Drawing.Size(32, 23);
            this.btnDBAddr.TabIndex = 3;
            this.btnDBAddr.Text = ">";
            this.btnDBAddr.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(205, 237);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "保存设置";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(124, 237);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "取消修改";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "日志文件位置";
            // 
            // tbxLogAddr
            // 
            this.tbxLogAddr.Location = new System.Drawing.Point(8, 103);
            this.tbxLogAddr.Name = "tbxLogAddr";
            this.tbxLogAddr.Size = new System.Drawing.Size(234, 21);
            this.tbxLogAddr.TabIndex = 7;
            // 
            // btnLogAddr
            // 
            this.btnLogAddr.Location = new System.Drawing.Point(248, 101);
            this.btnLogAddr.Name = "btnLogAddr";
            this.btnLogAddr.Size = new System.Drawing.Size(32, 23);
            this.btnLogAddr.TabIndex = 3;
            this.btnLogAddr.Text = ">";
            this.btnLogAddr.UseVisualStyleBackColor = true;
            // 
            // Option
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 272);
            this.Controls.Add(this.tbxLogAddr);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnLogAddr);
            this.Controls.Add(this.btnDBAddr);
            this.Controls.Add(this.tbxDBAddr);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chbSaveLog);
            this.Name = "Option";
            this.Text = "选项设置";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chbSaveLog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbxDBAddr;
        private System.Windows.Forms.Button btnDBAddr;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbxLogAddr;
        private System.Windows.Forms.Button btnLogAddr;
    }
}
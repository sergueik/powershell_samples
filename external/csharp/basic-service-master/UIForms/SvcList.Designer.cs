namespace ServiceMaster
{
    partial class SvcList
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
            this.label1 = new System.Windows.Forms.Label();
            this.chbAll = new System.Windows.Forms.CheckBox();
            this.btnOutput = new System.Windows.Forms.Button();
            this.chlbSvcs = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-36, -12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "导出当前服务列表配置";
            // 
            // chbAll
            // 
            this.chbAll.AutoSize = true;
            this.chbAll.Location = new System.Drawing.Point(5, 282);
            this.chbAll.Name = "chbAll";
            this.chbAll.Size = new System.Drawing.Size(48, 16);
            this.chbAll.TabIndex = 13;
            this.chbAll.Text = "全选";
            this.chbAll.UseVisualStyleBackColor = true;
            this.chbAll.CheckedChanged += new System.EventHandler(this.chbAll_CheckedChanged);
            // 
            // btnOutput
            // 
            this.btnOutput.Location = new System.Drawing.Point(267, 273);
            this.btnOutput.Name = "btnOutput";
            this.btnOutput.Size = new System.Drawing.Size(88, 32);
            this.btnOutput.TabIndex = 10;
            this.btnOutput.Text = "选择选定项";
            this.btnOutput.UseVisualStyleBackColor = true;
            this.btnOutput.Click += new System.EventHandler(this.btnOutput_Click);
            // 
            // chlbSvcs
            // 
            this.chlbSvcs.CheckOnClick = true;
            this.chlbSvcs.FormattingEnabled = true;
            this.chlbSvcs.Location = new System.Drawing.Point(3, 8);
            this.chlbSvcs.Name = "chlbSvcs";
            this.chlbSvcs.Size = new System.Drawing.Size(363, 260);
            this.chlbSvcs.TabIndex = 11;
            // 
            // SvcList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(367, 304);
            this.Controls.Add(this.chbAll);
            this.Controls.Add(this.btnOutput);
            this.Controls.Add(this.chlbSvcs);
            this.Controls.Add(this.label1);
            this.Name = "SvcList";
            this.Text = "服务列表";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chbAll;
        private System.Windows.Forms.Button btnOutput;
        private System.Windows.Forms.CheckedListBox chlbSvcs;
    }
}
namespace ServiceMaster.UIForms.Guides
{
    partial class GuidePt
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
            this.rbtNoPrinter = new System.Windows.Forms.RadioButton();
            this.rbtPrinter = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.rbtNoPrinter);
            this.splitContainer1.Panel2.Controls.Add(this.rbtPrinter);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Size = new System.Drawing.Size(514, 327);
            this.splitContainer1.SplitterDistance = 203;
            // 
            // rbtNoPrinter
            // 
            this.rbtNoPrinter.AutoSize = true;
            this.rbtNoPrinter.Location = new System.Drawing.Point(45, 97);
            this.rbtNoPrinter.Name = "rbtNoPrinter";
            this.rbtNoPrinter.Size = new System.Drawing.Size(47, 16);
            this.rbtNoPrinter.TabIndex = 10;
            this.rbtNoPrinter.TabStop = true;
            this.rbtNoPrinter.Text = "没有";
            this.rbtNoPrinter.UseVisualStyleBackColor = true;
            // 
            // rbtPrinter
            // 
            this.rbtPrinter.AutoSize = true;
            this.rbtPrinter.Location = new System.Drawing.Point(45, 75);
            this.rbtPrinter.Name = "rbtPrinter";
            this.rbtPrinter.Size = new System.Drawing.Size(35, 16);
            this.rbtPrinter.TabIndex = 9;
            this.rbtPrinter.TabStop = true;
            this.rbtPrinter.Text = "有";
            this.rbtPrinter.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "你的电脑有打印机么？";
            // 
            // GuidePt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.ClientSize = new System.Drawing.Size(514, 327);
            this.Name = "GuidePt";
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton rbtNoPrinter;
        private System.Windows.Forms.RadioButton rbtPrinter;
        private System.Windows.Forms.Label label2;

    }
}

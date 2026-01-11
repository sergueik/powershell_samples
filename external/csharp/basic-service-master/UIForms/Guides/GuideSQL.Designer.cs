namespace ServiceMaster.UIForms.Guides
{
    partial class GuideSQL
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
            this.rbtNoSQL = new System.Windows.Forms.RadioButton();
            this.rbtSQL = new System.Windows.Forms.RadioButton();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.rbtNoSQL);
            this.splitContainer1.Panel2.Controls.Add(this.rbtSQL);
            this.splitContainer1.Panel2.Controls.Add(this.label19);
            this.splitContainer1.Panel2.Controls.Add(this.label18);
            // 
            // rbtNoSQL
            // 
            this.rbtNoSQL.AutoSize = true;
            this.rbtNoSQL.Location = new System.Drawing.Point(43, 187);
            this.rbtNoSQL.Name = "rbtNoSQL";
            this.rbtNoSQL.Size = new System.Drawing.Size(59, 16);
            this.rbtNoSQL.TabIndex = 15;
            this.rbtNoSQL.TabStop = true;
            this.rbtNoSQL.Text = "不需要";
            this.rbtNoSQL.UseVisualStyleBackColor = true;
            // 
            // rbtSQL
            // 
            this.rbtSQL.AutoSize = true;
            this.rbtSQL.Location = new System.Drawing.Point(43, 145);
            this.rbtSQL.Name = "rbtSQL";
            this.rbtSQL.Size = new System.Drawing.Size(47, 16);
            this.rbtSQL.TabIndex = 14;
            this.rbtSQL.TabStop = true;
            this.rbtSQL.Text = "需要";
            this.rbtSQL.UseVisualStyleBackColor = true;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(41, 92);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(161, 12);
            this.label19.TabIndex = 13;
            this.label19.Text = "你需要经常使用SQL Server么";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(41, 54);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(185, 12);
            this.label18.TabIndex = 12;
            this.label18.Text = "系统检测发现你使用了SQL Server";
            // 
            // GuideSQL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.ClientSize = new System.Drawing.Size(518, 332);
            this.Name = "GuideSQL";
            this.Enter += new System.EventHandler(this.GuideSQL_Enter);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton rbtNoSQL;
        private System.Windows.Forms.RadioButton rbtSQL;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;

    }
}

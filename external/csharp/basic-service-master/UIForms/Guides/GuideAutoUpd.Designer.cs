namespace ServiceMaster.UIForms.Guides
{
    partial class GuideAutoUpd
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
            this.rbtNoAutoUpdate = new System.Windows.Forms.RadioButton();
            this.rbtAutoUpdate = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.rbtNoAutoUpdate);
            this.splitContainer1.Panel2.Controls.Add(this.rbtAutoUpdate);
            this.splitContainer1.Panel2.Controls.Add(this.label7);
            this.splitContainer1.Size = new System.Drawing.Size(514, 327);
            this.splitContainer1.SplitterDistance = 203;
            // 
            // rbtNoAutoUpdate
            // 
            this.rbtNoAutoUpdate.AutoSize = true;
            this.rbtNoAutoUpdate.Location = new System.Drawing.Point(19, 110);
            this.rbtNoAutoUpdate.Name = "rbtNoAutoUpdate";
            this.rbtNoAutoUpdate.Size = new System.Drawing.Size(35, 16);
            this.rbtNoAutoUpdate.TabIndex = 10;
            this.rbtNoAutoUpdate.TabStop = true;
            this.rbtNoAutoUpdate.Text = "否";
            this.rbtNoAutoUpdate.UseVisualStyleBackColor = true;
            // 
            // rbtAutoUpdate
            // 
            this.rbtAutoUpdate.AutoSize = true;
            this.rbtAutoUpdate.Location = new System.Drawing.Point(19, 75);
            this.rbtAutoUpdate.Name = "rbtAutoUpdate";
            this.rbtAutoUpdate.Size = new System.Drawing.Size(35, 16);
            this.rbtAutoUpdate.TabIndex = 9;
            this.rbtAutoUpdate.TabStop = true;
            this.rbtAutoUpdate.Text = "是";
            this.rbtAutoUpdate.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 40);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(138, 12);
            this.label7.TabIndex = 8;
            this.label7.Text = "你经常使用XP自动升级？";
            // 
            // GuideAutoUpd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.ClientSize = new System.Drawing.Size(514, 327);
            this.Name = "GuideAutoUpd";
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton rbtNoAutoUpdate;
        private System.Windows.Forms.RadioButton rbtAutoUpdate;
        private System.Windows.Forms.Label label7;
    }
}

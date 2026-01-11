namespace ServiceMaster.UIForms.Guides
{
    partial class GuideTask
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
            this.rbtNoKnownTask = new System.Windows.Forms.RadioButton();
            this.rbtTask = new System.Windows.Forms.RadioButton();
            this.rbtNoTask = new System.Windows.Forms.RadioButton();
            this.label10 = new System.Windows.Forms.Label();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.rbtNoKnownTask);
            this.splitContainer1.Panel2.Controls.Add(this.rbtTask);
            this.splitContainer1.Panel2.Controls.Add(this.rbtNoTask);
            this.splitContainer1.Panel2.Controls.Add(this.label10);
            // 
            // rbtNoKnownTask
            // 
            this.rbtNoKnownTask.AutoSize = true;
            this.rbtNoKnownTask.Location = new System.Drawing.Point(37, 127);
            this.rbtNoKnownTask.Name = "rbtNoKnownTask";
            this.rbtNoKnownTask.Size = new System.Drawing.Size(143, 16);
            this.rbtNoKnownTask.TabIndex = 14;
            this.rbtNoKnownTask.TabStop = true;
            this.rbtNoKnownTask.Text = "不知道什么是计划任务";
            this.rbtNoKnownTask.UseVisualStyleBackColor = true;
            // 
            // rbtTask
            // 
            this.rbtTask.AutoSize = true;
            this.rbtTask.Location = new System.Drawing.Point(37, 83);
            this.rbtTask.Name = "rbtTask";
            this.rbtTask.Size = new System.Drawing.Size(59, 16);
            this.rbtTask.TabIndex = 13;
            this.rbtTask.TabStop = true;
            this.rbtTask.Text = "经常用";
            this.rbtTask.UseVisualStyleBackColor = true;
            // 
            // rbtNoTask
            // 
            this.rbtNoTask.AutoSize = true;
            this.rbtNoTask.Location = new System.Drawing.Point(37, 105);
            this.rbtNoTask.Name = "rbtNoTask";
            this.rbtNoTask.Size = new System.Drawing.Size(107, 16);
            this.rbtNoTask.TabIndex = 12;
            this.rbtNoTask.TabStop = true;
            this.rbtNoTask.Text = "不常用或者不用";
            this.rbtNoTask.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(35, 49);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(125, 12);
            this.label10.TabIndex = 11;
            this.label10.Text = "你常常使用计划任务？";
            // 
            // GuideTask
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.ClientSize = new System.Drawing.Size(518, 332);
            this.Name = "GuideTask";
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton rbtNoKnownTask;
        private System.Windows.Forms.RadioButton rbtTask;
        private System.Windows.Forms.RadioButton rbtNoTask;
        private System.Windows.Forms.Label label10;
    }
}

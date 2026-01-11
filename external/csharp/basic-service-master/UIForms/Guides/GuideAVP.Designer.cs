namespace ServiceMaster.UIForms.Guides
{
    partial class GuideAVP
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
            this.rbtNoAVP = new System.Windows.Forms.RadioButton();
            this.rbtAVP = new System.Windows.Forms.RadioButton();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.rbtNoAVP);
            this.splitContainer1.Panel2.Controls.Add(this.rbtAVP);
            this.splitContainer1.Panel2.Controls.Add(this.label15);
            this.splitContainer1.Panel2.Controls.Add(this.label14);
            this.splitContainer1.Size = new System.Drawing.Size(514, 327);
            this.splitContainer1.SplitterDistance = 203;
            // 
            // rbtNoAVP
            // 
            this.rbtNoAVP.AutoSize = true;
            this.rbtNoAVP.Location = new System.Drawing.Point(45, 170);
            this.rbtNoAVP.Name = "rbtNoAVP";
            this.rbtNoAVP.Size = new System.Drawing.Size(59, 16);
            this.rbtNoAVP.TabIndex = 15;
            this.rbtNoAVP.TabStop = true;
            this.rbtNoAVP.Text = "不需要";
            this.rbtNoAVP.UseVisualStyleBackColor = true;
            // 
            // rbtAVP
            // 
            this.rbtAVP.AutoSize = true;
            this.rbtAVP.Location = new System.Drawing.Point(45, 136);
            this.rbtAVP.Name = "rbtAVP";
            this.rbtAVP.Size = new System.Drawing.Size(47, 16);
            this.rbtAVP.TabIndex = 14;
            this.rbtAVP.TabStop = true;
            this.rbtAVP.Text = "需要";
            this.rbtAVP.UseVisualStyleBackColor = true;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(43, 100);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(173, 12);
            this.label15.TabIndex = 13;
            this.label15.Text = "你真的非常需要使用卡巴斯基么";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(43, 65);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(173, 12);
            this.label14.TabIndex = 12;
            this.label14.Text = "系统检测发现你使用了卡巴斯基";
            // 
            // GuideAVP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.ClientSize = new System.Drawing.Size(514, 327);
            this.Name = "GuideAVP";
            this.Load += new System.EventHandler(this.GuideAVP_Load);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton rbtNoAVP;
        private System.Windows.Forms.RadioButton rbtAVP;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
    }
}

namespace ServiceMaster.UIForms.Guides
{
    partial class GuideVM
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
            this.rbtNoVM = new System.Windows.Forms.RadioButton();
            this.rbtVM = new System.Windows.Forms.RadioButton();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.rbtNoVM);
            this.splitContainer1.Panel2.Controls.Add(this.rbtVM);
            this.splitContainer1.Panel2.Controls.Add(this.label17);
            this.splitContainer1.Panel2.Controls.Add(this.label16);
            this.splitContainer1.Size = new System.Drawing.Size(514, 327);
            this.splitContainer1.SplitterDistance = 203;
            // 
            // rbtNoVM
            // 
            this.rbtNoVM.AutoSize = true;
            this.rbtNoVM.Location = new System.Drawing.Point(34, 161);
            this.rbtNoVM.Name = "rbtNoVM";
            this.rbtNoVM.Size = new System.Drawing.Size(47, 16);
            this.rbtNoVM.TabIndex = 13;
            this.rbtNoVM.TabStop = true;
            this.rbtNoVM.Text = "需要";
            this.rbtNoVM.UseVisualStyleBackColor = true;
            // 
            // rbtVM
            // 
            this.rbtVM.AutoSize = true;
            this.rbtVM.Location = new System.Drawing.Point(34, 130);
            this.rbtVM.Name = "rbtVM";
            this.rbtVM.Size = new System.Drawing.Size(59, 16);
            this.rbtVM.TabIndex = 14;
            this.rbtVM.TabStop = true;
            this.rbtVM.Text = "不需要";
            this.rbtVM.UseVisualStyleBackColor = true;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(32, 95);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(137, 12);
            this.label17.TabIndex = 12;
            this.label17.Text = "你经常使用虚拟机上网么";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(32, 61);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(201, 12);
            this.label16.TabIndex = 11;
            this.label16.Text = "系统检测发现你使用了VMware虚拟机";
            // 
            // GuideVM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.ClientSize = new System.Drawing.Size(514, 327);
            this.Name = "GuideVM";
            this.Enter += new System.EventHandler(this.GuideVM_Enter);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton rbtNoVM;
        private System.Windows.Forms.RadioButton rbtVM;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
    }
}

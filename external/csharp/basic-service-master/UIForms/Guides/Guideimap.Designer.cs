namespace ServiceMaster.UIForms.Guides
{
    partial class Guideimap
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
            this.NoDRW = new System.Windows.Forms.RadioButton();
            this.rbtInCD = new System.Windows.Forms.RadioButton();
            this.rbtNero = new System.Windows.Forms.RadioButton();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.NoDRW);
            this.splitContainer1.Panel2.Controls.Add(this.rbtInCD);
            this.splitContainer1.Panel2.Controls.Add(this.rbtNero);
            this.splitContainer1.Panel2.Controls.Add(this.label12);
            this.splitContainer1.Panel2.Controls.Add(this.label11);
            this.splitContainer1.Size = new System.Drawing.Size(514, 327);
            this.splitContainer1.SplitterDistance = 203;
            // 
            // NoDRW
            // 
            this.NoDRW.AutoSize = true;
            this.NoDRW.Location = new System.Drawing.Point(47, 217);
            this.NoDRW.Name = "NoDRW";
            this.NoDRW.Size = new System.Drawing.Size(83, 16);
            this.NoDRW.TabIndex = 16;
            this.NoDRW.TabStop = true;
            this.NoDRW.Text = "没有刻录机";
            this.NoDRW.UseVisualStyleBackColor = true;
            // 
            // rbtInCD
            // 
            this.rbtInCD.AutoSize = true;
            this.rbtInCD.Location = new System.Drawing.Point(47, 183);
            this.rbtInCD.Name = "rbtInCD";
            this.rbtInCD.Size = new System.Drawing.Size(200, 16);
            this.rbtInCD.TabIndex = 15;
            this.rbtInCD.TabStop = true;
            this.rbtInCD.Text = "有，但是使用Windows的InCD刻录";
            this.rbtInCD.UseVisualStyleBackColor = true;
            // 
            // rbtNero
            // 
            this.rbtNero.AutoSize = true;
            this.rbtNero.Location = new System.Drawing.Point(48, 151);
            this.rbtNero.Name = "rbtNero";
            this.rbtNero.Size = new System.Drawing.Size(214, 16);
            this.rbtNero.TabIndex = 14;
            this.rbtNero.TabStop = true;
            this.rbtNero.Text = "有，但是使用Nero等第三方刻录软件";
            this.rbtNero.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(46, 123);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(185, 12);
            this.label12.TabIndex = 13;
            this.label12.Text = "(包括CD-RW，Combo，DVD-RW)";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(46, 99);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(161, 12);
            this.label11.TabIndex = 12;
            this.label11.Text = "你的电脑的光驱是刻录机么？";
            // 
            // Guideimap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.ClientSize = new System.Drawing.Size(514, 327);
            this.Name = "Guideimap";
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton NoDRW;
        private System.Windows.Forms.RadioButton rbtInCD;
        private System.Windows.Forms.RadioButton rbtNero;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
    }
}

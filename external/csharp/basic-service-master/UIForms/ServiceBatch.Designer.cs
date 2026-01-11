namespace ServiceMaster
{
    partial class ServiceBatch
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
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOutput = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.svcSelect = new System.Windows.Forms.Button();
            this.rbtnDemand = new System.Windows.Forms.RadioButton();
            this.rbtnAuto = new System.Windows.Forms.RadioButton();
            this.rbtnDisable = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chbStart = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.chbStatus = new System.Windows.Forms.CheckBox();
            this.rbtnStart = new System.Windows.Forms.RadioButton();
            this.rbtnStop = new System.Windows.Forms.RadioButton();
            this.chbFileHead = new System.Windows.Forms.CheckBox();
            this.chbFileEnd = new System.Windows.Forms.CheckBox();
            this.lblColor = new System.Windows.Forms.Label();
            this.btnChangeColor = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(3, 24);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(397, 115);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "服务显示名称";
            this.columnHeader1.Width = 266;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "启动类型";
            this.columnHeader2.Width = 63;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "运行状态";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "将要生成批处理的服务列表";
            // 
            // btnOutput
            // 
            this.btnOutput.Location = new System.Drawing.Point(325, 252);
            this.btnOutput.Name = "btnOutput";
            this.btnOutput.Size = new System.Drawing.Size(75, 35);
            this.btnOutput.TabIndex = 2;
            this.btnOutput.Text = "生成批处理";
            this.btnOutput.UseVisualStyleBackColor = true;
            this.btnOutput.Click += new System.EventHandler(this.btnOutput_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(244, 252);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 35);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "清除选择";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // svcSelect
            // 
            this.svcSelect.Location = new System.Drawing.Point(163, 252);
            this.svcSelect.Name = "svcSelect";
            this.svcSelect.Size = new System.Drawing.Size(75, 35);
            this.svcSelect.TabIndex = 4;
            this.svcSelect.Text = "选择服务";
            this.svcSelect.UseVisualStyleBackColor = true;
            this.svcSelect.Click += new System.EventHandler(this.svcSelect_Click);
            // 
            // rbtnDemand
            // 
            this.rbtnDemand.AutoSize = true;
            this.rbtnDemand.Enabled = false;
            this.rbtnDemand.Location = new System.Drawing.Point(143, 6);
            this.rbtnDemand.Name = "rbtnDemand";
            this.rbtnDemand.Size = new System.Drawing.Size(71, 16);
            this.rbtnDemand.TabIndex = 5;
            this.rbtnDemand.Text = "设为手动";
            this.rbtnDemand.UseVisualStyleBackColor = true;
            this.rbtnDemand.CheckedChanged += new System.EventHandler(this.rbtnDemand_CheckedChanged);
            // 
            // rbtnAuto
            // 
            this.rbtnAuto.AutoSize = true;
            this.rbtnAuto.Enabled = false;
            this.rbtnAuto.Location = new System.Drawing.Point(315, 6);
            this.rbtnAuto.Name = "rbtnAuto";
            this.rbtnAuto.Size = new System.Drawing.Size(71, 16);
            this.rbtnAuto.TabIndex = 6;
            this.rbtnAuto.Text = "设为自动";
            this.rbtnAuto.UseVisualStyleBackColor = true;
            this.rbtnAuto.CheckedChanged += new System.EventHandler(this.rbtnAuto_CheckedChanged);
            // 
            // rbtnDisable
            // 
            this.rbtnDisable.AutoSize = true;
            this.rbtnDisable.Enabled = false;
            this.rbtnDisable.Location = new System.Drawing.Point(234, 6);
            this.rbtnDisable.Name = "rbtnDisable";
            this.rbtnDisable.Size = new System.Drawing.Size(71, 16);
            this.rbtnDisable.TabIndex = 7;
            this.rbtnDisable.Text = "设为禁用";
            this.rbtnDisable.UseVisualStyleBackColor = true;
            this.rbtnDisable.CheckedChanged += new System.EventHandler(this.rbtnDisable_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chbStart);
            this.panel1.Controls.Add(this.rbtnDisable);
            this.panel1.Controls.Add(this.rbtnAuto);
            this.panel1.Controls.Add(this.rbtnDemand);
            this.panel1.Location = new System.Drawing.Point(3, 145);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(397, 26);
            this.panel1.TabIndex = 8;
            // 
            // chbStart
            // 
            this.chbStart.AutoSize = true;
            this.chbStart.Location = new System.Drawing.Point(5, 7);
            this.chbStart.Name = "chbStart";
            this.chbStart.Size = new System.Drawing.Size(132, 16);
            this.chbStart.TabIndex = 8;
            this.chbStart.Text = "将列表中的服务全部";
            this.chbStart.UseVisualStyleBackColor = true;
            this.chbStart.CheckedChanged += new System.EventHandler(this.chbStart_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.chbStatus);
            this.panel2.Controls.Add(this.rbtnStart);
            this.panel2.Controls.Add(this.rbtnStop);
            this.panel2.Location = new System.Drawing.Point(3, 173);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(397, 26);
            this.panel2.TabIndex = 9;
            // 
            // chbStatus
            // 
            this.chbStatus.AutoSize = true;
            this.chbStatus.Location = new System.Drawing.Point(5, 4);
            this.chbStatus.Name = "chbStatus";
            this.chbStatus.Size = new System.Drawing.Size(132, 16);
            this.chbStatus.TabIndex = 8;
            this.chbStatus.Text = "将列表中的服务全部";
            this.chbStatus.UseVisualStyleBackColor = true;
            this.chbStatus.CheckedChanged += new System.EventHandler(this.chbStatus_CheckedChanged);
            // 
            // rbtnStart
            // 
            this.rbtnStart.AutoSize = true;
            this.rbtnStart.Enabled = false;
            this.rbtnStart.Location = new System.Drawing.Point(234, 3);
            this.rbtnStart.Name = "rbtnStart";
            this.rbtnStart.Size = new System.Drawing.Size(71, 16);
            this.rbtnStart.TabIndex = 7;
            this.rbtnStart.Text = "设为开始";
            this.rbtnStart.UseVisualStyleBackColor = true;
            this.rbtnStart.CheckedChanged += new System.EventHandler(this.rbtnStart_CheckedChanged);
            // 
            // rbtnStop
            // 
            this.rbtnStop.AutoSize = true;
            this.rbtnStop.Enabled = false;
            this.rbtnStop.Location = new System.Drawing.Point(143, 3);
            this.rbtnStop.Name = "rbtnStop";
            this.rbtnStop.Size = new System.Drawing.Size(71, 16);
            this.rbtnStop.TabIndex = 5;
            this.rbtnStop.Text = "设为停止";
            this.rbtnStop.UseVisualStyleBackColor = true;
            this.rbtnStop.CheckedChanged += new System.EventHandler(this.rbtnStop_CheckedChanged);
            // 
            // chbFileHead
            // 
            this.chbFileHead.AutoSize = true;
            this.chbFileHead.Location = new System.Drawing.Point(8, 205);
            this.chbFileHead.Name = "chbFileHead";
            this.chbFileHead.Size = new System.Drawing.Size(240, 16);
            this.chbFileHead.TabIndex = 10;
            this.chbFileHead.Text = "在批处理文件头处加入“按任意键继续”";
            this.chbFileHead.UseVisualStyleBackColor = true;
            // 
            // chbFileEnd
            // 
            this.chbFileEnd.AutoSize = true;
            this.chbFileEnd.Location = new System.Drawing.Point(8, 227);
            this.chbFileEnd.Name = "chbFileEnd";
            this.chbFileEnd.Size = new System.Drawing.Size(240, 16);
            this.chbFileEnd.TabIndex = 10;
            this.chbFileEnd.Text = "在批处理文件尾处加入“按任意键继续”";
            this.chbFileEnd.UseVisualStyleBackColor = true;
            // 
            // lblColor
            // 
            this.lblColor.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.lblColor.ForeColor = System.Drawing.Color.Snow;
            this.lblColor.Location = new System.Drawing.Point(254, 205);
            this.lblColor.Name = "lblColor";
            this.lblColor.Size = new System.Drawing.Size(70, 38);
            this.lblColor.TabIndex = 11;
            this.lblColor.Text = "批处理文字显示颜色";
            this.lblColor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnChangeColor
            // 
            this.btnChangeColor.Location = new System.Drawing.Point(355, 205);
            this.btnChangeColor.Name = "btnChangeColor";
            this.btnChangeColor.Size = new System.Drawing.Size(45, 38);
            this.btnChangeColor.TabIndex = 12;
            this.btnChangeColor.Text = "更改";
            this.btnChangeColor.UseVisualStyleBackColor = true;
            this.btnChangeColor.Click += new System.EventHandler(this.btnChangeColor_Click);
            // 
            // ServiceBatch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(407, 288);
            this.Controls.Add(this.btnChangeColor);
            this.Controls.Add(this.lblColor);
            this.Controls.Add(this.chbFileEnd);
            this.Controls.Add(this.chbFileHead);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.svcSelect);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOutput);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listView1);
            this.Name = "ServiceBatch";
            this.Text = "ServiceBatch";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOutput;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button svcSelect;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.RadioButton rbtnDemand;
        private System.Windows.Forms.RadioButton rbtnAuto;
        private System.Windows.Forms.RadioButton rbtnDisable;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chbStart;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox chbStatus;
        private System.Windows.Forms.RadioButton rbtnStart;
        private System.Windows.Forms.RadioButton rbtnStop;
        private System.Windows.Forms.CheckBox chbFileHead;
        private System.Windows.Forms.CheckBox chbFileEnd;
        private System.Windows.Forms.Label lblColor;
        private System.Windows.Forms.Button btnChangeColor;
    }
}
namespace PrintToPrinter
{
    partial class FormPrint
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblInfo = new System.Windows.Forms.Label();
            this.printDoc = new System.Drawing.Printing.PrintDocument();
            this.cboLocalPrinters = new System.Windows.Forms.ComboBox();
            this.btnPrint = new System.Windows.Forms.Button();
            this.cboNetworkPrinters = new System.Windows.Forms.ComboBox();
            this.radioLocalPrinters = new System.Windows.Forms.RadioButton();
            this.radioNetworkPrinters = new System.Windows.Forms.RadioButton();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblName = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.lblDriver = new System.Windows.Forms.Label();
            this.lblDeviceID = new System.Windows.Forms.Label();
            this.lblShared = new System.Windows.Forms.Label();
            this.lblNameValue = new System.Windows.Forms.Label();
            this.lblPortValue = new System.Windows.Forms.Label();
            this.lblDriverValue = new System.Windows.Forms.Label();
            this.lblDeviceIDValue = new System.Windows.Forms.Label();
            this.lblSharedValue = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo.Location = new System.Drawing.Point(12, 9);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(221, 13);
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Text = "I want to print to this specific printer: ";
            // 
            // printDoc
            // 
            this.printDoc.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDoc_PrintPage);
            // 
            // cboLocalPrinters
            // 
            this.cboLocalPrinters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLocalPrinters.FormattingEnabled = true;
            this.cboLocalPrinters.Location = new System.Drawing.Point(15, 59);
            this.cboLocalPrinters.Name = "cboLocalPrinters";
            this.cboLocalPrinters.Size = new System.Drawing.Size(255, 21);
            this.cboLocalPrinters.TabIndex = 1;
            this.cboLocalPrinters.SelectedIndexChanged += new System.EventHandler(this.cboLocalPrinters_SelectedIndexChanged);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(195, 175);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 2;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // cboNetworkPrinters
            // 
            this.cboNetworkPrinters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNetworkPrinters.FormattingEnabled = true;
            this.cboNetworkPrinters.Location = new System.Drawing.Point(15, 115);
            this.cboNetworkPrinters.Name = "cboNetworkPrinters";
            this.cboNetworkPrinters.Size = new System.Drawing.Size(255, 21);
            this.cboNetworkPrinters.TabIndex = 5;
            // 
            // radioLocalPrinters
            // 
            this.radioLocalPrinters.AutoSize = true;
            this.radioLocalPrinters.Location = new System.Drawing.Point(15, 36);
            this.radioLocalPrinters.Name = "radioLocalPrinters";
            this.radioLocalPrinters.Size = new System.Drawing.Size(88, 17);
            this.radioLocalPrinters.TabIndex = 6;
            this.radioLocalPrinters.TabStop = true;
            this.radioLocalPrinters.Text = "Local printers";
            this.radioLocalPrinters.UseVisualStyleBackColor = true;
            this.radioLocalPrinters.CheckedChanged += new System.EventHandler(this.radioLocalPrinters_CheckedChanged);
            // 
            // radioNetworkPrinters
            // 
            this.radioNetworkPrinters.AutoSize = true;
            this.radioNetworkPrinters.Location = new System.Drawing.Point(15, 92);
            this.radioNetworkPrinters.Name = "radioNetworkPrinters";
            this.radioNetworkPrinters.Size = new System.Drawing.Size(102, 17);
            this.radioNetworkPrinters.TabIndex = 7;
            this.radioNetworkPrinters.TabStop = true;
            this.radioNetworkPrinters.Text = "Network printers";
            this.radioNetworkPrinters.UseVisualStyleBackColor = true;
            this.radioNetworkPrinters.CheckedChanged += new System.EventHandler(this.radioNetworkPrinters_CheckedChanged);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(33, 175);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 8;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(114, 175);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 9;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblShared);
            this.groupBox1.Controls.Add(this.lblDeviceID);
            this.groupBox1.Controls.Add(this.lblDriver);
            this.groupBox1.Controls.Add(this.lblPort);
            this.groupBox1.Controls.Add(this.lblSharedValue);
            this.groupBox1.Controls.Add(this.lblDeviceIDValue);
            this.groupBox1.Controls.Add(this.lblDriverValue);
            this.groupBox1.Controls.Add(this.lblPortValue);
            this.groupBox1.Controls.Add(this.lblNameValue);
            this.groupBox1.Controls.Add(this.lblName);
            this.groupBox1.Location = new System.Drawing.Point(289, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(335, 188);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Printer informations";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(27, 29);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(38, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name:";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(36, 50);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(29, 13);
            this.lblPort.TabIndex = 0;
            this.lblPort.Text = "Port:";
            // 
            // lblDriver
            // 
            this.lblDriver.AutoSize = true;
            this.lblDriver.Location = new System.Drawing.Point(27, 72);
            this.lblDriver.Name = "lblDriver";
            this.lblDriver.Size = new System.Drawing.Size(38, 13);
            this.lblDriver.TabIndex = 1;
            this.lblDriver.Text = "Driver:";
            // 
            // lblDeviceID
            // 
            this.lblDeviceID.AutoSize = true;
            this.lblDeviceID.Location = new System.Drawing.Point(10, 96);
            this.lblDeviceID.Name = "lblDeviceID";
            this.lblDeviceID.Size = new System.Drawing.Size(55, 13);
            this.lblDeviceID.TabIndex = 1;
            this.lblDeviceID.Text = "DeviceID:";
            // 
            // lblShared
            // 
            this.lblShared.AutoSize = true;
            this.lblShared.Location = new System.Drawing.Point(21, 118);
            this.lblShared.Name = "lblShared";
            this.lblShared.Size = new System.Drawing.Size(44, 13);
            this.lblShared.TabIndex = 1;
            this.lblShared.Text = "Shared:";
            // 
            // lblNameValue
            // 
            this.lblNameValue.AutoSize = true;
            this.lblNameValue.Location = new System.Drawing.Point(71, 29);
            this.lblNameValue.Name = "lblNameValue";
            this.lblNameValue.Size = new System.Drawing.Size(96, 13);
            this.lblNameValue.TabIndex = 0;
            this.lblNameValue.Text = "No printer selected";
            // 
            // lblPortValue
            // 
            this.lblPortValue.AutoSize = true;
            this.lblPortValue.Location = new System.Drawing.Point(71, 50);
            this.lblPortValue.Name = "lblPortValue";
            this.lblPortValue.Size = new System.Drawing.Size(96, 13);
            this.lblPortValue.TabIndex = 0;
            this.lblPortValue.Text = "No printer selected";
            // 
            // lblDriverValue
            // 
            this.lblDriverValue.AutoSize = true;
            this.lblDriverValue.Location = new System.Drawing.Point(71, 72);
            this.lblDriverValue.Name = "lblDriverValue";
            this.lblDriverValue.Size = new System.Drawing.Size(96, 13);
            this.lblDriverValue.TabIndex = 0;
            this.lblDriverValue.Text = "No printer selected";
            // 
            // lblDeviceIDValue
            // 
            this.lblDeviceIDValue.AutoSize = true;
            this.lblDeviceIDValue.Location = new System.Drawing.Point(71, 96);
            this.lblDeviceIDValue.Name = "lblDeviceIDValue";
            this.lblDeviceIDValue.Size = new System.Drawing.Size(96, 13);
            this.lblDeviceIDValue.TabIndex = 0;
            this.lblDeviceIDValue.Text = "No printer selected";
            // 
            // lblSharedValue
            // 
            this.lblSharedValue.AutoSize = true;
            this.lblSharedValue.Location = new System.Drawing.Point(71, 118);
            this.lblSharedValue.Name = "lblSharedValue";
            this.lblSharedValue.Size = new System.Drawing.Size(96, 13);
            this.lblSharedValue.TabIndex = 0;
            this.lblSharedValue.Text = "No printer selected";
            // 
            // FormPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 210);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.radioNetworkPrinters);
            this.Controls.Add(this.radioLocalPrinters);
            this.Controls.Add(this.cboNetworkPrinters);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.cboLocalPrinters);
            this.Controls.Add(this.lblInfo);
            this.Name = "FormPrint";
            this.Text = "Print to specific printer";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInfo;
        private System.Drawing.Printing.PrintDocument printDoc;
        private System.Windows.Forms.ComboBox cboLocalPrinters;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.ComboBox cboNetworkPrinters;
        private System.Windows.Forms.RadioButton radioLocalPrinters;
        private System.Windows.Forms.RadioButton radioNetworkPrinters;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblShared;
        private System.Windows.Forms.Label lblDeviceID;
        private System.Windows.Forms.Label lblDriver;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.Label lblSharedValue;
        private System.Windows.Forms.Label lblDeviceIDValue;
        private System.Windows.Forms.Label lblDriverValue;
        private System.Windows.Forms.Label lblPortValue;
        private System.Windows.Forms.Label lblNameValue;
        private System.Windows.Forms.Label lblName;
    }
}


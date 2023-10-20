namespace Client
{
    partial class frmMain
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
        	this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
        	this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
        	this.btnSynchronousCall = new System.Windows.Forms.Button();
        	this.btnAsynchronousCall = new System.Windows.Forms.Button();
        	this.btnClickNew = new System.Windows.Forms.Button();
        	this.btnClickAnyway = new System.Windows.Forms.Button();
        	this.dgStudents = new System.Windows.Forms.DataGridView();
        	this.tableLayoutPanel1.SuspendLayout();
        	this.tableLayoutPanel2.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.dgStudents)).BeginInit();
        	this.SuspendLayout();
        	// 
        	// tableLayoutPanel1
        	// 
        	this.tableLayoutPanel1.ColumnCount = 1;
        	this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        	this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
        	this.tableLayoutPanel1.Controls.Add(this.dgStudents, 0, 1);
        	this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
        	this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
        	this.tableLayoutPanel1.Name = "tableLayoutPanel1";
        	this.tableLayoutPanel1.RowCount = 2;
        	this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
        	this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        	this.tableLayoutPanel1.Size = new System.Drawing.Size(724, 359);
        	this.tableLayoutPanel1.TabIndex = 0;
        	// 
        	// tableLayoutPanel2
        	// 
        	this.tableLayoutPanel2.ColumnCount = 4;
        	this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
        	this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
        	this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
        	this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
        	this.tableLayoutPanel2.Controls.Add(this.btnSynchronousCall, 0, 0);
        	this.tableLayoutPanel2.Controls.Add(this.btnAsynchronousCall, 1, 0);
        	this.tableLayoutPanel2.Controls.Add(this.btnClickNew, 2, 0);
        	this.tableLayoutPanel2.Controls.Add(this.btnClickAnyway, 3, 0);
        	
        	this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.tableLayoutPanel2.Location = new System.Drawing.Point(4, 4);
        	this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
        	this.tableLayoutPanel2.Name = "tableLayoutPanel2";
        	this.tableLayoutPanel2.RowCount = 1;
        	this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        	this.tableLayoutPanel2.Size = new System.Drawing.Size(716, 41);
        	this.tableLayoutPanel2.TabIndex = 0;
        	// 
        	// btnSynchronousCall
        	// 
        	this.btnSynchronousCall.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.btnSynchronousCall.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.btnSynchronousCall.Location = new System.Drawing.Point(4, 4);
        	this.btnSynchronousCall.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
        	this.btnSynchronousCall.Name = "btnSynchronousCall";
        	this.btnSynchronousCall.Size = new System.Drawing.Size(173, 33);
        	this.btnSynchronousCall.TabIndex = 0;
        	this.btnSynchronousCall.Text = "GET";
        	this.btnSynchronousCall.UseVisualStyleBackColor = true;
        	this.btnSynchronousCall.Click += new System.EventHandler(this.btnSynchronousGetCall_Click);
        	// 
        	// btnAsynchronousCall
        	// 
        	this.btnAsynchronousCall.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.btnAsynchronousCall.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.btnAsynchronousCall.Location = new System.Drawing.Point(185, 4);
        	this.btnAsynchronousCall.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
        	this.btnAsynchronousCall.Name = "btnAsynchronousCall";
        	this.btnAsynchronousCall.Size = new System.Drawing.Size(173, 33);
        	this.btnAsynchronousCall.TabIndex = 1;
        	this.btnAsynchronousCall.Text = "Async GET";
        	this.btnAsynchronousCall.UseVisualStyleBackColor = true;
        	this.btnAsynchronousCall.Click += new System.EventHandler(this.btnAsynchronousCall_Click);
        	
        	// btnClickNew
        	// 
this.btnClickNew.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.btnClickNew.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.btnClickNew.Location = new System.Drawing.Point(476, 4);
        	this.btnClickNew.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
        	this.btnClickNew.Name = "btnClickNew";
        	this.btnClickNew.Size = new System.Drawing.Size(173, 33);
        	this.btnClickNew.TabIndex = 2;
        	this.btnClickNew.Text = "POST";
        	this.btnClickNew.UseVisualStyleBackColor = true;
        	this.btnClickNew.Click += new System.EventHandler(this.btnSynchronousPostCall_Click); 

        	// 
        	// btnClickAnyway
        	// 
        	this.btnClickAnyway.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.btnClickAnyway.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.btnClickAnyway.Location = new System.Drawing.Point(476, 4);
        	this.btnClickAnyway.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
        	this.btnClickAnyway.Name = "btnClickAnyway";
        	this.btnClickAnyway.Size = new System.Drawing.Size(236, 33);
        	this.btnClickAnyway.TabIndex = 2;
        	this.btnClickAnyway.Text = "Async POST";
        	this.btnClickAnyway.UseVisualStyleBackColor = true;
        	this.btnClickAnyway.Click += new System.EventHandler(this.btnClickAnyway_Click);
        	// 
        	// dgStudents
        	// 
        	this.dgStudents.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        	this.dgStudents.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.dgStudents.GridColor = System.Drawing.Color.White;
        	this.dgStudents.Location = new System.Drawing.Point(4, 53);
        	this.dgStudents.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
        	this.dgStudents.Name = "dgStudents";
        	this.dgStudents.ReadOnly = true;
        	this.dgStudents.Size = new System.Drawing.Size(716, 302);
        	this.dgStudents.TabIndex = 1;
        	// 
        	// frmMain
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(724, 359);
        	this.Controls.Add(this.tableLayoutPanel1);
        	this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
        	this.Name = "frmMain";
        	this.Text = "RESTFul Service Test Client";
        	this.tableLayoutPanel1.ResumeLayout(false);
        	this.tableLayoutPanel2.ResumeLayout(false);
        	((System.ComponentModel.ISupportInitialize)(this.dgStudents)).EndInit();
        	this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btnSynchronousCall;
        private System.Windows.Forms.Button btnAsynchronousCall;
        private System.Windows.Forms.Button btnClickAnyway;
        private System.Windows.Forms.Button btnClickNew;
        private System.Windows.Forms.DataGridView dgStudents;
    }
}
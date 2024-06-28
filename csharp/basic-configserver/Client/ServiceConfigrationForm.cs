using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Text;
using System.IO;
using System.ServiceProcess;

namespace Client
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class CustomerForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox txtCustID;
		private System.Windows.Forms.TextBox txtZip;
		private System.Windows.Forms.TextBox txtTel;
		private System.Windows.Forms.TextBox txtCountry;
		private System.Windows.Forms.TextBox txtCity;
		private System.Windows.Forms.TextBox txtHouseNum;
		private System.Windows.Forms.TextBox txtFirstName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem5;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public CustomerForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.txtCustID = new System.Windows.Forms.TextBox();
			this.txtZip = new System.Windows.Forms.TextBox();
			this.txtTel = new System.Windows.Forms.TextBox();
			this.txtCountry = new System.Windows.Forms.TextBox();
			this.txtCity = new System.Windows.Forms.TextBox();
			this.txtHouseNum = new System.Windows.Forms.TextBox();
			this.txtFirstName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// txtCustID
			// 
			this.txtCustID.Location = new System.Drawing.Point(136, 56);
			this.txtCustID.Name = "txtCustID";
			this.txtCustID.TabIndex = 0;
			this.txtCustID.Text = "";
			// 
			// txtZip
			// 
			this.txtZip.Location = new System.Drawing.Point(504, 96);
			this.txtZip.Name = "txtZip";
			this.txtZip.Size = new System.Drawing.Size(168, 20);
			this.txtZip.TabIndex = 1;
			this.txtZip.Text = "textBox2";
			// 
			// txtTel
			// 
			this.txtTel.Location = new System.Drawing.Point(504, 56);
			this.txtTel.Name = "txtTel";
			this.txtTel.Size = new System.Drawing.Size(168, 20);
			this.txtTel.TabIndex = 2;
			this.txtTel.Text = "textBox3";
			// 
			// txtCountry
			// 
			this.txtCountry.Location = new System.Drawing.Point(136, 208);
			this.txtCountry.Name = "txtCountry";
			this.txtCountry.Size = new System.Drawing.Size(176, 20);
			this.txtCountry.TabIndex = 3;
			this.txtCountry.Text = "";
			// 
			// txtCity
			// 
			this.txtCity.Location = new System.Drawing.Point(136, 168);
			this.txtCity.Name = "txtCity";
			this.txtCity.Size = new System.Drawing.Size(176, 20);
			this.txtCity.TabIndex = 4;
			this.txtCity.Text = "";
			// 
			// txtHouseNum
			// 
			this.txtHouseNum.Location = new System.Drawing.Point(136, 128);
			this.txtHouseNum.Name = "txtHouseNum";
			this.txtHouseNum.Size = new System.Drawing.Size(176, 20);
			this.txtHouseNum.TabIndex = 5;
			this.txtHouseNum.Text = "";
			// 
			// txtFirstName
			// 
			this.txtFirstName.Location = new System.Drawing.Point(136, 88);
			this.txtFirstName.Name = "txtFirstName";
			this.txtFirstName.Size = new System.Drawing.Size(176, 20);
			this.txtFirstName.TabIndex = 6;
			this.txtFirstName.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 56);
			this.label1.Name = "label1";
			this.label1.TabIndex = 7;
			this.label1.Text = "Customer ID";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(384, 56);
			this.label2.Name = "label2";
			this.label2.TabIndex = 8;
			this.label2.Text = "Tel";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(24, 208);
			this.label4.Name = "label4";
			this.label4.TabIndex = 10;
			this.label4.Text = "Country";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(384, 96);
			this.label5.Name = "label5";
			this.label5.TabIndex = 11;
			this.label5.Text = "Zip";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(24, 168);
			this.label6.Name = "label6";
			this.label6.TabIndex = 12;
			this.label6.Text = "City";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(24, 128);
			this.label7.Name = "label7";
			this.label7.TabIndex = 13;
			this.label7.Text = "House No";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(24, 88);
			this.label8.Name = "label8";
			this.label8.TabIndex = 14;
			this.label8.Text = "First Name";
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem1});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem2,
																					  this.menuItem3,
																					  this.menuItem4,
																					  this.menuItem5});
			this.menuItem1.Text = "Options";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 0;
			this.menuItem2.Text = "Insert Record";
			this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 1;
			this.menuItem3.Text = "Rollback";
			this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 2;
			this.menuItem4.Text = "Commit";
			this.menuItem4.Click += new System.EventHandler(this.menuItem4_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 3;
			this.menuItem5.Text = "Clear Form";
			// 
			// CustomerForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(728, 266);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtFirstName);
			this.Controls.Add(this.txtHouseNum);
			this.Controls.Add(this.txtCity);
			this.Controls.Add(this.txtCountry);
			this.Controls.Add(this.txtTel);
			this.Controls.Add(this.txtZip);
			this.Controls.Add(this.txtCustID);
			this.Menu = this.mainMenu1;
			this.Name = "CustomerForm";
			this.Text = "Customer Form";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new CustomerForm());
		}

		private void menuItem2_Click(object sender, System.EventArgs e)
		{
			StringBuilder record = new StringBuilder();
			record.Append("CustomerID :" + this.txtCustID.Text);
			record.Append(" | First Name : " + this.txtFirstName.Text );
			record.Append(" | House No : " + this.txtHouseNum.Text);

			StreamWriter swRecord = new StreamWriter("c:\\Transaction.tmp",true);
			swRecord.WriteLine(record);
			swRecord.Flush();
			swRecord.Close();

		}

		private void menuItem3_Click(object sender, System.EventArgs e)
		{
			ServiceController controller = new ServiceController("DBWriter");
			controller.ExecuteCommand(200);
		}

		private void menuItem4_Click(object sender, System.EventArgs e)
		{
			ServiceController controller = new ServiceController("DBWriter");
			controller.ExecuteCommand(201);
		}

	}
}

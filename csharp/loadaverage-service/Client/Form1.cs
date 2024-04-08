using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Text;
using System.IO;
using System.ServiceProcess;

namespace TransactionService {

	public class CustomerForm : Form {
		private String categoryName = null;
		private String counterName = null;
		private ListBox txtCustID;
		private ListBox txtFirstName;
		private Label label1;
		private Label label8;
		private MainMenu mainMenu1;
		private MenuItem menuItem1;
		private MenuItem menuItem2;
		private MenuItem menuItem3;
		private MenuItem menuItem4;
		private MenuItem menuItem5;

		private System.ComponentModel.IContainer components;

		public CustomerForm()
		{
			InitializeComponent();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.txtCustID = new System.Windows.Forms.ListBox();
			this.txtFirstName = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// txtCustID
			// 
			this.txtCustID.ItemHeight = 16;
			this.txtCustID.Location = new System.Drawing.Point(163, 65);
			this.txtCustID.Name = "txtCustID";
			this.txtCustID.Size = new System.Drawing.Size(120, 20);
			this.txtCustID.TabIndex = 0;
			var utility = new Utility();
			
			var names  = utility.CategoryNames;
			this.txtCustID.Items.Clear();
			foreach(var name in names){
				this.txtCustID.Items.Add(name);
			}

			this.txtCustID.SelectedIndexChanged += new System.EventHandler(this.CustID_SelectedIndexChanged);
			// 
			// txtFirstName
			// 
			this.txtFirstName.ItemHeight = 16;
			this.txtFirstName.Location = new System.Drawing.Point(163, 102);
			this.txtFirstName.Name = "txtFirstName";
			this.txtFirstName.Size = new System.Drawing.Size(211, 20);
			this.txtFirstName.TabIndex = 6;
			this.txtFirstName.SelectedIndexChanged += new System.EventHandler(this.FirstName_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(29, 65);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(120, 26);
			this.label1.TabIndex = 7;
			this.label1.Text = "Category Name";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(29, 102);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(120, 26);
			this.label8.TabIndex = 14;
			this.label8.Text = "Counter Name";
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
			this.menuItem2.Click += new System.EventHandler(this.InsertRecord);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 1;
			this.menuItem3.Text = "Rollback";
			this.menuItem3.Click += new System.EventHandler(this.Rollback);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 2;
			this.menuItem4.Text = "Commit";
			this.menuItem4.Click += new System.EventHandler(this.Commit);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 3;
			this.menuItem5.Text = "Clear Form";
			// 
			// CustomerForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.ClientSize = new System.Drawing.Size(479, 166);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtFirstName);
			this.Controls.Add(this.txtCustID);
			this.Menu = this.mainMenu1;
			this.Name = "CustomerForm";
			this.Text = "Customer Form";
			this.ResumeLayout(false);

		}
		#endregion

		[STAThread]
		static void Main()
		{
			Application.Run(new CustomerForm());
		}

		private void InsertRecord(object sender, System.EventArgs e)
		{
			var record = new StringBuilder();
			record.Append("CustomerID :" + this.txtCustID.Text);
			record.Append(" | First Name : " + this.txtFirstName.Text);

			var swRecord = new StreamWriter("c:\\Transaction.tmp", true);
			swRecord.WriteLine(record);
			swRecord.Flush();
			swRecord.Close();

		}

		// The value must be between 128 and 256, inclusive.
		private void Operation(int command)
		{
			var controller = new ServiceController("DBWriter");
			// https://docs.microsoft.com/en-us/dotnet/api/system.serviceprocess.servicecontroller.waitforstatus?view=netframework-4.5
			// https://docs.microsoft.com/en-us/dotnet/api/system.serviceprocess.servicecontrollerstatus?view=netframework-4.5
			controller.WaitForStatus(ServiceControllerStatus.Running);			
			controller.ExecuteCommand(command);
		}

		private void Rollback(object sender, System.EventArgs e)
		{
			const int command = 200;
			Operation(command);
		}

		private void Commit(object sender, System.EventArgs e)
		{
			const int command = 201;
			Operation(command);
		}
		private void CustID_SelectedIndexChanged(object sender, EventArgs e)
		{
			categoryName = txtCustID.GetItemText(txtCustID.SelectedItem);
			var utility = new Utility();
			utility.CategoryName = categoryName;
			var names  = utility.CounterNames;
			this.txtFirstName.Items.Clear();
			foreach(var name in names){
				this.txtFirstName.Items.Add(name);
			}

		}


		private void FirstName_SelectedIndexChanged(object sender, EventArgs e)
		{
			counterName = txtFirstName.GetItemText(txtFirstName.SelectedItem);
		}


	}
}

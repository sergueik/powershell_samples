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
		private TextBox txtCustID;
		private TextBox txtZip;
		private TextBox txtTel;
		private TextBox txtCountry;
		private TextBox txtCity;
		private TextBox txtHouseNum;
		private TextBox txtFirstName;
		private Label label1;
		private Label label2;
		private Label label4;
		private Label label5;
		private Label label6;
		private Label label7;
		private Label label8;
		private MainMenu mainMenu1;
		private MenuItem menuItem1;
		private MenuItem menuItem2;
		private MenuItem menuItem3;
		private MenuItem menuItem4;
		private MenuItem menuItem5;

		private System.ComponentModel.Container components = null;

		public CustomerForm() {
			InitializeComponent();
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		private void InitializeComponent() {
			
			ServiceController[] services = ServiceController.GetServices();
			
			txtCustID = new TextBox();
			txtZip = new TextBox();
			txtTel = new TextBox();
			txtCountry = new TextBox();
			txtCity = new TextBox();
			txtHouseNum = new TextBox();
			txtFirstName = new TextBox();
			label1 = new Label();
			label2 = new Label();
			label4 = new Label();
			label5 = new Label();
			label6 = new Label();
			label7 = new Label();
			label8 = new Label();
			mainMenu1 = new MainMenu();
			menuItem1 = new MenuItem();
			menuItem2 = new MenuItem();
			menuItem3 = new MenuItem();
			menuItem4 = new MenuItem();
			menuItem5 = new MenuItem();
			SuspendLayout();
			// 
			// txtCustID
			// 
			txtCustID.Location = new Point(136, 56);
			txtCustID.Name = "txtCustID";
			txtCustID.TabIndex = 0;
			txtCustID.Text = "";
			// 
			// txtZip
			// 
			txtZip.Location = new Point(504, 96);
			txtZip.Name = "txtZip";
			txtZip.Size = new Size(168, 20);
			txtZip.TabIndex = 1;
			txtZip.Text = "textBox2";
			// 
			// txtTel
			// 
			txtTel.Location = new Point(504, 56);
			txtTel.Name = "txtTel";
			txtTel.Size = new Size(168, 20);
			txtTel.TabIndex = 2;
			txtTel.Text = "textBox3";
			// 
			// txtCountry
			// 
			txtCountry.Location = new Point(136, 208);
			txtCountry.Name = "txtCountry";
			txtCountry.Size = new Size(176, 20);
			txtCountry.TabIndex = 3;
			txtCountry.Text = "";
			// 
			// txtCity
			// 
			txtCity.Location = new Point(136, 168);
			txtCity.Name = "txtCity";
			txtCity.Size = new Size(176, 20);
			txtCity.TabIndex = 4;
			txtCity.Text = "";
			// 
			// txtHouseNum
			// 
			txtHouseNum.Location = new Point(136, 128);
			txtHouseNum.Name = "txtHouseNum";
			txtHouseNum.Size = new Size(176, 20);
			txtHouseNum.TabIndex = 5;
			txtHouseNum.Text = "";
			// 
			// txtFirstName
			// 
			txtFirstName.Location = new Point(136, 88);
			txtFirstName.Name = "txtFirstName";
			txtFirstName.Size = new Size(176, 20);
			txtFirstName.TabIndex = 6;
			txtFirstName.Text = "";
			// 
			// label1
			// 
			label1.Location = new Point(24, 56);
			label1.Name = "label1";
			label1.TabIndex = 7;
			label1.Text = "Customer ID";
			// 
			// label2
			// 
			label2.Location = new Point(384, 56);
			label2.Name = "label2";
			label2.TabIndex = 8;
			label2.Text = "Tel";
			// 
			// label4
			// 
			label4.Location = new Point(24, 208);
			label4.Name = "label4";
			label4.TabIndex = 10;
			label4.Text = "Country";
			// 
			// label5
			// 
			label5.Location = new Point(384, 96);
			label5.Name = "label5";
			label5.TabIndex = 11;
			label5.Text = "Zip";
			// 
			// label6
			// 
			label6.Location = new Point(24, 168);
			label6.Name = "label6";
			label6.TabIndex = 12;
			label6.Text = "City";
			// 
			// label7
			// 
			label7.Location = new Point(24, 128);
			label7.Name = "label7";
			label7.TabIndex = 13;
			label7.Text = "House No";
			// 
			// label8
			// 
			label8.Location = new Point(24, 88);
			label8.Name = "label8";
			label8.TabIndex = 14;
			label8.Text = "First Name";
			// 
			// mainMenu1
			// 
			mainMenu1.MenuItems.AddRange(new MenuItem[] {
				menuItem1
			});
			// 
			// menuItem1
			// 
			menuItem1.Index = 0;
			menuItem1.MenuItems.AddRange(new MenuItem[] {
				menuItem2,
				menuItem3,
				menuItem4,
				menuItem5
			});
			menuItem1.Text = "Options";
			// 
			// menuItem2
			// 
			menuItem2.Index = 0;
			menuItem2.Text = "Insert Record";
			menuItem2.Click += new System.EventHandler(InsertRecord);
			// 
			// menuItem3
			// 
			menuItem3.Index = 1;
			menuItem3.Text = "Rollback";
			menuItem3.Click += new System.EventHandler(Rollback);
			// 
			// menuItem4
			// 
			menuItem4.Index = 2;
			menuItem4.Text = "Commit";
			menuItem4.Click += new System.EventHandler(Commit);
			// 
			// menuItem5
			// 
			menuItem5.Index = 3;
			menuItem5.Text = "Clear Form";
			// 
			// CustomerForm
			// 
			AutoScaleBaseSize = new Size(5, 13);
			ClientSize = new Size(728, 266);
			Controls.Add(label8);
			Controls.Add(label7);
			Controls.Add(label6);
			Controls.Add(label5);
			Controls.Add(label4);
			Controls.Add(label2);
			Controls.Add(label1);
			Controls.Add(txtFirstName);
			Controls.Add(txtHouseNum);
			Controls.Add(txtCity);
			Controls.Add(txtCountry);
			Controls.Add(txtTel);
			Controls.Add(txtZip);
			Controls.Add(txtCustID);
			Menu = mainMenu1;
			Name = "CustomerForm";
			Text = "Customer Form";
			ResumeLayout(false);

		}
		#endregion

		[STAThread]
		static void Main() {
			Application.Run(new CustomerForm());
		}

		private void InsertRecord(object sender, System.EventArgs e) {
			var record = new StringBuilder();
			record.Append("CustomerID :" + this.txtCustID.Text);
			record.Append(" | First Name : " + this.txtFirstName.Text);
			record.Append(" | House No : " + this.txtHouseNum.Text);

			var swRecord = new StreamWriter("c:\\Transaction.tmp", true);
			swRecord.WriteLine(record);
			swRecord.Flush();
			swRecord.Close();

		}

		// The value must be between 128 and 256, inclusive.
		private void Operation(int command) {
			var controller = new ServiceController("DBWriter");
			// https://docs.microsoft.com/en-us/dotnet/api/system.serviceprocess.servicecontroller.waitforstatus?view=netframework-4.5
			// https://docs.microsoft.com/en-us/dotnet/api/system.serviceprocess.servicecontrollerstatus?view=netframework-4.5
			controller.WaitForStatus(ServiceControllerStatus.Running);			
			controller.ExecuteCommand(command);
		}

		private void Rollback(object sender, System.EventArgs e) {
			const int command = 200;
			Operation(command);
		}

		private void Commit(object sender, System.EventArgs e) {
			const int command = 201;
			Operation(command);
		}

	}
}

using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using Utils;
using System.Data;

// based on: https://learn.microsoft.com/en-us/answers/questions/1358539/get-chm-title
namespace Program {

	public partial class Control : Form {

		private Button button1;
		private Button button2;
		private Button button3;
		private OpenFileDialog openFileDialog1;
		private TextBox textBox1;
		private DataSet dataSet;
		private DataGrid dataGrid;
		private DataGridTableStyle dataGridTableStyle;
		private DataGridTextBoxColumn textCol;
		private Label versionLabel;
		private Label lblImage;
		private const string versionString = "0.2.0";
		private const string initialDirectory = @"C:\";
		private	string file = @"c:\Program Files\Oracle\VirtualBox\VirtualBox.chm";

		[STAThread]
		public static void Main() {
			// use GDI
			Application.SetCompatibleTextRenderingDefault(false);

			Application.EnableVisualStyles();
			Application.Run(new Control());
		}

		public Control() {
			InitializeComponent();
		}

		private void InitializeComponent() {
			SuspendLayout();

			openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			openFileDialog1.InitialDirectory = initialDirectory;
			openFileDialog1.RestoreDirectory = true;
			openFileDialog1.Title = "Browse Chm Files";
			openFileDialog1.DefaultExt = "chm";
			openFileDialog1.Filter = "chm files (*.chm)|*.chm|All files (*.*)|*.*";
			openFileDialog1.FilterIndex = 0;
			openFileDialog1.CheckFileExists = true;
			openFileDialog1.CheckPathExists = true;
			openFileDialog1.Multiselect = false;

			button1 = new Button();
			button1.Location = new Point(30, 34);
			button1.Name = "button1";
			button1.Size = new Size(90, 23);
			button1.TabIndex = 1;
			button1.Text = "Open";
			button1.Click += button1_Click;
			Controls.Add(button1);

			button2 = new Button();
			button2.Location = new Point(130, 34);
			button2.Name = "button2";
			button2.Size = new Size(90, 23);
			button2.TabIndex = 2;
			button2.Text = "Title";
			button2.Click += button2_Click;
			Controls.Add(button2);

			button3 = new Button();
			button3.Location = new Point(230, 34);
			button3.Name = "button3";
			button3.Size = new Size(90, 23);
			button3.TabIndex = 1;
			button3.Text = "List";
			button3.Click += button3_Click;
			Controls.Add(button3);
			
			textBox1 = new TextBox();
			textBox1.Location = new Point(30, 7);
			textBox1.Name = "textBox1";
			textBox1.Top = 7;
			textBox1.Left = 30;
			textBox1.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			Controls.Add(textBox1);

			textBox1.Size = new Size(200, 23);
			textBox1.TabIndex = 3;
			textBox1.Text = "";

			dataGrid = new DataGrid();
			((ISupportInitialize)(dataGrid)).BeginInit();

			dataGridTableStyle = new DataGridTableStyle();
			textCol = new DataGridTextBoxColumn();
			dataGrid.DataMember = "";
			dataGrid.HeaderForeColor = SystemColors.ControlText;
			dataGrid.Location = new Point(8, 58);
			dataGrid.Margin = new Padding(4);
			dataGrid.Name = "dataGrid";
			dataGrid.Size = new Size(333, 332);
			dataGrid.TabIndex = 1;
			dataGrid.TableStyles.AddRange(new DataGridTableStyle[] {
			dataGridTableStyle});
			dataGridTableStyle.AlternatingBackColor = Color.LightGray;
			dataGridTableStyle.DataGrid = dataGrid;
			dataGridTableStyle.GridColumnStyles.AddRange(new DataGridColumnStyle[] {textCol});
			dataGridTableStyle.HeaderForeColor = SystemColors.ControlText;
			dataGridTableStyle.MappingName = "Hosts";
			textCol.Format = "";
			textCol.FormatInfo = null;
			textCol.HeaderText = "hostname";
			textCol.MappingName = "hostname";
			textCol.Width = 300;

			versionLabel = new Label();
			versionLabel.BorderStyle = BorderStyle.None;
			versionLabel.Location = new Point(236, 399);
			versionLabel.Name = "versionLabel";
			versionLabel.Size = new Size(105, 23);
			versionLabel.Text = String.Format("Version: {0}",versionString);
			Controls.Add(versionLabel);

			AutoScaleDimensions = new SizeF(8F, 16F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(348, 429);
			Controls.Add(dataGrid);

			this.ResumeLayout(false);
			this.PerformLayout();
		}

		private void button1_Click(object sender, EventArgs e) {
			var dr = this.openFileDialog1.ShowDialog();
			if (dr == System.Windows.Forms.DialogResult.OK) {
				foreach (String fileName in openFileDialog1.FileNames)
					textBox1.Text = fileName;
			}
		}

		private void MakeDataSet() {
			dataSet = new DataSet("DataSet");
			var dataTable = new DataTable("Hosts");

			// Create two columns, and add them to the first table.
			var cHostId = new DataColumn("HostId", typeof(int));
			var chostname = new DataColumn("hostname");
			dataTable.Columns.Add(cHostId);
			dataTable.Columns.Add(chostname);

			// Add the tables to the DataSet.
			dataSet.Tables.Add(dataTable);

			DataRow newRow1;

			for (int i = 1; i < 5; i++) {
				newRow1 = dataTable.NewRow();
				newRow1["HostId"] = i;
				// Add the row to the Hosts table.
				dataTable.Rows.Add(newRow1);
			}
			dataTable.Rows[0]["hostname"] = "host1";
		}

		private void button2_Click(object sender, EventArgs eventArgs) {        
			String title = Chm.title(file); 
			// TODO: sender
			if (title != null)
				MessageBox.Show("Title = " + title, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

		}

		private void button3_Click(object sender, EventArgs eventArgs) {        
			try {
				Chm.Urls(file);
			} catch( Exception e) {
				MessageBox.Show(e.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}
	}
}


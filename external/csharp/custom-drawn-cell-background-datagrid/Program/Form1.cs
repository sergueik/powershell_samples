using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.OleDb;

namespace PaintCellsOfGrid {
	public class Form1 : Form {
		private DataGrid grid;
		private Button button;
		private DataSet dataSet;
		private DataGridTableStyle dataGridTableStyle;
		private System.ComponentModel.Container components = null;

		[STAThread]
		static void Main() {
			Application.Run(new Form1());
		}

		public Form1() {
			InitializeComponent();
			try {
				const string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Test.mdb";
				
				var connection = new OleDbConnection(connectionString);
				const string query = "SELECT * FROM Table1";
				dataSet = new DataSet(); 
				connection.Open();
				var myOleDbAdapter = new OleDbDataAdapter(query, connection);
				myOleDbAdapter.Fill(dataSet,"Table1");

				CreateDataGridStyle();
				grid.TableStyles.Add(dataGridTableStyle);

				grid.SetDataBinding(dataSet,"Table1");

			} catch(Exception e) {
				MessageBox.Show("Exception:" + e.ToString());
			}
		}

		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		private void InitializeComponent() {
			grid = new DataGrid();
			button = new Button();
			((System.ComponentModel.ISupportInitialize)(grid)).BeginInit();
			SuspendLayout();
			grid.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom) 
				| AnchorStyles.Left) 
				| AnchorStyles.Right);
			grid.DataMember = "";
			grid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			grid.Name = "grid";
			grid.ReadOnly = true;
			grid.Size = new Size(368, 224);
			grid.TabIndex = 0;

			button.Location = new Point(136, 248);
			button.Name = "button";
			button.TabIndex = 1;
			button.Text = "Exit";
			// button.Click += new System.EventHandler(button_Click);

			button.Click += (object sender, EventArgs e) => { Application.Exit(); };

			AutoScaleBaseSize = new Size(5, 13);
			ClientSize = new Size(368, 293);
			Controls.AddRange(new Control[] { button, grid } );
			Name = "Form1";
			Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(grid)).EndInit();
			ResumeLayout(false);
		}
		private void button_Click(object sender, System.EventArgs e) {
			Application.Exit();
		}

		private void CreateDataGridStyle()  {
			DataGridColumnStyle GridDelColumn;
			DataGridColumnStyle GridSeqStyle;
			dataGridTableStyle = new DataGridTableStyle();

			dataGridTableStyle.MappingName = "Table1";

			GridSeqStyle = new DataGridTextBoxColumn();
			GridSeqStyle.MappingName = "Column1";
			GridSeqStyle.HeaderText = "Column1";
			GridSeqStyle.Width = 100;
			dataGridTableStyle.GridColumnStyles.Add(GridSeqStyle);

			System.ComponentModel.PropertyDescriptorCollection pcol = this.BindingContext[dataSet, "Table1"].GetItemProperties();

			GridDelColumn = new ColumnStyle(pcol["Table1"]);
			GridDelColumn.MappingName = "Column2";
			GridDelColumn.HeaderText = "Column2";
			GridDelColumn.Width = 100;
			dataGridTableStyle.GridColumnStyles.Add(GridDelColumn);

			dataGridTableStyle.AllowSorting         = true;
			dataGridTableStyle.RowHeadersVisible    = true;
		}
	}
}

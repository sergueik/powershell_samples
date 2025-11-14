using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Collections.Generic;

using Utils;

// based on: https://learn.microsoft.com/en-us/answers/questions/1358539/get-chm-title
namespace Program {

	public partial class Control : Form {

		private Button button1;
		private Button button2;
		private Button button3;
		private OpenFileDialog openFileDialog1;
		private TextBox textBox1;
		private DataSet dataSet;
		private DataTable dataTable;
		private DataGrid dataGrid;
		private DataGridTableStyle dataGridTableStyle;
		private DataGridTextBoxColumn textCol;
		private DataGridBoolColumn checkCol;
		// private CheckBoxDataGridColumn checkCol;

		private Label versionLabel;
		private Label lblImage;
		private const string versionString = "0.4.0";
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

			checkCol = new DataGridBoolColumn();
			// checkCol = new CheckBoxDataGridColumn();
    		checkCol.HeaderText = "Select";
			checkCol.MappingName = "selected";
			checkCol.Width = 50;

			dataGridTableStyle.GridColumnStyles.AddRange(new DataGridColumnStyle[] { checkCol, textCol});
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

		private void MakeDataSet(List<string> files) {

			dataSet = new DataSet("DataSet");
			dataTable = new DataTable("Hosts");

			var selected = new DataColumn("selected", typeof(bool));
			selected.DefaultValue = false;
			dataTable.Columns.Add(selected);
			dataTable.Columns.Add(new DataColumn("HostId", typeof(int)));
			dataTable.Columns.Add(new DataColumn("hostname"));
			dataSet.Tables.Add(dataTable);

			DataRow newRow;

			for (int index = 1; index < files.Count ; index++) {
				newRow = dataTable.NewRow();
				newRow["HostId"] = index;
				newRow["hostname"] = files[index];
				dataTable.Rows.Add(newRow);
			}
			dataGrid.DataSource = dataTable;
		}

		private void button2_Click(object sender, EventArgs eventArgs) {
			String title = Chm.title(file);
			// TODO: sender
			if (title != null)
				MessageBox.Show("Title = " + title, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

		}

		private void button3_Click(object sender, EventArgs eventArgs) {
			try {
				var files = Chm.Urls(file);
				MakeDataSet(files);
			} catch( Exception e) {
				MessageBox.Show(e.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		// this only work with CheckBoxDataGridColumn
		/*
		private void dataGrid_MouseDown(object sender, MouseEventArgs mouseEventArgs) {
		    DataGrid.HitTestInfo hit = dataGrid.HitTest(mouseEventArgs.X, mouseEventArgs.Y);
		
		    if (hit.Type == DataGrid.HitTestType.ColumnHeader &&
		        hit.Column == 0) {
		        checkCol.ToggleSelectAll(dataTable);
		        dataGrid.Invalidate();
		    }
		
		}
		*/
	}
	// customization od DataGrid
	
	public class CheckBoxDataGridColumn : DataGridTextBoxColumn {
	    private bool selectAllChecked = false;
	    private readonly Bitmap checkedBitmap = SystemIcons.Shield.ToBitmap();
	    private readonly Bitmap uncheckedBitmap = SystemIcons.Application.ToBitmap();
	
	    public event EventHandler SelectAllChanged;
	
	    protected override void Paint( Graphics graphics, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight) {
	        base.Paint(graphics, bounds, source, rowNum, backBrush, foreBrush, alignToRight);
	
	        // draw checkbox inside the cell
	        bool value = Convert.ToBoolean(GetColumnValueAtRow(source, rowNum));
	        ControlPaint.DrawCheckBox(
	            graphics,
	            new Rectangle(bounds.X + 2, bounds.Y + 2, 12, 12),
	            value ? ButtonState.Checked : ButtonState.Normal);
	    }
	
	    public void PaintHeader(Graphics graphics, Rectangle bounds) {
	        ControlPaint.DrawCheckBox(
	            graphics,
	            new Rectangle(bounds.X + 2, bounds.Y + 2, 12, 12),
	            selectAllChecked ? ButtonState.Checked : ButtonState.Normal);
	    }
			
		public void ToggleSelectAll(DataTable table) {
		    selectAllChecked = !selectAllChecked;
		
		    foreach (DataRow row in table.Rows) {
		        row[this.MappingName] = selectAllChecked;
		    }
		
		    if (SelectAllChanged != null) {
		        SelectAllChanged(this, EventArgs.Empty);
		    }
		}

	}

}


using System;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Collections.Generic;
using Serilog;

using Utils;

/**
 * Copyright 2025 Serguei Kouzmine
 */
 
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
		private CheckBoxDataGridColumn checkHeaderCol;
		private bool selectAll = false;
		private Label versionLabel;
		private Label lblImage;
		private const string versionString = "0.6.0";
		private const string initialDirectory = @"C:\";
		private IniFile iniFile = IniFile.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini"));
		private	string file = @"c:\Program Files\Oracle\VirtualBox\VirtualBox.chm";
		
		private DataGridTableStyle tableStyle;

		[STAThread]
		public static void Main() {
			// use GDI
			Application.SetCompatibleTextRenderingDefault(false);

			Application.EnableVisualStyles();
			Application.Run(new Control());
		}

		public Control() {
			
			
		// initialize Serilog once at app start
		Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Seq("http://localhost:5341", apiKey: null).CreateLogger();
		
			InitializeComponent();
		}

		private string readValue(string section, string key, string defaultValue) {
			var	value = defaultValue;
			try {
				value = iniFile[section][key];
				if (value == null)
					value = defaultValue;
			} catch (Exception) {
				// ignore
			}
			return value;
		}
		

		private void InitializeComponent() {
			SuspendLayout();

			string fileName = readValue("CHM", "fileName", "PowerCollections.chm");
			string astBrowseDir  = readValue("CHM","lastBrowseDir",AppDomain.CurrentDomain.BaseDirectory);			
			file = Path.Combine(astBrowseDir, fileName );
			
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
    		checkCol.HeaderText = "Select";
			checkCol.MappingName = "selected";
			checkCol.Width = 50;
			checkCol.HeaderText = "";
			
			checkHeaderCol = new CheckBoxDataGridColumn();

			dataGrid.MouseDown += dataGrid_MouseDown;
    		dataGrid.Paint += dataGrid_Paint;
    		
			   // ---- checkbox column ----
		    checkHeaderCol = new CheckBoxDataGridColumn();
		    checkHeaderCol.MappingName = "IsSelected";
		    checkHeaderCol.HeaderText = "";
		    checkHeaderCol.Width = 30;
			dataGridTableStyle.GridColumnStyles.AddRange(new DataGridColumnStyle[] { checkCol, textCol });

			dataGridTableStyle.HeaderForeColor = SystemColors.ControlText;
			dataGridTableStyle.MappingName = "Hosts";
			tableStyle = new DataGridTableStyle();
		    tableStyle.MappingName = "Hosts";   // MUST MATCH DataTable.TableName

			textCol.Format = "";
			textCol.FormatInfo = null;
			textCol.HeaderText = "hostname";
			textCol.MappingName = "hostname";
			textCol.Width = 300;
			// dataGrid.MouseDown += new MouseEventHandler(this.dataGrid_MouseDown);
			Controls.Add(dataGrid);

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
	
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		private void button1_Click(object sender, EventArgs e) {
			var dr = this.openFileDialog1.ShowDialog();
			if (dr == System.Windows.Forms.DialogResult.OK) {
				foreach (String fileName in openFileDialog1.FileNames)
					textBox1.Text = fileName;
					file= textBox1.Text;
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
			if (title != null)
				MessageBox.Show("Title = " + title, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void button3_Click(object sender, EventArgs eventArgs) {
			List<string> files = new List<string>();

			try {		
		        files  = Chm.urls_structured(file);
			} catch( Exception e) {
				MessageBox.Show(e.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
	        if (files.Count == 0) {
				files  = Chm.urls_7zip(file);
			}
				
			if (files.Count > 0) {
				MakeDataSet(files);
			}
		}

		private void dataGrid_Paint(object sender, PaintEventArgs e){
		    // Ensure we have a table and at least one column
		    if (dataGrid.DataSource == null || dataGridTableStyle.GridColumnStyles.Count == 0)
		        return;
		
		    // Ensure column index 0 exists in the style
		    if (dataGridTableStyle.GridColumnStyles.Count <= 0)
		        return;
		
		    Rectangle rect;
		    try {
		        // For header, row = -1, column = 0 (first column)
		        rect = dataGrid.GetCellBounds(0, -1);
		    } catch {
		        // Safe fallback: do nothing if bounds cannot be retrieved
		        return;
		    }
		
		    if (rect.Width <= 0 || rect.Height <= 0)
		        return;
		
		    Rectangle cbRect = new Rectangle(rect.X + 4, rect.Y + 3, 14, 14);
		    ControlPaint.DrawCheckBox(
		        e.Graphics,
		        cbRect,
		        selectAll ? ButtonState.Checked : ButtonState.Normal
		    );
		    
		     // Draw the "Select All" text to the right of the checkbox
		    string headerText = "Select All";
		    using (Brush textBrush = new SolidBrush(dataGridTableStyle.HeaderForeColor)) {
		        Rectangle textRect = new Rectangle(cbRect.Right + 2, rect.Y, rect.Width - cbRect.Right - 2, rect.Height);
		        StringFormat stringFormat = new StringFormat  {
		            LineAlignment = StringAlignment.Center,
		            Alignment = StringAlignment.Near
		        };
		        e.Graphics.DrawString(headerText, dataGrid.Font, textBrush, textRect, stringFormat);
		    }
		    
		}
	
		private void dataGrid_MouseDown(object sender, MouseEventArgs mouseEventArgs) {
		    DataGrid.HitTestInfo hit = dataGrid.HitTest(mouseEventArgs.X, mouseEventArgs.Y);
		
		    if (hit.Type == DataGrid.HitTestType.ColumnHeader && hit.Column == 0) {
		        selectAll = !selectAll;
		
		        foreach (DataRow row in dataTable.Rows) {
		            row["selected"] = selectAll;  // use your existing column
		        }
		
		        dataGrid.Invalidate();
		    }
		}
		
	}
	

	// customization od DataGrid
	
	public class CheckBoxDataGridColumn : DataGridColumnStyle {
	    private bool selectAllChecked = false;
	    private readonly Bitmap checkedBitmap = SystemIcons.Shield.ToBitmap();
	    private readonly Bitmap uncheckedBitmap = SystemIcons.Application.ToBitmap();
	
	    public event EventHandler SelectAllChanged;
	
	    protected override void Abort(int rowNum) {
    	}
    	
	    protected override bool Commit(CurrencyManager dataSource, int rowNum) {
        	return true;
    	}
	    
	    protected override void Edit(CurrencyManager source, int rowNum, Rectangle bounds,
                                 bool readOnly, string instantText, bool cellIsVisible) {
    	}
	    
	    protected override Size GetPreferredSize(Graphics g, object value) {
	        return new Size(20, 20);
	    }

		protected override int GetMinimumHeight() {
	        return 20;
	    }
	
	    protected override int GetPreferredHeight(Graphics g, object value) {
	        return 20;
	    }
	
		protected override void Paint(Graphics graphics, Rectangle bounds, CurrencyManager source, int rowNum, bool alignToRight) {
	        Paint(graphics, bounds, source, rowNum, Brushes.White, Brushes.Black, alignToRight);
	    }
	    protected override void Paint(Graphics graphics, Rectangle bounds, CurrencyManager source, int rowNum) {
	        Paint(graphics, bounds, source, rowNum, Brushes.White, Brushes.Black, false);
	    }
	    protected override void Paint(Graphics graphics, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight) {
	        graphics.FillRectangle(backBrush, bounds);
	
	        object val = this.GetColumnValueAtRow(source, rowNum);
	        bool isChecked = false;
	
	        if (val is bool)
	            isChecked = (bool)val;
	
	        // Draw checkbox
	        var checkboxRectangle = new Rectangle(bounds.X + 4, bounds.Y + 2, 14, 14);
	        ControlPaint.DrawCheckBox(graphics, checkboxRectangle,
	            isChecked ? ButtonState.Checked : ButtonState.Normal);
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


using System;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Collections.Generic;
	
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Elasticsearch.Net;
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
		private const string versionString = "0.10.1";
		private const string initialDirectory = @"C:\";
		private IniFile iniFile = IniFile.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini"));
		private	string file = @"c:\Program Files\Oracle\VirtualBox\VirtualBox.chm";
		private static LoggerConfiguration loggerConfiguration = null;
		private DataGridTableStyle tableStyle;
		private const string endpoint = "http://192.168.99.100:9200"; // "http://localhost:9200"
		// docker-machine ip

		[STAThread]
		public static void Main() {
			// use GDI
			Application.SetCompatibleTextRenderingDefault(false);
			Application.EnableVisualStyles();

			ConfigureLogging();
			Telemetry.init();

			Log.Information("Application started.");

			Application.Run(new Control());
		}
		
	    static void ConfigureLogging() {
        var options = new ElasticsearchSinkOptions(new Uri(endpoint)) {
            DetectElasticsearchVersion = false,
            AutoRegisterTemplate = true,
            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
            IndexFormat = "serilog-app",
            BatchPostingLimit = 1,
            QueueSizeLimit = 1000
        };

        // options.ModifyConnectionSettings = conn => conn.BasicAuthentication("elastic", "5mOz5+0BJKzXNyxHcZ*D");
        // NOTE: OptionalSystem.TypeInitializationException: 
        // The type initializer for 'Elasticsearch.Net.DiagnosticsSerializerProxy' threw an exception. 
        // ---> System.IO.FileLoadException: Could not load file or assembly 'System.Diagnostics.DiagnosticSource, Version=4.0.3.1, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51' or one of its dependencies. 
        // The located assembly's manifest definition does not match the assembly reference.
        
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Elasticsearch(options)
            .CreateLogger();
    }
		public Control() {
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

			string fileName = readValue("CHM", "fileName", "api.chm");
			string lastBrowseDir  = readValue("CHM","lastBrowseDir", "");
			if (lastBrowseDir.Equals(""))
				lastBrowseDir = AppDomain.CurrentDomain.BaseDirectory;
			file = Path.Combine(lastBrowseDir, fileName );

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
			button1.Size = new System.Drawing.Size(90, 23);
			button1.TabIndex = 1;
			button1.Text = "Open";
			button1.Click += button1_Click;
			Controls.Add(button1);

			button2 = new Button();
			button2.Location = new Point(130, 34);
			button2.Name = "button2";
			button2.Size = new System.Drawing.Size(90, 23);
			button2.TabIndex = 2;
			button2.Text = "Title";
			button2.Click += button2_Click;
			Controls.Add(button2);

			button3 = new Button();
			button3.Location = new Point(230, 34);
			button3.Name = "button3";
			button3.Size = new System.Drawing.Size(90, 23);
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

			textBox1.Size = new System.Drawing.Size(200, 23);
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
			dataGrid.Size = new System.Drawing.Size(333, 332);
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
			dataGridTableStyle.MappingName = "files";
			tableStyle = new DataGridTableStyle();
		    tableStyle.MappingName = "files";   // MUST MATCH DataTable.TableName

			textCol.Format = "";
			textCol.FormatInfo = null;
			textCol.HeaderText = "filename";
			textCol.MappingName = "filename";
			textCol.Width = 300;
			// dataGrid.MouseDown += new MouseEventHandler(this.dataGrid_MouseDown);
			Controls.Add(dataGrid);

			versionLabel = new Label();
			versionLabel.BorderStyle = BorderStyle.None;
			versionLabel.Location = new Point(236, 399);
			versionLabel.Name = "versionLabel";
			versionLabel.Size = new System.Drawing.Size(105, 23);
			versionLabel.Text = String.Format("Version: {0}",versionString);
			Controls.Add(versionLabel);

			AutoScaleDimensions = new SizeF(8F, 16F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(348, 429);

			this.ResumeLayout(false);
			this.PerformLayout();
		}

		private void button1_Click(object sender, EventArgs e) {
			/*
			var cm = (CurrencyManager)BindingContext[dataGrid.DataSource, dataGrid.DataMember];
			var view = (DataView)cm.List;

			foreach (DataRowView drv in view) {
				var row = drv.Row;
				var table = row.Table;
				var columns = table.Columns;
				var x = columns.GetEnumerator();
				x.MoveNext();
				var z =	x.Current;
				bool isSelected = drv["selected"] != DBNull.Value &&
				                  (bool)drv["selected"];
  
				if (isSelected) {
					// This row is checked
					var Name = drv["filename"];
					// Console.Error.WriteLine(filename);
					var Local = drv["title"];
				}
			}
			*/
			var currencyManager = (CurrencyManager)BindingContext[dataGrid.DataSource, dataGrid.DataMember];
			var dataView = (DataView)currencyManager.List;
			
			foreach (DataRowView dataRowView in dataView) {
				// bool isSelected = dataRowView["selected"] is bool b && b;
			bool selected = dataRowView["selected"] != DBNull.Value &&
							                  (bool)dataRowView["selected"];
			    if (selected) {
			        string name  = Convert.ToString(dataRowView["filename"]);
			        string local = Convert.ToString(dataRowView["title"]);
			
			        // do something with the checked row
			    }
			}	
			//---
			var dr = this.openFileDialog1.ShowDialog();
			if (dr == System.Windows.Forms.DialogResult.OK) {
				foreach (String fileName in openFileDialog1.FileNames)
					textBox1.Text = fileName;
				file = textBox1.Text;
			}

		}

		private void MakeDataSet(List<string> files) {

			dataSet = new DataSet("DataSet");
			dataTable = new DataTable("files");

			var selected = new DataColumn("selected", typeof(bool));
			selected.DefaultValue = false;
			dataTable.Columns.Add(selected);

			dataTable.Columns.Add(new DataColumn("id", typeof(int)));
			dataTable.Columns.Add(new DataColumn("filename"));
			dataSet.Tables.Add(dataTable);

			DataRow newRow;

			for (int index = 1; index < files.Count ; index++) {
				newRow = dataTable.NewRow();
				newRow["id"] = index;
				newRow["filename"] = files[index];
				dataTable.Rows.Add(newRow);
			}
			dataGrid.DataSource = dataTable;
		}

		private void MakeDataSet(List<TocEntry> files){
		    if (files == null) return;

			dataSet = new DataSet("DataSet");
			dataTable = new DataTable("files");

		    // Columns: one visible, one hidden
		    dataTable.Columns.Add("title", typeof(string));
		    dataTable.Columns.Add("filename", typeof(string));

			var selected = new DataColumn("selected", typeof(bool));
			selected.DefaultValue = false;
			dataTable.Columns.Add(selected);

			foreach (var entry in files) {
		        var row = dataTable.NewRow();
		        row["title"] = entry.Local;
		        row["filename"] = entry.Name;// entry.Local;
		        dataTable.Rows.Add(row);
		    }

		    dataSet.Tables.Add(dataTable);
			// dataGrid.DataSource = dataSet.Tables[0];
			dataGrid.DataSource = dataTable;

		}

		private void button2_Click(object sender, EventArgs eventArgs) {
			String title = Chm.title(file);
			if (title != null)
				MessageBox.Show("Title = " + title, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void button3_Click(object sender, EventArgs eventArgs) {
			var tokens = new List<TocEntry>();
			try {
		        tokens  = Chm.toc_structured(file);
			} catch( Exception e) {
				MessageBox.Show(e.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
	        if (tokens.Count == 0) {
				try {
					tokens  = Chm.toc_7zip(file);
				} catch( Exception e) {
					MessageBox.Show(e.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			if (tokens.Count > 0) {
				MakeDataSet(tokens);
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
		        var textRect = new Rectangle(cbRect.Right + 2, rect.Y, rect.Width - cbRect.Right - 2, rect.Height);
		        var stringFormat = new StringFormat  {
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

	    protected override System.Drawing.Size GetPreferredSize(Graphics g, object value) {
	        return new System.Drawing.Size(20, 20);
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


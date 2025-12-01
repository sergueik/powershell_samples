using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Text.RegularExpressions;
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
		private TextBox textBox2;
		// TODO: add to components
		private TextBox textBox3;

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
		private const string versionString = "0.15.0";
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
			button3.Click += button3_click;
			Controls.Add(button3);

			textBox1 = new TextBox();
			textBox1.Location = new Point(30, 7);
			textBox1.Name = "textBox1";
			textBox1.Top = 7;
			textBox1.Left = 30;
			textBox1.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			textBox1.Size = new System.Drawing.Size(140, 23);
			textBox1.TabIndex = 3;
			textBox1.Text = "";
			Controls.Add(textBox1);

			textBox2 = new TextBox();
			textBox2.Location = new Point(170, 7);
			textBox2.Name = "textBox2";
			textBox2.Top = 7;
			textBox2.Left = 170;
			textBox2.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			textBox2.Size = new System.Drawing.Size(140, 23);
			textBox2.TabIndex = 4;
			textBox2.Text = "";
			Controls.Add(textBox2);

			textBox3 = new TextBox();
			textBox3.Location = new Point(170, 7);
			textBox3.Name = "textBox3";
			textBox3.Top = 398;
			textBox3.Left = 8;
			textBox3.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			textBox3.Size = new System.Drawing.Size(333, 46);
			textBox3.TabIndex = 0;
			textBox3.Text = "";

			textBox3.Multiline = true;
			textBox3.ScrollBars = ScrollBars.Vertical;
			textBox3.ReadOnly = true;
			textBox3.WordWrap = false;

			// textBox3.Dock = DockStyle.Bottom;

			Controls.Add(textBox3);

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
			versionLabel.Location = new Point(236, 450);
			versionLabel.Name = "versionLabel";
			versionLabel.Size = new System.Drawing.Size(105, 23);
			versionLabel.Text = String.Format("Version: {0}",versionString);
			Controls.Add(versionLabel);

			AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(348, 480);

			this.ResumeLayout(false);
			this.PerformLayout();
		}

		private void button1_Click(object sender, EventArgs e) {
			if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				file = openFileDialog1.FileNames.First();
				textBox1.Text = Chm.title(file);
			}
		}

		private void makeDataSet(List<string> files) {

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

		private void makeDataSet(List<TocEntry> files){
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


		private void button2_Click(object sender, EventArgs e) {
			var files = selectedFiles(dataGrid);

			if (files.Count == 0)
				return;
			// create object and populate instance properties
			var dialog = new DataDialog { Files = files };
			dialog.ShowDialog();

			var regex = new Regex("#.*$", RegexOptions.Compiled);
			// LINQ query using method syntax
			var extractFiles = files.Select(f => regex.Replace(f.Local, "")).Distinct().ToList();

			Chm.extract_7zip(file, extractFiles);
		}

		private List<TocEntry> selectedFiles(DataGrid dataGrid) {
		    var files = new List<TocEntry>();

		    // Attempt to get the underlying DataView safely
		    DataView dataView;
		    if (!currencyManagerAsDataView(dataGrid, out dataView))
		        return files;

		    foreach (DataRowView row in dataView) {
		        // Check 'selected' column safely
		        bool selected = row["selected"] != DBNull.Value && (bool)row["selected"];
		        if (!selected)
		            continue;

		        files.Add(new TocEntry
		        {
		            Name = Convert.ToString(row["filename"]),
		            Local = Convert.ToString(row["title"])
		        });
		    }
		    return files;
		}

		private static bool currencyManagerAsDataView(DataGrid dataGrid, out DataView dataView) {
		    dataView = null;
		    if (dataGrid == null || dataGrid.DataSource == null)
		        return false;

		    var currencyManager = dataGrid.BindingContext[dataGrid.DataSource, dataGrid.DataMember] as CurrencyManager;
		    if (currencyManager == null || currencyManager.Count == 0)
		        return false;

		    dataView = currencyManager.List as DataView;
		    return dataView != null;
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

		private void button3_click(object sender, EventArgs eventArgs) {
		    // update UI button to prevent multiple clicks
		    button3.Enabled = false;
		    // update UI
		    textBox2.Text = "Starting…";

		    // Start a worker thread
		    var workerThread = new Thread(() => {
		        const int timeout = 1200; // 2 minutes
		        List<TocEntry> results = null;
		        string tag = null;

		        var dataGatherers = new[] {
					new DataGatherer("MSITFS", Chm.toc_structured),
					new DataGatherer("7-zip", Chm.toc_7zip)
		        };

		        foreach (var dataGatherer in dataGatherers) {
					bool status = BackgroundRunner.openBackgroundThreadWithTimeout( file, timeout, (Func<string, List<TocEntry>>) dataGatherer.Run, out results);
					// Update UI
					this.Invoke((MethodInvoker)(() => {
						appendLog(String.Format("Method {0} {1}", dataGatherer.Tag, (status ? "succeeded" : "failed")));
						textBox2.Text = status ? "Done" : "Trying next method";
					}));
					
					if (status) {
						tag = dataGatherer.Tag;
						break;
						// quit after first successful extraction
					}
		        }

				// Populate the dataset and update caption on UI thread
				if (results != null && results.Count > 0) {
					this.Invoke((MethodInvoker)(() => {
						makeDataSet(results);
						dataGrid.CaptionText = "Loaded via " + tag;
					}));
				} else {
					this.Invoke((MethodInvoker)(() => textBox2.Text = "All methods failed or timed out" ));
				}

				// Re-enable the UI button
				this.Invoke((MethodInvoker)(() => button3.Enabled = true));
			});

			// NOTE: COM ( inside BackgroundRunner) needs to run on STA Thread
			workerThread.SetApartmentState(ApartmentState.STA);
			workerThread.IsBackground = true;
			workerThread.Start();
		}

		private void appendLog(string message) {
		    if (this.InvokeRequired) {
		        this.Invoke(new Action(() => appendLog(message)));
		        return;
		    }

		    textBox3.AppendText(message + Environment.NewLine);
		}

	}

   public sealed  class DataGatherer {
       private string tag;
	   private Func<string, List<TocEntry>> run;
       public string Tag { get {return tag;} }
       public Func<string, List<TocEntry>> Run { get {return run;} }

       public DataGatherer(string tag, Func<string, List<TocEntry>> run) {
          this.tag = tag;
           this.run = run;
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


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

	public partial class Launcher : Form {

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
		private Label versionLabel;
		private Label lblImage;
		private DataGridTableStyle tableStyle;

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

	}
}


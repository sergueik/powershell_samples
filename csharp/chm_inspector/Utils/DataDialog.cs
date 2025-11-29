using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Xml.Linq;
using System.Collections.Generic;


namespace Utils {

	public class DataDialog : BaseDialog {

		#region Windows Form Designer generated code

		private System.ComponentModel.Container components = null;
		private TextBox text1;
		protected Button buttonAction;
		private Label label1;
        private DataGrid dataGrid1;
		private List<TocEntry> files = new List<TocEntry>();
		public List<TocEntry> Files {
			set {
				files = value;
				
				DataRow newRow;
				var dataSet = new DataSet("DataSet");
				var dataTable = new DataTable("files");
	
				var selected = new DataColumn("selected", typeof(bool));
				selected.DefaultValue = false;
				dataTable.Columns.Add(selected);
	
				dataTable.Columns.Add(new DataColumn("id", typeof(int)));
				dataTable.Columns.Add(new DataColumn("filename"));
				dataTable.Columns.Add(new DataColumn("title"));
				dataSet.Tables.Add(dataTable);

				for (int index = 0; index < files.Count ; index++) {
					newRow = dataTable.NewRow();
					newRow["id"] = index;
					newRow["filename"] = files[index].Name;
					newRow["title"] = files[index].Local;
					dataTable.Rows.Add(newRow);
				}
				dataGrid1.DataSource = dataTable;
				dataGrid1.Refresh();
		
				this.Refresh();
				this.OnResize(null);
			} 
		}

		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataDialog));
			this.text1 = new TextBox();
			buttonAction = new Button();
			label1 = new Label();
			dataGrid1 = new DataGrid();
			((System.ComponentModel.ISupportInitialize)(dataGrid1)).BeginInit();
			this.SuspendLayout();

			cbCancel.Location = new Point(664, 399);
			cbCancel.Size = new Size(165, 53);
 
			cbOk.Location = new Point(479, 412);
			cbOk.Size = new Size(166, 40);
			cbOk.Text = "OK";

			text1.AcceptsReturn = true;
			text1.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right)));
			text1.Location = new Point(14, 14);
			text1.Multiline = true;
			text1.Name = "text1";
			text1.ScrollBars = ScrollBars.Both;
			text1.Size = new Size(813, 0);
			text1.TabIndex = 0;
			text1.Text = "Type some text here";
			text1.WordWrap = false;

			buttonAction.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Left)));
			buttonAction.Location = new Point(16, 405);
			buttonAction.Name = "buttonAction";
			buttonAction.Size = new Size(209, 40);
			buttonAction.TabIndex = 14;
			buttonAction.Text = "Do Some Action";
			buttonAction.Click += new EventHandler(this.buttonSave_Click);

			label1.Anchor = ((AnchorStyles)(((AnchorStyles.Bottom | AnchorStyles.Left) | AnchorStyles.Right)));
			label1.BorderStyle = BorderStyle.Fixed3D;
			label1.ForeColor = Color.Green;
			label1.Location = new Point(14, -2);
			label1.Name = "label1";
			label1.Size = new Size(813, 55);
			label1.TabIndex = 13;
			label1.Text = "This";

			dataGrid1.AlternatingBackColor = Color.Lavender;
			dataGrid1.BackColor = Color.WhiteSmoke;
			dataGrid1.BackgroundColor = Color.LightGray;
			dataGrid1.BorderStyle = BorderStyle.None;
			dataGrid1.CaptionBackColor = Color.LightSteelBlue;
			dataGrid1.CaptionForeColor = Color.MidnightBlue;
			dataGrid1.CaptionText = "Employees";
			dataGrid1.DataMember = "";
			dataGrid1.FlatMode = true;
			dataGrid1.Font = new Font("Tahoma", 8F);
			dataGrid1.ForeColor = Color.MidnightBlue;
			dataGrid1.GridLineColor = Color.Gainsboro;
			dataGrid1.GridLineStyle = DataGridLineStyle.None;
			dataGrid1.HeaderBackColor = Color.MidnightBlue;
			dataGrid1.HeaderFont = new Font("Tahoma", 8F, FontStyle.Bold);
			dataGrid1.HeaderForeColor = Color.WhiteSmoke;
			dataGrid1.LinkColor = Color.Teal;
			dataGrid1.Location = new Point(16, 105);
			dataGrid1.Name = "dataGrid1";
			dataGrid1.ParentRowsBackColor = Color.Gainsboro;
			dataGrid1.ParentRowsForeColor = Color.MidnightBlue;
			dataGrid1.SelectionBackColor = Color.CadetBlue;
			dataGrid1.SelectionForeColor = Color.WhiteSmoke;
			dataGrid1.Size = new Size(813, 279);
			dataGrid1.TabIndex = 20;

			var dataSet = new DataSet("DataSet");
			var dataTable = new DataTable("files");

			var selected = new DataColumn("selected", typeof(bool));
			selected.DefaultValue = false;
			dataTable.Columns.Add(selected);

			dataTable.Columns.Add(new DataColumn("id", typeof(int)));
			dataTable.Columns.Add(new DataColumn("filename"));
			dataTable.Columns.Add(new DataColumn("title"));
			dataSet.Tables.Add(dataTable);

			DataRow newRow;

			for (int index = 1; index < files.Count ; index++) {
				newRow = dataTable.NewRow();
				newRow["id"] = index;
				newRow["filename"] = files[index].Name;
				newRow["title"] = files[index].Local;
				dataTable.Rows.Add(newRow);
			}
			dataGrid1.DataSource = dataTable;
			dataGrid1.Refresh();
			
			AutoScaleBaseSize = new Size(9, 22);
			ClientSize = new Size(841, 464);
			this.Controls.Add(label1);
			this.Controls.Add(buttonAction);
			this.Controls.Add(text1);
			this.Controls.Add(dataGrid1);
			// System.Resources.MissingManifestResourceException: Could not find any resources appropriate for the specified culture or the neutral culture.  Make sure "Utils.DataDialog.resources" was correctly embedded or linked into assembly "Utils" at compile time, or that all the satellite assemblies required are loadable and fully signed.
			//
			// this.Icon = ((Icon)(resources.GetObject("$this.Icon")));
			this.Name = "DataDialog";
			this.ShowInTaskbar = true;
			this.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Selected Data Dialog";
			this.Controls.SetChildIndex(dataGrid1, 0);
			this.Controls.SetChildIndex(text1, 0);
			this.Controls.SetChildIndex(dataGrid1, 0);
			this.Controls.SetChildIndex(cbOk, 0);
			this.Controls.SetChildIndex(cbCancel, 0);
			this.Controls.SetChildIndex(buttonAction, 0);
			this.Controls.SetChildIndex(label1, 0);
			((System.ComponentModel.ISupportInitialize)(dataGrid1)).EndInit();
			this.ResumeLayout(false);
			this.Refresh();
			this.PerformLayout();

		}

		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion

		public DataDialog() {
			InitializeComponent();
		}

		public string TypedText {
			get {
				return this.text1.Text;
			}
		}

		private void buttonSave_Click(object sender, System.EventArgs e) {
			MessageBox.Show("Some action done :)");
		}

	}

}

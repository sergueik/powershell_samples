using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;

namespace Program
{
	public partial class ParsingErrorForm : Form
	{

		private TextBox textBoxErrors;
		private Button btnClose;
		private Button btnStopShowing;
		private System.ComponentModel.IContainer components = null;
		private TableLayoutPanel tableButtons;
		private FlowLayoutPanel flowPanelButtons;

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();

			this.textBoxErrors = new TextBox();
			this.btnClose = new Button();
			this.btnStopShowing = new Button();
			//
			// Form
			//
			this.Size = new System.Drawing.Size(600, 400);
			this.Text = "Parsing Errors";

			//
			// textBoxErrors
			//
			this.textBoxErrors.Multiline = true;
			this.textBoxErrors.ReadOnly = true;
			this.textBoxErrors.Dock = DockStyle.Fill;
			this.textBoxErrors.ScrollBars = ScrollBars.Both;
			this.textBoxErrors.Text = string.Join(Environment.NewLine, new List<string>());
			this.Controls.Add(this.textBoxErrors);

			//
			// tableButtons
			//
			this.tableButtons = new TableLayoutPanel();
			this.tableButtons.Dock = DockStyle.Bottom;
			this.tableButtons.ColumnCount = 3;
			this.tableButtons.RowCount = 1;
			this.tableButtons.Height = 50;

			this.tableButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));   // Left filler
			this.tableButtons.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));       // Buttons centered
			this.tableButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));   // Right filler

			//
			// btnClose
			//
			this.btnClose.Text = "Close";
			this.btnClose.AutoSize = true;
			this.btnClose.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.btnClose.Margin = new Padding(10);
			this.btnClose.Click += (s, e) => this.Close();

			//
			// btnStopShowing
			//
			this.btnStopShowing.Text = "Suppress";
			this.btnStopShowing.AutoSize = true;
			this.btnStopShowing.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.btnStopShowing.Margin = new Padding(10);
			this.btnStopShowing.Click += BtnStopShowing_Click;

			//
			// flowPanelButtons
			//
			this.flowPanelButtons = new FlowLayoutPanel();
			this.flowPanelButtons.AutoSize = true;
			this.flowPanelButtons.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.flowPanelButtons.FlowDirection = FlowDirection.LeftToRight;
			this.flowPanelButtons.WrapContents = false;
			this.flowPanelButtons.Margin = new Padding(0, 5, 0, 5);
			this.flowPanelButtons.Controls.Add(this.btnStopShowing);
			this.flowPanelButtons.Controls.Add(this.btnClose);

			this.tableButtons.Controls.Add(this.flowPanelButtons, 1, 0);
			this.Controls.Add(this.tableButtons);
		}
	}
}

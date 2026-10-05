using System;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using NPOI.XWPF.UserModel;

using NPOI;
using NPOI.Util;

using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace Program {
	// NOTE: Missing partial modifier on declaration of type 'Program.MainForm';
	// when
	// partial declaration of this type exists (CS0260)
	public class Program : Form {
		
		// NOTE: unnecessary "internal static class Program {...}"
		[STAThread]
		public static void Main() {
			Application.EnableVisualStyles();
			// https://learn.microsoft.com/en-us/dotnet/api/application.setcompatibletextrenderingdefault?view=netframework-4.5
			// NOTE: can only call this method before
			// the first window is created by Windows Forms application
			try {
				Application.SetCompatibleTextRenderingDefault(false);
			} catch(InvalidOperationException) {}
			Application.Run(new Program());
		}

		public Program() { InitializeComponent(); }
		private IContainer components = null;
		private Font font1 = null;
		private Font font2 = null;
 
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		// https://pbdd.org/wp-content/uploads/2015/06/WordPractice2007.docx
		private void InitializeComponent() {
			// ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Program));
			btnSelectFile = new Button();
			label1 = new Label();
			txtDocDirectory = new TextBox();
			label2 = new Label();
			label3 = new Label();
			txtSearchKey1 = new TextBox();
			txtReplace1 = new TextBox();
			btnReplace = new Button();
			btnClose = new Button();
			label6 = new Label();
			txtSearchKey3 = new TextBox();
			txtSearchKey4 = new TextBox();
			label8 = new Label();
			btn1 = new Button();
			label12 = new Label();
			txtBox1 = new TextBox();
			tabControl = new TabControl();
			tabPage1 = new TabPage();
			txtResult1 = new TextBox();
			tabPage2 = new TabPage();
			dataGridView = new DataGridView();
			dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
			dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
			dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
			dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
			tabControl.SuspendLayout();
			tabPage1.SuspendLayout();
			tabPage2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(dataGridView)).BeginInit();
			this.SuspendLayout();
			// 
			// btnSelectFile
			// 
			btnSelectFile.Location = new Point(112, 89);
			btnSelectFile.Margin = new Padding(5, 6, 5, 6);
			btnSelectFile.Name = "btnSelectFile";
			btnSelectFile.Size = new Size(207, 46);
			btnSelectFile.TabIndex = 0;
			btnSelectFile.Text = "Choose Folder";
			btnSelectFile.UseVisualStyleBackColor = true;
			btnSelectFile.Click += new System.EventHandler(btnSelectFile_Click);
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(341, 101);
			label1.Margin = new Padding(5, 0, 5, 0);
			label1.Name = "label1";
			label1.Size = new Size(120, 25);
			label1.TabIndex = 1;
			label1.Text = "Base folder：";
			// 
			// txtDocDirectory
			// 
			txtDocDirectory.Font = font1;
			txtDocDirectory.Location = new Point(486, 89);
			txtDocDirectory.Margin = new Padding(5, 6, 5, 6);
			txtDocDirectory.Name = "txtDocDirectory";
			txtDocDirectory.Size = new Size(256, 50);
			txtDocDirectory.TabIndex = 2;
			txtDocDirectory.TextAlign = HorizontalAlignment.Right;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(154, 161);
			label2.Margin = new Padding(5, 0, 5, 0);
			label2.Name = "label2";
			label2.Size = new Size(124, 25);
			label2.TabIndex = 3;
			label2.Text = "1.Text to find";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.BackColor = Color.Tan;
			label3.Location = new Point(154, 215);
			label3.Margin = new Padding(5, 0, 5, 0);
			label3.Name = "label3";
			label3.Size = new Size(179, 25);
			label3.TabIndex = 3;
			label3.Text = "Text to be replaced";
			// 
			// txtSearchKey1
			// 
			txtSearchKey1.Font = font1;
			txtSearchKey1.Location = new Point(341, 156);
			txtSearchKey1.Margin = new Padding(5, 6, 5, 6);
			txtSearchKey1.Name = "txtSearchKey1";
			txtSearchKey1.Size = new Size(401, 50);
			txtSearchKey1.TabIndex = 4;
			// 
			// txtReplace1
			// 
			txtReplace1.BackColor = SystemColors.Window;
			txtReplace1.Enabled = false;
			txtReplace1.Font = font1;
			txtReplace1.Location = new Point(341, 209);
			txtReplace1.Margin = new Padding(5, 6, 5, 6);
			txtReplace1.Name = "txtReplace1";
			txtReplace1.Size = new Size(401, 50);
			txtReplace1.TabIndex = 4;
			// 
			// btnReplace
			// 
			btnReplace.Location = new Point(112, 770);
			btnReplace.Margin = new Padding(5, 6, 5, 6);
			btnReplace.Name = "btnReplace";
			btnReplace.Size = new Size(137, 46);
			btnReplace.TabIndex = 5;
			btnReplace.Text = "Start";
			btnReplace.UseVisualStyleBackColor = true;
			btnReplace.Click += new System.EventHandler(this.scan);
			// 
			// btnClose
			// 
			btnClose.Location = new Point(605, 770);
			btnClose.Margin = new Padding(5, 6, 5, 6);
			btnClose.Name = "btnClose";
			btnClose.Size = new Size(137, 46);
			btnClose.TabIndex = 6;
			btnClose.Text = "Close";
			btnClose.UseVisualStyleBackColor = true;
			btnClose.Click += new System.EventHandler(btnClose_Click);
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new Point(154, 261);
			label6.Margin = new Padding(5, 0, 5, 0);
			label6.Name = "label6";
			label6.Size = new Size(124, 25);
			label6.TabIndex = 3;
			label6.Text = "3.Text to find";
			// 
			// txtSearchKey3
			// 
			txtSearchKey3.Font = font1;
			txtSearchKey3.Location = new Point(343, 258);
			txtSearchKey3.Margin = new Padding(5, 6, 5, 6);
			txtSearchKey3.Name = "txtSearchKey3";
			txtSearchKey3.Size = new Size(399, 50);
			txtSearchKey3.TabIndex = 4;
			// 
			// txtSearchKey4
			// 
			txtSearchKey4.Font = font1;
			txtSearchKey4.Location = new Point(343, 308);
			txtSearchKey4.Margin = new Padding(5, 6, 5, 6);
			txtSearchKey4.Name = "txtSearchKey4";
			txtSearchKey4.Size = new Size(399, 50);
			txtSearchKey4.TabIndex = 4;
			txtSearchKey4.TextChanged += new System.EventHandler(this.TxtSearchKey4TextChanged);
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Location = new Point(154, 308);
			label8.Margin = new Padding(5, 0, 5, 0);
			label8.Name = "label8";
			label8.Size = new Size(124, 25);
			label8.TabIndex = 3;
			label8.Text = "4.Text to find";
			// 
			// btn1
			// 
			btn1.Location = new Point(112, 702);
			btn1.Margin = new Padding(5, 6, 5, 6);
			btn1.Name = "btn1";
			btn1.Size = new Size(207, 46);
			btn1.TabIndex = 7;
			btn1.Text = "Choose File";
			btn1.UseVisualStyleBackColor = true;
			btn1.Click += new System.EventHandler(this.chooseFile);
			// 
			// label12
			// 
			label12.AutoSize = true;
			label12.Location = new Point(343, 710);
			label12.Margin = new Padding(5, 0, 5, 0);
			label12.Name = "label12";
			label12.Size = new Size(133, 25);
			label12.TabIndex = 8;
			label12.Text = "Heuristics File";
			// 
			// txtBox1
			// 
			txtBox1.Font = font1;
			txtBox1.Location = new Point(486, 710);
			txtBox1.Margin = new Padding(5, 6, 5, 6);
			txtBox1.Name = "txtBox1";
			txtBox1.Size = new Size(256, 50);
			txtBox1.TabIndex = 9;
			txtBox1.TextAlign = HorizontalAlignment.Right;
			// 
			// tabControl
			// 
			tabControl.Controls.Add(tabPage1);
			tabControl.Controls.Add(tabPage2);
			tabControl.Font = font1;
			tabControl.Location = new Point(20, 358);
			tabControl.Margin = new Padding(5, 0, 5, 0);
			tabControl.Name = "tabControl";
			tabControl.SelectedIndex = 0;
			tabControl.Size = new Size(720, 320);
			tabControl.TabIndex = 12;
			// 
			// tabPage1
			// 
			tabPage1.Controls.Add(txtResult1);
			tabPage1.Font = font1;
			tabPage1.Location = new Point(4, 51);
			tabPage1.Margin = new Padding(5, 0, 5, 0);
			tabPage1.Name = "tabPage1";
			tabPage1.Padding = new Padding(5, 0, 5, 0);
			tabPage1.Size = new Size(712, 265);
			tabPage1.TabIndex = 0;
			tabPage1.Text = "Log";
			tabPage1.UseVisualStyleBackColor = true;
			// 
			// txtResult1
			// 
			txtResult1.Dock = DockStyle.Fill;
			txtResult1.Font = font1;
			txtResult1.Location = new Point(5, 0);
			txtResult1.Margin = new Padding(4, 5, 4, 5);
			txtResult1.Multiline = true;
			txtResult1.Name = "txtResult1";
			txtResult1.ScrollBars = ScrollBars.Vertical;
			txtResult1.Size = new Size(702, 265);
			txtResult1.TabIndex = 9;
			// 
			// tabPage2
			// 
			tabPage2.Controls.Add(dataGridView);
			tabPage2.Font = font1;
			tabPage2.Location = new Point(4, 51);
			tabPage2.Margin = new Padding(5, 0, 5, 0);
			tabPage2.Name = "tabPage2";
			tabPage2.Padding = new Padding(5, 0, 5, 0);
			tabPage2.Size = new Size(712, 265);
			tabPage2.TabIndex = 1;
			tabPage2.Text = "Results";
			tabPage2.UseVisualStyleBackColor = true;
			// 
			// dataGridView
			// 
			dataGridView.AllowUserToAddRows = false;
			dataGridView.AllowUserToDeleteRows = false;
			dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			dataGridView.DefaultCellStyle.Font = font2;
			dataGridView.ColumnHeadersHeight = 32;
			dataGridView.Columns.AddRange(new DataGridViewColumn[] {
			dataGridViewTextBoxColumn1,
			dataGridViewTextBoxColumn2,
			dataGridViewTextBoxColumn3,
			dataGridViewTextBoxColumn4});
			dataGridView.Dock = DockStyle.Fill;
			dataGridView.Font = font2;
			dataGridView.Location = new Point(5, 0);
			dataGridView.Margin = new Padding(4, 5, 4, 5);
			dataGridView.Name = "dataGridView";
			dataGridView.ReadOnly = true;
			dataGridView.RowHeadersVisible = false;
			dataGridView.RowTemplate.Height = 32;
			dataGridView.Size = new Size(702, 265);
			dataGridView.TabIndex = 0;
			// 
			// dataGridViewTextBoxColumn1
			// 
			dataGridViewTextBoxColumn1.FillWeight = 60F;
			dataGridViewTextBoxColumn1.HeaderText = "Type";
			dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
			dataGridViewTextBoxColumn1.ReadOnly = true;
			// 
			// dataGridViewTextBoxColumn2
			// 
			dataGridViewTextBoxColumn2.FillWeight = 160F;
			dataGridViewTextBoxColumn2.HeaderText = "Name";
			dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
			dataGridViewTextBoxColumn2.ReadOnly = true;
			// 
			// dataGridViewTextBoxColumn3
			// 
			dataGridViewTextBoxColumn3.FillWeight = 260F;
			dataGridViewTextBoxColumn3.HeaderText = "Match";
			dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
			dataGridViewTextBoxColumn3.ReadOnly = true;
			// 
			// dataGridViewTextBoxColumn4
			// 
			dataGridViewTextBoxColumn4.FillWeight = 140F;
			dataGridViewTextBoxColumn4.HeaderText = "Path";
			dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
			dataGridViewTextBoxColumn4.ReadOnly = true;

			dataGridView.Rows.Add(
				"File",
				"report.pdf",
				"invoice",
				@"C:\Documents\report.pdf");
		
			dataGridView.Rows.Add(
				"File",
				"presentation.pptx",
				"architecture",
				@"C:\Documents\presentation.pptx");
		
			dataGridView.Rows.Add(
				"Directory",
				"2026",
				"2026",
				@"C:\Documents\2026");

			// 
			// Program
			// 
			this.AutoScaleMode = AutoScaleMode.None;
			this.ClientSize = new Size(834, 931);
			Controls.Add(txtBox1);
			Controls.Add(tabControl);
			Controls.Add(label12);
			Controls.Add(btn1);
			Controls.Add(btnClose);
			Controls.Add(btnReplace);
			Controls.Add(txtReplace1);
			Controls.Add(txtSearchKey4);
			Controls.Add(txtSearchKey3);
			Controls.Add(txtSearchKey1);
			Controls.Add(label8);
			Controls.Add(label6);
			Controls.Add(label3);
			Controls.Add(label2);
			Controls.Add(txtDocDirectory);
			Controls.Add(label1);
			Controls.Add(btnSelectFile);
			const string iconBase64 = "AAABAAEAMDAAAAEAIACoJQAAFgAAACgAAAAwAAAAYAAAAAEAIAAAAAAAACQAAAAAAAAAAAAAAAAAAAAAAAD+/v7///////////////////////////////////////7////+/v7/+f79//r9/f/+/v7/9v78//n++P/Q+/D/fO/U/ynjvP8A3rH/Ad+x/wHgsP8A36//AOCv/wLfr/8d3rb/XOvP/9v78f/5/Pj/9f78//f+/f/+/f/////////////////////////////////////////////////////////////////////////////////////////////+/v7///////////////////////////////////////z+/v/4//7//f/+/+z89f+D8Nj/Dduy/wHesP8C3q//AOGv/wDhsP8D3a3/AN+v/wHgsP8B4LD/BN2w/wTfr/8A4rD/AN+u/wTesP8A3a//UOfH/+n99f/7/v7////////////////////////////////////////////////////////////////////////////////////////////+/v7///////////////////////z+/f/8/f3/+f39//3+/P+n9eH/HeCz/wDfr/8F37D/AOCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8D4LD/CN6v/wDerv9s7c3/7v/6//z+/v/////////////////////////////////////////////////////////////////////////////////+/v7///////////////////////z+/v/9/f3/sfTi/wXcq/8G3q//AOCv/wPgsP8A36//AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8D4LD/BeCw/wLfr/8D36//Kd+5//P++v/+/v7////////////////////////////////////////////////////////////////////////////+/v7//v7+//7+/v////////////39/f/8/v7/+P7+//v9/P++9+v/PuS+/wXer/8A4LD/A9+w/wPgsP8D4LD/A+Cw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8D4LD/AeCv/wDgr/8D37D/AOGu/xret//5/vz//v7+//7+/v/+/v7////////////////////////////////////////////////////////////8/f3//fz9//b++v+z5dn/f9TB/7Xm2P/2/fn/+v78//r9/f/7/v3//fv8//X8+f+09OT/E+C2/wLdrP8B4bD/CN+w/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AOCv/wXer/8j4bb//v78//z8/f///v/////////////////////////////////////////////////////////////5/fz/s+3e/wOgff8Dp4D/AKmB/wOpgP8AqYL/BZ18/z2+ov/n+PH/9v37//39/f/7//7//v3+/+n98/8k3rb/BN2u/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/B92v/wDgr/8D36//efDV///9/v/+/v7////////////////////////////////////////////////////////////T8On/DqSE/wGpgP8AqYL/AKmC/wCpgv8AqYL/AKmC/wCogf8Cp4H/BqeB/2rMtP/v+/f/9/r5//39/f/8/v7/1fjx/xXbs/8D3q//CN6u/wPer/8A3q7/AOCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B37D/ANuq/+T78v/6//3//f////7+/v////////////////////////////////////////////////+l4NL/C6B//wGof/8AqYL/AKmC/wCogf8AqYL/AKiB/wCpgf8AqIH/AamC/wCphP8Fo4H/Rbmh/+j49f/6/Pz/9/r6//v++/9d6Mr/AOCy/wLgsP8A4LD/At+w/wHgsP8B4LD/AN+v/wDfr/8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8A36//AN+v/ybdt//2//z//P/+//7+/v/////////////////////////////////////////////////c9e//oOTR/w2ngv8BpX//AaV//wClfv8ApX7/AKiB/wCngP8AqIH/AKiB/wOpgv8Ep4H/AamB/wCmgf9hw6z/6/z4//v9/v/3/vz/tfXm/wvdr/8F4bH/At+v/wPgsP8D4LD/BOGx/wPgsP8D4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/At+u/wDdsP/L+O///P/+//7+/v/////////////////////////////////////////////////7/v3/+Pz8//b9/P/4/vr//fv7//D/9//1/fv/6Pr2/77n3f9+0bv/I66O/wSkgP8Ep4H/AqmB/wKngP8Dp4D/BKWB/6Hf0P/4/fz/+fz8//j7+v/J+Ov/mPHa/4bx1v9C6MT/C92v/wDfr/8D4bH/AeCw/wHgsP8B4LD/AeCw/wHgsP8A4LD/AOCw/wPfsP8r4Lv/+P/9//v7/P/////////////////////////////////////////////////9/v7/+f/+//b+/v/8/P3/+/38/xR0W/8Ffl3/C4Bf/ymKcv+Evq//+v/8//n++P+k5dL/IqSG/wGkgP8AqYL/AKd//wGqgv82sZb/8f76//P6+f+/9+v/Bdqu/wTdsf8B4LP/BuCx/wPgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8I3q//1vvw//n+/v/////////////////////////////////////////////////9/v7//v7+///////+/v7//v/8/wZ8X/8Af17/AH9e/wGAXP9Dm4P//f78/222o/82jHP/0PLl/9z27/8hqYz/DKiF/wCrgf8BqYL/FqmG/9r47P/3/v3/1vvv/wfdrv8C4LH/AeCw/wHfsP8A36//AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wDgsP8H37H/WObG//j+/P/////////////////////////////////////////////////+/v7//////////////////v/8/wZ8X/8BgF7/AYBe/wGBXP9Dm4P//f78/3C2nv8Aflv/A39e/wJ2Vf/m8Ob/2fTr/w+igP8DqIL/CKaB/wGmgv+V3cz//Pv7/8D15/8B3Kz/AeCw/wDfr/8A36//AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8A4K//CNqv//j+/P/////////////////////////////////////////////////+/v7//////////////////v/8/wZ8X/8BgF7/AYBe/wGBXP9Dm4P//f78/2m3of8Bf13/AH5e/wB+XP8Agl//IoJl/+L37/9iw6n/AaeA/weqg/8AqoL/acew//77/P+X9OD/AN+v/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8H36//AN+v/7f26P/////////////////////////////////////////////////+/v7//////////////////v/8/wZ8X/8BgF7/AYBe/wGBXP9Dm4P//f78/263of8DgF3/AH9d/wCAXv8AgF7/AIBe/wN7XP9+wKz/yvDl/wekgv8ArIT/CaaB/0i5nv/5/vv/3frz/yfbt/8H267/DNyu/wnfsP8L4rT/AeCw/wHgsP8B4LD/AeCw/wHgsP8D3bD/AuCw/1Hmxv/7/v3//P/+///////////////////////////////////////+/v7//////////////////v/8/wZ8X/8BgF7/AYBe/wGBXP9Dm4P//f78/263of8DgF3/AH9d/wCAXv8AgF7/AIBe/wGAX/8Ffl7/HYVo//f+9v8aq4r/BKmD/wOogv82tZj/8f37/8z67/+h8d//YezK/xHdsP8M3rD/AeCw/wHgsP8B4LD/AeCw/wHgsP8A3rD/Bt6v/w3ds//8//7//P/+//7+/v/////////////////////////////////+/v7//////////////////v/8/wZ8X/8BgF7/AYBe/wSBX/9EmoL//v38/222of8Eflz/AIBf/wCAXv8AgF7/AH9d/wN+Xv8AgF7/AH5b/wV4W//G6dr/UL2h/wWmgf8EqIP/KLCS//X/+v9F58T/AN2t/wDer/8C367/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/A96u/wTfsP/d+/P//f7///3+/v/////////////////////////////////+/v7//////////////////v/8/wd9YP8BgF7/AYBe/wGBX/9Fm4P///z8/2u2of8CfVv/AH9e/wCAXv8AgF7/AIBe/wCAXv8AgF7/AH9d/wCCX/8FeVr/mMu8/4DRvP8FpX7/Aqd//yitj//w/Pf/KuC7/wPgsf8D367/AeCw/wHgsP8B4LD/AeCw/wHgsP8C4LD/AeCw/wLfr/9f6Mv/+f79//7+/v/////////////////////////////////+/v7//////////////////v/8/wd9YP8BgF7/AYBe/wGBX/88l33/+f77/1yrlf8Cf1z/AH5d/wCAXv8AgF7/AIBe/wCAXv8AgF7/AIBe/wKCYP8AgF7/AYNh/16ok/+h4s//AaV//wSsg/8bqIr/+v76/yXdt/8A367/AeCw/wHgsP8B4LD/AeCw/wHgsP8C4LD/AeCw/wPgsP8M37X//f/+///+/P/////////////////////////////////+/v7//////////////v7//f38/wV7Xv8BgF7/AIBe/wOCYP8Eflz/FHZY/wB9W/8BgF7/AH9d/wCAXv8AgF7/AIBe/wCAXv8AgF7/AIBe/wCAXv8AgF7/AoBe/wOBX/9CkXj/0fHn/wCphP8Dp3//IK6O/+L+9/8M27H/B9+w/wTfsP8F3LD/AN+v/wHfr/8B36//AeCw/wHgsP8A26//5Pzz//z+/v/////////////////////////////////+/v7////////////+/f3//v/9/wt5Xv8CgV//AH9d/wSDYf8DgV//AH1b/wGAXv8CgV//AYBe/wCAXv8AgF7/AIBe/wCAXv8AgF7/AIBe/wCAXv8AgF7/AIBe/wOBX/8CgWH/H4Jn/+P78v8Cp4L/AamA/zOzlP/s+/X/E9qx/wTjsv8H3a7/AN+v/wDfr/8A4K//Ad+v/wHgsP8D3q//T+TE//79/v/////////////////////////////////+/v7////////////9/f7//f79/0Cbhf8CgF7/AH5c/wB/Xv8DgF7/AH9d/wCAXv8Af13/AIBe/wCAXv8AgF7/AIBe/wCAXv8AgF7/AIBe/wCAXv8AgF7/AIBe/wB/Xf8Af17/B31c/xV4Xv/j/fX/EqOC/wKrgv89tpj/+v75/3Dr0P8i37T/Hd+1/y/lu/8K4rP/At+v/wLfr/8B36//CN+0//P++v///v7//v////7////////////////////+/v7////////////8//7//v7+/8vp4/8AfV3/AH5c/wB/Xv8AgF7/AIBe/wCAXv8AgF7/AIBe/wCAXv8AgF7/AIBe/wCAXv8AgF7/AIBe/wCAXv8AgF7/AIBe/wCAXv8AgF7/BH9e/wCBXv8IeFv/3PXq/x+xjv8FqID/MbKU/+n79v8k27j/Dd2y/wPgsP8F4LD/A+Cw/wPgsP8G37D/CeCy/7L05P/9/v7//P7+//7+/v/////////////////+/v7////////////9/v7//P39//j+/P8YfGL/AYBd/wB/Xf8AgF7/AIBe/wCAXv8AgF7/AIBe/wCAXv8AgF7/AIBe/wCAXv8AgF7/AIBe/wCAXv8AgF7/AIBe/wCAXv8AgF7/AIFe/wF/Xf8Ff2D/BXxc/7/h0/9Ywan/A6iC/ySvjP/s/fT/Cdyw/wPfr/8D3q7/A+Cw/wPgsP8A3a//AN6s/wfesv/0/vv/+/39//7////////////////////+/v7///////////////////////39/f/h9+//BXxc/wB/XP8EgF7/AHxc/wCAXv8AgF7/AIBe/wV/Xf8CgV//Bn5f/wF9Xf8BfV3/BH1e/wB/X/8AgF3/AIFf/wB/Xf8AgF7/AYBe/wCAXv8AgF7/BH5a/wJ3Wv/D4dn/mNvK/wKge/8Pon7/8f33/xndsv8A4K//B9+w/wDfr/8B4LD/AOCw/wTfsP+z9eb/+P77///9/v/////////////////+/v7///////////////////////39/f/5+/r/0PHm/wJ5W/8Af17/AH1b/wCAXv8AgF7/AIBe/wB/Xf8HfV3/AH5c/wB1Vv8Fd1j/AHVW/wB4WP8AgF7/AH5c/wCAXv8AgF7/AH9d/wCAXv8BgF7/AH1a/4O4qP/6/Pv/+vz7/8306f8BoX7/B6qE/9z37f834L3/Bd2w/wDfr/8D37D/AeCw/wLgsP8H37P/+v/6//v9/v/////////////////+/v7///////////////////////79/v/6////9f/9/+b+9v8Ldlj/An9d/wB/Xf8Af13/AH9d/xd8Yv/P7uH/+/73//j++v/7/vn/9/76//3++f/j//T/KZN2/wF+XP8Aflz/AH9d/wGAX/8Be1r/vOva//7++//+/v7///3+//j//f/7//v/J6uM/wCkgP+d4c//evDU/wDdrv8F36//AOCw/wHfsP8H5bP/k+7Z//X8+//+/v7//v////7////+/v7///////////////////////////////////////v++//7/vr/jsi3/xB/Yv8Vg2n/+fnx/9r67v8k4Ln/At2x/wDdrP8B36//Ad+u/wDerv8W27P/o/Xj//f/+/8wjnX/Bnpc/1CciP/3/vn/+v79//v+/f/9//////////7////7//7//vz9/6zm2f8PpID/KbSS/9/57/8K3bL/At2t/wDjsf8D4LH/B9qu/+r89v/2/v3/+/79//j+/v/+/v7///////////////////////////////////////7+/v/9/f3/9/v7//X9+//x/vn/IeC5/wHfrv8C3a7/A92u/wTdrv8A36//BN+v/wrdsP8B4K//Ad+u/xLbsP/U+O3/9/78//r8/P/3/fz//P/+//7////+/v7///////7////8/v3/+P////37+//5/vv/bcax/wehff+Y3Mn/fu/V/wrbr/8B3rD/AeGu/yriu//5/v7/+/7+///////+/v7////////////////////////////////////////9/v/7/v7/+P78/+H89v8K3q//Bt2v/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wDfr/8J3a//r/Xn//P++////P3////////////////////////////////////////////+/v7//P39/+T7+P9XxKr/D51+/6bk0/9n6cv/Bd2u/wTisP986tD/+f36//r+///+/v7///////////////////////////////////////v8/P/8/f3/8P34/wvcsv8G4bH/AN+w/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/A96v/wHgsP8B3q//Ad+w/9X67f/6//3////////////////////////////////////////////7/v3//v7+//79/f/6/fr/8fz3/5/dz/8YqYz/dMq1/4Xx1v8P3LL/wPbq//39/f/+/v7///////////////////////////////////////j////8/P3/RefG/wDdsP8C4K7/BN+w/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AOCw/wHgsP8B36//AeCu/wvYr//7/fv////////////////////////////////////////////+/v7//P7+//39/f/5//7/+v7+//z8/P/5/vv/8fz3/8Dq3P9eyK//Xcay/9bw7P/+/v7///////////////////////////////////////39/f/Z+vH/Aduu/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AN6u/wbfrv+B7tf/+v79//3+/v/+/v7///////////////////////////////////////////////////////////////////////7+/v/7/v7//P39//3+/v/+/v7///////////////////////////////////////7+/P9M58f/AN+v/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/BN6w/wDirv8L2K3/9P78//z8/P/7//////////////////////////////////////////////////////////////////////////7+/v/9/v7//v7+//7+/v/+/v7///////////////////////////////////////7/+/8I3rD/Bd6v/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8A36//3fv0//T+/f/9/v7////////////////////////////////////////////////////////////////////////////////////////////+/v7///////////////////////////////////////P/+v8A3a7/Ad+v/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8A36//tPbn//f//v/+/v7////////////////////////////////////////////////////////////////////////////////////////////+/v7///////////////////////////////////////L9+P8D3a//AOCv/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AN+v/wHgsP8B4LD/h/Da//7+/v/8/v/////////////////////////////////////////////////////////////////////////////////////////////+/v7//////////////////////////////////////+z+9/8B3rD/Bd6v/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/iPDa//j+/f/8///////////////////////////////////////////////////////////////////////////////////////////////+/v7///////////////////////////////////////f++f8C3a//BN+v/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8A36//rPPi//z+/v///v/////////////////////////////////////////////////////////////////////////////////////////////+/v7//////////////////////////////////f////v+/P8G3LD/AeGv/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/BN2w/wrer/8C3q7/1/rz//f+/f/9///////////////////////////////////////////////////////////////////////////////////////////////+/v7//////////////////////////////////f////v//f855MD/At+w/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCv/wPerv8K2a3/+v78//38/f/6/f7////////////////////////////////////////////////////////////////////////////////////////////+/v7//////////////////////////////////f////n+/v+89+f/AN+v/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/A+Gu/wjesP9o58r/+v38//r+/P/+///////////////////////////////////////////////////////////////////////////////////////////////+/v7////////////////////////////////////////////3/v3/J+C3/wDgsP8K37D/AN+v/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/BOCw/wXfr//r/vr////////////////////////////////////////////////////////////////////////////////////////////////////////////+/v7///////////////////////////////////////b/+//9/P3/3fvy/wTcrf8A3q7/A9+w/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AN6u/6Px3v/2/fz////////////////////////////////////////////////////////////////////////////////////////////////////////////+/v7///////////////////////////////////////v//f/8//7/+/79/6/45v8A3rD/AuCu/wHfsf8C3rD/Ad6v/wHgsP8B4LD/AOCw/wDhsP8A36//AN+t/wLfrv8F3bD/gOrS//f++//9/vz////////////////////////////////////////////////////////////////////////////////////////////////////////////+/v7//////////////////////////////////////////////////v7+//P++//W+e7/AN6w/wnesP8B363/AN+s/wHgsP8B4LD/AN+v/wLer/8A367/Ad+v/wXZrf+I8dn//f39///+/v/2//3////////////////////////////////////////////////////////////////////////////////////////////////////////////+/v7//////////////////////////////////////////////////v7+//3+/v/6/fz/8/37/3Xs0f8A3bD/At6x/wHgsP8B4LD/AN+w/wXcsP8A3a//QOTB/+399//4/fz/+v38//j9/P/+/v7///////////////////////////////////////////////////////////////////////////////////////////////////////////8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=";
			byte[] iconBytes = Convert.FromBase64String(iconBase64);
			var iconStream = new MemoryStream(iconBytes, 0, iconBytes.Length);
			iconStream.Write(iconBytes, 0, iconBytes.Length);
			var iconImage = Image.FromStream(iconStream, true);
			var iconBitmap = new Bitmap(iconStream);
			IntPtr hicon = iconBitmap.GetHicon();
			Icon icon = Icon.FromHandle(hicon);

			this.Icon = icon;

			this.Margin = new Padding(5, 6, 5, 6);
			this.Name = "Program";
			this.Text = "word pdf search";
			this.Load += new System.EventHandler(this.ProgramLoad);
			tabControl.ResumeLayout(false);
			tabPage1.ResumeLayout(false);
			tabPage1.PerformLayout();
			tabPage2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(dataGridView)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private Button btnSelectFile;
		private Label label1;
		private TextBox txtDocDirectory;
		private Label label2;
		private Label label3;
		private TextBox txtSearchKey1;
		private TextBox txtReplace1;
		private Label label6;
		private TextBox txtSearchKey3;
		private Label label8;
		private TextBox txtSearchKey4;
		private Button btnReplace;
		private Button btnClose;
		private Button btn1;
		private Label label12;
		private TextBox txtBox1;
		private TabControl tabControl;
		private TabPage tabPage1;
		private TabPage tabPage2;
		private TextBox txtResult1;
		private DataGridView dataGridView;
		private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
		private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
		private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
		private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;

		private void Log(string message){
			Debug.WriteLine(message);
			// MessageBox.Show(message);

			if (txtResult1.InvokeRequired){
				txtResult1.Invoke(new Action<string>(Log), message);
              return;
			}
			txtResult1.AppendText(message + Environment.NewLine);
		}

		private void btnSelectFile_Click(object sender, EventArgs e) {
			var folderBrowserDialog = new FolderBrowserDialog();

			if (folderBrowserDialog.ShowDialog() == DialogResult.OK) {
				txtDocDirectory.Text = folderBrowserDialog.SelectedPath;
			}
		}

		private void scan(object sender, EventArgs eventArgs) {
			string docDirectory = txtDocDirectory.Text;
			Log(String.Format("Starting scan {0}", docDirectory));
			FileInfo[] files = { };
			DirectoryInfo directoryInfo = null;
			string filePath = null;
			ThreadPool.QueueUserWorkItem(
				// Error CS1593: Delegate 'System.Threading.WaitCallback' does not take 0 arguments
				(object state) => {
					directoryInfo = new DirectoryInfo(docDirectory);
					// Debug.WriteLine(String.Format("Scanning {0}", directoryInfo.FullName));
					Log(String.Format("Scanning {0}", directoryInfo.FullName));

					files = directoryInfo.GetFiles("*.pdf");
					
					foreach (FileInfo fileInfo in files) {
						// origin: https://github.com/UglyToad/PdfPig/blob/master/examples/ExtractTextWithNewlines.cs
						filePath = fileInfo.FullName;
						using (var document = PdfDocument.Open(filePath)) {
							// Debug.WriteLine(String.Format("Reading {0}", filePath));
							Log(String.Format("Reading {0}", filePath));
							foreach (var page in document.GetPages()) {
								
								var text = ContentOrderTextExtractor.GetText(page, true);

								Log(String.Format("text: {0}", text));
								// Debug.WriteLine(String.Format("text: {0}", text));
							}
						}
					}
					
					files = directoryInfo.GetFiles("*.pdf");

					foreach (FileInfo fileInfo in files) {
						filePath = fileInfo.FullName;
						var dic = new Dictionary<string, string> { };

						using (var stream = File.OpenRead(filePath)) {
							var document = new XWPFDocument(stream);

								if (txtSearchKey1.Text != "") {
									dic.Add(txtSearchKey1.Text, txtReplace1.Text);
								}

						}
					}
					files = directoryInfo.GetFiles("*.docx");

					foreach (FileInfo fileInfo in files) {
						filePath = fileInfo.FullName;
						var dic = new Dictionary<string, string> { };

						using (var stream = File.OpenRead(filePath)) {
							var document = new XWPFDocument(stream);

								if (txtSearchKey1.Text != "") {
									dic.Add(txtSearchKey1.Text, txtReplace1.Text);
								}

								if (txtSearchKey4.Text != "") {
									dic.Add(txtSearchKey4.Text, null);
								}

								if (txtSearchKey3.Text != "") {
									dic.Add(txtSearchKey3.Text, null);
								}

								foreach (var paragraph in document.Paragraphs) {
									ReplaceKey(paragraph, dic);
									//ReplaceKeyword(paragraph, dic);
								}
								// replace - not used
								/*
								using (var newstream = File.Create(fileInfo.Directory + "/" + fileInfo.Name)) { // "/poutput.docx"
									doc.Write(newstream);
									newstream.Flush();
								}
								
								*/
						}
					}
					MessageBox.Show("Done.");
				});
		}

		private void btnClose_Click(object sender, EventArgs e) {
			this.Close();
		}

		private void ReplaceKey(XWPFParagraph paragraph, IDictionary<string, string> redic)
		{
			string text = paragraph.ParagraphText;
			foreach (var kv in redic) {
				if (text.Contains(kv.Key)) {
					if (!String.IsNullOrEmpty(kv.Value))
						paragraph.ReplaceText(kv.Key, kv.Value);
				}
			}
		}
		// currently unused - keep for possible later use
		private void ReplaceKeyword(XWPFParagraph paragraph, IDictionary<string, string> redic) {
			string text = string.Empty;
			string styleid = paragraph.Style;
			var runs = paragraph.Runs;
			for (int i = 0; i < runs.Count; i++) {
				var run = runs[i];
				text = run.ToString();
				foreach (var kv in redic) {
					if (text.Contains(kv.Key)) {
						text = text.Replace(kv.Key, kv.Value);
					}
				}

				runs[i].SetText(text, 0);
			}
		}

		void chooseFile(object sender, EventArgs e) {
			var o = new OpenFileDialog();
			o.Title = "Choose a file";
			o.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			o.Filter = "SpreadSheet (*.xlsx)|*.xls|Text (*.txt)|*.docx|All files (*.*)| *.*";
			if (o.ShowDialog() == DialogResult.OK) {
				txtBox1.Tag = o.FileName;
				var size = 20; // approx width in characters
				txtBox1.Text = "…" + o.FileName.Substring(o.FileName.Length - size, size);
			}

		}
		void TxtDocDirectoryTextChanged(object sender, EventArgs e) {
	
		}
		void TabPageClick(object sender, EventArgs e) {
	
		}
		void TxtSearchKey4TextChanged(object sender, EventArgs e)
		{
	
		}
		void ProgramLoad(object sender, EventArgs e)
		{
	
		}
	}
}

param (
  [switch] $debug # currently unused
)

[bool]$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent -bor $DebugPreference -eq 'Continue'
$shared_assemblies_path = (resolve-path -path '.').Path
$shared_assemblies_path = 'C:\developer\sergueik\powershell_samples\csharp\basic-budget-searcher'
$shared_assemblies  = @(
'ICSharpCode.SharpZipLib.dll',
'NPOI.OOXML.dll',
'NPOI.OpenXml4Net.dll',
'NPOI.OpenXmlFormats.dll',
'NPOI.dll',
'UglyToad.PdfPig.Core.dll',
'UglyToad.PdfPig.DocumentLayoutAnalysis.dll',
'UglyToad.PdfPig.Fonts.dll',
'UglyToad.PdfPig.Package.dll',
'UglyToad.PdfPig.Tokenization.dll',
'UglyToad.PdfPig.Tokens.dll',
'UglyToad.PdfPig.dll'
)


  pushd $shared_assemblies_path

  $shared_assemblies | ForEach-Object {
    if ($host.Version.Major -gt 2) {
      Unblock-File -Path $_
    }
    write-debug $_
    # TODO: Add-Type : Unable to load one or more of the requested types. Retrieve the LoaderExceptions property for more information.
    Add-Type -Path $_
  }
  popd

$source = @"
using System;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using NPOI.XWPF.UserModel;
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
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Program());
		}

		public Program() { InitializeComponent(); }
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		// https://pbdd.org/wp-content/uploads/2015/06/WordPractice2007.docx
		private void InitializeComponent() {
			ComponentResourceManager resources = new ComponentResourceManager(typeof(Program));
			this.btnSelectFile = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.txtDocDirectory = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.txtSearchKey1 = new System.Windows.Forms.TextBox();
			this.txtReplace1 = new System.Windows.Forms.TextBox();
			this.btnReplace = new System.Windows.Forms.Button();
			this.btnClose = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.txtSearchKey3 = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.txtSearchKey4 = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.label12 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			//
			// btnSelectFile
			//
			this.btnSelectFile.Location = new System.Drawing.Point(112, 89);
			this.btnSelectFile.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.btnSelectFile.Name = "btnSelectFile";
			this.btnSelectFile.Size = new System.Drawing.Size(207, 46);
			this.btnSelectFile.TabIndex = 0;
			this.btnSelectFile.Text = "Choose Folder";
			this.btnSelectFile.UseVisualStyleBackColor = true;
			this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(341, 101);
			this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(120, 25);
			this.label1.TabIndex = 1;
			this.label1.Text = "Base folder：";
			//
			// txtDocDirectory
			//
			this.txtDocDirectory.Location = new System.Drawing.Point(486, 89);
			this.txtDocDirectory.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.txtDocDirectory.Name = "txtDocDirectory";
			this.txtDocDirectory.Size = new System.Drawing.Size(256, 29);
			this.txtDocDirectory.TabIndex = 2;
			this.txtDocDirectory.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtDocDirectory.TextChanged += new System.EventHandler(this.TxtDocDirectoryTextChanged);
			//
			// label2
			//
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(154, 161);
			this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(124, 25);
			this.label2.TabIndex = 3;
			this.label2.Text = "1.Text to find";
			//
			// label3
			//
			this.label3.AutoSize = true;
			this.label3.BackColor = System.Drawing.Color.Tan;
			this.label3.Location = new System.Drawing.Point(154, 215);
			this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(179, 25);
			this.label3.TabIndex = 3;
			this.label3.Text = "Text to be replaced";
			//
			// txtSearchKey1
			//
			this.txtSearchKey1.Location = new System.Drawing.Point(341, 156);
			this.txtSearchKey1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.txtSearchKey1.Name = "txtSearchKey1";
			this.txtSearchKey1.Size = new System.Drawing.Size(401, 29);
			this.txtSearchKey1.TabIndex = 4;
			//
			// txtReplace1
			//
			this.txtReplace1.BackColor = System.Drawing.SystemColors.Window;
			this.txtReplace1.Enabled = false;
			this.txtReplace1.Location = new System.Drawing.Point(341, 209);
			this.txtReplace1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.txtReplace1.Name = "txtReplace1";
			this.txtReplace1.Size = new System.Drawing.Size(401, 29);
			this.txtReplace1.TabIndex = 4;
			//
			// btnReplace
			//
			this.btnReplace.Location = new System.Drawing.Point(112, 770);
			this.btnReplace.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.btnReplace.Name = "btnReplace";
			this.btnReplace.Size = new System.Drawing.Size(137, 46);
			this.btnReplace.TabIndex = 5;
			this.btnReplace.Text = "Start replacing";
			this.btnReplace.UseVisualStyleBackColor = true;
			this.btnReplace.Click += new System.EventHandler(this.btnReplaceText_Click);
			//
			// btnClose
			//
			this.btnClose.Location = new System.Drawing.Point(605, 770);
			this.btnClose.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(137, 46);
			this.btnClose.TabIndex = 6;
			this.btnClose.Text = "Close";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			//
			// label6
			//
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(154, 261);
			this.label6.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(124, 25);
			this.label6.TabIndex = 3;
			this.label6.Text = "3.Text to find";
			//
			// txtSearchKey3
			//
			this.txtSearchKey3.Location = new System.Drawing.Point(343, 258);
			this.txtSearchKey3.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.txtSearchKey3.Name = "txtSearchKey3";
			this.txtSearchKey3.Size = new System.Drawing.Size(399, 29);
			this.txtSearchKey3.TabIndex = 4;
			//
			// label8
			//
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(154, 308);
			this.label8.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(124, 25);
			this.label8.TabIndex = 3;
			this.label8.Text = "4.Text to find";
			//
			// txtSearchKey4
			//
			this.txtSearchKey4.Location = new System.Drawing.Point(343, 308);
			this.txtSearchKey4.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.txtSearchKey4.Name = "txtSearchKey4";
			this.txtSearchKey4.Size = new System.Drawing.Size(399, 29);
			this.txtSearchKey4.TabIndex = 4;
			//
			// button1
			//
			this.button1.Location = new System.Drawing.Point(112, 702);
			this.button1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(207, 46);
			this.button1.TabIndex = 7;
			this.button1.Text = "Choose File";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.Button1Click);
			//
			// label12
			//
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(343, 710);
			this.label12.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(133, 25);
			this.label12.TabIndex = 8;
			this.label12.Text = "Heuristics File";
			//
			// textBox1
			//
			this.textBox1.Location = new System.Drawing.Point(486, 710);
			this.textBox1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(256, 29);
			this.textBox1.TabIndex = 9;
			this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			// MainForm
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(834, 931);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.btnReplace);
			this.Controls.Add(this.txtReplace1);
			this.Controls.Add(this.txtSearchKey4);
			this.Controls.Add(this.txtSearchKey3);
			this.Controls.Add(this.txtSearchKey1);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtDocDirectory);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnSelectFile);
			
			const string iconBase64 = "AAABAAEAMDAAAAEAIACoJQAAFgAAACgAAAAwAAAAYAAAAAEAIAAAAAAAACQAAAAAAAAAAAAAAAAAAAAAAAD+/v7///////////////////////////////////////7////+/v7/+f79//r9/f/+/v7/9v78//n++P/Q+/D/fO/U/ynjvP8A3rH/Ad+x/wHgsP8A36//AOCv/wLfr/8d3rb/XOvP/9v78f/5/Pj/9f78//f+/f/+/f/////////////////////////////////////////////////////////////////////////////////////////////+/v7///////////////////////////////////////z+/v/4//7//f/+/+z89f+D8Nj/Dduy/wHesP8C3q//AOGv/wDhsP8D3a3/AN+v/wHgsP8B4LD/BN2w/wTfr/8A4rD/AN+u/wTesP8A3a//UOfH/+n99f/7/v7////////////////////////////////////////////////////////////////////////////////////////////+/v7///////////////////////z+/f/8/f3/+f39//3+/P+n9eH/HeCz/wDfr/8F37D/AOCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8D4LD/CN6v/wDerv9s7c3/7v/6//z+/v/////////////////////////////////////////////////////////////////////////////////+/v7///////////////////////z+/v/9/f3/sfTi/wXcq/8G3q//AOCv/wPgsP8A36//AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8D4LD/BeCw/wLfr/8D36//Kd+5//P++v/+/v7////////////////////////////////////////////////////////////////////////////+/v7//v7+//7+/v////////////39/f/8/v7/+P7+//v9/P++9+v/PuS+/wXer/8A4LD/A9+w/wPgsP8D4LD/A+Cw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8D4LD/AeCv/wDgr/8D37D/AOGu/xret//5/vz//v7+//7+/v/+/v7////////////////////////////////////////////////////////////8/f3//fz9//b++v+z5dn/f9TB/7Xm2P/2/fn/+v78//r9/f/7/v3//fv8//X8+f+09OT/E+C2/wLdrP8B4bD/CN+w/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AOCv/wXer/8j4bb//v78//z8/f///v/////////////////////////////////////////////////////////////5/fz/s+3e/wOgff8Dp4D/AKmB/wOpgP8AqYL/BZ18/z2+ov/n+PH/9v37//39/f/7//7//v3+/+n98/8k3rb/BN2u/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/B92v/wDgr/8D36//efDV///9/v/+/v7////////////////////////////////////////////////////////////T8On/DqSE/wGpgP8AqYL/AKmC/wCpgv8AqYL/AKmC/wCogf8Cp4H/BqeB/2rMtP/v+/f/9/r5//39/f/8/v7/1fjx/xXbs/8D3q//CN6u/wPer/8A3q7/AOCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B37D/ANuq/+T78v/6//3//f////7+/v////////////////////////////////////////////////+l4NL/C6B//wGof/8AqYL/AKmC/wCogf8AqYL/AKiB/wCpgf8AqIH/AamC/wCphP8Fo4H/Rbmh/+j49f/6/Pz/9/r6//v++/9d6Mr/AOCy/wLgsP8A4LD/At+w/wHgsP8B4LD/AN+v/wDfr/8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8A36//AN+v/ybdt//2//z//P/+//7+/v/////////////////////////////////////////////////c9e//oOTR/w2ngv8BpX//AaV//wClfv8ApX7/AKiB/wCngP8AqIH/AKiB/wOpgv8Ep4H/AamB/wCmgf9hw6z/6/z4//v9/v/3/vz/tfXm/wvdr/8F4bH/At+v/wPgsP8D4LD/BOGx/wPgsP8D4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/At+u/wDdsP/L+O///P/+//7+/v/////////////////////////////////////////////////7/v3/+Pz8//b9/P/4/vr//fv7//D/9//1/fv/6Pr2/77n3f9+0bv/I66O/wSkgP8Ep4H/AqmB/wKngP8Dp4D/BKWB/6Hf0P/4/fz/+fz8//j7+v/J+Ov/mPHa/4bx1v9C6MT/C92v/wDfr/8D4bH/AeCw/wHgsP8B4LD/AeCw/wHgsP8A4LD/AOCw/wPfsP8r4Lv/+P/9//v7/P/////////////////////////////////////////////////9/v7/+f/+//b+/v/8/P3/+/38/xR0W/8Ffl3/C4Bf/ymKcv+Evq//+v/8//n++P+k5dL/IqSG/wGkgP8AqYL/AKd//wGqgv82sZb/8f76//P6+f+/9+v/Bdqu/wTdsf8B4LP/BuCx/wPgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8I3q//1vvw//n+/v/////////////////////////////////////////////////9/v7//v7+///////+/v7//v/8/wZ8X/8Af17/AH9e/wGAXP9Dm4P//f78/222o/82jHP/0PLl/9z27/8hqYz/DKiF/wCrgf8BqYL/FqmG/9r47P/3/v3/1vvv/wfdrv8C4LH/AeCw/wHfsP8A36//AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wDgsP8H37H/WObG//j+/P/////////////////////////////////////////////////+/v7//////////////////v/8/wZ8X/8BgF7/AYBe/wGBXP9Dm4P//f78/3C2nv8Aflv/A39e/wJ2Vf/m8Ob/2fTr/w+igP8DqIL/CKaB/wGmgv+V3cz//Pv7/8D15/8B3Kz/AeCw/wDfr/8A36//AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8A4K//CNqv//j+/P/////////////////////////////////////////////////+/v7//////////////////v/8/wZ8X/8BgF7/AYBe/wGBXP9Dm4P//f78/2m3of8Bf13/AH5e/wB+XP8Agl//IoJl/+L37/9iw6n/AaeA/weqg/8AqoL/acew//77/P+X9OD/AN+v/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8H36//AN+v/7f26P/////////////////////////////////////////////////+/v7//////////////////v/8/wZ8X/8BgF7/AYBe/wGBXP9Dm4P//f78/263of8DgF3/AH9d/wCAXv8AgF7/AIBe/wN7XP9+wKz/yvDl/wekgv8ArIT/CaaB/0i5nv/5/vv/3frz/yfbt/8H267/DNyu/wnfsP8L4rT/AeCw/wHgsP8B4LD/AeCw/wHgsP8D3bD/AuCw/1Hmxv/7/v3//P/+///////////////////////////////////////+/v7//////////////////v/8/wZ8X/8BgF7/AYBe/wGBXP9Dm4P//f78/263of8DgF3/AH9d/wCAXv8AgF7/AIBe/wGAX/8Ffl7/HYVo//f+9v8aq4r/BKmD/wOogv82tZj/8f37/8z67/+h8d//YezK/xHdsP8M3rD/AeCw/wHgsP8B4LD/AeCw/wHgsP8A3rD/Bt6v/w3ds//8//7//P/+//7+/v/////////////////////////////////+/v7//////////////////v/8/wZ8X/8BgF7/AYBe/wSBX/9EmoL//v38/222of8Eflz/AIBf/wCAXv8AgF7/AH9d/wN+Xv8AgF7/AH5b/wV4W//G6dr/UL2h/wWmgf8EqIP/KLCS//X/+v9F58T/AN2t/wDer/8C367/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/A96u/wTfsP/d+/P//f7///3+/v/////////////////////////////////+/v7//////////////////v/8/wd9YP8BgF7/AYBe/wGBX/9Fm4P///z8/2u2of8CfVv/AH9e/wCAXv8AgF7/AIBe/wCAXv8AgF7/AH9d/wCCX/8FeVr/mMu8/4DRvP8FpX7/Aqd//yitj//w/Pf/KuC7/wPgsf8D367/AeCw/wHgsP8B4LD/AeCw/wHgsP8C4LD/AeCw/wLfr/9f6Mv/+f79//7+/v/////////////////////////////////+/v7//////////////////v/8/wd9YP8BgF7/AYBe/wGBX/88l33/+f77/1yrlf8Cf1z/AH5d/wCAXv8AgF7/AIBe/wCAXv8AgF7/AIBe/wKCYP8AgF7/AYNh/16ok/+h4s//AaV//wSsg/8bqIr/+v76/yXdt/8A367/AeCw/wHgsP8B4LD/AeCw/wHgsP8C4LD/AeCw/wPgsP8M37X//f/+///+/P/////////////////////////////////+/v7//////////////v7//f38/wV7Xv8BgF7/AIBe/wOCYP8Eflz/FHZY/wB9W/8BgF7/AH9d/wCAXv8AgF7/AIBe/wCAXv8AgF7/AIBe/wCAXv8AgF7/AoBe/wOBX/9CkXj/0fHn/wCphP8Dp3//IK6O/+L+9/8M27H/B9+w/wTfsP8F3LD/AN+v/wHfr/8B36//AeCw/wHgsP8A26//5Pzz//z+/v/////////////////////////////////+/v7////////////+/f3//v/9/wt5Xv8CgV//AH9d/wSDYf8DgV//AH1b/wGAXv8CgV//AYBe/wCAXv8AgF7/AIBe/wCAXv8AgF7/AIBe/wCAXv8AgF7/AIBe/wOBX/8CgWH/H4Jn/+P78v8Cp4L/AamA/zOzlP/s+/X/E9qx/wTjsv8H3a7/AN+v/wDfr/8A4K//Ad+v/wHgsP8D3q//T+TE//79/v/////////////////////////////////+/v7////////////9/f7//f79/0Cbhf8CgF7/AH5c/wB/Xv8DgF7/AH9d/wCAXv8Af13/AIBe/wCAXv8AgF7/AIBe/wCAXv8AgF7/AIBe/wCAXv8AgF7/AIBe/wB/Xf8Af17/B31c/xV4Xv/j/fX/EqOC/wKrgv89tpj/+v75/3Dr0P8i37T/Hd+1/y/lu/8K4rP/At+v/wLfr/8B36//CN+0//P++v///v7//v////7////////////////////+/v7////////////8//7//v7+/8vp4/8AfV3/AH5c/wB/Xv8AgF7/AIBe/wCAXv8AgF7/AIBe/wCAXv8AgF7/AIBe/wCAXv8AgF7/AIBe/wCAXv8AgF7/AIBe/wCAXv8AgF7/BH9e/wCBXv8IeFv/3PXq/x+xjv8FqID/MbKU/+n79v8k27j/Dd2y/wPgsP8F4LD/A+Cw/wPgsP8G37D/CeCy/7L05P/9/v7//P7+//7+/v/////////////////+/v7////////////9/v7//P39//j+/P8YfGL/AYBd/wB/Xf8AgF7/AIBe/wCAXv8AgF7/AIBe/wCAXv8AgF7/AIBe/wCAXv8AgF7/AIBe/wCAXv8AgF7/AIBe/wCAXv8AgF7/AIFe/wF/Xf8Ff2D/BXxc/7/h0/9Ywan/A6iC/ySvjP/s/fT/Cdyw/wPfr/8D3q7/A+Cw/wPgsP8A3a//AN6s/wfesv/0/vv/+/39//7////////////////////+/v7///////////////////////39/f/h9+//BXxc/wB/XP8EgF7/AHxc/wCAXv8AgF7/AIBe/wV/Xf8CgV//Bn5f/wF9Xf8BfV3/BH1e/wB/X/8AgF3/AIFf/wB/Xf8AgF7/AYBe/wCAXv8AgF7/BH5a/wJ3Wv/D4dn/mNvK/wKge/8Pon7/8f33/xndsv8A4K//B9+w/wDfr/8B4LD/AOCw/wTfsP+z9eb/+P77///9/v/////////////////+/v7///////////////////////39/f/5+/r/0PHm/wJ5W/8Af17/AH1b/wCAXv8AgF7/AIBe/wB/Xf8HfV3/AH5c/wB1Vv8Fd1j/AHVW/wB4WP8AgF7/AH5c/wCAXv8AgF7/AH9d/wCAXv8BgF7/AH1a/4O4qP/6/Pv/+vz7/8306f8BoX7/B6qE/9z37f834L3/Bd2w/wDfr/8D37D/AeCw/wLgsP8H37P/+v/6//v9/v/////////////////+/v7///////////////////////79/v/6////9f/9/+b+9v8Ldlj/An9d/wB/Xf8Af13/AH9d/xd8Yv/P7uH/+/73//j++v/7/vn/9/76//3++f/j//T/KZN2/wF+XP8Aflz/AH9d/wGAX/8Be1r/vOva//7++//+/v7///3+//j//f/7//v/J6uM/wCkgP+d4c//evDU/wDdrv8F36//AOCw/wHfsP8H5bP/k+7Z//X8+//+/v7//v////7////+/v7///////////////////////////////////////v++//7/vr/jsi3/xB/Yv8Vg2n/+fnx/9r67v8k4Ln/At2x/wDdrP8B36//Ad+u/wDerv8W27P/o/Xj//f/+/8wjnX/Bnpc/1CciP/3/vn/+v79//v+/f/9//////////7////7//7//vz9/6zm2f8PpID/KbSS/9/57/8K3bL/At2t/wDjsf8D4LH/B9qu/+r89v/2/v3/+/79//j+/v/+/v7///////////////////////////////////////7+/v/9/f3/9/v7//X9+//x/vn/IeC5/wHfrv8C3a7/A92u/wTdrv8A36//BN+v/wrdsP8B4K//Ad+u/xLbsP/U+O3/9/78//r8/P/3/fz//P/+//7////+/v7///////7////8/v3/+P////37+//5/vv/bcax/wehff+Y3Mn/fu/V/wrbr/8B3rD/AeGu/yriu//5/v7/+/7+///////+/v7////////////////////////////////////////9/v/7/v7/+P78/+H89v8K3q//Bt2v/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wDfr/8J3a//r/Xn//P++////P3////////////////////////////////////////////+/v7//P39/+T7+P9XxKr/D51+/6bk0/9n6cv/Bd2u/wTisP986tD/+f36//r+///+/v7///////////////////////////////////////v8/P/8/f3/8P34/wvcsv8G4bH/AN+w/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/A96v/wHgsP8B3q//Ad+w/9X67f/6//3////////////////////////////////////////////7/v3//v7+//79/f/6/fr/8fz3/5/dz/8YqYz/dMq1/4Xx1v8P3LL/wPbq//39/f/+/v7///////////////////////////////////////j////8/P3/RefG/wDdsP8C4K7/BN+w/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AOCw/wHgsP8B36//AeCu/wvYr//7/fv////////////////////////////////////////////+/v7//P7+//39/f/5//7/+v7+//z8/P/5/vv/8fz3/8Dq3P9eyK//Xcay/9bw7P/+/v7///////////////////////////////////////39/f/Z+vH/Aduu/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AN6u/wbfrv+B7tf/+v79//3+/v/+/v7///////////////////////////////////////////////////////////////////////7+/v/7/v7//P39//3+/v/+/v7///////////////////////////////////////7+/P9M58f/AN+v/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/BN6w/wDirv8L2K3/9P78//z8/P/7//////////////////////////////////////////////////////////////////////////7+/v/9/v7//v7+//7+/v/+/v7///////////////////////////////////////7/+/8I3rD/Bd6v/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8A36//3fv0//T+/f/9/v7////////////////////////////////////////////////////////////////////////////////////////////+/v7///////////////////////////////////////P/+v8A3a7/Ad+v/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8A36//tPbn//f//v/+/v7////////////////////////////////////////////////////////////////////////////////////////////+/v7///////////////////////////////////////L9+P8D3a//AOCv/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AN+v/wHgsP8B4LD/h/Da//7+/v/8/v/////////////////////////////////////////////////////////////////////////////////////////////+/v7//////////////////////////////////////+z+9/8B3rD/Bd6v/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/iPDa//j+/f/8///////////////////////////////////////////////////////////////////////////////////////////////+/v7///////////////////////////////////////f++f8C3a//BN+v/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8A36//rPPi//z+/v///v/////////////////////////////////////////////////////////////////////////////////////////////+/v7//////////////////////////////////f////v+/P8G3LD/AeGv/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/BN2w/wrer/8C3q7/1/rz//f+/f/9///////////////////////////////////////////////////////////////////////////////////////////////+/v7//////////////////////////////////f////v//f855MD/At+w/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCv/wPerv8K2a3/+v78//38/f/6/f7////////////////////////////////////////////////////////////////////////////////////////////+/v7//////////////////////////////////f////n+/v+89+f/AN+v/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/A+Gu/wjesP9o58r/+v38//r+/P/+///////////////////////////////////////////////////////////////////////////////////////////////+/v7////////////////////////////////////////////3/v3/J+C3/wDgsP8K37D/AN+v/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/BOCw/wXfr//r/vr////////////////////////////////////////////////////////////////////////////////////////////////////////////+/v7///////////////////////////////////////b/+//9/P3/3fvy/wTcrf8A3q7/A9+w/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AeCw/wHgsP8B4LD/AN6u/6Px3v/2/fz////////////////////////////////////////////////////////////////////////////////////////////////////////////+/v7///////////////////////////////////////v//f/8//7/+/79/6/45v8A3rD/AuCu/wHfsf8C3rD/Ad6v/wHgsP8B4LD/AOCw/wDhsP8A36//AN+t/wLfrv8F3bD/gOrS//f++//9/vz////////////////////////////////////////////////////////////////////////////////////////////////////////////+/v7//////////////////////////////////////////////////v7+//P++//W+e7/AN6w/wnesP8B363/AN+s/wHgsP8B4LD/AN+v/wLer/8A367/Ad+v/wXZrf+I8dn//f39///+/v/2//3////////////////////////////////////////////////////////////////////////////////////////////////////////////+/v7//////////////////////////////////////////////////v7+//3+/v/6/fz/8/37/3Xs0f8A3bD/At6x/wHgsP8B4LD/AN+w/wXcsP8A3a//QOTB/+399//4/fz/+v38//j9/P/+/v7///////////////////////////////////////////////////////////////////////////////////////////////////////////8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=";
			byte[] iconBytes = Convert.FromBase64String(iconBase64);
			MemoryStream iconStream = new MemoryStream(iconBytes, 0, iconBytes.Length);
			iconStream.Write(iconBytes, 0, iconBytes.Length);
			var iconImage = Image.FromStream(iconStream, true);
			var iconBitmap = new Bitmap(iconStream);
			IntPtr hicon = iconBitmap.GetHicon();
			Icon icon = Icon.FromHandle(hicon);

			this.Icon = icon;
			// Icon.FromHandle(Properties.Resources.selenium.GetHicon());
			// this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.Name = "MainForm";
			this.Text = "word pdf search";
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
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox textBox1;
	
		private void btnSelectFile_Click(object sender, EventArgs e)
		{
			var folderBrowserDialog = new FolderBrowserDialog();

			if (folderBrowserDialog.ShowDialog() == DialogResult.OK) {
				txtDocDirectory.Text = folderBrowserDialog.SelectedPath;
			}
		}

		private void btnReplaceText_Click(object sender, EventArgs eventArgs)
		{
			string docDirectory = txtDocDirectory.Text;
			FileInfo[] files = { };
			DirectoryInfo directoryInfo = null;
			string filePath = null;
			ThreadPool.QueueUserWorkItem(
				// Error CS1593: Delegate 'System.Threading.WaitCallback' does not take 0 arguments
				(object state) => {
					directoryInfo = new DirectoryInfo(docDirectory);
					Debug.WriteLine(String.Format("Scanning {0}", directoryInfo.FullName));
					
					files = directoryInfo.GetFiles("*.pdf");
					
					foreach (FileInfo fileInfo in files) {
						// origin: https://github.com/UglyToad/PdfPig/blob/master/examples/ExtractTextWithNewlines.cs
						filePath = fileInfo.FullName;
						using (var document = PdfDocument.Open(filePath)) {
							Debug.WriteLine(String.Format("Reading {0}", filePath));
							foreach (var page in document.GetPages()) {
								
								var text = ContentOrderTextExtractor.GetText(page, true);

								Debug.WriteLine(String.Format("text: {0}", text));
							}
						}
					}
					files = directoryInfo.GetFiles("*.docx");

					foreach (FileInfo fileInfo in files) {
						filePath = fileInfo.FullName;
						var dic = new Dictionary<string, string> { };

						using (var stream = File.OpenRead(filePath)) {
							var document = new XWPFDocument(stream);

							try {
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
							} catch (Exception e) {
								MessageBox.Show("Exception: " + e.Message);
							}
						}
					}
					MessageBox.Show("Done.");
				});
		}
		private void btnClose_Click(object sender, EventArgs e)
		{
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
		private void ReplaceKeyword(XWPFParagraph paragraph, IDictionary<string, string> redic)
		{
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
		void MainFormLoad(object sender, EventArgs e)
		{
	
		}
		void Button1Click(object sender, EventArgs e)
		{
			var o = new System.Windows.Forms.OpenFileDialog();
			o.Title = "Select a file";
			o.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			o.Filter = "SpreadSheet (*.xlsx)|*.xls|Text (*.txt)|*.docx|All files (*.*)| *.*";
			if (o.ShowDialog() == DialogResult.OK) {
				textBox1.Tag = o.FileName;
				var size = 20; // approx width in characters
				textBox1.Text = "…" + o.FileName.Substring(o.FileName.Length - size, size);
			}

		}
		void TxtDocDirectoryTextChanged(object sender, EventArgs e)
		{
	
		}
	}
}
"@


add-type -typedefinition $source -language CSharp -ReferencedAssemblies 'System.Windows.Forms.dll','System.Drawing.dll','System.Data.dll', 'System.Collections.dll', 'System.xml.dll','System.Xml.Linq.dll',`
'C:\developer\sergueik\powershell_samples\csharp\basic-budget-searcher\ICSharpCode.SharpZipLib.dll','C:\developer\sergueik\powershell_samples\csharp\basic-budget-searcher\NPOI.OOXML.dll',`
'C:\developer\sergueik\powershell_samples\csharp\basic-budget-searcher\NPOI.OpenXml4Net.dll',`
'C:\developer\sergueik\powershell_samples\csharp\basic-budget-searcher\NPOI.OpenXmlFormats.dll',`
'C:\developer\sergueik\powershell_samples\csharp\basic-budget-searcher\NPOI.dll',`
'C:\developer\sergueik\powershell_samples\csharp\basic-budget-searcher\UglyToad.PdfPig.Core.dll',`
'C:\developer\sergueik\powershell_samples\csharp\basic-budget-searcher\UglyToad.PdfPig.DocumentLayoutAnalysis.dll',`
'C:\developer\sergueik\powershell_samples\csharp\basic-budget-searcher\UglyToad.PdfPig.Fonts.dll',`
'C:\developer\sergueik\powershell_samples\csharp\basic-budget-searcher\UglyToad.PdfPig.Package.dll',`
'C:\developer\sergueik\powershell_samples\csharp\basic-budget-searcher\UglyToad.PdfPig.Tokenization.dll',`
'C:\developer\sergueik\powershell_samples\csharp\basic-budget-searcher\UglyToad.PdfPig.Tokens.dll',`
'C:\developer\sergueik\powershell_samples\csharp\basic-budget-searcher\UglyToad.PdfPig.dll'
[Program.Program]::Main()
# TODO: Exception calling "Main" with "0" argument(s):
# "SetCompatibleTextRenderingDefault must be called before the first
# IWin32Window object is created in the application."

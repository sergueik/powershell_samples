using System;
using System.Windows.Forms;

namespace CustomDialog {

	public class SampleDialog : BaseDialog {

		#region Windows Form Designer generated code

		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TextBox text1;
		protected System.Windows.Forms.Button buttonAction;
		private System.Windows.Forms.Label label1;

		private void InitializeComponent() {
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SampleDialog));
			this.text1 = new System.Windows.Forms.TextBox();
			this.buttonAction = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// cbCancel
			// 
			this.cbCancel.Location = new System.Drawing.Point(310, 176);
			this.cbCancel.Name = "cbCancel";
			// 
			// cbOk
			// 
			this.cbOk.Location = new System.Drawing.Point(214, 176);
			this.cbOk.Name = "cbOk";
			this.cbOk.Text = "OK";
			// 
			// Line1
			// 
			this.Line1.Location = new System.Drawing.Point(6, 168);
			this.Line1.Name = "Line1";
			this.Line1.Size = new System.Drawing.Size(398, 6);
			// 
			// text1
			// 
			this.text1.AcceptsReturn = true;
			this.text1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.text1.Location = new System.Drawing.Point(8, 8);
			this.text1.Multiline = true;
			this.text1.Name = "text1";
			this.text1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.text1.Size = new System.Drawing.Size(394, 116);
			this.text1.TabIndex = 0;
			this.text1.Text = "Type some text here";
			this.text1.WordWrap = false;
			// 
			// buttonAction
			// 
			this.buttonAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonAction.Location = new System.Drawing.Point(8, 176);
			this.buttonAction.Name = "buttonAction";
			this.buttonAction.Size = new System.Drawing.Size(116, 24);
			this.buttonAction.TabIndex = 14;
			this.buttonAction.Text = "Do Some Action";
			this.buttonAction.Click += new System.EventHandler(this.buttonSave_Click);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label1.ForeColor = System.Drawing.Color.Green;
			this.label1.Location = new System.Drawing.Point(8, 132);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(394, 32);
			this.label1.TabIndex = 13;
			this.label1.Text = "This is a sample dialog. It illustrates the principle of making your own dialogs " +
				"by inheriting from BaseDialog form.";
			// 
			// SampleDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(410, 207);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonAction);
			this.Controls.Add(this.text1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "SampleDialog";
			this.ShowInTaskbar = true;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Sample Dialog";
			this.Controls.SetChildIndex(this.text1, 0);
			this.Controls.SetChildIndex(this.Line1, 0);
			this.Controls.SetChildIndex(this.cbOk, 0);
			this.Controls.SetChildIndex(this.cbCancel, 0);
			this.Controls.SetChildIndex(this.buttonAction, 0);
			this.Controls.SetChildIndex(this.label1, 0);
			this.ResumeLayout(false);

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

		public SampleDialog() {
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
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CustomDialog {

	public class BaseDialog : System.Windows.Forms.Form {

		#region Windows Form Designer generated code

		protected System.Windows.Forms.Button cbCancel;
		protected System.Windows.Forms.Button cbOk;
		protected Line Line1;
		private System.ComponentModel.Container components = null;

		private void InitializeComponent() {
			this.cbCancel = new System.Windows.Forms.Button();
			this.cbOk = new System.Windows.Forms.Button();
			this.Line1 = new CustomDialog.Line();
			this.SuspendLayout();
			// 
			// cbCancel
			// 
			this.cbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cbCancel.Location = new System.Drawing.Point(112, 220);
			this.cbCancel.Name = "cbCancel";
			this.cbCancel.Size = new System.Drawing.Size(92, 24);
			this.cbCancel.TabIndex = 11;
			this.cbCancel.Text = "Cancel";
			this.cbCancel.Click += new System.EventHandler(this.cbCancel_Click);
			// 
			// cbOk
			// 
			this.cbOk.Location = new System.Drawing.Point(12, 220);
			this.cbOk.Name = "cbOk";
			this.cbOk.Size = new System.Drawing.Size(92, 24);
			this.cbOk.TabIndex = 10;
			this.cbOk.Text = "Ok";
			this.cbOk.Click += new System.EventHandler(this.cbOk_Click);
			// 
			// Line1
			// 
			this.Line1.Location = new System.Drawing.Point(44, 208);
			this.Line1.Name = "Line1";
			this.Line1.Size = new System.Drawing.Size(304, 6);
			this.Line1.TabIndex = 12;
			// 
			// BaseDialog
			// 
			this.AcceptButton = this.cbOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cbCancel;
			this.ClientSize = new System.Drawing.Size(390, 251);
			this.Controls.Add(this.Line1);
			this.Controls.Add(this.cbCancel);
			this.Controls.Add(this.cbOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "BaseDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.ResumeLayout(false);

		}

		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion

		public BaseDialog() {
			InitializeComponent();
		}

		private void cbOk_Click(object sender, System.EventArgs e) {
			ExitOk();
		}

		private void cbCancel_Click(object sender, System.EventArgs e) {
			ExitCancel();
		}

		protected void ExitOk() {
			if(cbOk.Enabled==true) {
				if(OnOk()) {
					this.DialogResult = DialogResult.OK;
					this.Close();
				}
			}
		}

		protected void ExitCancel() {
			if(cbCancel.Enabled==true) {
				if(OnCancel()) {
					this.DialogResult = DialogResult.Cancel;
					this.Close();
				}
			}
		}

		protected virtual bool OnOk() {
			return true;
		}

		protected virtual bool OnCancel() {
			return true;
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad (e);
			OnResize(e);
		}

		protected override void OnResize(EventArgs e) {
			base.OnResize (e);
			Line1.Location = new Point(6, this.Size.Height - 64);
			Line1.Size = new Size(this.Size.Width - 18, 2);
			int okx = this.Size.Width - cbOk.Size.Width - 14;
			if(cbCancel.Visible && this.Visible)
				okx -= cbCancel.Size.Width + 4;
			cbOk.Location = new Point(okx, this.Size.Height - cbOk.Size.Height - 32);
			cbCancel.Location = new Point(this.Size.Width - cbCancel.Size.Width - 14, this.Size.Height - cbCancel.Size.Height - 32);
		}

	}

}
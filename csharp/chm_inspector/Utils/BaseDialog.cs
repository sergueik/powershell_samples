using System;
using System.Drawing;
using System.Windows.Forms;

namespace Utils {

	public class BaseDialog : Form {

		#region Windows Form Designer generated code

		protected System.Windows.Forms.Button cbCancel;
		protected System.Windows.Forms.Button cbOk;
		private System.ComponentModel.Container components = null;

		private void InitializeComponent() {
			cbCancel = new Button();
			cbOk = new Button();
			this.SuspendLayout();

			cbCancel.DialogResult = DialogResult.Cancel;
			cbCancel.Location = new Point(112, 220);
			cbCancel.Name = "cbCancel";
			cbCancel.Size = new Size(92, 24);
			cbCancel.TabIndex = 11;
			cbCancel.Text = "Cancel";
			cbCancel.Click += new EventHandler(cbCancel_Click);

			cbOk.Location = new Point(12, 220);
			cbOk.Name = "cbOk";
			cbOk.Size = new Size(92, 24);
			cbOk.TabIndex = 10;
			cbOk.Text = "Ok";
			cbOk.Click += new System.EventHandler(cbOk_Click);
			// 
			// BaseDialog
			// 
			this.AcceptButton = cbOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = cbCancel;
			this.ClientSize = new System.Drawing.Size(390, 251);

			this.Controls.Add(cbCancel);
			this.Controls.Add(cbOk);
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
			int okx = this.Size.Width - cbOk.Size.Width - 14;
			if(cbCancel.Visible && this.Visible)
				okx -= cbCancel.Size.Width + 4;
			cbOk.Location = new Point(okx, this.Size.Height - cbOk.Size.Height - 32);
			cbCancel.Location = new Point(this.Size.Width - cbCancel.Size.Width - 14, this.Size.Height - cbCancel.Size.Height - 32);
		}

	}

}

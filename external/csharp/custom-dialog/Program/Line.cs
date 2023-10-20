using System;
using System.Drawing;
using System.Windows.Forms;

namespace CustomDialog {

	public class Line : Control {

		#region Component Designer generated code
	
		private System.ComponentModel.Container components = null;

		private void InitializeComponent() {
			// 
			// Line
			// 
			this.Name = "Line";
			this.Size = new System.Drawing.Size(264, 150);

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

		public Line() {
			InitializeComponent();
		}

		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint (e);
			Redraw(e.Graphics);
		}

		private void Redraw(Graphics g) {
			// drawing dark edge of line
			Pen pen = new Pen(SystemColors.ControlDark, 1);
			Point p1 = new Point(1,  2);
			Point p2 = new Point(this.Width - 2, 2);
			g.DrawLine(pen, p1, p2);
			// drawing bright edge of line
			pen = new Pen(SystemColors.ControlLightLight, 1);
			p1 = new Point(1,  3);
			p2 = new Point(this.Width - 2, 3);
			g.DrawLine(pen, p1, p2);
			p1 = new Point(this.Width - 2, 2);
			g.DrawLine(pen, p1, p2);
		}

		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			// height shouldn't change, only length changes
			this.Height = 6;
			Redraw(this.CreateGraphics());
		}

	}

}
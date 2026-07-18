using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.IO;

namespace Utils {
	public class CustomDisabledIconRenderer: ToolStripProfessionalRenderer {
		protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e) {
			// No image? Let the base class handle it.
			if (e.Image == null) {
				base.OnRenderItemImage(e);
				return;
			}

			// Tag != null indicates that this item should use custom disabled appearance
			if (e.Item.Tag != null) {
				// TODO: intriduce enum
				// VmInfo vm = e.Item.Tag as VmInfo;
				// if (vm != null && vm.State == VmState.Saved)
				// NOTE: does not scale the image bitmap
				// https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.controlpaint.drawimagedisabled?view=netframework-4.5
				if (e.Image.Size.Width == e.ImageRectangle.Width && e.Image.Size.Height == e.ImageRectangle.Height)
					ControlPaint.DrawImageDisabled(e.Graphics, e.Image, e.ImageRectangle.X, e.ImageRectangle.Y, e.Item.BackColor);
				else {
					Debug.WriteLine(String.Format("Image size: {0} ImageRectangle: {1} ", e.Image.Size, e.ImageRectangle));
					using (var scaledBitmap = new Bitmap(e.Image, e.ImageRectangle.Width, e.ImageRectangle.Height)) {
						
						
						// create a Graphics object to modify the image
						using (Graphics graphics = Graphics.FromImage(scaledBitmap)) {
							using (var pen = new Pen(Color.Red, 2)) {
								graphics.DrawLine(pen, 0, e.ImageRectangle.Height, e.ImageRectangle.Width,  0 );
								graphics.DrawLine(pen, 0, 0, e.ImageRectangle.Width, e.ImageRectangle.Height);
							}						
						}
						// https://learn.microsoft.com/en-us/dotnet/api/system.drawing.graphics.drawimage?view=netframework-4.5
						e.Graphics.DrawImage(scaledBitmap, e.ImageRectangle.X, e.ImageRectangle.Y, e.ImageRectangle.Width, e.ImageRectangle.Height);

						// https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.controlpaint?view=netframework-4.5
						// ControlPaint.DrawImageDisabled(e.Graphics, e.Image, e.ImageRectangle.X, e.ImageRectangle.Y, e.Item.BackColor);

					}
				}
				// Will dispose the temporary bitmap automatically
				return;
			}

			// Fallback to the standard WinForms ToolStripProfessionalRenderer
			// implementation (image scaling, positioning and normal rendering).
			// https://learn.microsoft.com/en-us/dotnet/api/system.drawing.graphics.drawimage?view=netframework-4.5
			// https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.toolstriprenderer.onrenderitemimage?view=netframework-4.5
			// https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.toolstripprofessionalrenderer?view=netframework-4.5
			base.OnRenderItemImage(e);
		}
	}
}

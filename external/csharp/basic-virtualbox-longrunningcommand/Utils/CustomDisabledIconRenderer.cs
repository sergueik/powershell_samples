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

			// Tag != null manifests "draw using our custom disabled appearance".
			if (e.Item.Tag != null) {
				// TODO: intriduce enum
				// VmInfo vm = e.Item.Tag as VmInfo;
				// if (vm != null && vm.State == VmState.Saved)
				// NOTE: oncorrectly scaled on X
				Debug.WriteLine(String.Format("Image size: {0} ImageRectangle: {1} ", e.Image.Size, e.ImageRectangle));
				// https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.controlpaint.drawimagedisabled?view=netframework-4.5
				ControlPaint.DrawImageDisabled(e.Graphics, e.Image, e.ImageRectangle.X, e.ImageRectangle.Y, e.Item.BackColor);
				return;
			}

			// Fallback to default rendering.
			// https://learn.microsoft.com/en-us/dotnet/api/system.drawing.graphics.drawimage?view=netframework-4.5
			base.OnRenderItemImage(e);
		}
	}
}

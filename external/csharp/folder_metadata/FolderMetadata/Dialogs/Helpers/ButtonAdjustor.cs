using HKS.FolderMetadata.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HKS.FolderMetadata.Dialogs.Helpers
{
	/// <summary>
	/// The ButtonAdjustor class provides function to adjust buttons.
	/// </summary>
	public static class ButtonAdjustor
	{
		/// <summary>
		/// Changes the size of all buttons specified to the maximum required size. This ensures the same width/height for all buttons.
		/// During the process, the <see cref="ButtonBase.AutoSize"/> property is set to "false".
		/// </summary>
		/// <param name="buttons">The buttons to modify.</param>
		public static void AdjustButtonsSize(params Button[] buttons)
		{
			if (ArrayHelper.IsNullOrEmpty(buttons))
			{
				return;
			}

			Size largestSize = Size.Empty;

			foreach (Button button in buttons)
			{
				Size buttonSize = button.Size;
				bool widthLarger = (buttonSize.Width > largestSize.Width);
				bool heightLarger = (buttonSize.Height > largestSize.Height);

				if (widthLarger && heightLarger)
				{
					largestSize = buttonSize;
				}
				else if (widthLarger)
				{
					largestSize.Width = buttonSize.Width;
				}
				else if (heightLarger)
				{
					largestSize.Height = buttonSize.Height;
				}
			}

			foreach (Button button in buttons)
			{
				button.AutoSize = false;
				button.Size = largestSize;
			}
		}
	}
}

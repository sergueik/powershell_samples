using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using System.Windows.Forms;
   
namespace Program
{
	class Program
	{
		public static void Main()
		{
			var form = new WinForm();
   
			// form.Show(); // flash and close instantly 
			Application.Run(form);
		}

	}
}

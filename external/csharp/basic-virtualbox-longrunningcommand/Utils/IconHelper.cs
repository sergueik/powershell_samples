using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.IO;

namespace Utils {
	public class IconHelper {
			// name fragment based lookup
			// support the icons copied from Virtualbox resources directory in github
			private static KeyValuePair<string, string>[] iconLookup = {
				new KeyValuePair<string, string>("windows", "os_win_other.png"),
				new KeyValuePair<string, string>("microsoft", "os_win_other.png"),

				new KeyValuePair<string, string>("ubuntu", "os_ubuntu.png"),
				new KeyValuePair<string, string>("debian", "os_debian.png"),
				new KeyValuePair<string, string>("red hat", "os_redhat.png"),
				new KeyValuePair<string, string>("redhat", "os_redhat.png"),
				new KeyValuePair<string, string>("rhel", "os_redhat.png"),
				new KeyValuePair<string, string>("fedora", "os_fedora.png"),
				new KeyValuePair<string, string>("arch", "os_archlinux.png"),
				new KeyValuePair<string, string>("opensuse", "os_opensuse.png"),
				new KeyValuePair<string, string>("suse", "os_opensuse.png"),
				new KeyValuePair<string, string>("gentoo", "os_gentoo.png"),
				new KeyValuePair<string, string>("mandriva", "os_mandriva.png"),
				new KeyValuePair<string, string>("oracle linux", "os_oracle.png"),

				new KeyValuePair<string, string>("freebsd", "os_freebsd.png"),
				new KeyValuePair<string, string>("netbsd", "os_netbsd.png"),
				new KeyValuePair<string, string>("openbsd", "os_openbsd.png"),

				new KeyValuePair<string, string>("macos", "os_macosx.png"),
				new KeyValuePair<string, string>("mac os", "os_macosx.png"),
				new KeyValuePair<string, string>("os x", "os_macosx.png"),

				new KeyValuePair<string, string>("solaris", "os_solaris.png"),
				new KeyValuePair<string, string>("qnx", "os_qnx.png"),

				// Keep generic Linux last.
				new KeyValuePair<string, string>("linux", "os_linux.png")
			};
			
			// https://learn.microsoft.com/mt-mt/DOTNET/api/system.windows.forms.toolstripitem.image?view=netframework-4.5
			public static Image getImage(string os) { 
				// NOTE: Windows Forms does not include built-in, pre-coded enum references (like StockIcons) that automatically populate the Image property
				Image image = null;
				var filename = "Resources/os_other.png";
				Debug.WriteLine(String.Format("Determine icon for {0}", os));
				foreach (KeyValuePair<string, string> keyValuePair  in iconLookup) {
					if (os.Contains(keyValuePair.Key)) {
						filename = "Resources/" + keyValuePair.Value;
						break;
					}
				}
				string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

				if (File.Exists(iconPath)) {
					using (Image imageFromFile = Image.FromFile(iconPath)) {
						image = new Bitmap(imageFromFile);
					}
				} 
				return image;
			}
	}
}

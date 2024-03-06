add-type -typedefinition @'
using System;
	public class CustomAction {
		public static String GetDateTime() {
			return String.Format("{0:HH:mm}", DateTime.Now.Add(new TimeSpan(0, 0, 2, 0)));
		}

}
	
'@ 

get-date

write-output ('{0}' -f [CustomAction]::GetDateTime()) 
<#
  add-type : Cannot add type. The type name 'CustomAction' already exists. - quit and start Powershell
#>
<#

lets check one of the first search results:
the https://answers.microsoft.com
[discussion](https://answers.microsoft.com/en-us/windows/forum/all/the-mysterious-folder-in-use-file-or-folder-is/b1d4f424-928a-434c-815c-59a7ecac497e) says: 
 *Folder in use . . . file or folder is open in another program* - This __has__ been a problem for years, maybe decades... finally decide your only recourse is to restart the computer

#>
# changed signatures (almost)
add-type -typedefinition @'
using System;
public class CustomAction {
	[CustomAction]
	// temporarily made an instance method
		public /* static */ /* ActionResult */ String GetDateTime(/* Session session */) {
			var result = String.Format("{0:HH:mm}", DateTime.Now.Add(new TimeSpan(0, 0, 2, 0)));
			return result;
		}

}
	
'@ 

$o =  new-object CustomAction

<#

	// add-type : c:\Users\Serguei\AppData\Local\Temp\xvwpnsfs.0.cs(3) :
	// 'CustomAction' is not an attribute class

#>
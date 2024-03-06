add-type -typedefinition @'
using System;
	public class CustomAction {
	// [CustomAction]
		public String GetDateTime() {
			var result = String.Format("{0:HH:mm}", DateTime.Now.Add(new TimeSpan(0, 0, 2, 0)));
			return result;
		}

}
	
'@ 
# changed signature
$o =  new-object CustomAction
write-output 'Current Date: ' 
get-date

$o | get-member

write-output ('{0} => {1}' -f $o.GetType(), $o.GetDateTime()) 

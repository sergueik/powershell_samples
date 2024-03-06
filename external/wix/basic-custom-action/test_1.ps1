add-type -typedefinition @'
using Microsoft.Deployment.WindowsInstaller;
using System;
	public class CustomAction {
	[CustomAction]
		public ActionResult GetDateTime( Session session ) {
			session["START_TIME"] = String.Format("{0:HH:mm}", DateTime.Now.Add(new TimeSpan(0, 0, 2, 0)));
			return ActionResult.Success;
		}

}
	
'@ 

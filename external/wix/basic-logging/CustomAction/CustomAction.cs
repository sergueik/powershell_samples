using Microsoft.Deployment.WindowsInstaller;
using System.Diagnostics;
using System.IO;
using System;

namespace Utils {
	public class CustomAction {
		[CustomAction]
		public static ActionResult SessionLog(Session session) {


    CustomActionData data = session.CustomActionData;

    //Access each argument like this:

    string arg1 = data["Arg1"];
    string arg2 = data["Arg2"];
    string arg3 = data["Arg3"];
			session.Log("Custom Action Data Arguments: {0}={2} {2} {3}", "Arg1", arg1, "Arg2", arg2);
			return ActionResult.Success;
		}
		
	}
}

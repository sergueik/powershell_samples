using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace WebRequestExample
{
	//Refence Web :
	//http://weblogs.asp.net/tgraham/archive/2004/08/12/213469.aspx

	//In machine.config change :
	//<servicePointManager checkCertificateName="true"  checkCertificateRevocationList="false" /> 
	//by
	//<servicePointManager checkCertificateName="false" checkCertificateRevocationList="false" /> 

	//Instance class like this : 
	//System.Net.ServicePointManager.CertificatePolicy = new MyPolicy();
	public class MyPolicy : ICertificatePolicy 
	{
		public bool CheckValidationResult(
			ServicePoint srvPoint
			, X509Certificate certificate
			, WebRequest request
			, int certificateProblem) 
		{
			//Return True to force the certificate to be accepted.
			return true;
		} // end CheckValidationResult
	} 
}

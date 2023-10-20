using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Web;
using Utils;

namespace Service {
	public class Service : IService {

		// https://stackoverflow.com/questions/1354749/wcf-service-to-accept-a-post-encoded-multipart-form-data
		// https://stackoverflow.com/questions/2959890/posting-multipart-form-data-to-a-wcf-rest-service-the-action-changes
		public string Upload(Stream data) {

        // Get header info from WebOperationContext.Current.IncomingRequest.Headers
        // open and decode the multipart data, save to the desired place
        long incomingLength = WebOperationContext.Current.IncomingRequest.ContentLength;
        string[] result = new string[incomingLength];
        int cnter = 0;
        int arrayVal = -1;
        do
        {
            if (arrayVal != -1) result[cnter++] = Convert.ToChar(arrayVal).ToString();

            arrayVal = data.ReadByte();
        } while (arrayVal != -1);
        String response = String.Join("" , result); 
        Console.Error.WriteLine(response);
        return response;
        // return incomingLength.ToString();
        	
	    }
		
		public string Postdata(Stream data) {
			var reader = new StreamReader(data);  
			return reader.ReadToEnd();  
		}
		
		public Data DoLogin(string user, string pass) {
			
			try {
				var result = new Data();
				result.Point = new Point(1.2, 3.4);
				return result;
			} catch (Exception) {
				return new Data();
			}
		}
	}
}

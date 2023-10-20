using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Utils;

namespace Service {
    [ServiceContract]
    public interface IService {
        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Wrapped,
             UriTemplate = "/login/{username}/{password}")]
        [return: MessageParameter(Name = "Data")]
        Data DoLogin(string username, string password);

        [OperationContract(Name = "PostSampleMethod")]
		[WebInvoke(Method = "POST",  UriTemplate = "/postdata")]  
		string Postdata(Stream data);
		
		[OperationContract]
		[return: MessageParameter(Name = "FileData")]
    	[WebInvoke(Method = "POST", UriTemplate = "/Upload", BodyStyle = WebMessageBodyStyle.Bare
        )]
    string Upload(Stream data);

    }

    

    public class USER {
        public string username { get; set; }
        public string password { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
    }
}

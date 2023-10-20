// Author - Anshu Dutta
// Contact - anshu.dutta@gmail.com
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestWebService
{
    public class Service:IHttpHandler
    {
        #region Private Members

        private Company.Employee emp;
        private DAL.DAL dal;
        private string connString;
        private ErrorHandler.ErrorHandler errHandler;

        #endregion

        #region Handler
        bool IHttpHandler.IsReusable
        {
            get { throw new NotImplementedException(); }
        }

        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            try
            {                
                string url = Convert.ToString(context.Request.Url);                
                connString = Properties.Settings.Default.ConnectionString;
                dal = new DAL.DAL(connString);
                errHandler = new ErrorHandler.ErrorHandler();

                //Handling CRUD
                switch (context.Request.HttpMethod)
                {
                    case "GET":
                        //Perform READ Operation                   
                        READ(context);
                        break;
                    case "POST":
                        //Perform CREATE Operation
                        CREATE(context);
                        break;
                    case "PUT":
                        //Perform UPDATE Operation
                        UPDATE(context);
                        break;
                    case "DELETE":
                        //Perform DELETE Operation
                        DELETE(context);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                
                errHandler.ErrorMessage = ex.Message.ToString();
                context.Response.Write(errHandler.ErrorMessage);                
            }
        }

        #endregion Handler

        #region CRUD Functions
        /// <summary>
        /// GET Operation
        /// </summary>
        /// <param name="context"></param>
        private void READ( HttpContext context)
        {
            //HTTP Request - //http://server.com/virtual directory/employee?id={id}
            //http://localhost/RestWebService/employee
            try
            {
                int employeeCode = Convert.ToInt16(context.Request["id"]);
                
                //HTTP Request Type - GET"
                //Performing Operation - READ"
                //Data sent via query string
                //POST - Data sent as name value pair and resides in the <form section> of the browser
                emp = dal.GetEmployee(employeeCode);
                if (emp==null)               
                    context.Response.Write(employeeCode + "No Employee Found");                             
                              
                string serializedEmployee = Serialize(emp);                
                context.Response.ContentType = "text/xml";
                WriteResponse(serializedEmployee);
            }
            catch (Exception ex)
            {
                WriteResponse("Error in READ");
                errHandler.ErrorMessage = dal.GetException();
                errHandler.ErrorMessage = ex.Message.ToString();                
            }            
        }
        /// <summary>
        /// POST Operation
        /// </summary>
        /// <param name="context"></param>
        private void CREATE(HttpContext context)
        {
            try
            {
               
                // HTTP POST sends name/value pairs to a web server
                // dat is sent in message body

                //The most common use of POST, by far, is to submit HTML form data to CGI scripts.
                
                // This Post task handles cookies and remembers them across calls. 
                // This means that you can post to a login form, receive authentication cookies, 
                // then subsequent posts will automatically pass the correct cookies. 
                // The cookies are stored in memory only, they are not written to disk and 
                // will cease to exist upon completion of the build.
              
                // The POST Request structure - Typical POST Request
                // POST /path/script.cgi HTTP/1.0
                // From: frog@jmarshall.com
                // User-Agent: HTTPTool/1.0
                // Content-Type: application/x-www-form-urlencoded
                // Content-Length: 32

                // home=Cosby&favorite+flavor=flies

                // Extract the content of the Request and make a employee class
                // The message body is posted as bytes. read the bytes
                byte[] PostData = context.Request.BinaryRead(context.Request.ContentLength);
                //Convert the bytes to string using Encoding class
                string str = Encoding.UTF8.GetString(PostData);              
                // deserialize xml into employee class
                Company.Employee emp = Deserialize(PostData);                
                // Insert data in database
                dal.AddEmployee(emp);               
            }
            catch (Exception ex)
            {

                WriteResponse("Error in CREATE");
                errHandler.ErrorMessage = dal.GetException();
                errHandler.ErrorMessage = ex.Message.ToString();                
            }
        }
        /// <summary>
        /// PUT Operation
        /// </summary>
        /// <param name="context"></param>
        private void UPDATE(HttpContext context)
        {
            //The PUT Method

            // The PUT method requests that the enclosed entity be stored
            // under the supplied URL. If the URL refers to an already 
            // existing resource, the enclosed entity should be considered
            // as a modified version of the one residing on the origin server. 
            // If the URL does not point to an existing resource, and that 
            // URL is capable of being defined as a new resource by the 
            // requesting user agent, the origin server can create the 
            // resource with that URL.
            // If the request passes through a cache and the URL identifies 
            // one or more currently cached entities, those entries should 
            // be treated as stale. Responses to this method are not cacheable.


            // Common Problems
            // The PUT method is not widely supported on public servers 
            // due to security concerns and generally FTP is used to 
            // upload new and modified files to the webserver. 
            // Before executing a PUT method on a URL, it may be worth 
            // checking that PUT is supported using the OPTIONS method.
            
            try
            {
                // context.Response.Write("Update");
                // Read the data in the message body of the PUT method
                // The code expects the employee class as data to be updated

                byte[] PUTRequestByte = context.Request.BinaryRead(context.Request.ContentLength);
                context.Response.Write(PUTRequestByte);

                // Deserialize Employee
                Company.Employee emp = Deserialize(PUTRequestByte);
                dal.UpdateEmployee(emp);
                //context.Response.Write("Employee Updtated Sucessfully");
                WriteResponse("Employee Updtated Sucessfully");
            }
            catch (Exception ex)
            {

                WriteResponse("Error in CREATE");
                errHandler.ErrorMessage = dal.GetException();
                errHandler.ErrorMessage = ex.Message.ToString();                
            }
        }
        /// <summary>
        /// DELETE Operation
        /// </summary>
        /// <param name="context"></param>
        private void DELETE(HttpContext context)
        {
            try
            {
                int EmpCode = Convert.ToInt16(context.Request["id"]);
                dal.DeleteEmployee(EmpCode);
                WriteResponse("Employee Deleted Successfully");
            }
            catch (Exception ex)
            {
                
                WriteResponse("Error in CREATE");
                errHandler.ErrorMessage = dal.GetException();
                errHandler.ErrorMessage = ex.Message.ToString();                
            }
        }

        #endregion

        #region Utility Functions
        /// <summary>
        /// Method - Writes into the Response stream
        /// </summary>
        /// <param name="strMessage"></param>
        private static void WriteResponse(string strMessage)
        {
            HttpContext.Current.Response.Write(strMessage);            
        }

        /// <summary>
        /// Method - Deserialize Class XML
        /// </summary>
        /// <param name="xmlByteData"></param>
        /// <returns></returns>
        private Company.Employee Deserialize(byte[] xmlByteData)
        {
            try
            {
                XmlSerializer ds = new XmlSerializer(typeof(Company.Employee));
                MemoryStream memoryStream = new MemoryStream(xmlByteData);
                Company.Employee emp = new Company.Employee();
                emp = (Company.Employee)ds.Deserialize(memoryStream);
                return emp;
            }
            catch (Exception ex)
            {
                
                errHandler.ErrorMessage = dal.GetException();
                errHandler.ErrorMessage = ex.Message.ToString();
                throw;
            }
        }

        /// <summary>
        /// Method - Serialize Class to XML
        /// </summary>
        /// <param name="emp"></param>
        /// <returns></returns>
        private String Serialize(Company.Employee emp)
        {
            try
            {
                String XmlizedString = null;
                XmlSerializer xs = new XmlSerializer(typeof(Company.Employee));
                //create an instance of the MemoryStream class since we intend to keep the XML string 
                //in memory instead of saving it to a file.
                MemoryStream memoryStream = new MemoryStream();
                //XmlTextWriter - fast, non-cached, forward-only way of generating streams or files 
                //containing XML data
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                //Serialize emp in the xmlTextWriter
                xs.Serialize(xmlTextWriter, emp);
                //Get the BaseStream of the xmlTextWriter in the Memory Stream
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                //Convert to array
                XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
                return XmlizedString;
            }
            catch (Exception ex)
            {
                errHandler.ErrorMessage = ex.Message.ToString();
                throw;
            }           

        }

        /// <summary>
        /// To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
        /// </summary>
        /// <param name="characters">Unicode Byte Array to be converted to String</param>
        /// <returns>String converted from Unicode Byte Array</returns>
        private String UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        #endregion
    }
}

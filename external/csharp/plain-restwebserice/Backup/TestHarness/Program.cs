// Author - Anshu Dutta
// Contact - anshu.dutta@gmail.com
using System;
using System.Xml;
using System.Web;
using System.Net;
using System.IO;
using System.Xml.Serialization;
using Company;
using DAL;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// REST Web Service Test Harness
// Contains Test Methods written for testing intermediate project
// functionalities. Comment / uncomment as needed
namespace TestHarness
{
    class Program    {   
        
        static void Main(string[] args)
        {
            Employee emp = new Employee(); ;
            string strConnString = @"Data Source=SYNPUNEHCRV-105\SQLEXPRESS;Initial Catalog=Company;Integrated Security=True";
            DAL.DAL dal = new DAL.DAL(strConnString);

            #region "Test database Functionalities"

            //TestSelectCommand(emp, dal);
            //TestInsertCommand(emp, dal);
            //TestUpdateCommand(emp, dal);
            //TestDeleteCommand(emp, dal);
            //TestXMLSerialization();

            #endregion

            #region Test HTTP Methods
            //GenerateGetRequest();
            //GeneratePOSTRequest();
            //GeneratePUTRequest();
            GenerateDELETERequest();
            #endregion

            Console.ReadLine();
        }

        private static void TestSelectCommand(Employee emp, DAL.DAL dal)
        {
            Console.WriteLine("Testing Select command");
            emp = dal.GetEmployee(3550);
            PrintEmployeeInformation(emp);
        }

        private static void TestInsertCommand(Employee emp, DAL.DAL dal)
        {
            Console.WriteLine("Testing Insert Command");
            emp = new Employee();
            emp.FirstName = "Eva";
            emp.LastName = "Brown";
            emp.EmpCode = 1110;
            emp.Designation = "Architect";
            dal.AddEmployee(emp);
            Employee newEmp = new Employee();
            newEmp = dal.GetEmployee(1110);
            PrintEmployeeInformation(newEmp);           
        }

        private static void TestUpdateCommand(Employee emp, DAL.DAL dal)
        {
            Console.WriteLine("Testing Update Command");
            emp = new Employee();
            emp.FirstName = "Anne";
            emp.LastName = "Brown";
            emp.EmpCode = 1110;
            emp.Designation = "HR";
            dal.UpdateEmployee(emp);
            PrintEmployeeInformation(emp);
        }

        private static void TestDeleteCommand(Employee emp, DAL.DAL dal)
        {
            Console.WriteLine("Testing Delete Command");
            dal.DeleteEmployee(1110);
        }

        private static void PrintEmployeeInformation(Employee emp)
        {
            Console.WriteLine("Emplyee Number - {0}", emp.EmpCode);
            Console.WriteLine("Employee First Name - {0}", emp.FirstName);
            Console.WriteLine("Employee Last Name - {0}", emp.LastName);
            Console.WriteLine("Employee Designation - {0}", emp.Designation);
        }

        private static void TestXMLSerialization()
        {
            Console.WriteLine("Testing Serialization.....");
            Employee emp = new Employee();
            emp.FirstName = "Eva";
            emp.LastName = "Brown";
            emp.EmpCode = 1110;
            emp.Designation = "Architect";           
            Console.WriteLine(SerializeXML(emp));
        }

        /// <summary>
        /// Serialize XML
        /// </summary>
        /// <param name="emp"></param>
        /// <returns></returns>
        private static String SerializeXML(Company.Employee emp)
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
            catch (Exception)
            {                
                throw;
            }
        }
        private static String UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        /// <summary>
        /// Test GET Method
        /// </summary>
        private static void GenerateGetRequest()
        {
            //Generate get request
            string url = "http://localhost/RestWebService/employee?id=3550";
            HttpWebRequest GETRequest = (HttpWebRequest)WebRequest.Create(url);
            GETRequest.Method = "GET";

            Console.WriteLine("Sending GET Request");
            HttpWebResponse GETResponse = (HttpWebResponse)GETRequest.GetResponse();
            Stream GETResponseStream = GETResponse.GetResponseStream();
            StreamReader sr = new StreamReader(GETResponseStream);

            Console.WriteLine("Response from Server");
            Console.WriteLine(sr.ReadToEnd());
            Console.ReadLine();
        }

        /// <summary>
        /// Test POST Method
        /// </summary>
        private static void GeneratePOSTRequest()
        {
            Console.WriteLine("Testing POST Request");
            string strURL = "http://localhost/RestWebService/employee";
            string strFirstName = "FirstName";
            string strLastName = "LastName";
            int EmpCode=111;
            string strDesignation ="Janitor";

            // The client will be oblivious to server side data type
            // So Employee class is not being used here. Code - commented
            // To send a POST request -
            // 1. Create a Employee xml object in a memory stream
            // 2. Create a HTTPRequest object with the required URL
            // 3. Set the Method Type = POST and content type = txt/xml
            // 4. Get the HTTPRequest in a stream.
            // 5. Write the xml in the content of the stream
            // 6. Get a response from the erver.

            // Through Employee Class - not recommended
            //Employee emp = new Employee();
            //emp.FirstName = strFirstName;
            //emp.LastName = strLastName;
            //emp.EmpCode = EmpCode;
            //emp.Designation = strDesignation;
            //string str = SerializeXML(emp);           

            // Create the xml document in a memory stream - Recommended       
            
            byte[] dataByte = GenerateXMLEmployee(strFirstName,strLastName,EmpCode,strDesignation);
                        
            HttpWebRequest POSTRequest = (HttpWebRequest)WebRequest.Create(strURL);
            //Method type
            POSTRequest.Method = "POST";
            // Data type - message body coming in xml
            POSTRequest.ContentType = "text/xml";
            POSTRequest.KeepAlive = false;
            POSTRequest.Timeout = 5000;
            //Content length of message body
            POSTRequest.ContentLength = dataByte.Length;

            // Get the request stream
            Stream POSTstream = POSTRequest.GetRequestStream();
            // Write the data bytes in the request stream
            POSTstream.Write(dataByte, 0, dataByte.Length);                     

            //Get response from server
            HttpWebResponse POSTResponse = (HttpWebResponse)POSTRequest.GetResponse();
            StreamReader reader = new StreamReader(POSTResponse.GetResponseStream(),Encoding.UTF8) ;
            Console.WriteLine("Response");
            Console.WriteLine(reader.ReadToEnd().ToString());           
        }

        /// <summary>
        /// Test PUT Method
        /// </summary>
        private static void GeneratePUTRequest()
        {
            Console.WriteLine("Testing PUT Request");
            string Url = "http://localhost/RestWebService/employee";
            string strFirstName = "FName";
            string strLastName = "LName";
            int EmpCode = 111;
            string strDesignation = "Assistant";

            byte[] dataByte = GenerateXMLEmployee(strFirstName, strLastName, EmpCode, strDesignation);

            HttpWebRequest PUTRequest = (HttpWebRequest)HttpWebRequest.Create(Url);
            // Decorate the PUT request
            PUTRequest.Method = "PUT";
            PUTRequest.ContentType = "text/xml";
            PUTRequest.ContentLength = dataByte.Length;

            // Write the data byte stream into the request stream
            Stream PUTRequestStream = PUTRequest.GetRequestStream();
            PUTRequestStream.Write(dataByte,0,dataByte.Length);

            //Send request to server and get a response.
            HttpWebResponse PUTResponse = (HttpWebResponse)PUTRequest.GetResponse();
            //Get the response stream
            StreamReader PUTResponseStream = new StreamReader(PUTResponse.GetResponseStream(),Encoding.UTF8);
            Console.WriteLine(PUTResponseStream.ReadToEnd().ToString());

        }

        /// <summary>
        /// Test DELETE Method
        /// </summary>
        private static void GenerateDELETERequest()
        {
            string Url = "http://localhost/RestWebService/employee?id=111";
            HttpWebRequest DELETERequest = (HttpWebRequest)HttpWebRequest.Create(Url);
            
            DELETERequest.Method = "DELETE";
            HttpWebResponse DELETEResponse = (HttpWebResponse) DELETERequest.GetResponse();

            StreamReader DELETEResponseStream = new StreamReader(DELETEResponse.GetResponseStream(), Encoding.UTF8);
            Console.WriteLine("Response Received");
            Console.WriteLine(DELETEResponseStream.ReadToEnd().ToString());
        }

        /// <summary>
        /// Generate a Employee XML stream of bytes
        /// </summary>
        /// <param name="strFirstName"></param>
        /// <param name="strLastName"></param>
        /// <param name="intEmpCode"></param>
        /// <param name="strDesignation"></param>
        /// <returns>Employee XML in bytes</returns>
        private static byte[] GenerateXMLEmployee(string strFirstName, string strLastName, int intEmpCode, string strDesignation)
        {
            // Create the xml document in a memory stream - Recommended
            MemoryStream mStream = new MemoryStream();
            //XmlTextWriter xmlWriter = new XmlTextWriter(@"C:\Employee.xml", Encoding.UTF8);
            XmlTextWriter xmlWriter = new XmlTextWriter(mStream, Encoding.UTF8);
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Employee");
            xmlWriter.WriteStartElement("FirstName");
            xmlWriter.WriteString(strFirstName);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("LastName");
            xmlWriter.WriteString(strLastName);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("EmpCode");
            xmlWriter.WriteValue(intEmpCode);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("Designation");
            xmlWriter.WriteString(strDesignation);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
            xmlWriter.Close();
            return mStream.ToArray();
        }
    }
}

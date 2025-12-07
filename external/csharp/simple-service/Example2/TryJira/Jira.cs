using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;

namespace TryJira
{
    class Jira
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
        public string JsonString { get; set; }
        public IEnumerable<string> filePaths { get; set; }

        public void AddJiraIssue()
        {
            string restUrl = String.Format("{0}rest/api/2/issue/", Url);
            HttpWebResponse response = null;
            HttpWebRequest request = WebRequest.Create(restUrl) as HttpWebRequest; 
            request.Method = "POST";
            request.Accept = "application/json";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Basic " + Utility.GetEncodedCredentials(UserName,Password));
            byte[] data = Encoding.UTF8.GetBytes(JsonString);
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
            }
            using (response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var reader = new StreamReader(response.GetResponseStream());
                    string str = reader.ReadToEnd();
                    Console.WriteLine("The server returned '{0}'\n{1}", response.StatusCode, str);
                    var jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var sData = jss.Deserialize<Dictionary<string, string>>(str);
                    string issueKey = sData["key"].ToString();
                    
                    AddAttachments(issueKey, filePaths);
                }
            }
            request.Abort();
        }
          
        public void AddAttachments(string issueKey,IEnumerable<string> filePaths)
        {
            string restUrl = String.Format("{0}rest/api/2/issue/{1}/attachments", Url, issueKey);
            var filesToUpload = new List<FileInfo>();
            foreach (var filePath in filePaths)
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("File '{0}' doesn't exist", filePath);
                }
                var file = new FileInfo(filePath);
                filesToUpload.Add(file);
            }
            if (filesToUpload.Count <= 0)
            {
                Console.WriteLine("No file to Upload");
            }
            PostFile(restUrl,filesToUpload);
        }

        private void PostFile(string restUrl,IEnumerable<FileInfo> filePaths)
        {
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            String boundary = String.Format("----------{0:N}", Guid.NewGuid());
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            foreach (var filePath in filePaths)
            {
                var fs = new FileStream(filePath.FullName, FileMode.Open, FileAccess.Read);
                var data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                fs.Close();
                writer.WriteLine("--{0}", boundary);
                writer.WriteLine("Content-Disposition: form-data; name=\"file\"; filename=\"{0}\"", filePath.Name);
                writer.WriteLine("Content-Type: application/octet-stream");
                writer.WriteLine();
                writer.Flush();
                stream.Write(data, 0, data.Length);
                writer.WriteLine();
            }
            writer.WriteLine("--" + boundary + "--");
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            request = WebRequest.Create(restUrl) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
            request.Accept = "application/json";
            request.Headers.Add("Authorization", "Basic " + Utility.GetEncodedCredentials(UserName, Password));
            request.Headers.Add("X-Atlassian-Token", "nocheck");
            request.ContentLength = stream.Length;
            using (Stream requestStream = request.GetRequestStream())
            {
                stream.WriteTo(requestStream);
                requestStream.Close();
            }
            using (response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var reader = new StreamReader(response.GetResponseStream());
                    Console.WriteLine("The server returned '{0}'\n{1}", response.StatusCode, reader.ReadToEnd());
                }
            }
            request.Abort();
        }
    }
}

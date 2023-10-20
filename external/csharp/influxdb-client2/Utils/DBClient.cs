using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LogDynamicsLab.InfluxDataAccess
{
    /// <summary>
    /// Instances of this Class are able to connect to the InfluxDB REST API and access the timeseries data.
    /// </summary>
    public class DBClient
    {
        /// <summary>
        /// Server-Address or IP Field
        /// </summary>
        private string _server;

        /// <summary>
        /// Server-Address or IP
        /// </summary>
        public string Server
        {
            get { return _server; }
            set { _server = value; }
        }

        /// <summary>
        /// Port Number Field
        /// </summary>
        private int _port;

        /// <summary>
        /// Port Number on which Influx-Rest-API is listening.
        /// </summary>
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        /// <summary>
        /// Database-Name Field
        /// </summary>
        private string _database;

        /// <summary>
        /// Database-Name
        /// </summary>
        public string Database
        {
            get { return _database; }
            set { _database = value; }
        }

        /// <summary>
        /// User Field
        /// </summary>
        private string _user;

        /// <summary>
        /// Username
        /// </summary>
        public string User
        {
            get { return _user; }
            set { _user = value; }
        }

        /// <summary>
        /// Password Field
        /// </summary>
        private string _password;

        /// <summary>
        /// Password for connecting to DB
        /// </summary>
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        /// <summary>
        /// Constructor for Client Object, capable for assessing data from DB.
        /// </summary>
        /// <param name="server">Server Address or IP</param>
        /// <param name="port">Port Number</param>
        /// <param name="database">Database-Name</param>
        /// <param name="user">Username</param>
        /// <param name="password">Password</param>
        public DBClient(string server, int port, string database, string user, string password)
        {
            Server = server;
            Port = port;
            Database = database;
            User = user;
            Password = password;
        }

        /// <summary>
        /// Helper for transforming Millseconds since Epoche (UNIX Timestamp) to DateTime-Object
        /// </summary>
        /// <param name="milliseconds">Millseconds since Epoche</param>
        /// <returns>DateTime-Object</returns>
        private DateTime MillisecondsSinceEpocheToDateTime(double milliseconds)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(milliseconds);
        }

        /// <summary>
        /// Helper for transforming DateTime-Object to Millseconds since Epoche (UNIX Timestamp) 
        /// </summary>
        /// <param name="datetime">DateTime-Ibject</param>
        /// <returns>Millseconds since Epoche</returns>
        private long DateTimeToMillisecondsSinceEpoche(DateTime datetime)
        {
            TimeSpan t = datetime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long res = Convert.ToInt64(t.TotalMilliseconds);
            return res;
        }

        public List<string> getTimesSeriesNames()
        {
            string query = "list series";
            string uristring = String.Format("http://{0}:{1}/db/{2}/series?q={3}&u={4}&p={5}", Server, Port.ToString("0"), Database, query, User, Password);
            Uri url = new Uri(uristring);
            string json = ReadTextFromUrl(url);

            List<string> result = new List<string>();

            var objects = JArray.Parse(json); // parse as array  
            foreach (JObject root in objects)
            {
                foreach (KeyValuePair<String, JToken> app in root)
                {

                    if (app.Key == "points")
                    {
                        foreach (var e in app.Value)
                        {
                            result.Add(e.Last.ToString());
                        }
                        //var entries = JArray.Parse(app.Value); // parse as array  

                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Function for accessing time series data in InfluxDB based on a given name and attribute as well as timespan (from/to)
        /// </summary>
        /// <param name="timeseriesname">Name of the timeseries</param>
        /// <param name="attribute">Name of the attribute in the column vector which is considered</param>
        /// <param name="from">DateTime entry (Results will be bigger than given 'from' timestamp)</param>
        /// <param name="to">DateTime entry (Results will be smaller than given 'to' timestamp)</param>
        /// <returns>Resulting data from InfluxDB</returns>
        public List<KeyValuePair<DateTime, object>> GetTimeSeries(string timeseriesname, string attribute, DateTime from, DateTime to)
        {
            string query = String.Format("SELECT {0} FROM {1} WHERE time > {2} AND time < {3}", attribute, timeseriesname, DateTimeToMillisecondsSinceEpoche(from), DateTimeToMillisecondsSinceEpoche(to));
            return GetTimeSeries(query, attribute);
        }

        /// <summary>
        /// Function for accessing time series data in InfluxDB based on a given name and attribute
        /// </summary>
        /// <param name="timeseriesname">Name of the timeseries</param>
        /// <param name="attribute">Name of the attribute in the column vector which is considered</param>
        /// <returns>Resulting data from InfluxDB</returns>
        public List<KeyValuePair<DateTime, object>> GetTimeSeriesAll(string timeseriesname, string attribute)
        {
            string query = String.Format("SELECT {0} FROM {1}", attribute, timeseriesname);
            return GetTimeSeries(query, attribute);
        }

        /// <summary>
        /// Function for accessing time series data in InfluxDB based on a given name and attribute with limited number of results
        /// </summary>
        /// <param name="timeseriesname">Name of the timeseries</param>
        /// <param name="attribute">Name of the attribute in the column vector which is considered</param>
        /// <param name="limit">Limits the number of results</param>
        /// <returns>Resulting data from InfluxDB</returns>
        public List<KeyValuePair<DateTime, object>> GetTimeSeries(string timeseriesname, string attribute, int limit)
        {
            string query = String.Format("SELECT {0} FROM {1} LIMIT {2}", attribute, timeseriesname, limit);
            return GetTimeSeries(query, attribute);
        }

        /// <summary>
        /// Function for accessing time series data in InfluxDB based on a given name and attribute as well as timespan
        /// </summary>
        /// <param name="timeseriesname">Name of the timeseries</param>
        /// <param name="attribute">Name of the attribute in the column vector which is considered</param>
        /// <param name="timespan">Timespan Object which describes the span between 'now' minus the timespan and 'now'</param>
        /// <returns>Resulting data from InfluxDB</returns>
        public List<KeyValuePair<DateTime, object>> GetTimeSeries(string timeseriesname, string attribute, TimeSpan timespan)
        {
            string query = String.Format("SELECT {0} FROM {1} WHERE time > now() - {2}s", attribute, timeseriesname, timespan.TotalSeconds);
            return GetTimeSeries(query, attribute);
        }

        public void GetTimeSeriesCSVToFileAll(string timeseriesname, string file)
        {
            string query = String.Format("SELECT * FROM {0}", timeseriesname);
            GetTimeSeriesCSVToFile(query, file);
        }
        public void GetTimeSeriesCSVToFile(string timeseriesname, TimeSpan timespan, string file)
        {
            string query = String.Format("SELECT * FROM {0} WHERE time > now() - {1}s", timeseriesname, timespan.TotalSeconds);
            GetTimeSeriesCSVToFile(query, file);
        }

        public void GetTimeSeriesCSVToFile(string timeseriesname, DateTime from, DateTime to, string file)
        {
            string time1 = String.Format("'{0}-{1}-{2} {3}:{4}:{5}'", from.Year, from.Month, from.Date, from.Hour, from.Minute, from.Second);
            string time2 = String.Format("'{0}-{1}-{2} {3}:{4}:{5}'", to.Year, to.Month, to.Date, to.Hour, to.Minute, to.Second);
            string query = String.Format("SELECT * FROM {0} WHERE time > '{1:yyyy-MM-dd hh:mm:ss}' AND time < '{2:yyyy-MM-dd hh:mm:ss}'", timeseriesname, from, to);
            GetTimeSeriesCSVToFile(query, file);
        }

        public void GetTimeSeriesCSVToFile(string query, string file)
        {
            try
            {
                string uristring = String.Format("http://{0}:{1}/db/{2}/series?q={3}&u={4}&p={5}", Server, Port.ToString("0"), Database, query, User, Password);
                Uri url = new Uri(uristring);
                string json = ReadTextFromUrl(url);
                Console.WriteLine(json);
                if (json != "[]")
                {
                    List<InfluxReturnObject> ro = JsonConvert.DeserializeObject<List<InfluxReturnObject>>(json);

                    int numcolumns = ro[0].Columns.Count;
                    int numrows = ro[0].Points.Count;

                    System.IO.TextWriter writeFile = new StreamWriter(file);

                    string temp = "";
                    for (int i = 0; i < numcolumns; i++)
                    {
                        if (i != numcolumns - 1)
                            temp = string.Format("{0}{1};", temp, ro[0].Columns[i]);
                        else
                            temp = temp + ro[0].Columns[i];
                    }

                    writeFile.WriteLine(temp);

                    for (int j = 0; j < numrows; j++)
                    {
                        temp = "";
                        for (int i = 0; i < numcolumns; i++)
                        {
                            if (i != numcolumns - 1)
                                temp = string.Format("{0}{1};", temp, ro[0].Points[j][i]);
                            else
                                temp = temp + ro[0].Points[j][i];
                        }
                        writeFile.WriteLine(temp);
                    }


                    writeFile.Flush();
                    writeFile.Close();
                    writeFile = null;

                    //List<KeyValuePair<DateTime, object>> res = new List<KeyValuePair<DateTime, object>>();
                    //int indexOfTime = ro[0].Columns.IndexOf("time");
                    //int indexOfAttribute = ro[0].Columns.IndexOf(attribute);
                    //for (int i = ro[0].Points.Count - 1; i > -1; i--)
                    //{
                    //    List<object> templ = ro[0].Points[i];
                    //    long pkey = (long)templ[indexOfTime];
                    //    object pvalue = templ[indexOfAttribute];
                    //    DateTime key = MillisecondsSinceEpocheToDateTime(pkey);
                    //    KeyValuePair<DateTime, object> kvp = new KeyValuePair<DateTime, object>(key, pvalue);
                    //    res.Add(kvp);
                    //}
                    //return null;
                }
                //else
                //return null;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// Function for accessing time series data in InfluxDB based on a given query
        /// </summary>
        /// <param name="query">InfluxDB-based Query</param>
        /// <param name="attribute">Name of the attribute in the column vector which is considered</param>
        /// <returns>Resulting data from InfluxDB</returns>
        public List<KeyValuePair<DateTime, object>> GetTimeSeries(string query, string attribute)
        {
            try
            {
                string uristring = String.Format("http://{0}:{1}/db/{2}/series?q={3}&u={4}&p={5}", Server, Port.ToString("0"), Database, query, User, Password);
                Uri url = new Uri(uristring);
                string json = ReadTextFromUrl(url);
                if (json != "[]")
                {
                    List<InfluxReturnObject> ro = JsonConvert.DeserializeObject<List<InfluxReturnObject>>(json);
                    List<KeyValuePair<DateTime, object>> res = new List<KeyValuePair<DateTime, object>>();
                    int indexOfTime = ro[0].Columns.IndexOf("time");
                    int indexOfAttribute = ro[0].Columns.IndexOf(attribute);
                    for (int i = ro[0].Points.Count - 1; i > -1; i--)
                    {
                        List<object> templ = ro[0].Points[i];
                        long pkey = (long)templ[indexOfTime];
                        object pvalue = templ[indexOfAttribute];
                        DateTime key = MillisecondsSinceEpocheToDateTime(pkey);
                        KeyValuePair<DateTime, object> kvp = new KeyValuePair<DateTime, object>(key, pvalue);
                        res.Add(kvp);
                    }
                    return res;
                }
                else
                    return null;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// Function for adding new data to a timeseries. This function should be used for persitently saving results of algorithmic processing in InfluxDB.
        /// </summary>
        /// <param name="timeseriesname">Name of the timeseries</param>
        /// <param name="attribute">Optional name of the attribute for the datapoints (default = value)</param>
        /// <param name="timeseries">Timeseries of the Data which will be submitted</param>
        /// <returns>true, if successfully inserted</returns>
        public bool PostTimeSeriesData(string timeseriesname, List<KeyValuePair<DateTime, object>> timeseries, string attribute = "value")
        {
            string uristring = String.Format("http://{0}:{1}/db/{2}/series?u={3}&p={4}", Server, Port.ToString("0"), Database, User, Password);
            Uri url = new Uri(uristring);
            InfluxReturnObject o = new InfluxReturnObject();
            o.Name = timeseriesname;
            o.Columns = new List<string>() { "time", attribute };
            o.Points = new List<List<object>>();
            foreach (var e in timeseries)
            {
                List<object> templist = new List<object>();
                templist.Add(DateTimeToMillisecondsSinceEpoche(e.Key));
                templist.Add(e.Value);
                o.Points.Add(templist);
            }
            string json = JsonConvert.SerializeObject(o);
            return PostJSONToURL(url, json);
        }

        public bool PostTimeSeriesData(string timeseriesname, List<KeyValuePair<DateTime, List<object>>> timeseries, List<string> columns)
        {
            string uristring = String.Format("http://{0}:{1}/db/{2}/series?u={3}&p={4}", Server, Port.ToString("0"), Database, User, Password);
            Uri url = new Uri(uristring);
            InfluxReturnObject o = new InfluxReturnObject();
            o.Name = timeseriesname;
            o.Columns = new List<string>() { "time" };
            o.Columns.AddRange(columns);
            o.Points = new List<List<object>>();
            foreach (var e in timeseries)
            {
                if (e.Value.Count == columns.Count)
                {
                    List<object> templist = new List<object>();
                    templist.Add(DateTimeToMillisecondsSinceEpoche(e.Key));
                    templist.AddRange(e.Value);
                    o.Points.Add(templist);
                }
            }
            string json = JsonConvert.SerializeObject(o);
            return PostJSONToURL(url, json);
        }

        /// <summary>
        /// Helperfunction for GET-Request
        /// </summary>
        /// <param name="url">URL for GET-Request</param>
        /// <returns>Result Body as String, JSON expected in this context</returns>
        private string ReadTextFromUrl(Uri url)
        {
            using (var client = new WebClient())
            using (var stream = client.OpenRead(url))
            using (var textReader = new StreamReader(stream, Encoding.UTF8, true))
            {
                return textReader.ReadToEnd();
            }
        }

        private string ReadTextFromUrl2(Uri remoteUri)
        {
            var request = (HttpWebRequest)WebRequest.Create(remoteUri);
            request.Timeout = 100000;
            request.AllowWriteStreamBuffering = true;

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var s = response.GetResponseStream())
            using (var fs = new MemoryStream())
            {
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = s.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fs.Write(buffer, 0, bytesRead);
                    bytesRead = s.Read(buffer, 0, buffer.Length);
                }
                fs.Position = 0;
                var sr = new StreamReader(fs);
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// Helperfunction for POST-Request
        /// </summary>
        /// <param name="url">URL for POST-Request</param>
        /// <param name="json">JSON to be submitted to InfluxDB</param>
        /// <returns>Result Body as String</returns>
        private bool PostJSONToURL(Uri url, string json)
        {
            //try
            //{
            var cli = new WebClient();
            cli.Headers[HttpRequestHeader.ContentType] = "application/json";
            string res = cli.UploadString(url, "[" + json + "]");
            Console.WriteLine();
            Console.WriteLine("[" + json + "]");
            Console.WriteLine();
            return true;
            //}
            //catch(Exception err)
            // {
            //     return false;
            //}
        }

    }
}

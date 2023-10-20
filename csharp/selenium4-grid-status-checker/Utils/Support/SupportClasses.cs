using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace Utils.Support {
    public class CookiedHttpWebRequestFactory {
        private static Object synchLock = new Object();
        // This dictionary keeps the cookie container for each domain.
        private static Dictionary<string, CookieContainer> containers
            = new Dictionary<string, CookieContainer>();

		// https://learn.microsoft.com/en-us/dotnet/api/system.net.webrequest.create?view=netframework-4.5#system-net-webrequest-create(system-string)
        // https://learn.microsoft.com/en-us/dotnet/api/system.net.httpwebrequest.-ctor?view=netframework-4.5
        public static HttpWebRequest Create(string url) {
            var request = (HttpWebRequest)WebRequest.Create(url);

            string domain = (new Uri(url)).GetLeftPart(UriPartial.Authority);

            // try to get a container from the dictionary, if it is in the
            // dictionary, use it. Otherwise, create a new one and put it
            // into the dictionary and use it.
            CookieContainer container;
            lock (synchLock) {
                if (!containers.TryGetValue(domain, out container)) {
                    container = new CookieContainer();
                    containers[domain] = container;
                }
            }

            // Assign the cookie container to the HttpWebRequest object
            request.CookieContainer = container;

            return request;
        }
    }

    // Defines an adapter interface for the serializers
    public interface ISerializerAdapter {
        string Serialize(object obj);
        T Deserialize<T>(string input);
    }

    // An implementation of ISerializerAdapter based on the JavaScriptSerializer
    // https://learn.microsoft.com/en-us/dotnet/api/system.web.script.serialization.javascriptserializer?view=netframework-4.5
    // NOTE: only available starting with 4.7.2
    // for earlier versions of .NET Framework, Microsoft recommends using Newtonsoft.Json
    // .NET Framework 4.7.2 was released on 30 April 2018
    public class JavaScriptSerializerAdapter : ISerializerAdapter
    {
        private JavaScriptSerializer serializer;
        public JavaScriptSerializerAdapter() {
            serializer = new JavaScriptSerializer();
        }

        public string Serialize(object obj) {
            return serializer.Serialize(obj);
        }

        public T Deserialize<T>(string input) {
            return serializer.Deserialize<T>(input);
        }
    }

    public class ClientConfiguration {
        public string ContentType { get; set; }
        public string Accept { get; set; }
        public bool RequrieSession { get; set; }
        public ISerializerAdapter OutBoundSerializerAdapter { get; set; }
        public ISerializerAdapter InBoundSerializerAdapter { get; set; }
    }

    // The delegates use for asychronous calls
    public delegate void RestCallBack<T>(Exception ex, T result);
    public delegate void RestCallBackNonQuery(Exception ex);
}

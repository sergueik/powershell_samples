using LightCaseClient.Support;

namespace LightCaseClient
{
    // Synchronous proxy implementation
    public static partial class GenericProxies
    {
        // ********************** Synchronous GET *************************
        public static TR RestGet<TR>(string url, ClientConfiguration configuration)
        {
            var clientConfig = configuration ?? DefaultConfiguration;
            var request = CreateRequest(url, clientConfig);
            request.Accept = clientConfig.Accept;
            request.Method = "GET";

            return ReceiveData<TR>(request, clientConfig);
        }

        // Overload method
        public static TR RestGet<TR>(string url)
        {
            return RestGet<TR>(url, DefaultConfiguration);
        }


        // ******** Synchronous GET, no response expected *******
        public static void RestGetNonQuery(string url,
            ClientConfiguration configuration)
        {
            var clientConfig = configuration ?? DefaultConfiguration;
            var request = CreateRequest(url, clientConfig);
            request.Method = "GET";

            request.GetResponse().Close();
        }

        // Overload method
        public static void RestGetNonQuery(string url)
        {
            RestGetNonQuery(url, DefaultConfiguration);
        }

        // ***************** Synchronous POST ********************
        public static TR RestPost<TR, TI>(string url,
            TI data, ClientConfiguration configuration)
        {
            var clientConfig = configuration ?? DefaultConfiguration;
            var request = CreateRequest(url, clientConfig);
            request.ContentType = clientConfig.ContentType;
            request.Accept = clientConfig.Accept;
            request.Method = "POST";

            PostData(request, clientConfig, data);
            return ReceiveData<TR>(request, clientConfig);
        }

        // Overload method
        public static TR RestPost<TR, TI>(string url, TI data)
        {
            return RestPost<TR, TI>(url, data, DefaultConfiguration);
        }

        // ****** Synchronous GET, no respons expected ******
        public static void RestPostNonQuery<TI>(string url,
            TI data, ClientConfiguration configuration)
        {
            var clientConfig = configuration ?? DefaultConfiguration;
            var request = CreateRequest(url, clientConfig);
            request.ContentType = clientConfig.ContentType;
            request.Accept = clientConfig.Accept;
            request.Method = "POST";

            PostData(request, clientConfig, data);
            request.GetResponse().Close();
        }

        // Overload method
        public static void RestPostNonQuery<TI>(string url, TI data)
        {
            RestPostNonQuery(url, data, DefaultConfiguration);
        }
    }
}

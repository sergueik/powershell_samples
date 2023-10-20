using System;
using System.Runtime.Remoting.Messaging;
using Utils.Support;

namespace Utils {
    // Asynchronous proxy implementation
    public static partial class GenericProxies {
        private delegate TR GetDelegate<TR>(string url, ClientConfiguration configuration);
        private delegate void GetNonQueryDelegate(string url, ClientConfiguration configuration);
        private delegate TR PostDelegate<TR, TI>(string url, TI data, ClientConfiguration configuration);
        private delegate void PostNonQueryDelegate<TI>(string url, TI data, ClientConfiguration configuration);

        // asynchronous Get
        public static void RestGetAsync<T>(string url, RestCallBack<T> callback) {
            RestGetAsync<T>(url, callback, defaultConfiguration);
        }

        public static void RestGetAsync<T>(string url, RestCallBack<T> callback, ClientConfiguration configuration) {
            var get = new GetDelegate<T>(RestGet<T>);
            get.BeginInvoke(url, configuration,
            ar => {
                var result = (AsyncResult)ar;
                var del = (GetDelegate<T>)result.AsyncDelegate;
                var value = default(T);
                Exception e = null;

                try { value = del.EndInvoke(result); }
                catch (Exception ex) { e = ex; }

                if (callback != null) { callback(e, value); }

            }, null);
        }


        // asynchronous Get, no response expected
        public static void RestGetNonQueryAsync(string url, RestCallBackNonQuery callback) {
            RestGetNonQueryAsync(url, callback, defaultConfiguration);
        }

        public static void RestGetNonQueryAsync(string url, RestCallBackNonQuery callback, ClientConfiguration configuration) {
            var get = new GetNonQueryDelegate(RestGetNonQuery);
            get.BeginInvoke(url, configuration,
            ar => {
                var result = (AsyncResult)ar;
                var del = (GetNonQueryDelegate)result.AsyncDelegate;
                Exception e = null;

                try { del.EndInvoke(result); }
                catch (Exception ex) { e = ex; }

                if (callback != null) { callback(e); }

            }, null);

        }

        // asynchronous Post
        public static void RestPostAsync<TR, TI>(string url, TI data, RestCallBack<TR> callback) {
            RestPostAsync<TR, TI>(url, data, callback, defaultConfiguration);
        }

        public static void RestPostAsync<TR, TI>(string url, TI data, RestCallBack<TR> callback, ClientConfiguration configuration) {
            var post = new PostDelegate<TR, TI>(RestPost<TR, TI>);
            post.BeginInvoke(url, data, configuration,
            ar => {
                var result = (AsyncResult)ar;
                var del = (PostDelegate<TR, TI>)result.AsyncDelegate;
                var value = default(TR);
                Exception e = null;

                try { value = del.EndInvoke(result); }
                catch (Exception ex) { e = ex; }

                if (callback != null) { callback(e, value); }

            }, null);
        }

        // asynchronous Post, not response expected
        public static void RestPostNonQueryAsync<TI>(string url, TI data, RestCallBackNonQuery callback) {
            RestPostNonQueryAsync(url, data, callback, defaultConfiguration);
        }

        public static void RestPostNonQueryAsync<T>(string url, T data, RestCallBackNonQuery callback, ClientConfiguration configuration) {
            var post = new PostNonQueryDelegate<T>(RestPostNonQuery);
            post.BeginInvoke(url, data, configuration,
            ar => {
                var result = (AsyncResult)ar;
                var del = (PostNonQueryDelegate<T>)result.AsyncDelegate;
                Exception e = null;

                try { del.EndInvoke(result); }
                catch (Exception ex) { e = ex; }

                if (callback != null) { callback(e); }

            }, null);
        }

    }
}

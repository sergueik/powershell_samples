using System;
using NUnit.Framework;
using System.Net;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Test {
    [TestFixture]
    public class HttpServerTest {
        [Test]
        public void test1() {
            var request = (HttpWebRequest)WebRequest.Create("http://localhost:4050/");
            var response = request.GetResponse();
            Assert.AreEqual("text/html; charset=UTF-8", response.Headers["Content-Type"]);
            Assert.AreEqual("ExampleServer", response.Headers["Server"]);
        }

        [Test]
        public void test2() {
            var request = (HttpWebRequest)WebRequest.Create("http://localhost:4050/");
            request.Method = "POST";
            var requestStream = request.GetRequestStream();
            var data = Encoding.UTF8.GetBytes("a=10&b=15");
            requestStream.Write(data,0,data.Length);
            var response = request.GetResponse();
            Assert.AreEqual("text/html; charset=UTF-8", response.Headers["Content-Type"]);
            Assert.AreEqual("ExampleServer", response.Headers["Server"]);
            using (var reader = new StreamReader(response.GetResponseStream())) {
                var content = reader.ReadToEnd();
                Assert.AreEqual("This is the data returned via the POST method:a=10;b=15", content);
            }
        }
    }
}

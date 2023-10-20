using System;
using NUnit.Framework;

using Utils;
using Utils.Support;
using TestUtils;
using System.IO;
using System.Net;
using System.Collections.Generic;

namespace Test {

	[TestFixture]
	public class HeadTest {
		private readonly GridStatusLoader gridStatusLoader = new GridStatusLoader();
		public ClientConfiguration clientConfig = GenericProxies.defaultConfiguration;

		[TestFixtureSetUp]
		public void SetUp() {
		}
		// curl  -X HEAD http://192.168.0.68:4444/status
		// curl: (18) transfer closed with outstanding read data remaining
		// with headers only, works fine
		
		[Test]
		public void test01() {
			// TODO: debug: works with curl
			// curl -I -X HEAD http://reqbin.com/sample/head/json
			// HTTP/1.1 301 Moved Permanently
			// Location: https://reqbin.com/sample/head/json
			// var url = "http://reqbin.com/sample/head/json";
			var url = "http://localhost:4444/status";		
			try {
				var status = GenericProxies.RestHead(url);
				Console.Error.WriteLine("HTTP status: " + status );
			} catch(WebException e){
				Console.Error.WriteLine(e.ToString());
				// System.Net.WebException: 
				// Unable to connect to the remote server
				// System.Net.Sockets.SocketException:
				// No connection could be made because the target machine actively refused it
			}
		}
	}
}

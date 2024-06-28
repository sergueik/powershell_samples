using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using System.Threading;

using NUnit.Framework;
using Utils;

namespace Test {

	[TestFixture]
	public class Test {
		private StringBuilder verificationErrors = new StringBuilder();
		private SimpleHTTPServer pageServer;
		private int port = 0;

		[TestFixtureSetUp]
		public void SetUp() {

			// check that the process is elevated - this is required so it can create web servers
			bool isProcessElevated =  ElevationChecker.IsProcessElevated(false);
			Assert.IsTrue(isProcessElevated, "This test needs to run elevated");

			// initialize custom HttpListener subclass to host the local files
			// https://docs.microsoft.com/en-us/dotnet/api/system.net.httplistener?redirectedfrom=MSDN&view=netframework-4.7.2
			String documentRoot = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", "");
			
			Console.Error.WriteLine(String.Format("Using document root path: {0}", documentRoot));
			pageServer = new SimpleHTTPServer(documentRoot);
			// NOTE: the constructor calls 
			// pageServer.Initialize() and pageServer.Listen();
			port = pageServer.Port;
			Console.Error.WriteLine(String.Format("Using Port {0}", port));

		}
		// NOTE: nunit 2.7 warning:
		// Warning CS0618: 'NUnit.Framework.TestFixtureSetUpAttribute' is obsolete: 'Use OneTimeSetUpAttribute'
		// this is not defined for nunit 2.6.4
		// Error CS0246: The type or namespace name 'OneTimeTearDownAttributeAttribute' 
		// could not be foundt
		// [OneTimeTearDownAttribute]
		// [TestFixtureSetUpAttribute]
		[TestFixtureTearDownAttribute]
		public void TearDown() {
			if (pageServer!= null) {
				Console.Error.WriteLine("Stopping pageserver: " + pageServer);
				pageServer.Stop();
			}
			Assert.IsEmpty(verificationErrors.ToString());
		}

		// the test is run simply to have the server running
		[Test]
		public void test() {
			// Thread.Sleep(1000000);
			Common.Port  = port;
			Common.GetLocalHostPageContent("dummy.htm");
		}
		
	}
}

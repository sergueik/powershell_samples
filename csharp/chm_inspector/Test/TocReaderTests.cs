using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using Utils;
using NLog;

namespace Tests {
	[TestFixture]
	public class TocReaderTests {
		private const string file = @"C:\Program Files\Oracle\VirtualBox\VirtualBox.chm";
		private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

	    [TestFixtureSetUp]   // NUnit 2.x attribute
	    public void Setup() {
	    	var config = new NLog.Config.LoggingConfiguration();
	    	var fileTarget = new NLog.Targets.FileTarget();
	 	  	fileTarget.Name = "logfile"; // optional but helpful
		    // set filename: use ${basedir} so it's next to exe; create logs/ subfolder if you like
		    fileTarget.FileName = "${basedir}/logs/chm_inspector.log";
		    fileTarget.Layout = "${longdate}|${level}|${logger}|${message}";    
	        config.AddTarget("logfile", fileTarget);
	        config.LoggingRules.Add(new NLog.Config.LoggingRule("*", NLog.LogLevel.Debug, fileTarget));
	        LogManager.Configuration = config;
	    }

	    [Test]
		public void test1() {
			var entries = Chm.toc_structured(file);
	
            // Assert
            Assert.IsNotNull(entries, "The entries list should not be null");
            Assert.IsNotEmpty(entries, "The entries list should not be empty");

            foreach (var entry in entries) {
                Assert.IsFalse(string.IsNullOrEmpty(entry.Name), "Entry Name should not be null or empty");
                Assert.IsFalse(string.IsNullOrEmpty(entry.Local), "Entry Local should not be null or empty");
            }
		}

		[Test]
		public void test2() {
			Dictionary<string,string> toc = Chm.toc_7zip(file);

			// Assert
            Assert.IsNotNull(toc, "The dictionary should not be null");
            Assert.IsNotEmpty(toc, "The dictionary should not be empty");

            foreach (var kvp in toc) {
                Assert.IsFalse(string.IsNullOrEmpty(kvp.Key), "Key (Name) should not be null or empty");
                Assert.IsFalse(string.IsNullOrEmpty(kvp.Value), "Value (Local) should not be null or empty");
                Console.Error.WriteLine("{0}: {1}", kvp.Key, kvp.Value);
            }		
		}
	}
}
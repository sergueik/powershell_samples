using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Timers;
using TransactionService;

using NUnit.Framework;

namespace Tests {

	[TestFixture]
	public class PerformanceMetadataTest {
		[Test]
		public void test1() {
			try {
				var utility = new PerformanceMetadataUtility();
				var names = utility.CategoryNames;
				Assert.IsNotNull(names);
				Console.Error.WriteLine(String.Format("Loaded {0} Category names", names.Count));
			} catch (Exception e) {
				Console.Error.WriteLine("Exception: " + e.ToString());
				Assert.Fail();
			}
		}

		[Test]
		public void test2() {
			try {
				var utility = new PerformanceMetadataUtility();
				utility.CategoryName = "Processor";
				var names = utility.CounterNames;
				Assert.IsNotNull(names);
				Console.Error.WriteLine(String.Format("Loaded {0} Counter names", names.Count));
			} catch (Exception e) {
				Console.Error.WriteLine("Exception: " + e.ToString());
				Assert.Fail();
			}
		}

		[Test]
		public void test3() {
			try {
				var utility = new PerformanceMetadataUtility();
				utility.CategoryName = "System";
				var names = utility.CounterNames;
				Assert.IsNotNull(names);
				Console.Error.WriteLine(String.Format("Loaded {0} Counter names", names.Count));
			} catch (Exception e) {
				Console.Error.WriteLine("Exception: " + e.ToString());
				Assert.Fail();
			}
		}


		[Test]
		public void test4() {
			try {
				var utility = new PerformanceMetadataUtility();
				utility.CategoryName = "Memory";
				utility.CounterName = "Available MBytes";
				Assert.IsTrue(utility.Valid);
				var instanceName = utility.InstanceName;
				Assert.IsNull(instanceName);

				Console.Error.WriteLine(String.Format("Category {0} Counter {1} is {2}", utility.CategoryName, utility.CounterName, (utility.Valid ? "Valid" : "Invalid")));
			} catch (Exception e) {
				Console.Error.WriteLine("Exception: " + e.ToString());
				Assert.Fail();
			}
		}

		[Test]
		public void test5(){
			try {
				var utility = new PerformanceMetadataUtility();
				utility.CategoryName = "PhysicalDisk";
				utility.CounterName = "Disk Write Bytes/sec";
				// NOTE: Error CS0815: Cannot assign <null> to an implicitly-typed local variable
				string instanceName = null;
				Assert.IsNull(instanceName);
				Assert.IsTrue(utility.Valid);
				instanceName = utility.InstanceName;
				Assert.IsNotNull(instanceName);
				Console.Error.WriteLine(String.Format("Category {0} Counter {1} is {2}", utility.CategoryName, utility.CounterName, (utility.Valid ? "Valid" : "Invalid")));
			} catch (Exception e) {
				Console.Error.WriteLine("Exception: " + e.ToString());
				Assert.Fail();
			}
		}
	}
}

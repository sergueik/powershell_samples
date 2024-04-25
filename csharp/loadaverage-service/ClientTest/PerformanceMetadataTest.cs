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

				Console.Error.WriteLine(String.Format(@"Category ""{0}"" {1} Counter ""{2}"" is {3}", utility.CategoryName, (utility.InstanceName ==null ? "" : String.Format(@"Instance ""{0}""" , utility.InstanceName)), utility.CounterName, (utility.Valid ? "Valid" : "Invalid")));
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
				Assert.IsTrue(utility.Valid);
				instanceName = utility.InstanceName;
				Assert.IsNotNull(instanceName);
				Console.Error.WriteLine(String.Format(@"Category ""{0}"" {1} Counter ""{2}"" is {3}", utility.CategoryName, (utility.InstanceName ==null ? "" : String.Format(@"Instance ""{0}""" , utility.InstanceName)), utility.CounterName, (utility.Valid ? "Valid" : "Invalid")));
			} catch (Exception e) {
				Console.Error.WriteLine("Exception: " + e.ToString());
				Assert.Fail();
			}
		}

		// System.InvalidOperationException: Category does not exist
		[Test]
		[ExpectedException(typeof(System.InvalidOperationException))]
		public void test6(){
				var utility = new PerformanceMetadataUtility();
				utility.CategoryName = "Foo";
				utility.CounterName = "Bar";
				Assert.IsFalse(utility.Valid);
				Console.Error.WriteLine(String.Format(@"Category ""{0}"" {1} Counter ""{2}"" is {3}", utility.CategoryName, (utility.InstanceName ==null ? "" : String.Format(@"Instance ""{0}""" , utility.InstanceName)), utility.CounterName, (utility.Valid ? "Valid" : "Invalid")));
		}
		
		
		[Test]
		public void test7() {
			try {
				var utility = new PerformanceMetadataUtility();
				utility.CategoryName = "Memory";
				utility.CounterName = "Available MBytes";
				String counterHelp = utility.CounterHelp;
				Assert.IsNotNull(counterHelp);

				Console.Error.WriteLine(String.Format(@"Category ""{0}"" {1} Counter ""{2}"" Counter Help: {3}", utility.CategoryName, (utility.InstanceName ==null ? "" : String.Format(@"Instance ""{0}""" , utility.InstanceName)), utility.CounterName, counterHelp));
			} catch (Exception e) {
				Console.Error.WriteLine("Exception: " + e.ToString());
				Assert.Fail();
			}
		}
		[Test]
		public void test8() {
			try {
				var utility = new PerformanceMetadataUtility();
				utility.CategoryName = "PhysicalDisk";
				utility.CounterName = "Disk Write Bytes/sec";
				String counterHelp = utility.CounterHelp;
				Assert.IsNotNull(counterHelp);

				Console.Error.WriteLine(String.Format(@"Category ""{0}"" {1} Counter ""{2}"" Counter Help: {3}", utility.CategoryName, (utility.InstanceName ==null ? "" : String.Format(@"Instance ""{0}""" , utility.InstanceName)), utility.CounterName, counterHelp));
			} catch (Exception e) {
				Console.Error.WriteLine("Exception: " + e.ToString());
				Assert.Fail();
			}
		}


	}
}

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
				var names  = utility.CategoryNames;
				Assert.IsNotNull(names);
				Console.Error.WriteLine(String.Format("Loaded {0} Category names", names.Count));
			} catch (Exception e){
				Console.Error.WriteLine("Exception: " + e.ToString());
				Assert.Fail();
			}
		}

		[Ignore]
		[Test]
		public void test2() {
			try {
				var utility = new PerformanceMetadataUtility();
				utility.CategoryName = "Processor";
				var names = utility.CounterNames;
				Assert.IsNotNull(names);
				Console.Error.WriteLine(String.Format("Loaded {0} Counter names", names.Count));
			} catch (Exception e){
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
			} catch (Exception e){
				Console.Error.WriteLine("Exception: " + e.ToString());
				Assert.Fail();
			}
		}

		// System.ArgumentException: Counter is not single instance, an instance name needs to be specified.
	}
}

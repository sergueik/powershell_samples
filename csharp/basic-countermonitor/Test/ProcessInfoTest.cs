using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using System.Reflection;
using Utils;

namespace Test
{
	[TestFixture]
	public class ProcessInfoTest
	{
		private string result = null;
		[Test]
		public void test1()
		{
			var pid = 20340;
			result = Utils.ProcessInfo.getProcessInstanceName(pid);
			Assert.IsNotNull(result);
			Console.Error.WriteLine(result);
			StringAssert.IsMatch(@"VirtualBox(?:#\d+)?", result);
			foreach (var c in new PerformanceCounterCategory("Process").GetCounters("explorer")) {
				Console.WriteLine(c.CounterName);
			}
			var cat = new PerformanceCounterCategory("Process");

			foreach (string counter in cat.GetCounters("_Total").Select(c => c.CounterName)) {
				Console.WriteLine(counter);
			}
		}
	}
	
}

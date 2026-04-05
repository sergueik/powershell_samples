using System;
using System.Threading;

using NUnit.Framework;
using Utils;

namespace Test {
	[TestFixture]
	public class DiscoverTest {
		private Discover discover = null;
		private int interval = 100;
		private int cnt = 0;
		private string argument;
		private Predicate<string> checkCondition;

		[SetUp]
		public void setUp() {
			argument = "should exit when cnt is 10";
			checkCondition = (string s) => {
				cnt = cnt + 1;
				Console.Error.WriteLine("cnt :" + cnt);
				return s.EndsWith(cnt.ToString());
			};
			discover = new Discover(interval, checkCondition, argument);
		}
		
		[Test]
		public void test1() {
			discover.startPolling();
			Thread.Sleep(1500);
			Assert.AreEqual(10, cnt);
		}

		[Test]
		public void test2() {
			var exception = Assert.Throws<ArgumentException>(() => { 
				discover = new Discover(0, checkCondition, argument);
			});

			Assert.That(exception.Message, Is.EqualTo("invalid interval"));
		}

		[Test]
		public void test3() {
			var exception = Assert.Throws<ArgumentException>(() => { 
				discover = new Discover(interval, checkCondition, "                ");
			});
			Assert.That(exception.Message, Is.EqualTo("invalid argument"));
		}

		[Test]
		public void test4() {
			checkCondition = (string value)=>ProcessInfo.getProcessIDsByCommandLine(null, value).Count !=0 ;
			discover = new Discover(interval, checkCondition, argument);
			discover.startPolling();
			discover.stop();
			int id= 12345;
			checkCondition = (string value)=>ProcessInfo.getProcessInstanceName(value).Length !=0 ;
			discover = new Discover(interval, checkCondition, id.ToString());
			discover.startPolling();
			discover.stop();
		}
	}
}

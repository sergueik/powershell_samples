using System;
using System.Threading;

using NUnit.Framework;
using Utils;

namespace Test {
	[TestFixture]
	public class DiscoverTest {
		private Discover discover1 = null;
		private int interval = 100;
		private int cnt = 0;
		private string argument;
		private string argument1;
		private string argument2;
		private Predicate<string> checkCondition;

		[SetUp]
		public void setUp() {
			argument = "should exit when cnt is 10";
			checkCondition = (string s) => {
				cnt = cnt + 1;
				Console.Error.WriteLine("cnt :" + cnt);
				return s.EndsWith(cnt.ToString());
			};
			discover1 = new Discover(interval, checkCondition, argument);
		}
		
		[Test]
		public void test1() {
			discover1.startCheckingIfFinished();
			Thread.Sleep(1500);
			Assert.AreEqual(10, cnt);
		}

		[Test]
		public void test2() {
			var exception = Assert.Throws<ArgumentException>(() => { 
				discover1 = new Discover(0, checkCondition, argument);
			});

			Assert.That(exception.Message, Is.EqualTo("invalid interval"));
		}

		[Test]
		public void test3() {
			var exception = Assert.Throws<ArgumentException>(() => { 
				discover1 = new Discover(interval, checkCondition, "                ");
			});
			Assert.That(exception.Message, Is.EqualTo("invalid argument"));
		}

		[Test]
		public void test4() {
			checkCondition = (string value)=>ProcessInfo.getProcessIDsByCommandLine(null, value).Count !=0 ;
			discover1 = new Discover(interval, checkCondition, argument);
			discover1.startCheckingIfFinished();
			discover1.stop();
			int id= 12345;
			checkCondition = (string value)=>ProcessInfo.getProcessInstanceName(value).Length !=0 ;
			discover1 = new Discover(interval, checkCondition, id.ToString());
			discover1.startCheckingIfFinished();
			discover1.stop();
		}

		[Test]
		public void test5() {
			// NOTE: Cannot assign lambda expression to an implicitly-typed local variable (CS0815)
			Func<string, string> /* var */ getResult = (string arg) => {
		        // usa lambda
				cnt++;
				Console.Error.WriteLine("cnt :" + cnt);
				return cnt == 10 ? "DONE" : "";	};
			var discover2 = new Discover(  interval, getResult,argument);
			cnt = 0;
			discover2.startPollingForResult();
		
			Thread.Sleep(2500);
		
			Assert.AreEqual("DONE", discover2.Result);
		}
		[Test]
		public void test6() {
			argument1 = "argument1";
			argument2 = "argument2";
			Func<string, string, string> getResult2 = (string arg1,string arg2) => {
				cnt++;
				Console.Error.WriteLine(String.Format("argument1: {0}| arguhment2: {1}|cnt : {2}" , arg1,  arg2, cnt));
				return cnt == 10 ? "DONE" : "";	};
			// NOTE: variable name
			// 'argument1,2' conflicts with the declaration 'Test.DiscoverTest.argument1,2' (CS0135)
			var discover2 = new Discover(  interval, getResult2,argument1,argument2);
			cnt = 0;
			discover2.startPollingForResult();
		
			Thread.Sleep(2500);
		
			Assert.AreEqual("DONE", discover2.Result);
		}
	}
}

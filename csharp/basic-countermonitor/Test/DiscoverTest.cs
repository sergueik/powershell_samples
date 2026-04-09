using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
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
				Debug.WriteLine("cnt :" + cnt);
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
			checkCondition = (string value)=>ProcessInfo.getProcessIDByCommandLine(null, value).Count !=0 ;
			discover1 = new Discover(interval, checkCondition, argument);
			discover1.startCheckingIfFinished();
			discover1.stop();
			int id= 12345;
			checkCondition = (string value)=>ProcessInfo.getPerformanceCountertInstance(value).Length !=0 ;
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
				Debug.WriteLine("cnt :" + cnt);
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
				Debug.WriteLine(String.Format("argument1: {0}| arguhment2: {1}|cnt : {2}" , arg1,  arg2, cnt));
				return cnt == 10 ? "DONE" : "";	};
			// NOTE: variable name
			// 'argument1,2' conflicts with the declaration 'Test.DiscoverTest.argument1,2' (CS0135)
			var discover2 = new Discover(  interval, getResult2,argument1,argument2);
			cnt = 0;
			discover2.startPollingForResult();
		
			Thread.Sleep(2500);
		
			Assert.AreEqual("DONE", discover2.Result);
		}

		[Test]
		public void test7() {
			var name = "java";
			// NOTE: not "java.exe"
			var jar = "example.way2automation.jar";
			Func<string, string, string> getResult2 = (string argument1,string argument2) => {
				List<int>results = ProcessInfo.getProcessIDByCommandLine(argument1, argument2);
				if (results.Count > 1)
				{
					// TODO: should this be considerd error ?
				} else
					Debug.WriteLine(String.Format("filename: {0}| variable: {1}|pid: {2}" , argument1,  argument2, results[0]));
				return results[0].ToString();	};
			// NOTE:
			// var discover2 = new Discover(  interval, ProcessInfo.getProcessIDByCommandLine,argument1,argument2);
			// The call is ambiguous between the following methods or properties:
			// 'Utils.Discover.Discover(int, System.Func<string,string,string>, string, string)' and
			// 'Utils.Discover.Discover(int, System.Func<string,string,bool>, string, string)' (CS0121)
			var discover2 = new Discover(  interval, getResult2, name, jar); 
			cnt = 0;
			discover2.startPollingForResult();
		
			Thread.Sleep(2500);
		
			Assert.NotNull(discover2.Result);
			StringAssert.IsMatch("[1-9][09]*",  discover2.Result);
			Debug.WriteLine(String.Format("Found pid of process {0} with argument {1}: {2}", name, jar, discover2.Result));

		}
		/*
		 NOTE: - these are all wrong
		 where.exe java.exe
C:\Program Files\Common Files\Oracle\Java\javapath\java.exe
C:\Program Files (x86)\Common Files\Oracle\Java\javapath\java.exe
		 */
		[Test]
		public void test8() {
			var name = "java.exe";
			var mainClass = "example.Application";
			Func<string, string, string> getResult2 = (string argument1,string argument2) => {
				Debug.WriteLine("test 8");
				List<int>results = ProcessInfo.getProcessIDByCommandLine(argument1, argument2);
				if (results.Count > 1)
				{
				} else
					Debug.WriteLine(String.Format("filename: {0}| variable: {1}|pid: {2}" , argument1,  argument2, results[0]));
				return results[0].ToString();	};
			// NOTE:
			// var discover2 = new Discover(  interval, ProcessInfo.getProcessIDByCommandLine,argument1,argument2);
			// The call is ambiguous between the following methods or properties:
			// 'Utils.Discover.Discover(int, System.Func<string,string,string>, string, string)' and
			// 'Utils.Discover.Discover(int, System.Func<string,string,bool>, string, string)' (CS0121)
			var discover2 = new Discover(  interval, getResult2, name, mainClass);
			// this finds the java.exe process launched by maven
			discover2.startPollingForResult();
		
			Thread.Sleep(2500);
			Assert.NotNull(discover2.Result);
			StringAssert.IsMatch("[1-9][09]*",  discover2.Result);
			Debug.WriteLine(String.Format("Found pid of process {0} with argument {1}: {2}", name, mainClass, discover2.Result));
		}

		[Test]
		public void test9() {
			
			var mainClass = "example.Application";
			Func<string, string, string> getResult2 = (string argument1,string argument2) => {
				Debug.WriteLine("test 9");
				List<int>results = ProcessInfo.getProcessIDByCommandLine(argument1, argument2);
				if (results.Count > 1)
				{
				} else
					Debug.WriteLine(String.Format("filename: {0}| variable: {1}|pid: {2}" , argument1,  argument2, results[0]));
				return results[0].ToString();	};
			var discover2 = new Discover(  interval, getResult2,"java.exe",mainClass);
			discover2.startPollingForResult();
			Thread.Sleep(2500);
			Assert.NotNull(discover2.Result);
			StringAssert.IsMatch("[1-9][09]*",  discover2.Result);
			var pid = discover2.Result;
			var result = ProcessInfo.getPerformanceCountertInstance(pid);
			StringAssert.IsMatch(String.Format("{0}(?:#[1-9][09]*)?", "java"),  result);
			Debug.WriteLine(String.Format("Found performance counter for process id {0}: {1}",pid, result));
		}

		[Test]
		public void test10() {
			
			var mainClass = "example.Application";
			Func<string, string, string> getResult2 = (string argument1,string argument2) => {
				Debug.WriteLine("test 9");
				List<int>results = ProcessInfo.getProcessIDByCommandLine(argument1, argument2);
				if (results.Count > 1)
				{
				} else
					Debug.WriteLine(String.Format("filename: {0}| variable: {1}|pid: {2}" , argument1,  argument2, results[0]));
				return results[0].ToString();	};
			var discover2 = new Discover(  interval, getResult2,"java.exe",mainClass);
			discover2.startPollingForResult();
			Thread.Sleep(2500);
			Assert.NotNull(discover2.Result);
			StringAssert.IsMatch("[1-9][09]*",  discover2.Result);
			var pid = discover2.Result;
			Debug.WriteLine(String.Format("Finding performance counter for process name:{0} id: {1}","java", pid));
			Func<string, string, string> getResult3 = (string argument1,String argument2) => ProcessInfo.getPerformanceCountertInstance(argument1,argument2);			
			var discover3 = new Discover( interval, getResult3, "java", pid);
			Thread.Sleep(2500);
			Assert.NotNull(discover3.Result);
			StringAssert.IsMatch(String.Format("{0}(?:#[1-9][09]*)?", "java"), discover3.Result);
			Debug.WriteLine(String.Format("Found performance counter for process {0} with pid {1}: {2}", "java", pid, discover3.Result));
			
		}
		[Test]
		public void test11() {
			var name = "java";
			var jar = "example.way2automation.jar";
			Func<string, string, string> getResult2 = (string argument1,string argument2) => {
				Debug.WriteLine("test 9");
				List<int>results = ProcessInfo.getProcessIDByCommandLine(argument1, argument2);
				if (results.Count > 1)
				{
				} else
					Debug.WriteLine(String.Format("filename: {0}| variable: {1}|pid: {2}" , argument1,  argument2, results[0]));
				return results[0].ToString();	};
			var discover2 = new Discover(  interval, getResult2,name,jar); 
			discover2.startPollingForResult();
			Thread.Sleep(2500);
			Assert.NotNull(discover2.Result);
			StringAssert.IsMatch("[1-9][09]*",  discover2.Result);
			var pid = discover2.Result;
			var result = ProcessInfo.getPerformanceCountertInstance(name, pid);
			StringAssert.IsMatch(String.Format("{0}(?:#[1-9][09]*)?", name),  result);
			Debug.WriteLine(String.Format("Found performance counter for process {0} with pid {1}: {2}", name, pid, result));
		}

		[Test]
		public void test12() {
			
			var name = "java";
			var mainClass = "example.Application";
			Func<string, string, string> getResult2 = (string argument1,string argument2) => {
				Debug.WriteLine("test 12");
				List<int>results = ProcessInfo.getProcessIDByCommandLine(argument1, argument2);
				if (results.Count > 1)
				{
				} else
					Debug.WriteLine(String.Format("filename: {0}| variable: {1}|pid: {2}" , argument1,  argument2, results[0]));
				return results[0].ToString();	};
			var discover2 = new Discover(  interval, getResult2, name + ".exe",mainClass);
			discover2.startPollingForResult();
			Thread.Sleep(2500);
			Assert.NotNull(discover2.Result);
			StringAssert.IsMatch("[1-9][09]*",  discover2.Result);
			var pid = discover2.Result;
			var result = ProcessInfo.getPerformanceCountertInstance(name, pid);
			StringAssert.IsMatch(String.Format("{0}(?:#[1-9][09]*)?", name),  result);
			Debug.WriteLine(String.Format("Found performance counter for process name: {0} id:{1} counter:{2}",name, pid, result));
		}

	}
}

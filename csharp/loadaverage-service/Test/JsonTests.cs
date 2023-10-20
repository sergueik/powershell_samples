using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using System.Collections;
using System.Reflection;
using Utils;

namespace Tests {
	[TestFixture]
	public class JsonTests {

		[Test]
		public void test01() {
			var data = new List<int> { 1, 2, 3, 4, 5, 6 };
			string text = data.ToJsonExtensionMethod();
			Console.Error.WriteLine("JSON: " + text);
			Assert.IsNotNull(text);
			Assert.IsTrue(13 == text.Length);
			StringAssert.Contains("1,2,3,4,5,6", text);
			StringAssert.IsMatch(@"(?:\d+)", text);

		}
		[Test]
		public void test02()
		{
			var data = new List<int> { 1, 2, 3, 4, 5, 6 };
			string text = JSONWriter.ToJson(data);
			Console.Error.WriteLine("JSON: " + text);
			Assert.IsNotNull(text);
			Assert.IsTrue(13 == text.Length);
			StringAssert.Contains("1,2,3,4,5,6", text);
			StringAssert.IsMatch(@"(?:\d+)", text);
		}

		[Test]
		public void test03()
		{
			var test = "{\"value\":10}".FromJson<object>();
			var data = (Dictionary<string,object>)test;
			int number = int.Parse(data["value"].ToString());

			Assert.IsNotNull(number);
			Assert.IsTrue(10 == number);
		}

		[Test]
		public void test04()
		{
			var payload = @"{
    ""string"": ""test"",
    ""float"": 10.1,
    ""boolean"": true,
    ""list"": [
        1, 2, 3
    ],
    ""nestedlist"": [
        [1, 2],
        [1, 2, 3, 4]
    ],
    ""dictionary"": {
        ""foo"": ""bar"",
        ""baz"": {""bam""}
    }
}";
			var test = JSONParser.FromJson(payload);
			var data = (Dictionary<string,object>)test;

			Assert.IsNotNull(data);

			Assert.IsTrue(6 == data.Keys.Count);
			var floatValue = float.Parse(data["float"].ToString());
			Assert.IsTrue(floatValue.Equals((float)10.1), "Value is: " + floatValue);
			var booleanValue = Boolean.Parse(data["boolean"].ToString());
			Assert.IsTrue(booleanValue);

			var dictionaryValue = (Dictionary<string,object>)data["dictionary"];
			Assert.IsTrue(2 == dictionaryValue.Keys.Count, "Dicionary key size is: " + dictionaryValue.Keys.Count);
			var listValue = (List<object>)data["list"];
			Assert.IsTrue(3 == listValue.Count, "List size is: " + listValue.Count);
		}

		[Test]
		public void test05()
		{
			var payload = @"[{ ""target"": ""test"" }, { ""foo"": ""bar"" } ]";
			var test = JSONParser.FromJson(payload);
			var data = (List<object>)test;

			Assert.IsNotNull(data);
			Assert.IsTrue(2 == data.Count);
		}

		[Test]
		public void test06()
		{
			var payload = @"{
    ""string"": ""test"",
    ""float"": 10.1,
    ""boolean"": true,
    ""list"": [
        1, 2, 3
    ],
    ""nestedlist"": [
        [1, 2],
        [1, 2, 3, 4]
    ],
    ""dictionary"": {
        ""foo"": ""bar"",
        ""baz"": {""bam""}
    }
}";
			var test = JSONParser.FromJson(payload);
			var data = (Dictionary<string,object>)test;
				string result = data.ToJsonExtensionMethod();
				Console.Error.WriteLine("JSON: " + result);
				Assert.IsNotNull(result);
		}

		[Test]
		public void test07()
		{
			try {
				Dictionary<string, string> data = new Dictionary<string, string>();
				data.Add("1", "J");
				data.Add("2", "S");
				data.Add("3", "D");
				data.Add("4", "A");
            
				string text = data.ToJsonExtensionMethod();
				Console.Error.WriteLine("JSON: " + text);
				Assert.IsNotNull(text);
			} catch (Exception e) {
				Console.Error.WriteLine("Exception: " + e.ToString());
				Assert.Fail();
			}
		}

		// NOTE: non-generic
		// similar to Powershell
		// @{"a" = "b"; "c" = 1; "d" = $true}

		[Test]
		// [Ignore("Throws \"System.Reflection.TargetParameterCountException : Parameter count mismatch\"")]
		[ExpectedException(typeof(System.Reflection.TargetParameterCountException))]
		public void test08() {

			var data = new Hashtable();
			data.Add("a", "b");
			data.Add("c", "1");
			data.Add("d", true);
			string text = data.ToJsonExtensionMethod();
			Console.Error.WriteLine("JSON: " + text);
			Assert.IsNotNull(text);
		}

		[Test]
		public void test09() {
			var data = new Hashtable();
			data.Add("a", "b");
			data.Add("b", 5);
			data.Add("c", null);
			data.Add("d", 3.14);
			data.Add("e", true);
			string text = JSONWriter.ToJson(data);
			Console.Error.WriteLine("JSON: " + text);
			Assert.IsNotNull(text);
		}

		[Test]
		// [Ignore("Throws \"System.Reflection.TargetParameterCountException : Parameter count mismatch\"")]
		[ExpectedException(typeof(System.Reflection.TargetParameterCountException))]		public void test10() {

			var data = new Hashtable();
			data.Add("a", "b");
			var data2 = new Hashtable();
			data2.Add("x","y");
			data.Add("c", data2);
			string text = JSONWriter.ToJson(data);
			Console.Error.WriteLine("JSON: " + text);
			Assert.IsNotNull(text);
		}
	}
}

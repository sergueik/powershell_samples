using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;

using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

using Utils;

namespace Test
{

	[TestFixture]
	public class JSONHelperTest
	{
		// private NameValueCollection appSettings;
		private const string data = @"
			[
  {
    ""netid"": ""tcp"",
    ""state"": ""ESTAB"",
    ""local"": ""127.0.0.1:22"",
    ""remote"": ""127.0.0.1:51432""
  },
  {
    ""netid"": ""tcp"",
    ""state"": ""LISTEN"",
    ""local"": ""0.0.0.0:22"",
    ""remote"": ""0.0.0.0:*""
  },
  {
    ""netid"": ""tcp"",
    ""state"": ""TIME-WAIT"",
    ""local"": ""192.168.1.10:54321"",
    ""remote"": ""192.168.1.20:443""
  }
]
			";
		[SetUp]
		public void SetUp()
		{
		}

		[Test]
		public void test1()
		{
			object result = null;
			result = JSONHelper.deserialize<object>(data);
			Assert.IsNotNull(result);
		}
		
		[Test]
		public void test2()
		{
			object result = null;
			result = JSONHelper.deserialize<object>(data);
			Assert.IsNotNull(result);
			// https://github.com/dotnet/docs/blob/main/docs/csharp/language-reference/operators/type-testing-and-cast.md
			object []results  = result as  object[];
			Assert.IsNotNull(results);
		}
		
		[Test]
		public void test3()
		{
			object result = null;
			result = JSONHelper.deserialize<object>(data);
			Assert.IsNotNull(result);
			List<object>results  = result as List<object>;
			Assert.IsNull(results);
		}

 	[Test]
		public void test4()
		{
			List<SocketInfo> result = null;
			result = JSONHelper.deserialize<List<SocketInfo>>(data);
			Assert.IsNotNull(result);
			Assert.IsTrue(result.Count > 1);
			Assert.IsNotNull(result[0].state);
		}

		[Test]
		[ExpectedException(typeof(System.InvalidOperationException))]
		// Type 'System.Collections.Generic.Dictionary`2[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Object, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]' is not supported for deserialization of an array.
 
		public void test5()
		{
			Dictionary<string, object> result = null;
			result = JSONHelper.deserialize(data);
			Assert.IsNotNull(result);
		}
		
	}
	public class SocketInfo
	{
		public string netid { get; set; }
		public string state { get; set; }
		public string local { get; set; }
		public string remote { get; set; }
	}
}

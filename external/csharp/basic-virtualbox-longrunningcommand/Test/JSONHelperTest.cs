using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;

using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

using Utils;

namespace Test {

	[TestFixture]
	public class JSONHelperTest {
		private string data = null;
		[SetUp]
		public void SetUp() {
			data = @"
[
  {
    ""state"": ""LISTEN"",
    ""recv-q"": 0,
    ""send-q"": 128,
    ""local"": ""0.0.0.0:22"",
    ""peer"": ""0.0.0.0:*""
  },
  {
    ""state"": ""LISTEN"",
    ""recv-q"": 0,
    ""send-q"": 4096,
    ""local"": ""0.0.0.0:8443"",
    ""peer"": ""0.0.0.0:*""
  },
  {
    ""state"": ""LISTEN"",
    ""recv-q"": 0,
    ""send-q"": 4096,
    ""local"": ""0.0.0.0:8480"",
    ""peer"": ""0.0.0.0:*""
  },
  {
    ""state"": ""LISTEN"",
    ""recv-q"": 0,
    ""send-q"": 128,
    ""local"": ""[::]:22"",
    ""peer"": ""[::]:*""
  },
  {
    ""state"": ""LISTEN"",
    ""recv-q"": 0,
    ""send-q"": 4096,
    ""local"": ""[::]:8443"",
    ""peer"": ""[::]:*""
  },
  {
    ""state"": ""LISTEN"",
    ""recv-q"": 0,
    ""send-q"": 4096,
    ""local"": ""[::]:8080"",
    ""peer"": ""[::]:*""
  }
]
	";
	
	/*
State   Recv-Q   Send-Q     Local Address:Port      Peer Address:Port  Process  
LISTEN  0        128              0.0.0.0:22             0.0.0.0:*              
LISTEN  0        4096             0.0.0.0:8443           0.0.0.0:*              
LISTEN  0        128              0.0.0.0:51413          0.0.0.0:*              
LISTEN  0        4096           127.0.0.1:42961          0.0.0.0:*              
LISTEN  0        4096             0.0.0.0:8081           0.0.0.0:*              
LISTEN  0        128                 [::]:22                [::]:*              
LISTEN  0        4096                [::]:8443              [::]:*              
LISTEN  0        128                 [::]:51413             [::]:*              
LISTEN  0        4096                [::]:8081              [::]:*
*/
		}

		[Test]
		public void test1() {
			object result = null;
			result = JSONHelper.deserialize<object>(data);
			Assert.IsNotNull(result);
		}
		
		[Test]
		public void test2() {
			object result = null;
			result = JSONHelper.deserialize<object>(data);
			Assert.IsNotNull(result);
			// https://github.com/dotnet/docs/blob/main/docs/csharp/language-reference/operators/type-testing-and-cast.md
			object []results = result as object[];
			Assert.IsNotNull(results);
		}
		
		[Test]
		public void test3() {
			object result = null;
			result = JSONHelper.deserialize<object>(data);
			Assert.IsNotNull(result);
			List<object>results = result as List<object>;
			Assert.IsNull(results);
		}

	 	[Test]
		public void test4() {
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
	public class SocketInfo {
		public string netid { get; set; }
		public string state { get; set; }
		public string local { get; set; }
		public string remote { get; set; }
	}
}

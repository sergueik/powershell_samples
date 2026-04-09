using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using NUnit.Framework;

using Utils;
using TestUtils;

namespace Test {

	[TestFixture]
	public class EnvVarsTest {
		private string input =null;
		private string result =null;
		private string value =null;
		
		[Test]
		public void test1() {
			var vars = new List<string> {
				"TEMP",
				"Path",
				"SystemRoot"
			};

			input = "${env:TEMP} ${TEMP} ${env:SystemRoot} ${env:Path}";
			result = EnvVars.ResolveEnvVars(input);
			Assert.IsNotNull(result);
			vars.ForEach((string name) => {

				value = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User);
				if (value == null)
					value = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);	
				Debug.WriteLine(String.Format("environment: {0}: {1}", name, value));
				StringAssert.Contains(value, result);
			});
			StringAssert.DoesNotMatch(@"\$(?:\{(?:env:)?(\w+)\}|(\w+))", result);
		}

		[Test]
		public void test2() {

			input = "${env:UNKNOWN}";
			result = EnvVars.ResolveEnvVars(input);
			Debug.WriteLine(String.Format(@"Result: ""{0}""",result));
			Assert.AreEqual(String.Empty, result);
		}
		
		[Test]
		public void test3() {
			// Set a user-level variable
			var name = "UserValue";
			Environment.SetEnvironmentVariable(name, "value", EnvironmentVariableTarget.User);
			input = String.Format("${{env:{0}}}", name);
			// Retrieve a user-level variable
			result = EnvVars.ResolveEnvVars(input);
			Debug.WriteLine("Result: " + result);
			Assert.IsNotNull(result);

			value = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User);
			StringAssert.Contains(value, result);
		}
	}
}

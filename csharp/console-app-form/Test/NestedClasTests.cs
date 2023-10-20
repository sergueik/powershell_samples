using System;
using NUnit.Framework;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

using Utils;

// origin: https://stackoverflow.com/questions/55981217/serialize-a-nested-object-json-net
// https://www.newtonsoft.com/json/help/html/SerializeObject.htm

namespace Test {

	[TestFixture]
	public class NestedClasTests {
		private StringBuilder verificationErrors = new StringBuilder();
			private static string json = null;
		private List<User> users;
		private Roles roles;
		private Account account;

		[SetUp]
		public void setUp() {
			users = new List<User>() {
				new User() { key = "value1" },
				new User() { key = "value2" }
			};

			roles = new Roles();
			roles.Users = users;
			roles.Role = "Admin";

			account = new Account {
				Email = "james@example.com",
				Active = true,
				// CreatedDate = new DateTime(2013, 1, 20, 0, 0, 0, DateTimeKind.Utc),
				Roles = roles
			};

		}

		[TearDown]
		public void tearDown() {
			account = null;
			users = null;
			roles = null;
			Assert.AreEqual("", verificationErrors.ToString());
		}

		[Test]
		public void test() {

			json = JsonConvert.SerializeObject(account, Newtonsoft.Json.Formatting.Indented);
			Console.Error.WriteLine(json);
			account = JsonConvert.DeserializeObject<Account>(json);
			Assert.IsNotNull(account);
			Console.Error.WriteLine(account.Roles.Users.Count);
			StringAssert.Contains("Admin", account.Roles.Role);
			StringAssert.Contains("value1", account.Roles.Users[0].key);
		}

		public class Account {
			public string Email { get; set; }
			public bool Active { get; set; }
			public Roles Roles { get; set; }
        
		}
		
		public class Roles {
			public List<User> Users { get; set; }
			public string Role { get; set; }
		}

		public class User {
			public string key { get; set; }
		}
	}
}

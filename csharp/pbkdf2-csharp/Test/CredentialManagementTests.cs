using System;
using System.Text;
using NUnit.Framework;
using System.Linq;
using Utils;

namespace Test {

	[TestFixture]
	public class CredentialManagementTests {
		private StringBuilder verificationErrors = new StringBuilder();
		private string password = "test";
		const string userName = "MyCompany.MyApp.ApiBootstrap.v1";
		private CredentialManagementHelper sut;

		[SetUp]
		public void setUp() {
			verificationErrors.Clear();
			sut = new CredentialManagementHelper();
			sut.UserName = userName;
			sut.Password = password;

		}

		[TearDown]
		public void tearDown() {
			Assert.AreEqual("", verificationErrors.ToString());
		}

		[Test]
		public void test1() {
			sut.SavePassword() ;
			var result = sut.GetPassword();
			Assert.IsNotNull(result);
			Assert.AreEqual( result, password);
		}


	}
}


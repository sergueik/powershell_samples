using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using NUnit.Framework;

using Utils;
using TestUtils;

namespace Tests {

	[TestFixture]
	public class MessageHelperTest {
		private HRESULT hresult;
		private string message;
		
		[Test] 
		public void test1() {
			hresult = unchecked((HRESULT)0x80041318);
			message = MessageHelper.Msg(hresult);
			StringAssert.Contains("XML", message, "Expect an incorrectly formatted XML message");
		}

		[Test] 
		public void test2() {
			hresult = unchecked((HRESULT)0x8003000F);
			message = MessageHelper.Msg(hresult);
			StringAssert.Contains("Unknown HRESULT", message, "Expect an Unknown HRESULT message");
	
		}
	}
}


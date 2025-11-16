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
		// NOTE: C# 5 does not support expression-bodied member (EBM)
		// private static HRESULT HR(uint value) => (HRESULT)unchecked((int)value)
		// using the classic form C# 5 version
		// NOTE: EBMs appear like but are not lambdas
		private static HRESULT toHRESULT(uint value) {
   			return (HRESULT)unchecked((int)value);
		}		
		private HRESULT hresult;
		private string message;
		
		[Test] 
		public void test1() {
			hresult = toHRESULT(0x80041318);
			message = MessageHelper.Msg(hresult);
			StringAssert.Contains("XML", message, "Expect an incorrectly formatted XML message");
		}

		[Test] 
		public void test2() {
			hresult = toHRESULT(0x8003000F);
			message = MessageHelper.Msg(hresult);
			StringAssert.Contains("Unknown HRESULT", message, "Expect an Unknown HRESULT message");
	
		}
	}
}


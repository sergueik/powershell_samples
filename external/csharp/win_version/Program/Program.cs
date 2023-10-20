
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace win_version_csharp {
	internal class Program {
		static bool status = false;
		static void Main(string[] args) {
			VersionInfo info = new VersionInfo() {  Major = 0,
				Minor = 0, BuildNum = 0
			};
			try {
				status = WinVersion.GetVersion(out  info);
			} catch (ExecutionEngineException e) {
				// cannot be caught
			}
			
			if (status) {
				Console.WriteLine("Windows Version: {0}.{1}.{2}", info.Major, info.Minor, info.BuildNum);
			}
			Console.WriteLine("IsBuildNumGreaterOrEqual(Windows_11_22H2): {0}", 
				WinVersion.IsBuildNumGreaterOrEqual((uint)(BuildNumber.Windows_11_22H2)));
			Console.ReadKey();
		}
	}
}

using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Diagnostics;

// NOTE: th learn about how applications can be designed without barriers
// the canonical introduction is:
// https://learn.microsoft.com/en-us/windows/win32/winprog64/file-system-redirector
// see also:
// https://learn.microsoft.com/en-us/windows/win32/winprog64/registry-redirector
namespace Utils {
	public class ArchitectureChecker {

		// https://learn.microsoft.com/en-us/dotnet/api/system.environment.is64bitoperatingsystem?view=netframework-4.5
		public static bool is64BitOperatingSystem() {
			var status = Environment.Is64BitOperatingSystem;
			Debug.WriteLine("Windows Platform Detection: is 64 bit: {0}" , status);
			Debug.WriteLine("%ProgramW6432%: {0} %ProgramFiles%: {1}", Environment.GetEnvironmentVariable("ProgramW6432"), Environment.GetEnvironmentVariable("ProgramFiles"));
			return status;
		}

		public static void checkAssemblyArchitecture() {
			checkAssemblyArchitecture(Assembly.GetExecutingAssembly().Location);
		}

		public static void checkAssemblyArchitecture(string assemblyPath) {
	        try {
	            // Read metadata without executing the assembly
	            Assembly assembly = Assembly.ReflectionOnlyLoadFrom(assemblyPath);
	            Module module = assembly.ManifestModule;
	            // https://learn.microsoft.com/en-us/dotnet/api/system.reflection.portableexecutablekinds?view=netframework-4.5
	            // NOTE: non-nullable value type
	            PortableExecutableKinds peKinds = PortableExecutableKinds.ILOnly;
	            // https://learn.microsoft.com/en-us/dotnet/api/system.reflection.imagefilemachine?view=netframework-4.5
	            // NOTE: non-nullable value type
	            ImageFileMachine machine = ImageFileMachine.I386;
	            module.GetPEKind(out peKinds, out machine);
	
	            // Check if it is exclusively a 32-bit assembly
	            if ((peKinds & PortableExecutableKinds.Required32Bit) != 0) {
	                Debug.WriteLine("The assembly requires 32-bit execution (x86).");
	            }
	            else if ((peKinds & PortableExecutableKinds.PE32Plus) != 0) {
	                Debug.WriteLine("The assembly is compiled for 64-bit execution (x64).");
	            }
	            else if ((peKinds & PortableExecutableKinds.ILOnly) != 0) {
	                Debug.WriteLine("The assembly is AnyCPU (MSIL).");
	            }
	        } catch (Exception e) {
	        	Debug.WriteLine(String.Format("Failed to load or parse assembly: {0}",e.Message));
	        }
	    }
	}
}

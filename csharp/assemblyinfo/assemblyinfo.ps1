$code = @'

using System.Reflection;
using System;
using System.Collections.Generic;


// NOTE: "using" clause must precede all other elements defined in the namespace except extern alias declarations

[assembly: AssemblyVersion("1.2.3.4")]
[assembly: AssemblyFileVersion("1.2.3.4")]
[assembly: AssemblyDescription("Description Attribute")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]

[assembly: AssemblyProduct("Dummy Product")]
// NOTE: assembly-level attributes must be declared at the outermost level — not nested in a class.


public class Dummy
{
	private static Assembly assembly = typeof(Dummy).Assembly;

	// https://stackoverflow.com/questions/10203575/simplified-way-to-get-assembly-description-in-c
	// https://stackoverflow.com/questions/187495/how-to-read-assembly-attributes
	public  static string getDescription() { 
        
		var attribute = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>();
		return (attribute != null) ? attribute.Description : "N/A"; 
	}

	public  static string  getProduct() { 
		object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);

		AssemblyProductAttribute attribute = null;
		if (attributes.Length > 0) {
			attribute = attributes[0] as AssemblyProductAttribute;
		}
		return (attribute != null) ? attribute.Product : "N/A"; 
	}
	public static string getVersion() {
		return typeof(Dummy).Assembly.GetName().Version.ToString();

	}

	public static string getFullyQualifiedName() {
		// https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assembly.getmodules?view=netframework-4.8#system-reflection-assembly-getmodules
		Module[] modules = assembly.GetModules();
		
		List<string> module_list = new List<string>();
		// Console.WriteLine("\tModules in the assembly:");
		foreach (Module module in modules) {
			module_list.Add(module.Name);
			// Console.WriteLine("\t{0}", module);
		}
		return String.Join(", ", module_list);   
	}
}

'@

Add-Type -TypeDefinition $code -Language CSharp
write-output("{0} {1} {2}" -f [Dummy]::getProduct(), [Dummy]::getVersion(), [Dummy]::getFullyQualifiedName())

[Dummy]::getDescription()

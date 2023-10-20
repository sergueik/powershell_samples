# Determine which .NET Framework versions are installed (specific to .NET Framework)
# origin:
# https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed
# see also: https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/versions-and-dependencies
# quote:
# .NET Framework 4.8 is the last version of .NET Framework.
# .NET Framework is serviced monthly with security and reliability bug fixes.
# .NET Framework will continue to be included with Windows, with no plans to remove it.
# You don't need to migrate your .NET Framework apps, but for new development, use .NET 5 or later.
# NOTE: the output of the below will contain all properties, instead of the specified
# Get-ItemProperty -path 'HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full' -name 'Release'
# therefore one either composes subshell style
# (Get-ItemProperty "HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full").Release
# or applies one extra filtering
# Get-ItemProperty -path 'HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full' -name 'Release' | select-object -expandproperty release
$caller_class = 'VerisonHelper'

# NOTE: challenge with filling "/Project/PropertyGroup/TargetFrameworkVersion":
# error MSB4184:
# The expression "[Microsoft.Build.Utilities.ToolLocationHelper]::GetPathToStandardLibraries(.NETFramework, v4.8 or later, '', x86)"
# cannot be evaluated. Input string was not in a correct format.
# error MSB4184:
# The expression "[Microsoft.Build.Utilities.ToolLocationHelper]::GetPathToStandardLibraries(.NETFramework, v5, '', x86)"
# cannot be evaluated. Version string portion was too short or too long.
# warning MSB3644:
# The reference assemblies for framework ".NETFramework,Version=v4.5.2" were not found.
# To resolve this, install the SDK or Targeting Pack for this framework version or
# retarget your application to a version of the framework for which you have the SDK or Targeting Pack installed.
# Note that assemblies will be resolved from the Global Assembly Cache (GAC) and will be used in place of reference assemblies.
# Therefore your assembly may not be correctly targeted for the framework you intend.
# see also: https://github.com/lchapiro/CheckDotNet
# NOTE: this  script is only concerned with obscure release/build of the 4.5.x family of .Net Frameworks
# for detection / inventory of older releases of .Net framework see https://www.codeproject.com/Articles/17501/Using-managed-code-to-detect-what-NET-Framework-ve
#
Add-Type -TypeDefinition @"

using System;
using Microsoft.Win32;
public class ${caller_class}  {

	private string version;
	public string Version {
		get { return version; }
	}
	const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";

  public ${caller_class}() {

		using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey)) {
			if (ndpKey != null && ndpKey.GetValue("Release") != null) {
				version = CheckFor45PlusVersion((int)ndpKey.GetValue("Release"));
				Console.Error.WriteLine(String.Format(".NET Framework Version: {0}", version));
			} else {
				Console.Error.WriteLine(".NET Framework Version 4.5 or later is not detected.");
			}
		}
	}

	// Checking the version using >= enables forward compatibility.
	private string CheckFor45PlusVersion(int releaseKey) {

		if (releaseKey >= 528449)
			return "4.8";
		if (releaseKey >= 528372)
			return "4.8";
		if (releaseKey >= 528049)
			return "4.8";
		if (releaseKey >= 528040)
			return "4.8";
		if (releaseKey >= 461808)
			return "4.7.2";
		if (releaseKey >= 461308)
			return "4.7.1";
		if (releaseKey >= 460798)
			return "4.7";
		if (releaseKey >= 394802)
			return "4.6.2";
		if (releaseKey >= 394254)
			return "4.6.1";
		if (releaseKey >= 393295)
			return "4.6";
		if (releaseKey >= 379893)
			return "4.5.2";
		if (releaseKey >= 378675)
			return "4.5.1";
		if (releaseKey >= 378389)
			return "4.5";
		// This code should never execute. A non-null release key should mean
		// that 4.5 or later is installed.
		return "No 4.5 or later version detected";
	}
}
"@ -ReferencedAssemblies 'mscorlib.dll'
$o = new-object -typename $caller_class
write-host $o.Version

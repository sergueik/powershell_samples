param (
  [string]$buldfile = 'alertmessage.sln'
)
# NOTE: several projects
# warning MSB3644
# the reference assemblies for framework ".NETFramework,Version=v4.0" were not found.
# # To resolve this, install the SDK or Targeting Pack for this framework version or retarget your application to a version of the framework for which you have the SDK or Targeting Pack installed. 
# # Note that assemblies will be resolved from the Global Assembly Cache (GAC) and will be used in place of reference assemblies. Therefore your assembly may not be correctly targeted for the framework you intend.
# solved by
# https://stackoverflow.com/questions/5876946/net-4-0-build-server-reference-assemblies-warnings-msb3644

$framework_path = 'c:\Windows\Microsoft.NET\Framework\v4.0.30319'
$env:path="${env:path};${framework_path}"
msbuild.exe -p:FrameworkPathOverride="${framework_path}" $buldfile
# NOTE:
# alertmessage.sln.metaproj : warning MSB4121: The project configuration
# for project "Utils" was not specified
# in the solution file for the solution configuration "Debug|x86"

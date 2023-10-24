write-host 'Cleanup'
remove-item -recurse -force Utils/obj,Utils/bin/,Program/bin/,Program/obj/,Test/bin/,Test/obj/ -erroraction SilentlyContinue

$buildfile = 'loadaverage-service.sln'
$framework_path = 'c:\Windows\Microsoft.NET\Framework\v4.0.30319'
$env:path="${env:path};${framework_path}"
write-host 'Build'
msbuild.exe -p:FrameworkPathOverride="${framework_path}" $buildfile /p:Configuration=Release /p:Platform=x86 /t:"Clean,Build"


write-host 'Build'

$msbuild = "${framework_path}\MSBuild.exe"
invoke-expression -command "$msbuild -p:FrameworkPathOverride=""${framework_path}"" $buildfile  /p:Configuration=Release /p:Platform=x86 /t:Clean,Build"

cmd %%-/c tree.com

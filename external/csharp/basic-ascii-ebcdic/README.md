### Info


[ASCII-EBCDIC-Converter](https://github.com/adnanmasood/ASCII-EBCDIC-Converter) console tool

### Usage

```cmd
.\aec.exe -help
```
```text
Usage: aec -operation=[encode|decode] -data=<string> -inputfile=<filename> -outputfile=<filename>
```


```cmd
.\aec.exe -data=12345 -operation=encode
```
```text
EBCDIC bytes (hex): F1F2F3F4F5
```
```sh
.\aec.exe -data=F1F2F3F4F5 -operation=decode
```
```text
Converted back to ASCII: 12345
```

```cmd
echo 1234567890abcdefghijklmnopqrstuvwxyz>example.txt
```
.\aec.exe -inputfile=example.txt -operation=encode -outputfile=result.txt
```
```text
EBCDIC bytes (hex): F1F2F3F4F5F6F7F8F9F0818283848586878889919293949596979899A2A3A4A5A6A7A8A90D25
```

```cmd
type result.txt
```
```text
≥≤⌠⌡÷≈°∙≡üéâäàåçêëæÆôöòûùÿÖóúñÑªº¿⌐
```

![app1](https://github.com/sergueik/powershell_samples/blob/master/external/csharp/basic-ascii-ebcdic/screenshots/capture-online.png)


```cmd
.\aec.exe -inputfile=result.txt -operation=decode
```
```text
Converted back to ASCII: 1234567890abcdefghijklmnopqrstuvwxyz
```
#### Upstream Version


update `aec.exe.config` XML:
```xml
<configuration>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5" />
  </startup>
  <appSettings>
    <add key="sourcefilename" value="source.txt" />
    <add key="outputfilename" value="destination.txt" />
    <!-- ascii|ebcdic-->
    <add key="convertto" value="ascii" />

    <!-- ebcdic codepage-->
    <add key="codepage" value="IBM037" />

    <!-- for linebreak after n bytes -->
    <add key="crlf" value="true" />
    <add key="skipbytesforcrlf" value="3000" />
  </appSettings>
</configuration>
```
e.g.
```powershell
cd '.\bin\Debug'
$name = 'aec.exe.config'
$xml = [xml](get-content -path $name )
$xml.Normalize()
$xml.configuration.appSettings.add | where-object { $_.key -eq 'crlf'} | foreach-object { $_.value = 'false' }
$xml.SelectSingleNode(('/configuration/appSettings/add[@key="{0}"]' -f 'crlf')).value     = 'false'

$xml.configuration.appSettings.add | where-object { $_.key -eq 'convertto'} | foreach-object { $_.value = 'ebcdic' }
$xml.configuration.appSettings.add | where-object { $_.key -eq 'sourcefilename'} | foreach-object { $_.value = 'example.txt' }	
$xml.configuration.appSettings.add | where-object { $_.key -eq 'outputfilename'} | foreach-object { $_.value = 'output.txt' }
# display
$xml.configuration.appSettings.add
# Note: saves into wrong directory
$xml.Save( $name )
cd '..\..'
```
this will print
```text
key              value
---              -----
sourcefilename   example.txt
outputfilename   output.txt
convertto        ebcdic
codepage         IBM037
crlf             false
skipbytesforcrlf 3000
```
create a dummy text file 'example.txt':
```powershell
$sourcefilename = 'example.txt'
out-file -filepath $sourcefilename  -inputobject '0123456789abcdefghijklmnopqrstuvwxyz' -encoding ascii -force
get-content -path $sourcefilename
```
this will print
```text
0123456789abcdefghijklmnopqrstuvwxyz
```

### Building in IDE less Environment
```cmd
path=%path%;c:\Windows\Microsoft.NET\Framework\v4.0.30319
```
```cmd
msbuild.exe basic-ascii-ebcdic.sln /t:clean,build
```
```text
Done Building Project "Utils\Utils.csproj" (default targets).
Done Building Project "Program.csproj" (default targets).
Done Building Project "basic-ascii-ebcdic.sln" (clean;build target(s)).
```
run
```
bin\Debug\aec.exe
```


### See Also

  * [ASCII EBCDIC translation tables](http://www.simotime.com/asc2ebc1.htm)
  * [wiki](http://en.wikipedia.org/wiki/EBCDIC)
  * Online ASCII and EBCDIC bytes (in hex), and vice versa  [convertor](https://www.longpelaexpertise.com.au/toolsCode.php) and help recource
  * nodejs [module](https://github.com/Voakie/ebcdic-ascii) for converting between EBCDIC and ASCII (ISO-8859-1)
  * [how to convert between ASCII and EBCDIC character code](https://mskb.pkisolutions.com/kb/216399) with explicit convesion table (VB.net)
  * https://stackoverflow.com/questions/12490458/vb-net-c-ascii-to-ebcdic
  * https://learn.microsoft.com/en-us/host-integration-server/core/convert2
  * EBCDIC Converter [VS Code Extension](https://marketplace.visualstudio.com/items?itemName=coderAllan.vscode-ebcdicconverter) and [source](https://github.com/CoderAllan/vscode-ebcdicconverter) (nodejs)

### Author

[Serguei Kouzmine](mailto:kouzmine_serguei@yahoo.com)

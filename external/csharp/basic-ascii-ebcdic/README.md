### Info

[ASCII-EBCDIC-Converter](https://github.com/adnanmasood/ASCII-EBCDIC-Converter)
console tool
### Usage

#### Upsream Version

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
$xml.configuration.appSettings.add | where-object { $_.key -eq 'crlf'}  | foreach-object { $_.value = 'false' }
$xml.configuration.appSettings.add | where-object { $_.key -eq 'convertto'}  | foreach-object { $_.value = 'ebcdic' }
$xml.configuration.appSettings.add | where-object { $_.key -eq 'sourcefilename'}  | foreach-object { $_.value = 'example.txt' }
$xml.configuration.appSettings.add | where-object { $_.key -eq 'outputfilename'}  | foreach-object { $_.value = 'output.txt' }
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
run
```
bin\Debug\aec.exe
```

this will print
```
=====================================================================
aec.exe - Simple ASCII / EBCDIC Convertor Util
Uses the application config file for values so make sure it's there.
Comments/Questions - adnan.masood@owasp.org
=====================================================================


Output file written: output.txt

All Done. Bye now.
```
but the file will not be written:
```cmd
dir output.txt
```
```text
Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
-a----         1/14/2026   6:04 PM              0 output.txt
```
```cmd
copy .\aec.exe.config ..\..\App.config
```
that is because `convertTo` attribute name is controversial, it means actually `inputType`. Update application config file once again:
```powershell
$xml = [xml](get-content -path $name )
$xml.Normalize()
$xml.configuration.appSettings.add | where-object { $_.key -eq 'convertto'}  | foreach-object { $_.value = 'ascii' }
# display
$xml.configuration.appSettings.add
# Note: saves into wrong directory
$xml.Save( $name )

```
#### Updated
### See Also

  * [ASCII EBCDIC translation tables](http://www.simotime.com/asc2ebc1.htm)
  * EBCDIC [wiki](http://en.wikipedia.org/wiki/EBCDIC)
  * Online ASCII and EBCDIC bytes (in hex), and vice versa  [convertor](https://www.longpelaexpertise.com.au/toolsCode.php) and help recource
  * nodeje [module](https://github.com/Voakie/ebcdic-ascii) for converting between EBCDIC and ASCII (ISO-8859-1)
  * [ow to convert between ASCII and EBCDIC character code](https://mskb.pkisolutions.com/kb/216399)

### Info

this directory contains code from the [article](https://www.codeproject.com/Articles/18229/How-to-run-PowerShell-scripts-from-C) modified to utilize the
`app.config` feature of loading script from specially decorated element demonstrated in [article](https://www-jo.se/f.pfleger/custom-config-cdata/)

### Usage

This application is interacrive, but the initil value of the Powershell script is loaded from `app.config`

```xml
<configuration>
  <configSections>
	<section name="script" type="Utils.ScriptElement, Utils"/>
  </configSections>
  <script name="script 1">
    <source><![CDATA[
             get-childitem -path $env:TEMP | select-object -property FullName
          ]]></source>
  </script>
</configuration>
```
NOTE: the configuration file is compiled from `app.config` which belongs to the main `Program` folder and becomes named after the Program Assembly name with the `.config` syntax
```sh
PowershellRunnerForm.exe.config
```
but is referencing the `Utils` fully qualified class `Utils.ScriptElement` and the `Utils` assembly which compiles stores next to the  main assembly:

```sh
Utils.dll
```
in the source the form text box contents are defined as follows:
```c#
var scriptElement = ConfigurationManager.GetSection("script") as Utils.ScriptElement;
textBoxScript.Text = scriptElement.Source.Value;
```
### Limitations

There can not be multiple `<script>` elements in `app.config` -  one has to create unique tags for each occurence.

### See Also

 * [powerShell Executable Generator](https://github.com/nyanhp/ExeWrapper) relying on compiling the code on the fly by `add-type`
 * [discusssion](https://stackoverflow.com/questions/6140021/app-config-and-csc-exe)  `csc.exe` compiler options for collecting specific `App.config` resource
 * [discussion](https://stackoverflow.com/questions/20379029/cdata-in-appsettings-value-attribute) of `[![CDATA'' in the `appSettings` `value` attribute
 * [example](https://www-jo.se/f.pfleger/custom-config-cdata/)implementing multiline `[![CDATA]` in the Custom Configuration Sections of `App.config`
  * [discussion](http://forum.oszone.net/thread-351767.html) (in Russian) of instaling [ps2exe](https://github.com/ikarstein/ps2exe) on different versions of Powershell
  * [PS2EXE-GUI](https://github.com/Hope-IT-Works/PS2EXE-GUI) - advanced graphical user interface for [ps2exe](https://github.com/ikarstein/ps2exe)
### Note


The following dependency assembly is causing problems in runtime

```text
C:\windows\Microsoft.Net\assembly\GAC_MSIL\System.Management.Automation\v4.0_3.0.0.0__31bf3856ad364e35\System.Management.Automation.dll
```

the `hintpath` informaion about the same is not resolvable
```cmd
..\..\..\..\..\..\..\Program Files\Reference Assemblies\Microsoft\WindowsPowerShell\v1.0\System.Management.Automation.dll
```
### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)

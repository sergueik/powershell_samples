### Info


Refactored and combined code from __MiniHttpd: an HTTP web server library__ [article](https://www.codeproject.com/Articles/11342/MiniHttpd-an-HTTP-web-server-library)

### Testing
```cmd
netsh.exe http delete urlacl url=http://+:9091/
netsh.exe http add urlacl url=http://+:9091/ user=%COMPUTERNAME%\%USERNAME%
```
this will respond with
```sh
URL reservation successfully added
```
this will need to be done again  if a different HTTP port used

Also,configure the "advanced firewall rule"
```cmd
set NAME=MiniHttpdConsole
set PROGRAM=%CD%\Tests\bin\Debug\MiniHttpdConsole.exe
netsh.exe advfirewall firewall Delete rule name="%NAME%"
netsh.exe advfirewall firewall add rule name="%NAME%" dir=in action=allow program="C:\Program Files (x86)\Google\Chrome\Application\chrome.exe" enable=yes
```
```cmd
netsh.exe advfirewall firewall show rule name="%NAME%"
```
```text
Rule Name:                            MiniHttpdConsole
----------------------------------------------------------------------
Enabled:                              Yes
Direction:                            In
Profiles:                             Domain,Private,Public
Grouping:
LocalIP:                              Any
RemoteIP:                             Any
Protocol:                             Any
Edge traversal:                       No
Action:                               Allow
Ok.

```

![Firewall Rule Dialog](https://github.com/sergueik/powershell_samples/blob/master/external/csharp/simple-httpserver/screenshots/firewall-capture.jpg)

one can use Control Panel app for [the same](https://www.tenforums.com/tutorials/70903-add-remove-allowed-apps-through-windows-firewall-windows-10-a.html)
```cmd
control firewall.cpl
```

![Control Panel Rule Information](https://github.com/sergueik/powershell_samples/blob/master/external/csharp/simple-httpserver/screenshots/control_panel_capture.png)

Open web url `http://localhost:9091/src/examples/` and navigate to page `http://localhost:9091/src/examples/rrdJFlot.html` where enter imported file URL
`/data/example_rrds/example4.rrd`

![Web Page](https://github.com/sergueik/powershell_samples/blob/master/external/csharp/simple-httpserver/screenshots/page_capture.png)

### See Also
  * [Simple HTTP Server in C#](https://www.codeproject.com/Articles/137979/Simple-HTTP-Server-in-C) - somewhat too primitive - and the [original project](https://github.com/jeske/SimpleHttpServer)
  * [single-threaded HTTP server](https://www.codeproject.com/Articles/12462/A-single-threaded-HTTP-server)
  * [client example](https://briangrinstead.com/blog/multipart-form-post-in-c)
  * [REST server for file upload via HTTP](https://github.com/waifei/file-rest-server) - on __IIS__
  * https://github.com/JSIStudios/SimpleRestServices
  * https://github.com/UsaRandom/RestEasy/tree/master
 

### Author
[Serguei Kouzmine](mailto:kouzmine_serguei@yahoo.com)

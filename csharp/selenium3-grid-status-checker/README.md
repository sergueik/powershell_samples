

__System Tray Selenum 3.x Grid Status Checker___

processing grid status page via `System.Windows.Forms.WebBrowser.WebBrowser`.

### Notes:

Default setting of `System.Windows.Forms.Browser` is somewhat pathetic. It cannot load grid console page of Selenium __3.x__ due to error in loading  the `jqury.js` library:
```txt
Object doesn't support property or method 'addEventListener'
```
Tweaking the `HKEY_CURRENT_USER\SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION` does not appear to fix this
```cmd
Couldn't process file Resources.resx due to its being in the Internet or Restricted zone or having the mark of the web on the file - remove the mark of the web if you want to process these files
```


### Mock Testing

* if there is no operational grid, one can run the static console page stub from [sergueik/springboot_study/basic-thymeleaf-jsp](https://github.com/sergueik/springboot_study/tree/master/basic-thymeleaf-jsp)
```sh
pushd ../../../springboot_study/basic-thymeleaf-jsp
mvn -Dmaven.test.skip=true spring-boot:run
```
the minimal Selenium 3 Grid Console page stub will be shown on `http://localhost:8080/resources/static/page.html`

![selenium 3 grid console stub](https://github.com/sergueik/powershell_samples/blob/master/csharp/selenium3-grid-status-checker/screenshots/capture-grid-console-stub.png)

To configure the tray app to pick this location need to direct to  one will need to uncomment the `key="ServiceUrl"` entry in `app.config`:
```xml 
<add key="ServiceUrl" value="http://localhost:8080/resources/static/page.html"/>
```

The `ServiceUrl` setting overrides the `ServiceUrlTemplate` and skips hub processing.	to suppress this behavior, comment the setting or clear the value.

The stub will be shown as:

![selenium 3 grid status indicator stub](https://github.com/sergueik/powershell_samples/blob/master/csharp/selenium3-grid-status-checker/screenshots/capture-grid-status-indicator-stub.png)


### Selenium 4


with Selenium 4 one can query

```sh
http://192.168.0.125:4444/status
```

and parse the  JSON path `.value.nodes[].uri` from JSON which will look like:
```json
{
  "value": {
    "ready": true,
    "message": "Selenium Grid ready.",
    "nodes": [
      {
        "id": "08a8911d-6c3d-4b3a-bfc9-ac30101dae6e",
        "uri": "http:\u002f\u002f192.168.0.125:5557",
        "maxSessions": 1,
        "osInfo": {
          "arch": "x86",
          "name": "Windows 10",
          "version": "10.0"
        },
        "heartbeatPeriod": 60000,
        "availability": "UP",
        "version": "4.0.0 (revision 3a21814679)",
        "slots": [
          {
            "lastStarted": "1970-01-01T00:00:00Z",
            "session": null,
            "id": {
              "hostId": "08a8911d-6c3d-4b3a-bfc9-ac30101dae6e",
              "id": "7a126527-bb91-43ef-a8ea-1ab50d616059"
            },
            "stereotype": {
              "browserName": "firefox",
              "platformName": "WIN10"
            }
          },
          {
            "lastStarted": "1970-01-01T00:00:00Z",
            "session": null,
            "id": {
              "hostId": "08a8911d-6c3d-4b3a-bfc9-ac30101dae6e",
              "id": "c8df4605-50eb-4192-91ca-0a5c78396231"
            },
            "stereotype": {
              "browserName": "chrome",
              "platformName": "WIN10"
            }
          }
        ]
      },
      {
        "id": "a9d785ad-b376-486e-b8d4-41d6483dda5d",
        "uri": "http:\u002f\u002f192.168.0.125:5555",
        "maxSessions": 1,
        "osInfo": {
          "arch": "x86",
          "name": "Windows 10",
          "version": "10.0"
        },
        "heartbeatPeriod": 60000,
        "availability": "UP",
        "version": "4.0.0 (revision 3a21814679)",
        "slots": [
          {
            "lastStarted": "1970-01-01T00:00:00Z",
            "session": null,
            "id": {
              "hostId": "a9d785ad-b376-486e-b8d4-41d6483dda5d",
              "id": "ac091ade-34de-4579-92bd-289840d6e735"
            },
            "stereotype": {
              "browserName": "firefox",
              "platformName": "WIN10"
            }
          },
          {
            "lastStarted": "1970-01-01T00:00:00Z",
            "session": null,
            "id": {
              "hostId": "a9d785ad-b376-486e-b8d4-41d6483dda5d",
              "id": "c8f5887a-f7a1-483b-ab57-dc212bab5f03"
            },
            "stereotype": {
              "browserName": "chrome",
              "platformName": "WIN10"
            }
          }
        ]
      }
    ]
  }
}
```
or
```JSON
{
    "ready": false,
    "message": "Unable to read distributor status."
  }
}

```
in case of a failure
or read the UI on `http://192.168.0.125:4444/ui/index.html` or `http://192.168.0.125:4444/ui/index.html#/`

![new console](https://github.com/sergueik/powershell_samples/blob/master/csharp/selenium3-grid-status-checker/screenshots/capture-selenium4-ui.png)

using Css Selector `div.MuiContainer-root div.MuiCardContent-root div.MuiGrid-item`

The UI layout is considerably more complex than with __3.x__ earlier releases

![old console](https://github.com/sergueik/powershell_samples/blob/master/csharp/selenium3-grid-status-checker/screenshots/capture-selenium3-grid-console.png)



and relies on Javascript which makes it unparsable by
old `System.Windows.Forms.WebBrowser.WebBrowser` class in Windows Forms / .Net  Framework __4.x__ - That class hosts the IE browser which is not a modern
Javascript-capable browser.
```html
<html lang="en">

<head>

<meta charset="utf-8"/>
<link href="/ui/favicon.svg" rel="icon" type="image/svg">

<meta content="width=device-width,initial-scale=1" name="viewport"/>

<link href="/ui/logo192.png" rel="apple-touch-icon">

<link href="/ui/manifest.json" rel="manifest"/>

<title>Selenium Grid</title>

<link href="/ui/static/css/main.b5f7dd99.chunk.css" rel="stylesheet"/>

</head>

<body>

<noscript>You need to enable JavaScript to run this app.</noscript>

<div id="root"></div>

<script>! function(e) {
    function r(r) {
        for (var n, i, l = r[0], f = r[1], a = r[2], c = 0, s = []; c <
            l.length; c++) i = l[c], Object.prototype.hasOwnProperty.call(o, i) && o[i] && s.push(o[i][0]), o[i] = 0;
        for (n in f) Object.prototype.hasOwnProperty.call(f, n) && (e[n] = f[n]);
        for (p && p(r); s.length;) s.shift()();
        return u.push.apply(u, a || []), t()
    }

    function t() {
        for (var e, r = 0; r <
            u.length; r++) {
            for (var t = u[r], n = !0, l = 1; l <
                t.length; l++) {
                var f = t[l];
                0 !== o[f] && (n = !1)
            }
            n && (u.splice(r--, 1), e = i(i.s = t[0]))
        }
        return e
    }
    var n = {},
        o = {
            1: 0
        },
        u = [];

    function i(r) {
        if (n[r]) return n[r].exports;
        var t = n[r] = {
            i: r,
            l: !1,
            exports: {}
        };
        return e[r].call(t.exports, t, t.exports, i), t.l = !0, t.exports
    }
    i.m = e, i.c = n, i.d = function(e, r, t) {
        i.o(e, r) || Object.defineProperty(e, r, {
            enumerable: !0,
            get: t
        })
    }, i.r = function(e) {
        "undefined" != typeof Symbol && Symbol.toStringTag && Object.defineProperty(e, Symbol.toStringTag, {
            value: "Module"
        }), Object.defineProperty(e, "__esModule", {
            value: !0
        })
    }, i.t = function(e, r) {
        if (1 & r && (e = i(e)), 8 & r) return e;
        if (4 & r && "object" == typeof e && e && e.__esModule) return e;
        var t = Object.create(null);
        if (i.r(t), Object.defineProperty(t, "default", {
                enumerable: !0,
                value: e
            }), 2 & r && "string" != typeof e)
            for (var n in e) i.d(t, n, function(r) {
                return e[r]
            }.bind(null, n));
        return t
    }, i.n = function(e) {
        var r = e && e.__esModule ? function() {
            return e.default
        } : function() {
            return e
        };
        return i.d(r, "a", r), r
    }, i.o = function(e, r) {
        return Object.prototype.hasOwnProperty.call(e, r)
    }, i.p = "/ui/";
    var l = this.webpackJsonpgrid_ui = this.webpackJsonpgrid_ui || [],
        f = l.push.bind(l);
    l.push = r, l = l.slice();
    for (var a = 0; a <
        l.length; a++) r(l[a]);
    var p = f;
    t()
}([])
</script>

<script src="/ui/static/js/2.f1ad6d8b.chunk.js"	>
</script>

<script src="/ui/static/js/main.485aaafa.chunk.js">
</script>

</body>
</html>

```
will fail to load the data into `<div id="root">` when no Javascript is enabled. To get the node inventory, the page performs the following graphql request:
```Javascript

{
    "operationName": "GetNodes",
    "variables": {},
    "query": "query GetNodes {
    nodesInfo {
        nodes {
            id
            uri
            status
            maxSession
            slotCount
            stereotypes
            version
            sessionCount
            osInfo {
                version
                name
                arch
                __typename
            }
            __typename
        }
        __typename
    }
}
"}

```

and gets back JSON
```JSON
{
    "data": {
        "nodesInfo": {
            "nodes": [{
                "id": "bc600cb4-095b-47cf-adb7-69428e5e6d9d",
                "uri": "http:\u002f\u002f192.168.0.125:5555",
                "status": "UP",
                "maxSession": 1,
                "slotCount": 2,
                "stereotypes": "[\n  {\n    \"slots\": 1,\n    \"stereotype\": {\n      \"browserName\": \"firefox\",\n      \"platformName\": \"WIN10\"\n    }\n  },\n  {\n    \"slots\": 1,\n    \"stereotype\": {\n      \"browserName\": \"chrome\",\n      \"platformName\": \"WIN10\"\n    }\n  }\n]",
                "version": "4.0.0 (revision 3a21814679)",
                "sessionCount": 0,
                "osInfo": {
                    "version": "10.0",
                    "name": "Windows 10",
                    "arch": "amd64",
                    "__typename": "OsInfo"
                },
                "__typename": "Node"
            }],
            "__typename": "Nodes"
        }
    }
}
	
```
and
```Javascript
{
    "operationName": "Summary",
    "variables": {},
    "query": "query Summary {\n  grid {\n    uri\n    totalSlots\n    nodeCount\n    maxSession\n    sessionCount\n    sessionQueueSize\n    version\n    __typename\n  }\n}\n"
}
```
and gets
```JSON
{
    "data": {
        "grid": {
            "uri": "http:\u002f\u002f192.168.0.125:4444",
            "totalSlots": 2,
            "nodeCount": 1,
            "maxSession": 1,
            "sessionCount": 0,
            "sessionQueueSize": 0,
            "version": "4.0.0 (revision 3a21814679)",
            "__typename": "Grid"
        }
    }
}
```
NOTE it will show the error
```text
Unable to find /index.html
```
if run by older JDK than __11__

### Edit Configuration

one can modify the application configuration file `config.ini` deployed to application directory, by using the "Config" menu. The tray application will wait for the editor to be closed:

![modify the application configuration file](https://github.com/sergueik/powershell_samples/blob/master/csharp/selenium3-grid-status-checker/screenshots/capture-edit-config.png)

### TODO
  * Compare `System.Windows.Forms.WebBrowser.WebBrowser` and `Microsoft.Web.WebView2.WinForms.WebView2`.

NOTE: `Microsoft.Web.WebView2.WinForms.dll` is not installed with .Net Framework and needs to be [installed](https://learn.microsoft.com/en-us/microsoft-edge/webview2/get-started/winforms) from the WebView2 SDK nuget package. This may [lead](https://learn.microsoft.com/en-us/answers/questions/456717/deployed-c-app-using-webview2-cannot-find-the-runt.html) to dependency conflicts.

This [control](https://learn.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.winforms.webview2?view=webview2-dotnet-1.0.1418.22) is effectively a wrapper around the Microsoft Edge [WebView2 COM API](https://aka.ms/webview2).

### Note

The `System.Windows.Forms.WebBrowser` control allows one
host Web pages and other browser-enabled documents inside Windows Forms applications, but does not work well with textmode Powershell scripts.
In the below snipet the `$doc` will be `$null`:
```powershell
add-type -assembly System.Windows.Forms
$browser = new-object System.Windows.Forms.WebBrowser
$doc = $browser.Document
```
therefore the next code would just fail
```powershell
$domdoc = $browser.Document.DomDocument
$domdoc.open()
```
the working [script](https://github.com/sergueik/powershell_selenium/blob/master/utils/parse_grid_console.ps1) performing grid page parse is in [powershell selenum repository](https://github.com/sergueik/powershell_selenium)

Apparently the `System.Windows.Forms.WebBrowser` control is for [embedding in the UI](https://www.powershellgallery.com/packages/PSMyClaims/1.1.0.2/Content/Invoke-WebBrowser.ps1)

### Version

To check the application version in the command line, run
```powershell
get-item SeleniumClient.exe | select-object -expandproperty VersionInfo | select-object -expandProperty FileVersion
```
```sh
1.9.0.0
```

On the dialog we only display `Major`, `Minor` and `Build` fields
### Startup Balloon Tip Message

*  the textbook code
```csharp
notifyIcon.BalloonTipText = "polls status of Selenium Grid";
notifyIcon.BalloonTipTitle = "System Tray Selenium Grid Status Checker";
processIcon.DisplayBallonMessage(null, 10000);
```
does not work when there are many system tray applications and may need more work

![selenium 3 grid startup Baloon](https://github.com/sergueik/powershell_samples/blob/master/csharp/selenium3-grid-status-checker/screenshots/capture-baloon.png)
### See Also

  * https://www.codeproject.com/Tips/627796/Doing-a-NotifyIcon-Program-the-Right-Wayhttps://www.codeproject.com/Articles/7827/Customizing-WinForm-s-System-Menu
  * https://stackoverflow.com/questions/56107/what-is-the-best-way-to-parse-html-in-c
  * https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.datagrid?view=netframework-4.0
  * https://stackoverflow.com/questions/6409839/reading-dataset

  * https://www.benefitagent.com/doc/Rendering%20Issues%20using%20WebBrowser%20Control%20in%20Windows.html
  * https://stackoverflow.com/questions/12216301/object-doesnt-support-property-or-method-webbrowser-control
  * https://github.com/TomRom27/LectioDivina
  * https://csharp.hotexamples.com/examples/mshtml/IHTMLDocument2/-/php-ihtmldocument2-class-examples.html
  * https://newbedev.com/html-how-to-load-html-code-in-web-browser-in-c-vs-code-example
  * https://social.msdn.microsoft.com/Forums/vstudio/en-US/3875b32a-0a08-4c35-acee-233f14c5057b/parsing-a-html-file-in-a-console-app?forum=vbgeneral
  * https://csharp.hotexamples.com/examples/mshtml/IHTMLDocument2/-/php-ihtmldocument2-class-examples.html
  * https://newbedev.com/html-how-to-load-html-code-in-web-browser-in-c-vs-code-example
  * https://stackoverflow.com/questions/69909422/selenium-grid-unable-to-start-selenium-grid-hub
  * https://stackoverflow.com/questions/153748/how-to-inject-javascript-in-webbrowser-control
  * https://stackoverflow.com/questions/5216194/where-is-ihtmlscriptelement/5216255#5216255
  * [an INI file handling class using C# and p/invoke](https://www.codeproject.com/Articles/1966/An-INI-file-handling-class-using-C)
   * [another tray app with behavior](https://www.codeproject.com/Articles/104644/Close-Your-Application-to-the-Notification-Area) (VB.NEt)
   * A homemade  [dialo gmessage box ](https://github.com/chris-mackay/DialogMessage) for Windows
   * Enabling IE11 emulation mode in `System.Windows.Forms.WebBrowser` [link](https://stackoverflow.com/questions/38514184/how-can-i-get-the-webbrowser-control-to-show-modern-contents/38514446#38514446) - appears to deal with Registry. [another](https://www.sapien.com/blog/2020/11/05/a-simple-fix-for-problems-with-windows-forms-webbrowser/) example of that
  *  webview2 [control](https://learn.microsoft.com/en-us/microsoft-edge/webview2/?WT.mc_id=DT-MVP-5003235) - relies on embedding of Microsoft Edge rendering engine
  * [enabling](https://stackoverflow.com/questions/59269515/upgrading-webbrowser-class-into-webview-class-in-winforms-application/59271260#59271260) WebView compatible control
  * [discussion](https://www.cyberforum.ru/csharp-net/thread106428-page2.html#post8424851) of implementing single instance forms (in Russian)
  * setting the Internet Explorer engine compatibility
    + http://www.devhut.net/webbrowser-activex-control-google-maps-invalid-character-scripting-error/
    + https://stackoverflow.com/questions/17922308/use-latest-version-of-internet-explorer-in-the-webbrowser-control/17922508#17922508
    + https://learn.microsoft.com/en-us/previous-versions/windows/internet-explorer/ie-developer/general-info/ee330730(v=vs.85)


### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)

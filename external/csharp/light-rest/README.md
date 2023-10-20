### Info

Code from __RESTful Made Simple - A Generic REST Service Client Library__  [article](https://www.codeproject.com/Articles/331350/A-Generic-REST-Client-Library)

It was modified to reach `http://catfact.ninja/fact` instead of the application running locally (it does not appear to handle HTTPS but it is unlikely that Selenium Grid status page needs one.
With __Selenium 4 Grid__ one will make a GET request to __Grid Status__ 
[endpoint](https://www.selenium.dev/documentation/grid/advanced_features/endpoints/) `http://localhost:4444/status` and receive the JSON:
```json

{
  "value": {
    "ready": true,
    "message": "Selenium Grid ready.",
    "nodes": [
      {
        "id": "340232e5-36dd-4014-9a86-7770e45579a6",
        "uri": "http:\u002f\u002f10.0.2.15:5555",
        "maxSessions": 1,
        "osInfo": {
          "arch": "amd64",
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
              "hostId": "340232e5-36dd-4014-9a86-7770e45579a6",
              "id": "49d6090b-798d-4b0b-9ce7-8a7a7400e962"
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
              "hostId": "340232e5-36dd-4014-9a86-7770e45579a6",
              "id": "e6928dba-2a7b-4f4c-9c39-51e2ed542db6"
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

when there is one node `UP`
and  
```json
{
  "value": {
    "ready": true,
    "message": "Selenium Grid ready.",
    "nodes": [
      {
        "id": "340232e5-36dd-4014-9a86-7770e45579a6",
        "uri": "http:\u002f\u002f10.0.2.15:5555",
        "maxSessions": 1,
        "osInfo": {
          "arch": "amd64",
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
              "hostId": "340232e5-36dd-4014-9a86-7770e45579a6",
              "id": "49d6090b-798d-4b0b-9ce7-8a7a7400e962"
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
              "hostId": "340232e5-36dd-4014-9a86-7770e45579a6",
              "id": "e6928dba-2a7b-4f4c-9c39-51e2ed542db6"
            },
            "stereotype": {
              "browserName": "chrome",
              "platformName": "WIN10"
            }
          }
        ]
      },
      {
        "id": "a5227c28-7e4a-4e4e-998e-1005be850447",
        "uri": "http:\u002f\u002f10.0.2.15:5556",
        "maxSessions": 1,
        "osInfo": {
          "arch": "amd64",
          "name": "Windows 10",
          "version": "10.0"
        },
        "heartbeatPeriod": 60000,
        "availability": "DOWN",
        "version": "4.0.0 (revision 3a21814679)",
        "slots": [
          {
            "lastStarted": "1970-01-01T00:00:00Z",
            "session": null,
            "id": {
              "hostId": "a5227c28-7e4a-4e4e-998e-1005be850447",
              "id": "2769b0ff-569e-4fd2-a8f4-f5e6b72576e5"
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
              "hostId": "a5227c28-7e4a-4e4e-998e-1005be850447",
              "id": "a510d576-28e1-4982-822b-1aa3e1550ebe"
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
when there is two nodes `UP`
To set up Selenium 4 Grid environment hostd on Windows node, use batch files from [selenium java utils](https://github.com/sergueik/selenium_java/tree/master/utils) directory
### Running the App
Configure the app via `app.config` or `Client.exe.config`:
```xml
<?xml version="1.0"?>
<configuration>
  <appSettings>
    <add key="ServiceUrl" value="http://192.168.0.142:4444/status"/>
  </appSettings>
<startup><supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.0"/></startup></configuration>

```
* start the Selenium 4 hub on the specified node, monitor it bootstrap message to begin the Application tests:

![hub bootstrap](https://github.com/sergueik/powershell_ui_samples/blob/master/external/csharp/light-rest/screenshots/capture-hub-bootstrap-healthcheck.png)

* observe the RAW JSON through the browser

![browser status JSON](https://github.com/sergueik/powershell_ui_samples/blob/master/externl/csharp/light-rest/screenshots/capture-browser-status.png)

* run the `Synchronous Call` and see the datagrid render the `Node` objects with default settings:

![datagrid](https://github.com/sergueik/powershell_ui_samples/blob/master/external/csharp/light-rest/screenshots/capture-application-grid-status.png)

### Building

* rebuild the complex project in the IDE or commandline
 + open console, navigate to the project directory (`C:\developer\sergueik\powershell_ui_samples\external\csharp\light-rest`) open Poweshell console:

```sh
start powershell.exe -noprofile
```
and run in Powershell console

```powershell
$buildfile = 'light-rest.sln'
$framework_path = 'c:\Windows\Microsoft.NET\Framework\v4.0.30319'
$env:path="${env:path};${framework_path}"
msbuild.exe -p:FrameworkPathOverride="${framework_path}" /t:Clean,Build $buldfile
```
alternatively
```powershell
$buildfile = 'light-rest.sln'
$framework_path = 'c:\Windows\Microsoft.NET\Framework\v4.0.30319'
$msbuild = "${framework_path}\MSBuild.exe"
invoke-expression -command "$msbuild -p:FrameworkPathOverride=""${framework_path}"" /t:Clean,Build $buildfile"
```
* run the application
```powershelll
.\Client\bin\Debug\Client.exe
```

![console run](https://github.com/sergueik/powershell_ui_samples/blob/master/external/csharp/light-rest/screenshots/capture-console-test-run.png)

### Mocking the Grid
* run basic Springboot app returning the prepared JSON, passed back and forth through gson to ensure it is valid:
```java
@GetMapping(value = "/status", produces = MediaType.APPLICATION_JSON_VALUE)
	public Data json() {
		final String result = "{ \"value\": { \"ready\": true, \"message\": \"Selenium Grid ready.\", \"nodes\": [ { \"id\": \"340232e5-36dd-4014-9a86-7770e45579a6\", \"uri\": \"http:\u002f\u002f10.0.2.15:5555\", \"maxSessions\": 1, \"osInfo\": { \"arch\": \"amd64\", \"name\": \"Windows 10\", \"version\": \"10.0\" }, \"heartbeatPeriod\": 60000, \"availability\": \"UP\", \"version\": \"4.0.0 (revision 3a21814679)\", \"slots\": [ { \"lastStarted\": \"1970-01-01T00:00:00Z\", \"session\": null, \"id\": { \"hostId\": \"340232e5-36dd-4014-9a86-7770e45579a6\", \"id\": \"49d6090b-798d-4b0b-9ce7-8a7a7400e962\" }, \"stereotype\": { \"browserName\": \"firefox\", \"platformName\": \"WIN10\" } }, { \"lastStarted\": \"1970-01-01T00:00:00Z\", \"session\": null, \"id\": { \"hostId\": \"340232e5-36dd-4014-9a86-7770e45579a6\", \"id\": \"e6928dba-2a7b-4f4c-9c39-51e2ed542db6\" }, \"stereotype\": { \"browserName\": \"chrome\", \"platformName\": \"WIN10\" } } ] } ] } }";
		Data data = gson.fromJson(result, Data.class);
		Node node1 = new Node();
		node1.setAvailability("UP");
		node1.setId("node1 added manually");
		Node node2 = new Node();
		node2.setAvailability("DOWN");
		node2.setId("node2 added manually");
		// for GUID constructor
		// see also:
		// http://www.java2s.com/Code/Java/Development-Class/RandomGUID.htm
		Value value = data.getValue();
		List<Node> nodes = value.getNodes();
		nodes.add(node1);
		nodes.add(node2);
		value.setNodes(nodes);
		data.setValue(value);
		return data;
	}
```
* configure app toconnect to that server:
```xml
<?xml version="1.0"?>
<configuration>
  <appSettings>
    <add key="ServiceUrl" value="http://192.168.0.25:8085/basic/status"/>
  </appSettings>
<startup><supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5"/></startup></configuration>

```
![datagrid-mock](https://github.com/sergueik/powershell_ui_samples/blob/master/external/csharp/light-rest/screenshots/capture-application-grid-status-mockserver.png)

### See Also

  * [testing site](https://apipheny.io/free-api/)
  * [convert Json to C# Classes Online](https://json2csharp.com)
  * __JSONVue__ [chrome extension](https://chrome.google.com/webstore/detail/jsonvue/chklaanhfefbnpoihckbnefhakgolnmc) and [fork](https://github.com/gildas-lormeau/JSONVue) - provides generic formatting
  * __JSON Formatter__ [chrome extension](https://chrome.google.com/webstore/detail/json-formatter/bcjindcccaagfpapjjmafapmmgkkhgoa/related) and [repository](https://github.com/callumlocke/json-formatter)

### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)

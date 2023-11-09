### Info


__Selenum 4 Grid Status Checker___

Processing grid status JSON via [LightCaseClient](https://www.codeproject.com/Articles/331350/A-Generic-REST-Client-Library) - custom `System.Net.WebRequest`-based REST client .
Most of the code is unchanged from [System Tray Selenum 3.x Grid Status Checker](https://github.com/sergueik/powershell_samples/blob/master/csharp/selenium-grid-status-checker)

![new console](https://github.com/sergueik/powershell_samples/blob/master/csharp/selenium4-grid-status-checker/screenshots/capture-app.png)

### Usage


Selenium 4 one status is JSON:

```sh
http://192.168.0.125:4444/status
```
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
        "availability": "DOWN",
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
where nodes may be `UP` or `DOWN`,
or with JSON

```JSON
{
  "value": {
    "ready": false,
    "message": "Selenium Grid not ready.",
    "nodes": [
    ]
  }
}
```
when grid is in poor health,
or with JSON
```JSON
{
    "ready": false,
    "message": "Unable to read distributor status."
  }
}

```
in case of a hub failure

### Mock Testing

* if there is no operational grid, one can run the static JSON grid status stub from [sergueik/springboot_study/basic-grid-mock](https://github.com/sergueik/springboot_study/tree/master/basic-grid-mock)

```sh
pushd ../../../springboot_study/basic-grid-mock
mvn -Dmaven.test.skip=true spring-boot:run
```
the minimal Selenium 4 Grid JSON wll be returned on : 
```sh
curl -x http://localhost:4444/status
```
```json
{
  "value": {
    "ready": true,
    "message": "Selenium Grid ready.",
    "nodes": [
      {
        "id": "c0cdd050-8012-49d2-a841-e63d188c4b61",
        "uri": "http://node00:5555",
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
              "hostId": "c0cdd050-8012-49d2-a841-e63d188c4b61",
              "id": "2ec59c0f-9d92-4426-8a4f-2c2edceb416d"
            },
            "stereotype": {
              "browserName": "chrome",
              "platformName": "WIN10"
            }
          },
          {
            "lastStarted": "1970-01-01T00:00:00Z",
            "session": null,
            "id": {
              "hostId": "c0cdd050-8012-49d2-a841-e63d188c4b61",
              "id": "77ed2893-d75d-433c-b37d-552fe306da9c"
            },
            "stereotype": {
              "browserName": "firefox",
              "platformName": "WIN10"
            }
          }
        ]
      },
      {
        "id": "38cf6aeb-9b2c-4656-972f-e0a217c87e8c",
        "uri": "http://node01:5555",
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
              "hostId": "38cf6aeb-9b2c-4656-972f-e0a217c87e8c",
              "id": "588ae115-bb08-4a58-a21f-f8b6bc11cc47"
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
              "hostId": "38cf6aeb-9b2c-4656-972f-e0a217c87e8c",
              "id": "aee68a44-e3d8-4aab-ac7d-6b23211b94d1"
            },
            "stereotype": {
              "browserName": "chrome",
              "platformName": "WIN10"
            }
          }
        ]
      },
      {
        "id": "5bba6684-0c39-40d8-82c6-6fa9678fc472",
        "uri": "http://node02:5555",
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
              "hostId": "5bba6684-0c39-40d8-82c6-6fa9678fc472",
              "id": "fe10eb71-a805-46a6-aa98-5ab0db2c365e"
            },
            "stereotype": {
              "browserName": "chrome",
              "platformName": "WIN10"
            }
          },
          {
            "lastStarted": "1970-01-01T00:00:00Z",
            "session": null,
            "id": {
              "hostId": "5bba6684-0c39-40d8-82c6-6fa9678fc472",
              "id": "b9136c4a-b094-4b3d-adaf-e6370b1d1534"
            },
            "stereotype": {
              "browserName": "firefox",
              "platformName": "WIN10"
            }
          }
        ]
      },
      {
        "id": "8a6258d8-d606-4391-b9f5-42dad1f38802",
        "uri": "http://node03:5555",
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
              "hostId": "8a6258d8-d606-4391-b9f5-42dad1f38802",
              "id": "ea010321-5b54-4afb-9a81-1175d38b7c3d"
            },
            "stereotype": {
              "browserName": "chrome",
              "platformName": "WIN10"
            }
          },
          {
            "lastStarted": "1970-01-01T00:00:00Z",
            "session": null,
            "id": {
              "hostId": "8a6258d8-d606-4391-b9f5-42dad1f38802",
              "id": "b186b433-38dd-49c5-9283-d2a934eb0eaa"
            },
            "stereotype": {
              "browserName": "firefox",
              "platformName": "WIN10"
            }
          }
        ]
      },
      {
        "id": "38d1296a-0fbe-4692-a0fb-17bed8e5558b",
        "uri": "http://node04:5555",
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
              "hostId": "38d1296a-0fbe-4692-a0fb-17bed8e5558b",
              "id": "448140eb-86bc-443b-8aee-6bbf549ed5c4"
            },
            "stereotype": {
              "browserName": "chrome",
              "platformName": "WIN10"
            }
          },
          {
            "lastStarted": "1970-01-01T00:00:00Z",
            "session": null,
            "id": {
              "hostId": "38d1296a-0fbe-4692-a0fb-17bed8e5558b",
              "id": "9f4a5c79-39c5-4631-845b-0817cb555966"
            },
            "stereotype": {
              "browserName": "firefox",
              "platformName": "WIN10"
            }
          }
        ]
      },
      {
        "id": "node1 added manually",
        "uri": null,
        "maxSessions": 0,
        "osInfo": null,
        "heartbeatPeriod": 0,
        "availability": "UP",
        "version": null,
        "slots": []
      },
      {
        "id": "node2 added manually",
        "uri": null,
        "maxSessions": 0,
        "osInfo": null,
        "heartbeatPeriod": 0,
        "availability": "DOWN",
        "version": null,
        "slots": []
      }
    ]
  }
}

```
The transitional Selenium 4 grid JSON will be returned on
```sh
curl -s http://localhost:4444/status?version=3
```
```json
{
  "status": 0,
  "value": {
    "ready": true,
    "message": "Hub has capacity",
    "build": {
      "revision": "63f7b50",
      "time": "2018-02-07T22:42:28.403Z",
      "version": "3.9.2"
    },
    "os": {
      "arch": "x86",
      "name": "Windows 7",
      "version": "6.1"
    },
    "java": {
      "version": "1.8.0_101"
    }
  }
}

```
finally to get the minimal Selenium 3 Grid Console page stub shown on `http://localhost:8080/resources/static/page.html` use
[sergueik/springboot_study/basic-thymeleaf-jsp](https://github.com/sergueik/springboot_study/tree/master/basic-thymeleaf-jsp)

```sh
pushd ../../../springboot_study/basic-thymeleaf-jsp
mvn -Dmaven.test.skip=true spring-boot:run
```

To configure the tray app to pick one of these stub hubs one needs to uncomment the `key="ServiceUrl"` entry in the `app.config`:
```xml 
<add key="ServiceUrl" value="http://localhost:8080/resources/static/page.html"/>
```

The `ServiceUrl` setting overrides the `ServiceUrlTemplate` and skips hub processing.	to suppress this behavior, comment the setting or clear the value.
Only one stub hub is possible to use - because the hub selection is suppressed in this mode.

![selenium 3 grid console stub](https://github.com/sergueik/powershell_samples/blob/master/csharp/selenium3-grid-status-checker/screenshots/capture-grid-console-stub.png)


### Older Release Detection

download grid 3.9.1 from the relevant link  found on [https://www.selenium.dev/downloads/#previous-releases](https://www.selenium.dev/downloads/#previous-releases) page
```sh
curl -L -O  https://github.com/SeleniumHQ/selenium/releases/download/selenium-3.9.1/selenium-server-standalone-3.9.1.jar
```

The *very old* __3.x__ release of __Selenium Server__ e.g. __3.3.1__ will respond http://localhost:4444/status with a "Whoops! The URL specified routes to this help page." HTML page (not a redirect - in fact the HEAD request is timing out)
The app yet need to detect this

The *latest* __3.x__ releases of __Selenium Server__ 
e.g. __3.9.1__ will respond with alternaive JSON:

```json
{
  "status": 0,
  "value": {
    "ready": true,
    "message": "Hub has capacity",
    "build": {
      "revision": "63f7b50",
      "time": "2018-02-07T22:42:28.403Z",
      "version": "3.9.1"
    },
    "os": {
      "arch": "x86",
      "name": "Windows 7",
      "version": "6.1"
    },
    "java": {
      "version": "1.8.0_101"
    }
  }
}
```
### See Also
  * [code2flow](https://app.code2flow.com) - online graphviz-like flowchart creator Copyright © 2013-2022, Code Charm, Inc.
  * __graphviz__ [download page](https://graphviz.org/download/)
  * __Selenium__ [download page](https://www.selenium.dev/downloads/)
  * [discussion](https://www.cyberforum.ru/csharp-net/thread106428-page2.html#post8424851) of implementing single instance forms (in Russian)


### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)

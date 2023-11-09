### Usage

* run the ShareDevelop elevated
* in SharpDevelop run the test. This will launch the dummy HTTP server on a random TCP port locally
```text
DomainUsage: Single
Selected test(s): Test
Using Webroot path: C:\developer\sergueik\powershell_samples\external\csharp\basic-configserver\Test\bin\Debug
Using Port 61920
```
* use that TCP port to get the file
```sh
curl -Os http://localhost:61920/data.json
```
check the file hash to match
```sh
md5sum.exe  data.json
```
```text
0dfa1329f15fefa8648856794eb33244 *data.json
```
```sh
md5sum.exe  Test/data.json
0dfa1329f15fefa8648856794eb33244 *Test/data.json
```
### Note

* receiving bogus exception on several machines:
```text
Exception: System.Net.ProtocolViolationException: Bytes to be written to the stream exceed the Content-Length bytes size specified.
   at System.Net.HttpResponseStream.Write(Byte[] buffer, Int32 offset, Int32 size)
   at Utils.SimpleHTTPServer.Process(HttpListenerContext context)
```

while same code works on other machines.
The size of the bytes sent as logged by the server 
```text
About to send 303 bytes
```
match the `Content-Length: ` header seen by the client:
```text


curl -vI "http://localhost:64536/data.json?name=index.html"
* Connected to localhost (::1) port 64536 (#0)
> HEAD /data.json?name=index.html HTTP/1.1
> Host: localhost:64536
> User-Agent: curl/7.75.0
> Accept: */*
>
* Mark bundle as not supporting multiuse
< HTTP/1.1 500 Internal Server Error
< Content-Length: 303
< Content-Type: text/html
< Last-Modified: Thu, 12 Oct 2023 21:11:25 GMT
< Server: Microsoft-HTTPAPI/2.0
< Date: Fri, 13 Oct 2023 01:59:45 GMT
<

* Connection #0 to host localhost left intact
```
the `curl` command works without error without
`-I` flag:
```sh
curl -sv "http://127.0.0.1:64607/data.json?name=data.json"
``` 
```text
{
  "host1": {
    "netstat": [
      22,
      443,
      3306
    ]
  },
  "host2": {
    "netstat": [
    ]
  },
  "host3": {}
}

* Uses proxy env variable no_proxy == '192.168.99.103,192.168.99.100'
*   Trying 127.0.0.1:64607...
* Connected to 127.0.0.1 (127.0.0.1) port 64607 (#0)
> GET /data.json?name=data.json HTTP/1.1
> Host: 127.0.0.1:64607
> User-Agent: curl/7.75.0
> Accept: */*
>
* Mark bundle as not supporting multiuse
< HTTP/1.1 200 OK
< Content-Length: 148
< Content-Type: application/json
< Last-Modified: Thu, 12 Oct 2023 21:11:25 GMT
< Server: Microsoft-HTTPAPI/2.0
< Hash: 0DFA1329F15FEFA8648856794EB33244
< Date: Fri, 13 Oct 2023 02:24:21 GMT
<
{ [148 bytes data]
* Connection #0 to host 127.0.0.1 left intact
```
```sh
curl -vI "http://127.0.0.1:64587/data.json?name=index.html"
```
```text
HEAD /data.json?name=index.html HTTP/1.1
> Host: 127.0.0.1:64587
> User-Agent: curl/7.75.0
> Accept: */*
>
* Mark bundle as not supporting multiuse
< HTTP/1.1 500 Internal Server Error
< Content-Length: 303
< Content-Type: text/html
< Last-Modified: Thu, 12 Oct 2023 21:11:25 GMT
< Server: Microsoft-HTTPAPI/2.0
< Date: Fri, 13 Oct 2023 02:19:25 GMT
<

```
``` sh
 curl -sv "http://127.0.0.1:64676/data.json?name=data.json&hash=0DFA1329F15FEFA8648856794EB33244"
 ```
 ```text
* Uses proxy env variable no_proxy == '192.168.99.103,192.168.99.100'
*   Trying 127.0.0.1:64676...
* Connected to 127.0.0.1 (127.0.0.1) port 64676 (#0)
> GET /data.json?name=data.json&hash=0DFA1329F15FEFA8648856794EB33244 HTTP/1.1
> Host: 127.0.0.1:64676
> User-Agent: curl/7.75.0
> Accept: */*
>
* Mark bundle as not supporting multiuse
< HTTP/1.1 304 Not Modified
< Content-Length: 0
< Server: Microsoft-HTTPAPI/2.0
< Date: Fri, 13 Oct 2023 02:43:35 GMT
<
* Connection #0 to host 127.0.0.1 left intact
```
```sh
curl -sv "http://127.0.0.1:64676/data.json?name=data.json&hash=0DFA1329F15FEFA8648856794EB33243"
 ```
 ```text
{
  "host1": {
    "netstat": [
      22,
      443,
      3306
    ]
  },
  "host2": {
    "netstat": [
    ]
  },
  "host3": {}
}

* Uses proxy env variable no_proxy == '192.168.99.103,192.168.99.100'
*   Trying 127.0.0.1:64676...
* Connected to 127.0.0.1 (127.0.0.1) port 64676 (#0)
> GET /data.json?name=data.json&hash=0DFA1329F15FEFA8648856794EB33243 HTTP/1.1
> Host: 127.0.0.1:64676
> User-Agent: curl/7.75.0
> Accept: */*
>
* Mark bundle as not supporting multiuse
< HTTP/1.1 200 OK
< Content-Length: 148
< Content-Type: application/json
< Last-Modified: Thu, 12 Oct 2023 22:26:26 GMT
< Server: Microsoft-HTTPAPI/2.0
< Hash: 0DFA1329F15FEFA8648856794EB33244
< Date: Fri, 13 Oct 2023 02:43:38 GMT
<
{ [148 bytes data]
* Connection #0 to host 127.0.0.1 left intact

```

### Powershell Script
* start the server

``` powershell
. .\configserver.ps1 -documentRoot ./test
```
this will print the PORT server listens to
```text
49693
press any key to stop the server
````

* run client with the specific random TCP port logged in the server console

```sh
PORT=49693
curl -sv "http://127.0.0.1:$PORT/data.json?name=data.json"
```
server will print to console
```text
Processing name: data.json
no hash
About to inspect file C:\developer\sergueik\powershell_samples\external\csharp\basic-configserver\./test\data.json
Sending C:\developer\sergueik\powershell_samples\external\csharp\basic-configserver\./test\data.json
About to send 148 bytes
```
the client will print
```text
* processing: http://127.0.0.1:49693/data.json?name=data.json
*   Trying 127.0.0.1:49693...
* Connected to 127.0.0.1 (127.0.0.1) port 49693
> GET /data.json?name=data.json HTTP/1.1
> Host: 127.0.0.1:49693
> User-Agent: curl/8.2.1
> Accept: */*
>
< HTTP/1.1 200 OK
< Content-Length: 148
< Content-Type: application/json
< Last-Modified: Fri, 13 Oct 2023 01:46:14 GMT
< Server: Microsoft-HTTPAPI/2.0
< Hash: 0DFA1329F15FEFA8648856794EB33244
< Date: Fri, 13 Oct 2023 18:29:04 GMT
<
{ [148 bytes data]
* Connection #0 to host 127.0.0.1 left intact
```
* provide hash
```sh
curl -sv "http://127.0.0.1:$PORT/data.json?name=data.json&hash=0DFA1329F15FEFA8648856794EB33244"
```
server console logs:

```text
Processing name: data.json
Processing hash: 0DFA1329F15FEFA8648856794EB33244
About to inspect file C:\developer\sergueik\powershell_samples\external\csharp\basic-configserver\./test\data.json
Unmodified: data.json
```
 client will print

```text
processing: http://127.0.0.1:49693/data.json?name=data.json&hash=0DFA1329F15FEFA8648856794EB33244
* Uses proxy env variable no_proxy == '192.168.99.100'
*   Trying 127.0.0.1:49693...
* Connected to 127.0.0.1 (127.0.0.1) port 49693
> GET /data.json?name=data.json&hash=0DFA1329F15FEFA8648856794EB33244 HTTP/1.1
> Host: 127.0.0.1:49693
> User-Agent: curl/8.2.1
> Accept: */*
>
< HTTP/1.1 304 Not Modified
< Content-Length: 0
< Server: Microsoft-HTTPAPI/2.0
< Date: Fri, 13 Oct 2023 18:31:20 GMT
<
* Connection #0 to host 127.0.0.1 left intact
```
provide an invalid hash
```sh
curl -v "http://localhost:$PORT/xx?name=data.json&hash=0D000000000000000000000000"
```
the server will log
```text
About to inspect file C:\developer\sergueik\powershell_samples\external\csharp\basic-configserver\./test\data.json
Sending C:\developer\sergueik\powershell_samples\external\csharp\basic-configserver\./test\data.json
About to send 148 bytes
```
the client will print
```text
* processing: http://localhost:64646/xx?name=data.json&hash=0DFA1329F15FEFA8648856794EB33243
* Connected to localhost (::1) port 64646
> GET /xx?name=data.json&hash=0DFA1329F15FEFA8648856794EB33243 HTTP/1.1
> Host: localhost:64646
> User-Agent: curl/8.2.1
> Accept: */*
>
< HTTP/1.1 200 OK
< Content-Length: 148
< Content-Type: application/json
< Last-Modified: Fri, 13 Oct 2023 01:46:14 GMT
< Server: Microsoft-HTTPAPI/2.0
< Hash: 0DFA1329F15FEFA8648856794EB33244
< Date: Fri, 13 Oct 2023 07:51:01 GMT
<
[148 bytes data]
{
  "host1": {
    "netstat": [
      22,
      443,
      3306
    ]
  },
  "host2": {
    "netstat": [
    ]
  },
  "host3": {}
}
* Connection #0 to host localhost left intact
```

* provide a non-existing configuration name:
```sh
curl -v "http://localhost:$PORT/xx?name=data2.json"
```
the client will receive

```text
 Connected to localhost (::1) port 49711
> GET /xx?name=data2.json HTTP/1.1
> Host: localhost:49711
> User-Agent: curl/8.2.1
> Accept: */*
>
< HTTP/1.1 404 Not Found
< Transfer-Encoding: chunked
< Server: Microsoft-HTTPAPI/2.0
< Date: Fri, 13 Oct 2023 18:38:31 GMT
<
{ [5 bytes data]

```
#### Note

when run as non-elevated user is throwing the exception: leads to exception with dialog:
```text
powershell has stopped working
A problem caused the program to stop working correctly. 
Windows will close the program and notify you if a solution is available.
```
the eventlog entry `1001` is created but the details are vague and somewhat misleading:

```text
Fault bucket 1418192411905508501, type 5
Event Name: PowerShell
Response: Not available
Cab Id: 0

Problem signature:
P1: powershell.exe
P2: 6.3.9600.20719
P3: System.Net.HttpListenerException
P4: System.Net.HttpListenerException
P5: unknown
P6: System.Net.HttpListener.AddAllPrefixes
P7: unknown
P8: 
P9: 
P10: 

Attached files:
AppData\Local\Temp\WER9B53.tmp.mdmp

These files may be available here:
AppData\Local\Microsoft\Windows\WER\ReportArchive\Critical_powershell.exe_ead0e9118a7347e97390a724209c71653b9efe_00000000_19619eaf

Analysis symbol: 
Rechecking for solution: 0
Report Id: de870101-69d8-11ee-8264-e4f89c86d886
Report Status: 4104
Hashed bucket: f62abcff7b6d1647d3ae6e7c16c84495
```

this is being detected by the script and it quits when run by non-elevated user

### Author


[Serguei Kouzmine](kouzmine_serguei@yahoo.com)

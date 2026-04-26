
### Info

clone of a standalone REST server [project](https://github.com/sachabarber/REST)
intended to evaluate tough experience of port it to .NET Core

### Usage

this is a simple REST server running on the .Net Framework, the client app is included, the sample calls
```sh
curl -s http://localhost:8001/people/1
```
return XML data
```xml
<?xml version="1.0"?>
<Person xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Id>1</Id>
  <FirstName>Sam</FirstName>
  <LastName>Beird</LastName>
</Person>

```
and in JSON
```sh
curl -s http://localhost:8001/accounts -X GET -H "content-type: application/json"
```
```json

[
  {
    "Id": 1,
    "AccountNumber": "11558836",
    "SortCode": "11-22-44"
  },
  {
    "Id": 2,
    "AccountNumber": "12345678",
    "SortCode": "22-88-78"
  }
]

```
```sh
curl -s http://localhost:8001/accounts/2 -X GET -H "content-type: application/json"
```

```JSON
{
  "Id": 2,
  "AccountNumber": "12345678",
  "SortCode": "22-88-78"
}
```

it is also features an RPC call-style endpoint example
```sh
curl -s http://localhost:8001/users/GetUserByTheirId/1
```
```json
{"Id":1,"UserName":"Charles Bensi"}

```
### Running in Wine

```sh
# wine ./RESTServerConsoleHost.exe
```
```text
error: XDG_RUNTIME_DIR not set in the environment.

Unhandled Exception: System.Net.HttpListenerException: Path not found
   at System.Net.HttpListener.CreateRequestQueueHandle()
   at System.Net.HttpListener.Start()
   at RESTServer.HttpServer.<Run>d__0.MoveNext() in c:\developer\sergueik\powers
hell_samples\external\csharp\standalone-restserver\RESTServer\HttpServer.cs:line
 39
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.AsyncMethodBuilderCore.<ThrowAsync>b__1(Ob
ject state)
   at System.Threading.QueueUserWorkItemCallback.WaitCallback_Context(Object sta
te)
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionCo
ntext, ContextCallback callback, Object state, Boolean preserveSyncCtx)
   at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, C
ontextCallback callback, Object state, Boolean preserveSyncCtx)
   at System.Threading.QueueUserWorkItemCallback.System.Threading.IThreadPoolWor
kItem.ExecuteWorkItem()
   at System.Threading.ThreadPoolWorkQueue.Dispatch()
   at System.Threading._ThreadPoolWaitCallback.PerformWaitCallback

```
### See Also

  * https://www.codeproject.com/Articles/1190475/Porting-a-NET-Framework-Library-to-NET-Core
  * https://www.codeproject.com/Articles/1126303/Introduction-to-NET-Core

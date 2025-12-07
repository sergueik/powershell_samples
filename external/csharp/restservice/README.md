### Info


[replica os stanalone Rest Service C#](https://github.com/eccosuprastyo/dotnet-csharp/tree/master/restservice)

documented in [Rest API C# Console Application](https://www.c-sharpcorner.com/article/rest-api-csharp-console-application/), including how to 
configure application using using FILE attribute of APPSETTINGS.

### Usage

* configure the system
Without netsh configuration, running the service requires switching to `runas` elevated account and running the command:
```cmd
netsh.exe http delete urlacl url=http://+:9001/
netsh.exe http add urlacl url=http://+:9001/ user=%COMPUTERNAME%\%USERNAME%
```
this will respond with
```sh
URL reservation successfully added
```

NOTE: there is no `/login` url in the `netsh` command since the WCF-style `app.config` is binding the endpoint to the `baseAddress="http://localhost:9001"`


* test `GET` request
```sh
USER=user
PASSWORD=pass
curl -X GET http://localhost:9001/login/$USER/$PASSWORD
```

will return sample JSON:

```json
{
  "Data": {
    "Point": {
      "X":1.2,
      "Y":3.4
    }
  }  
}
```
* test `POST` request
```sh
curl -s -X POST http://localhost:9001/postdata -d "Lorem ipsum dolor sit"
```
will respond with posted data body wrapped in XML:
```xml
<string xmlns="http://schemas.microsoft.com/2003/10/Serialization/">Lorem ipsum dolor sit</string>
```
### Work in Progress

Removing `App.config` from the project and replacing with configuation-through-code. This is not finished:

* fixed error  observed after removing the `App.config`:
```text
System.InvalidOperationException: Could not find a base address that matches scheme http for the endpoint with binding WebHttpBinding.
```
* the Appication is behaving as WCF service not REST service:

requesting the page `http://localhost:9001/login/camellabs/camellabs` results in the following:
```text
The message with To 'http://localhost:9001/login/camellabs/camellabs' cannot be processed at the receiver, due to an 
AddressFilter mismatch at the EndpointDispatcher. Check that the sender and receiver's EndpointAddresses agree.
```
### See Also

 * [Configuring HTTP and HTTPS](https://docs.microsoft.com/en-us/dotnet/framework/wcf/feature-details/configuring-http-and-https?redirectedfrom=MSDN)
 * [to store and retrieve custom information from an application configuration file](https://docs.microsoft.com/en-us/troubleshoot/dotnet/csharp/store-custom-information-config-file)
 * [configure Windows Scheduled Task scehdule to every minute](https://lazywinadmin.com/2012/03/run-this-task-every-minute.html) 
 * [Constructing](https://stackoverflow.com/questions/25450902/host-wcf-service-without-config-file) WCF service instance without `App.config`. Note: the .net assembly `System.Configuration.ConfigurationManager.dll`
[referenced](https://stackoverflow.com/questions/58982297/what-is-the-best-way-to-update-an-app-config-file-for-c-sharp-in-visual-studio)  in a similar thread is found nowhere on a vanilly Windows system, maybe installed as pat of SDK.
 * [nancy](http://nancyfx.org) REST server
 * article on [WCF as REST Service](https://www.codeproject.com/Articles/344512/Building-and-Testing-WCF-RESTful-services) with detailed explanation of entpoints
 * article on [WCF POST,GET annotations](https://www.c-sharpcorner.com/UploadFile/surya_bg2000/developing-wcf-restful-services-with-get-and-post-methods)



### Author
[Serguei Kouzmine](mailto:kouzmine_serguei@yahoo.com)

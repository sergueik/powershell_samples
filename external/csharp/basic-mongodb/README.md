### Info



a replica of [simple generic MongoDb repository (Driver 2.0.1) for C# .NEt Framewrk 4.5](https://github.com/Talento90/repository.mongodb.net). 
converted from `Microsoft.VisualStudio.TestTools.UnitTesting` to `Nunit.Framework` (some tests could be failing because the exception was not translated to new `Nunit` annotation recommendation:
```c#
[ExpectedException(typeof(ArgumentNullException))]
```
the attempt to use recommended pattetn fails the SharpDevelop compiler:
```c#
var ex = Assert.Throws<ArgumentNullException>(() => {
    	await this._prodRepository.Update(null); 
	});
```
### Usage

* run the mongodb in Docker or locally:

```sh
CONTAINER ID        IMAGE               COMMAND                  CREATED
     STATUS              PORTS                                 NAMES
2c5e9b9ca161        mongodb             "./run.sh mongod --b"   7 days ago
    Up 15 minutes       0.0.0.0:27017->27017/tcp, 28017/tcp   mongo

```
* modify the `Core.Repository.MongoDb.Tests/App.config`	 with the address mongodb can be connected through:
```XML
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <connectionStrings>
    <add name="MongoDb.ConnectionString" connectionString="mongodb://192.168.99.100:27017/ProductsTest"/>
  </connectionStrings>
<startup><supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5.1"/></startup></configuration>
```
* run tests in IDE

This project demonstrates basic CRUD methods (Get, Delete, Update, Insert, GetAll, Pagination) 

### Note

MongoDB c# projects exhibit high fragility.
### See Also
  * https://docs.nunit.org/2.4.8/exception.html
  * https://stackoverflow.com/questions/15014461/use-nunit-assert-throws-method-or-expectedexception-attribute
### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)

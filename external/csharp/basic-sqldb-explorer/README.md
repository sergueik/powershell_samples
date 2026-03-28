### Info

Replica  of [SQLDatabase.Net.Explorer](https://github.com/MenNoWar/SQLDatabase.Net.Explorer) a simple gui for `SQLDatabase.Net`

### Background
The `SQLDatabase.Net` package is an embedded SQL database provided as a single DLL that requires zero configuration
### NOTE

The [project website](https://sqldatabase.net/) is HTTP Error 503 down

### Usage
> NOTE Nuget __2.6.4__ is unable to restore package on its own

```sh
curl -skLo ~/Downloads/sqldatabase.net.2.0.1.nupkg https://www.nuget.org/api/v2/package/SQLDatabase.Net/2.0.1
```
```sh
unzip -ql ~/Downloads/sqldatabase.net.2.0.1.nupkg | grep dll
```
```text
  1145856  2017-10-23 20:44   lib/net40-client/SQLDatabase.Net.dll
  1145856  2017-10-23 20:44   lib/net45/SQLDatabase.Net.dll
  1145856  2017-10-23 20:44   lib/net46/SQLDatabase.Net.dll
```
```sh
unzip -x ~/Downloads/sqldatabase.net.2.0.1.nupkg  lib/net45/SQLDatabase.Net.dll -d packages
```
```text
Archive:  /c/Users/kouzm/Downloads/sqldatabase.net.2.0.1.nupkg
  inflating: packages/lib/net45/SQLDatabase.Net.dll
```

update dependencies in `Project.csproj`, `Utils.csproj`


![Explorer](screenshots/capture-sqldb-explorer.png)

### See Also

  * [SQLDatabaseServerClient](https://github.com/sqldatabase/SQLDatabaseServerClient) net 4.5 client library code to access database and cache server.
  * `CSVFile.cs` [source](https://github.com/sqldatabase/embedded-dotnetcore20/blob/master/SQLDatabase.Net.Core.Examples/SQLDatabase.Net.Core.Examples/CSVFile.cs)

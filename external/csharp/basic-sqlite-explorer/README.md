### Info
Replica  of [SQLDatabase.Net.Explorer](https://github.com/MenNoWar/SQLDatabase.Net.Explorer) a simple gui for SQLDatabase.Net.Explorer

### Usage

curl -skLo ~/Downloads/sqldatabase.net.2.0.1.nupkg https://www.nuget.org/api/v2/package/SQLDatabase.Net/2.0.1

kouzm@sergueik23 MINGW64 /c/developer/sergueik/powershell_samples/external/csharp/basic-sqlite-explorer (master)
$ unzip -ql ~/Downloads/sqldatabase.net.2.0.1.nupkg | grep dll
  1145856  2017-10-23 20:44   lib/net40-client/SQLDatabase.Net.dll
  1145856  2017-10-23 20:44   lib/net45/SQLDatabase.Net.dll
  1145856  2017-10-23 20:44   lib/net46/SQLDatabase.Net.dll

unzip -x ~/Downloads/sqldatabase.net.2.0.1.nupkg  lib/net45/SQLDatabase.Net.dll -d packages
Archive:  /c/Users/kouzm/Downloads/sqldatabase.net.2.0.1.nupkg
  inflating: packages/lib/net45/SQLDatabase.Net.dll


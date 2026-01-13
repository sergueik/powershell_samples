### Info

Replica of the __Embed an Assembly as a Resource__ [project](https://www.codeproject.com/Articles/56197/Embed-an-Assembly-as-a-Resource) hosting the proxy to load the dependency assembly from application class resource for simplifying the deployment. NOTE: no conversion is required when storing the binary inside the Resources directory:

> Note: no need to change the file extension

```sh
VERSION=3.2.0
curl -skLo ~/Downloads/log4net.$VERSION.nupkg https://www.nuget.org/api/v2/package/log4net/$VERSION
```
examine the targets to find the vintage platform version
```sh
VERSION=3.2.0
unzip -l ~/Downloads/log4net.$VERSION.nupkg | grep -i log4net.dll | grep -i net4
```
```text
VERSION=3.2.0
unzip -qjo ~/Downloads/log4net.$VERSION.nupkg lib/net462/log4net.dll
cp log4net.dll Program/Resources
```
```powershell
get-item .\log4net.dll | select-object -expandproperty VersionInfo).FileVersion
```
### See Also

 * [working with embedded resources in Project's assembly](https://weblogs.asp.net/hajan/working-with-embedded-resources-in-assembly)
 * `log4net` [nuget](https://www.nuget.org/packages/log4net)

---
### Author

[Serguei Kouzmine](mailto:kouzmine_serguei@yahoo.com)

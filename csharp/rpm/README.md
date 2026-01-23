### Info

 tail log lines counter Service example

### Usage

```powershell
$env:path="${env:path};c:\Windows\Microsoft.NET\Framework\v4.0.30319"
msbuild.exe Program\Program.csproj
```
```powershell
.\Program\bin\Debug\Util.exe .\data.txt 1000
```
```text
data.txt
Length: 2161 / 2161
Rpm: 33 / 33
```
```powershell
Program\bin\Debug\Util.exe data.txt 10 lorem
```
```text
data.txt
Length: 2251 / 2251
Rpm: 35 / 35
Rpm(lorem): 4

```
### See Also:

* [continuously read tail file with C#](https://stackoverflow.com/questions/3791103/continuously-read-file-with-c-sharp)

  
### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)

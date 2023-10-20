### Info
Code from __C# (.NET) Interface for 7-Zip Archive DLLs__
[codeproject article](https://www.codeproject.com/Articles/27148/C-NET-Interface-for-7-Zip-Archive-DLLs)

### Upgrading 7-zip Dependency

Need to download [32-bit Windows x86 release of 7-zip](https://www.7-zip.org/a/7z2201.exe)
After installling, copy into bin directory. E.g. on 32-bit Windows machine:
```cmd
cd bin\Debug
copy "c:\Program Files\7-Zip\7z.dll" .
```

run tests
```cmd
.\SevenZip.exe l test.zip
```
```text
Archive: test.zip
List:
5 items.
0 7z.dll
1 SevenZip.exe
2 SevenZip.exe.config
3 SevenZip.pdb
4 test.tar
```
```cmd
.\SevenZip.exe e test.zip 3
```
```text
Archive: test.zip
Extracting: SevenZip.pdb kOK
```
### See Also

* https://www.codeproject.com/Articles/209731/Csharp-use-Zip-archives-without-external-libraries



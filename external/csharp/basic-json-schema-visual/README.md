### Info
replica of 
[Visual JSON Editor](https://github.com/RicoSuter/VisualJsonEditor) by [Rico Suter](http://rsuter.com) 


a JSON schema based file editor for Windows 
it uses a [C# implementation](https://github.com/RicoSuter/NJsonSchema) of JSON Schema named [NJsonSchema](http://njsonschema.org/) as key dependency

The project also uses subtle extension libraries [](https://www.nuget.org/api/v2/package/Namotion.Reflection/2.0.5)



### Usage 
```sh
export VERSION=10.5.0
curl -skLo ~/Downloads/njsonschema.zip https://www.nuget.org/api/v2/package/NJsonSchema/$VERSION
```
```sh
unzip -ql ~/Downloads/njsonschema.zip
mkdir -p packages/NJsonSchema.10.5.2/lib/net45
unzip -xj -d packages/NJsonSchema.10.5.2/lib/net45  ~/Downloads/njsonschema.zip lib/net45/*
```

```sh
curl -skLo ~/Downloads/namotion.zip https://www.nuget.org/api/v2/package/Namotion.Reflection/2.0.5

```

```
mkdir -p packages/Namotion.Reflection.2.0.5/lib/net45
unzip -xj -d packages/Namotion.Reflection.2.0.5/lib/net45 ~/Downloads/namotion.zip lib/net45/*
```
When opening a JSON file, the application auto-generates an editor GUI based on the provided JSON schema. The goal is to make JSON authoring more effective and easier

### Troubleshooting

Task could not find "AL.exe" using the SdkToolsPath ""
or the registry key "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft SDKs\Windows\v8.0A\WinSDK-NetFx40Tools-x86".

Make sure the SdkToolsPath is set and the tool exists in the correct processor specific location under the SdkToolsPath and that the Microsoft Windows SDK is installed (MSB3086)

Al.exe, or the Assembly Linker, is a command-line tool included with the .NET Framework that generates files with an assembly manifest from .NET modules or resource files (.resources). It is primarily used to create satellite assemblies (for localization), manage strong-named assemblies, and combine multiple modules into a single DLL or EXE.



### Info


### Tesrting
ignore the error
```text
Unable to find version '3.4.1' of package 'Google_GenerativeAI'.
Exited with code: 1
```

```sh
curl -skLo  ~/Downloads/google_generativeai.3.4.1.nupkg https://www.nuget.org/api/v2/package/Google_GenerativeAI/3.4.1
```
```sh
unzip -l ~/Downloads/google_generativeai.3.4.1.nupkg
```
```text
Archive:  /c/Users/kouzm/Downloads/google_generativeai.3.4.1.nupkg
  Length      Date    Time    Name
---------  ---------- -----   ----
      509  2025-11-08 09:19   _rels/.rels
     3362  2025-11-08 09:19   Google_GenerativeAI.nuspec
  3049984  2025-11-08 09:19   lib/net462/GenerativeAI.dll
  1060568  2025-11-08 09:19   lib/net462/GenerativeAI.xml
  2720256  2025-11-08 09:18   lib/net6.0/GenerativeAI.dll
  1063542  2025-11-08 09:18   lib/net6.0/GenerativeAI.xml
  2718208  2025-11-08 09:18   lib/net7.0/GenerativeAI.dll
  1063542  2025-11-08 09:18   lib/net7.0/GenerativeAI.xml
  3055104  2025-11-08 09:19   lib/net8.0/GenerativeAI.dll
  1063542  2025-11-08 09:18   lib/net8.0/GenerativeAI.xml
  3055104  2025-11-08 09:19   lib/net9.0/GenerativeAI.dll
  1063542  2025-11-08 09:19   lib/net9.0/GenerativeAI.xml
  3051008  2025-11-08 09:18   lib/netstandard2.0/GenerativeAI.dll
  1060568  2025-11-08 09:18   lib/netstandard2.0/GenerativeAI.xml
    34603  2025-11-08 09:17   README.md
   362871  2025-11-08 09:17   favicon.png
      644  2025-11-08 09:19   [Content_Types].xml
      909  2025-11-08 09:19   package/services/metadata/core-properties/c031ebb05d0841a48cd51ea0e31768fe.psmdcp
    12982  2025-11-08 01:20   .signature.p7s
---------                     -------
 24440848                     19 files


```

```sh
unzip -x ~/Downloads/google_generativeai.3.4.1.nupkg lib/net462/GenerativeAI.dll
mkdir packages/GenerativeAI3.4.1
mv lib packages/GenerativeAI3.4.1

```
repeat with `Microsoft.Extensions.Logging.Abstractions`
```sh
curl -skLo ~/Downloads/Microsoft.Extensions.Logging.Abstractions.9.0.3.nupkg https://www.nuget.org/api/v2/package/Microsoft.Extensions.Logging.Abstractions/9.0.3
```

```sh
unzip -l ~/Downloads/Microsoft.Extensions.Logging.Abstractions.9.0.3.nupkg
```
```text
Length      Date    Time    Name
---------  ---------- -----   ----
      535  2025-02-11 23:23   _rels/.rels
     2692  2025-02-11 23:23   Microsoft.Extensions.Logging.Abstractions.nuspec
    69416  2025-02-11 23:20   lib/net462/Microsoft.Extensions.Logging.Abstractions.dll
    99749  2025-02-11 23:20   lib/net462/Microsoft.Extensions.Logging.Abstractions.xml
    66320  2025-02-11 23:18   lib/net8.0/Microsoft.Extensions.Logging.Abstractions.dll
    88118  2025-02-11 23:18   lib/net8.0/Microsoft.Extensions.Logging.Abstractions.xml
    67352  2025-02-11 23:11   lib/net9.0/Microsoft.Extensions.Logging.Abstractions.dll
    88118  2025-02-11 23:11   lib/net9.0/Microsoft.Extensions.Logging.Abstractions.xml
    69408  2025-02-11 23:20   lib/netstandard2.0/Microsoft.Extensions.Logging.Abstractions.dll
    99749  2025-02-11 23:20   lib/netstandard2.0/Microsoft.Extensions.Logging.Abstractions.xml
    21784  2025-02-11 23:10   analyzers/dotnet/roslyn3.11/cs/cs/Microsoft.Extensions.Logging.Generators.resources.dll
    22280  2025-02-11 23:10   analyzers/dotnet/roslyn3.11/cs/de/Microsoft.Extensions.Logging.Generators.resources.dll
    21768  2025-02-11 23:10   analyzers/dotnet/roslyn3.11/cs/es/Microsoft.Extensions.Logging.Generators.resources.dll
    22280  2025-02-11 23:10   analyzers/dotnet/roslyn3.11/cs/fr/Microsoft.Extensions.Logging.Generators.resources.dll
    21792  2025-02-11 23:10   analyzers/dotnet/roslyn3.11/cs/it/Microsoft.Extensions.Logging.Generators.resources.dll
    22312  2025-02-11 23:10   analyzers/dotnet/roslyn3.11/cs/ja/Microsoft.Extensions.Logging.Generators.resources.dll
    21776  2025-02-11 23:10   analyzers/dotnet/roslyn3.11/cs/ko/Microsoft.Extensions.Logging.Generators.resources.dll
    21768  2025-02-11 23:10   analyzers/dotnet/roslyn3.11/cs/pl/Microsoft.Extensions.Logging.Generators.resources.dll
    21800  2025-02-11 23:10   analyzers/dotnet/roslyn3.11/cs/pt-BR/Microsoft.Extensions.Logging.Generators.resources.dll
    23832  2025-02-11 23:10   analyzers/dotnet/roslyn3.11/cs/ru/Microsoft.Extensions.Logging.Generators.resources.dll
    21768  2025-02-11 23:10   analyzers/dotnet/roslyn3.11/cs/tr/Microsoft.Extensions.Logging.Generators.resources.dll
    20744  2025-02-11 23:10   analyzers/dotnet/roslyn3.11/cs/zh-Hans/Microsoft.Extensions.Logging.Generators.resources.dll
    20744  2025-02-11 23:10   analyzers/dotnet/roslyn3.11/cs/zh-Hant/Microsoft.Extensions.Logging.Generators.resources.dll
    75024  2025-02-11 23:10   analyzers/dotnet/roslyn3.11/cs/Microsoft.Extensions.Logging.Generators.dll
    21784  2025-02-11 23:10   analyzers/dotnet/roslyn4.0/cs/cs/Microsoft.Extensions.Logging.Generators.resources.dll
    22280  2025-02-11 23:10   analyzers/dotnet/roslyn4.0/cs/de/Microsoft.Extensions.Logging.Generators.resources.dll
    21768  2025-02-11 23:10   analyzers/dotnet/roslyn4.0/cs/es/Microsoft.Extensions.Logging.Generators.resources.dll
    22280  2025-02-11 23:10   analyzers/dotnet/roslyn4.0/cs/fr/Microsoft.Extensions.Logging.Generators.resources.dll
    21792  2025-02-11 23:10   analyzers/dotnet/roslyn4.0/cs/it/Microsoft.Extensions.Logging.Generators.resources.dll
    22312  2025-02-11 23:10   analyzers/dotnet/roslyn4.0/cs/ja/Microsoft.Extensions.Logging.Generators.resources.dll
    21776  2025-02-11 23:10   analyzers/dotnet/roslyn4.0/cs/ko/Microsoft.Extensions.Logging.Generators.resources.dll
    21768  2025-02-11 23:10   analyzers/dotnet/roslyn4.0/cs/pl/Microsoft.Extensions.Logging.Generators.resources.dll
    21800  2025-02-11 23:10   analyzers/dotnet/roslyn4.0/cs/pt-BR/Microsoft.Extensions.Logging.Generators.resources.dll
    23832  2025-02-11 23:10   analyzers/dotnet/roslyn4.0/cs/ru/Microsoft.Extensions.Logging.Generators.resources.dll
    21768  2025-02-11 23:10   analyzers/dotnet/roslyn4.0/cs/tr/Microsoft.Extensions.Logging.Generators.resources.dll
    20744  2025-02-11 23:10   analyzers/dotnet/roslyn4.0/cs/zh-Hans/Microsoft.Extensions.Logging.Generators.resources.dll
    20744  2025-02-11 23:10   analyzers/dotnet/roslyn4.0/cs/zh-Hant/Microsoft.Extensions.Logging.Generators.resources.dll
    93448  2025-02-11 23:10   analyzers/dotnet/roslyn4.0/cs/Microsoft.Extensions.Logging.Generators.dll
    21784  2025-02-11 23:10   analyzers/dotnet/roslyn4.4/cs/cs/Microsoft.Extensions.Logging.Generators.resources.dll
    22280  2025-02-11 23:10   analyzers/dotnet/roslyn4.4/cs/de/Microsoft.Extensions.Logging.Generators.resources.dll
    21768  2025-02-11 23:10   analyzers/dotnet/roslyn4.4/cs/es/Microsoft.Extensions.Logging.Generators.resources.dll
    22280  2025-02-11 23:10   analyzers/dotnet/roslyn4.4/cs/fr/Microsoft.Extensions.Logging.Generators.resources.dll
    21792  2025-02-11 23:10   analyzers/dotnet/roslyn4.4/cs/it/Microsoft.Extensions.Logging.Generators.resources.dll
    22312  2025-02-11 23:10   analyzers/dotnet/roslyn4.4/cs/ja/Microsoft.Extensions.Logging.Generators.resources.dll
    21776  2025-02-11 23:10   analyzers/dotnet/roslyn4.4/cs/ko/Microsoft.Extensions.Logging.Generators.resources.dll
    21768  2025-02-11 23:10   analyzers/dotnet/roslyn4.4/cs/pl/Microsoft.Extensions.Logging.Generators.resources.dll
    21800  2025-02-11 23:10   analyzers/dotnet/roslyn4.4/cs/pt-BR/Microsoft.Extensions.Logging.Generators.resources.dll
    23832  2025-02-11 23:10   analyzers/dotnet/roslyn4.4/cs/ru/Microsoft.Extensions.Logging.Generators.resources.dll
    21768  2025-02-11 23:10   analyzers/dotnet/roslyn4.4/cs/tr/Microsoft.Extensions.Logging.Generators.resources.dll
    20744  2025-02-11 23:10   analyzers/dotnet/roslyn4.4/cs/zh-Hans/Microsoft.Extensions.Logging.Generators.resources.dll
    20744  2025-02-11 23:10   analyzers/dotnet/roslyn4.4/cs/zh-Hant/Microsoft.Extensions.Logging.Generators.resources.dll
    78096  2025-02-11 23:10   analyzers/dotnet/roslyn4.4/cs/Microsoft.Extensions.Logging.Generators.dll
     1507  2025-02-11 23:21   buildTransitive/netstandard2.0/Microsoft.Extensions.Logging.Abstractions.targets
     1507  2025-02-11 23:21   buildTransitive/net8.0/Microsoft.Extensions.Logging.Abstractions.targets
     1507  2025-02-11 23:21   buildTransitive/net462/Microsoft.Extensions.Logging.Abstractions.targets
     2140  2025-01-27 22:48   Icon.png
        0  2025-02-11 22:58   useSharedDesignerContext.txt
     5715  2025-02-11 22:58   PACKAGE.md
     1139  2025-02-11 22:58   LICENSE.TXT
    75640  2025-02-11 22:58   THIRD-PARTY-NOTICES.TXT
      697  2025-02-11 23:21   buildTransitive/netcoreapp2.0/Microsoft.Extensions.Logging.Abstractions.targets
      697  2025-02-11 23:21   buildTransitive/net461/Microsoft.Extensions.Logging.Abstractions.targets
      783  2025-02-11 23:23   [Content_Types].xml
     1047  2025-02-11 23:23   package/services/metadata/core-properties/2888e5582c5a409e9f696b10f7bf4039.psmdcp
    25625  2025-03-11 09:32   .signature.p7s
---------                     -------
  1869973                     65 files

```
```sh
unzip -x ~/Downloads/Microsoft.Extensions.Logging.Abstractions.9.0.3.nupkg lib/net462/Microsoft.Extensions.Logging.Abstractions.dll
```

```sh
mkdir -p packages/Microsoft.Extensions.Logging.Abstractions.9.0.3
mv lib packages/Microsoft.Extensions.Logging.Abstractions.9.0.3
```

```sh
curl -skLo  ~/Downloads/Microsoft.Bcl.AsyncInterfaces.9.0.3 https://www.nuget.org/api/v2/package/Microsoft.Bcl.AsyncInterfaces/9.0.3
unzip -l ~/Downloads/Microsoft.Bcl.AsyncInterfaces.9.0.3.nupkg |grep lib/net462
```
```text
unzip -x ~/Downloads/Microsoft.Bcl.AsyncInterfaces.9.0.3.nupkg lib/net462/Microsoft.Bcl.AsyncInterfaces.dll
```

```text
Archive:  /c/Users/kouzm/Downloads/Microsoft.Bcl.AsyncInterfaces.9.0.3.nupkg
  inflating: lib/net462/Microsoft.Bcl.AsyncInterfaces.dll
```
```sh
mkdir -p packages/Microsoft.Bcl.AsyncInterfaces.9.0.3
```

```sh
mv lib/ packages/Microsoft.Bcl.AsyncInterfaces.9.0.3
```
```text
System.ArgumentException: API Key is required for Google Gemini AI.
```
- set the API key
### See Also

 * `ClipGen` [article #1](https://habr.com/ru/articles/891246) (in Russian) and [article #2](https://habr.com/ru/articles/974706/) (in Russian) and [telegram download](https://t.me/VETA14/14) and [repository](https://github.com/Veta-one/clipgen) - AI-powered clipboard enhancement utility with hotkeys for instant text correction, translation, rewriting, and image analysis using Google Gemini API. NOTE: in Python, requires `PyQT5` dependency for UI

 *  [post](https://cloud.google.com/blog/topics/developers-practitioners/introducing-google-gen-ai-net-sdk) introduces the new Google Gen AI .NET SDK [repository](https://github.com/googleapis/dotnet-genai/) and [API Reference](https://googleapis.github.io/dotnet-genai/api/index.html) and [nuget](https://www.nuget.org/packages/Google.GenAI), enabling C#/.NET developers to use Gemini from Google AI that requires __.NET__ __8.0__ and __netstandard2.1__ - no support for .Net Framework
 * `Google_GenerativeAI` [nuget package](https://www.nuget.org/packages/Google_GenerativeAI/) that suports .Net Framework and [source repository](https://github.com/gunpal5/Google_GenerativeAI) -  most complete C# .Net SDK for Google Generative AI and Vertex AI (Google Gemini.
 * [WSH Clipboard access](https://www.codeproject.com/articles/WSH-Clipboard-Access)
 * [TalkingClipboard](https://www.codeproject.com/articles/Talking-Clipboard)
 * [Clipboard little helper](https://www.codeproject.com/articles/Clipboard-little-helper)
 * [Windows Clipboard Formats](https://www.codeproject.com/articles/Windows-Clipboard-Formats)
  * [Clipboard handling with .NET](https://www.codeproject.com/articles/Clipboard-handling-with-NET)
  * [Clipboard backup in C# using WWindows API](https://www.codeproject.com/articles/Clipboard-backup-in-C-)

---
### Author


[Serguei Kouzmine](mailto:kouzmine_serguei@yahoo.com)

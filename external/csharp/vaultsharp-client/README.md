### Info

This directory contains the code from the  [samples](https://github.com/rajanadar/VaultSharp/tree/master/test/VaultSharp.Samples)
directory
of the [VaultSharp](https://github.com/rajanadar/VaultSharp) project

   - a cross-platform .NET Library for HashiCorp's Vault [repo](https://github.com/rajanadar/VaultSharp) for accessing Vault from .Net 4.5 and later. Includes [sample test console app](https://github.com/rajanadar/VaultSharp/blob/master/test/VaultSharp.Samples/HowToRunThisTestProgram.md) and the nuget package is available on [nuget](https://www.nuget.org/packages/VaultSharp) with the versions supporting .Net Framework __4.5__ are __1.4.0__ to __1.7.1__ (may be critical for Powershell invoker) and the latest version supporting all kinds of .Net including .Net Framwork __4.6.2__ is __1.13.0__

   The removal of __4.5__ support  was tagged a Breaking Change  by project author

NOTE: to workaround the error
```text
   'VaultSharp' already has a dependency defined for 'Newtonsoft.Json'.
```
just download direcly and unzip into `packages/Vault.1.6.0`
### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)

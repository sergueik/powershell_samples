### Info


this directory contains replica with code fixes of the __C# AES 256 bits Encryption Library with Salt__
[project](https://www.codeproject.com/Articles/769741/Csharp-AES-bits-Encryption-Library-with-Salt)
with the achieved compatibility with Perl [Crypt::PBE](https://metacpan.org/pod/Crypt::PBE) and intended goal of compatibility 
and Java [Jasypt](http://www.jasypt.org/) using AES/SHA512.

NOTE, Jasypt CLI
 is currently [broken](https://github.com/jasypt/jasypt/issues/122) 
in selecting PBE/AES hashing / encryption, but default is compatble

The application has no external dependencies - the `System.Security.Cryptography` is available in the system GAC.

### Usage

```powershell
.\Program\bin\Debug\Program.exe -value:text -password=password -debug -strong
```
this will print to console

```text
debug: true
password: password
value: text
use SHA512: True
salt: 4CD04A92ABBD40777CAE3C556F0FC914
key: 70F734313DAA13C534E19EC90BCEACAD1DEED08EB595956EAC836607AED7FD39
iv: B18BB7E5D5193B66BE38B3BFDE449A81
data: 695ACD3E560F7F913816D6A90118EBD3
salt: 4CD04A92ABBD40777CAE3C556F0FC914
iv: B18BB7E5D5193B66BE38B3BFDE449A81
result: 4CD04A92ABBD40777CAE3C556F0FC914B18BB7E5D5193B66BE38B3BFDE449A81695ACD3E
560F7F913816D6A90118EBD3
encrypted: TNBKkqu9QHd8rjxVbw/JFLGLt+XVGTtmvjizv95EmoFpWs0+Vg9/kTgW1qkBGOvT
encrypted: TNBKkqu9QHd8rjxVbw/JFLGLt+XVGTtmvjizv95EmoFpWs0+Vg9/kTgW1qkBGOvT

```
the reversal

```powershell
.\Program\bin\Debug\Program.exe -operation:decrypt -value:TNBKkqu9QHd8rjxVbw/JFLGLt+XVGTtmvjizv95EmoFpWs0+Vg9/kTgW1qkBGOvT -password=password -debug -strong
```
will print to console
```text
debug: true
password: password
value: TNBKkqu9QHd8rjxVbw/JFLGLt+XVGTtmvjizv95EmoFpWs0+Vg9/kTgW1qkBGOvT
use SHA512: True
reading: 48 bytes of payload
reading: 16 bytes of data
salt: 4CD04A92ABBD40777CAE3C556F0FC914
iv: B18BB7E5D5193B66BE38B3BFDE449A81
data: 695ACD3E560F7F913816D6A90118EBD3
key: 70F734313DAA13C534E19EC90BCEACAD1DEED08EB595956EAC836607AED7FD39
iv: B18BB7E5D5193B66BE38B3BFDE449A81
decrypted: text
```

* NOTE, the `iv`, `salt`, and `key` are logged to be identical during `encrypt` and `decrypt` operations

### Perl / Java / C# compatible Encryption Demo

From a long list of Perl-supported  `Crypt::PBE` methods - the `PBEWithHmacSHA1AndAES_256` will work without `-strong` flag for .Net app:

```Perl
GetOptions('value=s' => \$value, 'password=s' => \$password, 'debug' => \$debug, 'operation=s' => \$operation);

$pbe =  PBEWithHmacSHA1AndAES_256($password, undef, $debug);
print "password: $password$/";
$encrypted = decode_base64($value);
print $value, $/;
$encrypted = $pbe->encrypt( $value, 1000 );
print encode_base64($encrypted), $/;
print 'Decrypting' , $/;
print $pbe->decrypt($encrypted),
```
the `Rfc2898DeriveBytes` used in this project [operates ](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.rfc2898derivebytes?view=netframework-4.5) using `HMACSHA1` hash function
* encrypt some text on Perl side:

 + build [image](https://github.com/sergueik/springboot_study/tree/master/basic-jasypt-perl):

```sh
IMAGE=basic-perl-crypt-jasypt
docker build -t $IMAGE -f Dockerfile .
```
 + run the container
```sh
NAME=example-perl-jasypt
docker container rm $NAME
docker run --name $NAME -it $IMAGE sh
```
 + in the container:
```sh
perl jasypt.pl  -debug -value 'example message' -password secret
```
```text
password: secret
value: example message
Encrypting
cODQYjzrdclKP1B7QBkNhDczYlFu9aNZANKDFcE0Vl5vcPKmCmNJofDYt4Xhpswy

Decrypting
example message
```
NOTE: the encrypted value will be different in every run since the derived key will be generated based on password and salt.
Feed just produced value to the .net app:

* decript it in .net
```sh
Program\bin\Debug\Program.exe -operation=decrypt -password=secret -value=Q7cRD4PDHSWJMXbvP+K3J/9aDxl5UEjCjOXU++GI8SVIW7lO8ijHJ5LuedHgZAiq
```
```text
example message
```

![tests](https://github.com/sergueik/powershell_samples/blob/master/csharp/pbkdf2-csharp/screenshots/capture-tests.png)

### Embedded In Powershell

```powershell
. .\pbkdf2.ps1 -value 'hello,world' -password secret -operation encrypt
```
```text
WuoVU9SiBMVRcAl2WzZn9hSQqMh97bmiGzl9wyyvrygXZk4JmmQBErZMQX95dpo7
```


```powershell
. .\pbkdf2.ps1 -value 'WuoVU9SiBMVRcAl2WzZn9hSQqMh97bmiGzl9wyyvrygXZk4JmmQBErZMQX95dpo7' -password secret -operation decrypt
```

```text
hello,world
```



```powershell
. .\pbkdf2.ps1 -value 'hello, world of AES' -password secret -operation encrypt -strong
```
```text
GD2UA8MKCvCkL+qr/QTvGl76V1G5S5A2oXPUDVdD0qF70esUEuAi17/sGn8N5aHrUIWwSZbToe2p/IXSNojt/Q==
```
```powershell
. .\pbkdf2.ps1 -value 'GD2UA8MKCvCkL+qr/QTvGl76V1G5S5A2oXPUDVdD0qF70esUEuAi17/sGn8N5aHrUIWwSZbToe2p/IXSNojt/Q==' -password secret -operation decrypt -strong
```
```text
hello, world of AES
```
Suppose one has an "application.properties" file in a typical java project with the value

```java
name=ENC(paGsbiPV3aspdDtM1XKSw12yqOPv02ngdJV3aNRTEOMaTD544tIv7N99s0y5wRLGwv7Y7nShCMwGuIqGOLIhzw==)
```
one can read plain value by running the `pbkdf2.ps1` and providing the paths to properties file and secret key file like this:

```powershell
. .\pbkdf2.ps1 -key 'x\key.txt' -properties 'application.properties' -name 'name' -operation decrypt
```
this will print
```text
password: secret
value_data: ENC(paGsbiPV3aspdDtM1XKSw12yqOPv02ngdJV3aNRTEOMaTD544tIv7N99s0y5wRLGwv7Y7nShCMwGuIqGOLIhzw==)
value: paGsbiPV3aspdDtM1XKSw12yqOPv02ngdJV3aNRTEOMaTD544tIv7N99s0y5wRLGwv7Y7nShCMwGuIqGOLIhzw==
```
```text
hello, world of AES
```
### Java compatible Encryption / Decryption



There is a defect [122](https://github.com/jasypt/jasypt/issues/122) reported in latest Jasypt release:

```text
The following algorithms result in the encrypt.sh utility giving the "Operation not possible (Bad input or parameters)" error:

PBEwithHMACSHA1andAES_128
PBEwithHMACSHA1andAES_256
PBEwithHMACSHA224andAES_128
PBEwithHMACSHA224andAES_256
PBEwithHMACSHA256andAES_128
PBEwithHMACSHA256andAES_256
PBEwithHMACSHA384andAES_128
PBEwithHMACSHA384andAES_256
PBEwithHMACSHA512andAES_128
PBEwithHMACSHA512andAES_256
```

From source code inspection it appears only the `PBEWithHMACSHA512AndAES_256` is referenced 

and setting it to other value , in particular the one we ae intestested in
```java
encryptor.setAlgorithm("PBEwithHMACSHA5AndAES_256");
```
is leading to
```text
org.jasypt.exceptions.EncryptionOperationNotPossibleException
```

The ealier build, e.g. __1.9.1__ (available from Maven central)
appear to be pretend to encrypt with this algorythm
```
./jasypt-1.9.1/bin/encrypt.sh input='hello,world' password=secret algorithm=PBEWithHMACSHA1AndAES_256  verbose=true
```
```text
----ENVIRONMENT-----------------

Runtime: Oracle Corporation Java HotSpot(TM) 64-Bit Server VM 25.161-b12



----ARGUMENTS-------------------

verbose: true
algorithm: PBEWithHMACSHA1AndAES_256
input: hello,world
password: secret



----OUTPUT----------------------

jC/GNzmkjkA6dpCM5J7zNr4EYTMqH+igAAa8Z1/Lkp0=


```

but the same jar is unable to decrypt the value it produced

```sh
./jasypt-1.9.1/bin/decrypt.sh input='jC/GNzmkjkA6dpCM5J7zNr4EYTMqH+igAAa8Z1/Lkp0=' password=secret algorithm=PBEWithHMACSHA1AndAES_256  verbose=true
```
```text
----ENVIRONMENT-----------------

Runtime: Oracle Corporation Java HotSpot(TM) 64-Bit Server VM 25.161-b12



----ARGUMENTS-------------------

verbose: true
algorithm: PBEWithHMACSHA1AndAES_256
input: jC/GNzmkjkA6dpCM5J7zNr4EYTMqH+igAAa8Z1/Lkp0=
password: secret



----ERROR-----------------------

Operation not possible (Bad input or parameters)

```
and neither Perl or .Net can 
```cmd
Program\bin\Debug\Program.exe -operation=decrypt -password=secret -value:jC/GNzmkjkA6dpCM5J7zNr4EYTMqH+igAAa8Z1/Lkp0= -debug
```
```text
debug: true
password: secret
value: jC/GNzmkjkA6dpCM5J7zNr4EYTMqH+igAAa8Z1/Lkp0=
reading: 32 bytes of payload
reading: 0 bytes of data
salt: O/ 79 Z@:v?O z 6
salt: 8C2FC63739A48E403A76908CE49EF336
iv: _?a3*?  ? g_E'?
iv(hex): BE0461332A1FE8A00006BC675FCB929D
data:
data(hex):
decrypted:
```
### Critical fix

 * [original](https://github.com/sergueik/powershell_samples/blob/cfde0a91a503e15d000363535fab594c995d52a3/external/csharp/pbkdf2-csharp/Program/AES.cs#L82)
 * [fixed](https://github.com/sergueik/powershell_samples/blob/master/csharp/pbkdf2-csharp/Utils/AES.cs#L55)

### Changing the Hashing Algorithm

* Note: One has to upgrade to .Net Framework __4.8__ if not already.
Use the __Microsoft .NET Framework 4.8 offline installer for Windows__
[download link](https://support.microsoft.com/en-us/topic/microsoft-net-framework-4-8-offline-installer-for-windows-9d23f658-3b97-68ab-d013-aa3c3e7495e0)

* One can provide the argument to `Rfc2898DeriveBytes` constructor:
```cs
deriveBytes = new Rfc2898DeriveBytes(password, salt, 1000, System.Security.Cryptography.HashAlgorithmName.SHA512 );
```

instead of default
```cs
deriveBytes = new Rfc2898DeriveBytes(password, salt, 1000);
```

this signature is only 
[supported](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.rfc2898derivebytes.-ctor?view=netframework-4.8#system-security-cryptography-rfc2898derivebytes-ctor(system-string-system-byte()-system-int32-system-security-cryptography-hashalgorithmname)) on the installed (last) release of .Net Framework or .Net Core family

the constant is [defined](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.hashalgorithmname?view=netframework-4.5) in earlier releases too.

#### Testing

```sh
perl -I . jasypt.pl -operation encrypt -password password -value 'test'
```
```text
/sTJrNjHLNl5DM+5U7pI9xT317S0K7iYc9mWqDYFyDgNJ998VLmy7ZsICyhDobXz
```

```powershell
.\Program\bin\Debug\Program.exe -operation:decrypt -value:/sTJrNjHLNl5DM+5U7pI9xTSBXmHytNR2rLYlO75GFuhpLoOyrK/7NTGS8s1VS0y -password=password -strong
```

```text
test
```

```powershell
.\Program\bin\Debug\Program.exe -value:text -password=password -strong
```

```text
9ZFbWIbDbTzvI5htTqObCSMQUxVuRbx6l+bMEVeih9XGndctuDnj+n8jR+2Udrew
```

```Perl
perl -I . jasypt.pl -operation decrypt -password password -value '9ZFbWIbDbTzvI5htTqObCSMQUxVuRbx6l+bMEVeih9XGndctuDnj+n8jR+2Udrew'
```

```text
$P = password
$S =  f5915b5886c36d3cef23986d4ea39b09
$T = 0a2fc014d18c385eacddfb3dc82dd9927e4b78c2ca37055783fcd61e70a2eb78231053156e45bc7a97e6cc1157a287d5e7d4c0d2a2db9feb86b4b0909c005480
text
```
(there is some residual debug logging taking place in the current Perl module revision)

### Compatibility Test with Java / Jasypt

update `src/main/resources/application.properties` with encrypted material produced by .Net:

```java
jasypt.encryptor.algorithm=PBEWithHMACSHA512AndAES_256
defaultPassword = ENC(/sTJrNjHLNl5DM+5U7pI9xTSBXmHytNR2rLYlO75GFuhpLoOyrK/7NTGS8s1VS0y)
```

and `src/main/resources/key.txt`
```text
password
```
run
```sh
mvn spring-boot:run
```

this will show in console
```text
  .   ____          _            __ _ _
 /\\ / ___'_ __ _ _(_)_ __  __ _ \ \ \ \
( ( )\___ | '_ | '_| | '_ \/ _` | \ \ \ \
 \\/  ___)| |_)| | | | | || (_| |  ) ) ) )
  '  |____| .__|_| |_|_| |_\__, | / / / /
 =========|_|==============|___/=/_/_/_/
 :: Spring Boot ::        (v2.3.4.RELEASE)

2023-09-27 17:41:21.901  INFO 14106 --- [           main] example.Application                      : Starting Application on lenovoy40-1 with PID 14106 (/home/sergueik/src/springboot_study/basic-jasypt/target/classes started by sergueik in /home/sergueik/src/springboot_study/basic-jasypt)
2023-09-27 17:41:21.904  INFO 14106 --- [           main] example.Application                      : No active profile set, falling back to default profiles: default
2023-09-27 17:41:22.466  INFO 14106 --- [           main] ptablePropertiesBeanFactoryPostProcessor : Post-processing PropertySource instances
2023-09-27 17:41:22.489  INFO 14106 --- [           main] c.u.j.EncryptablePropertySourceConverter : Converting PropertySource configurationProperties [org.springframework.boot.context.properties.source.ConfigurationPropertySourcesPropertySource] to AOP Proxy
2023-09-27 17:41:22.490  INFO 14106 --- [           main] c.u.j.EncryptablePropertySourceConverter : Converting PropertySource servletConfigInitParams [org.springframework.core.env.PropertySource$StubPropertySource] to EncryptablePropertySourceWrapper
2023-09-27 17:41:22.491  INFO 14106 --- [           main] c.u.j.EncryptablePropertySourceConverter : Converting PropertySource servletContextInitParams [org.springframework.core.env.PropertySource$StubPropertySource] to EncryptablePropertySourceWrapper
2023-09-27 17:41:22.491  INFO 14106 --- [           main] c.u.j.EncryptablePropertySourceConverter : Converting PropertySource systemProperties [org.springframework.core.env.PropertiesPropertySource] to EncryptableMapPropertySourceWrapper
2023-09-27 17:41:22.491  INFO 14106 --- [           main] c.u.j.EncryptablePropertySourceConverter : Converting PropertySource systemEnvironment [org.springframework.boot.env.SystemEnvironmentPropertySourceEnvironmentPostProcessor$OriginAwareSystemEnvironmentPropertySource] to EncryptableSystemEnvironmentPropertySourceWrapper
2023-09-27 17:41:22.492  INFO 14106 --- [           main] c.u.j.EncryptablePropertySourceConverter : Converting PropertySource random [org.springframework.boot.env.RandomValuePropertySource] to EncryptablePropertySourceWrapper
2023-09-27 17:41:22.492  INFO 14106 --- [           main] c.u.j.EncryptablePropertySourceConverter : Converting PropertySource applicationConfig: [classpath:/application.properties] [org.springframework.boot.env.OriginTrackedMapPropertySource] to EncryptableMapPropertySourceWrapper
2023-09-27 17:41:22.558  INFO 14106 --- [           main] c.u.j.filter.DefaultLazyPropertyFilter   : Property Filter custom Bean not found with name 'encryptablePropertyFilter'. Initializing Default Property Filter
2023-09-27 17:41:22.816  INFO 14106 --- [           main] o.s.b.w.embedded.tomcat.TomcatWebServer  : Tomcat initialized with port(s): 8080 (http)
2023-09-27 17:41:22.837  INFO 14106 --- [           main] o.apache.catalina.core.StandardService   : Starting service [Tomcat]
2023-09-27 17:41:22.838  INFO 14106 --- [           main] org.apache.catalina.core.StandardEngine  : Starting Servlet engine: [Apache Tomcat/9.0.38]
2023-09-27 17:41:22.920  INFO 14106 --- [           main] o.a.c.c.C.[Tomcat].[localhost].[/]       : Initializing Spring embedded WebApplicationContext
2023-09-27 17:41:22.920  INFO 14106 --- [           main] w.s.c.ServletWebServerApplicationContext : Root WebApplicationContext: initialization completed in 960 ms
2023-09-27 17:41:22.963  INFO 14106 --- [           main] c.u.j.r.DefaultLazyPropertyResolver      : Property Resolver custom Bean not found with name 'encryptablePropertyResolver'. Initializing Default Property Resolver
2023-09-27 17:41:22.965  INFO 14106 --- [           main] c.u.j.d.DefaultLazyPropertyDetector      : Property Detector custom Bean not found with name 'encryptablePropertyDetector'. Initializing Default Property Detector
2023-09-27 17:41:22.979  INFO 14106 --- [           main] tConfig$$EnhancerBySpringCGLIB$$e9603806 : loading /home/sergueik/src/springboot_study/basic-jasypt/src/main/resources/key.txt
2023-09-27 17:41:22.979  INFO 14106 --- [           main] tConfig$$EnhancerBySpringCGLIB$$e9603806 : Read raw data: "password



" 13 chars
2023-09-27 17:41:22.979  INFO 14106 --- [           main] tConfig$$EnhancerBySpringCGLIB$$e9603806 : Trimmed data: "password": 8 chars
2023-09-27 17:41:22.980  INFO 14106 --- [           main] tConfig$$EnhancerBySpringCGLIB$$e9603806 : Read: "password"
2023-09-27 17:41:22.982  INFO 14106 --- [           main] c.u.j.encryptor.DefaultLazyEncryptor     : Found Custom Encryptor Bean org.jasypt.encryption.pbe.PooledPBEStringEncryptor@26b894bd with name: jasyptStringEncryptor
2023-09-27 17:41:23.417  INFO 14106 --- [           main] o.s.s.concurrent.ThreadPoolTaskExecutor  : Initializing ExecutorService 'applicationTaskExecutor'
2023-09-27 17:41:23.555  INFO 14106 --- [           main] o.s.b.w.embedded.tomcat.TomcatWebServer  : Tomcat started on port(s): 8080 (http) with context path ''
2023-09-27 17:41:23.567  INFO 14106 --- [           main] example.Application                      : Started Application in 2.135 seconds (JVM running for 2.557)
##############################
Username is -------->user
Endpoint is -------->https://user:test@localhost:30000
##############################
```
### Compatibility with Python Encryptor

```sh
python app3.py --operation encrypt --value test --password password --debug
```
```text
running debug mode
salt (encrypt): a4d9147d4a8eb9b5c3b3b1aa2065c2d2
key (encrypt): 953db9e0f12f7b21e251c72d235d0aee24b76379d0afe1d24381ed629c8d4762
iv (encrypt): d189feeb68263b455163ac27f1980357
encrypted: pNkUfUqOubXDs7GqIGXC0tGJ/utoJjtFUWOsJ/GYA1dEcoPnnq7WJS6zxwYU3HjO
```
```sh
python app3.py --operation decrypt --value 'pNkUfUqOubXDs7GqIGXC0tGJ/utoJjtFUWOsJ/GYA1dEcoPnnq7WJS6zxwYU3HjO' --password password --debug
```
```text
running debug mode
salt (decrypt): a4d9147d4a8eb9b5c3b3b1aa2065c2d2
key (decrypt): 953db9e0f12f7b21e251c72d235d0aee24b76379d0afe1d24381ed629c8d4762
iv (decrypt): d189feeb68263b455163ac27f1980357
decrypted: test
```

```powershell
Program\bin\Debug\Program.exe -operation:decrypt -password:password -value:pNkUfUqOubXDs7GqIGXC0tGJ/utoJjtFUWOsJ/GYA1dEcoPnnq7WJS6zxwYU3HjO -debug -strong
```
```text
debug: true
password: password
value: pNkUfUqOubXDs7GqIGXC0tGJ/utoJjtFUWOsJ/GYA1dEcoPnnq7WJS6zxwYU3HjO
use SHA512: True
reading: 48 bytes of payload
reading: 16 bytes of data
salt: A4D9147D4A8EB9B5C3B3B1AA2065C2D2
iv: D189FEEB68263B455163AC27F1980357
data: 447283E79EAED6252EB3C70614DC78CE
key: 953DB9E0F12F7B21E251C72D235D0AEE24B76379D0AFE1D24381ED629C8D4762
iv: D189FEEB68263B455163AC27F1980357
decrypted: test
```
### NOTE

When using [SharpDevelop](https://github.com/icsharpcode/SharpDevelop) for managing the project, ignore the startup warning:
```text
The reference assemblies for framework ".NETFramework,Version=v4.8" were not found. To resolve this, install the SDK or Targeting Pack for this framework version or retarget your application to a version of the framework for which you have the SDK or Targeting Pack installed. Note that assemblies will be resolved from the Global Assembly Cache (GAC) and will be used in place of reference assemblies. Therefore your assembly may not be correctly targeted for the framework you intend
```
This message is harmless -  the project successfully compiles in SharpDevelop
```text
Build started.
Compiling Utils
Compiling Program
Compiling Test
Build finished successfully. (00:00:09.6828762)
```
and runs afterwards

#### Powershell-Specific List Challenge and Workaround

* reading the contents of a file `a.txt` with a *single line of text* creates `String`:

```powershell
$x = get-content -path ( resolve-path 'a.txt').path

$x.geTType()
```
```text
IsPublic IsSerial Name                                     BaseType
-------- -------- ----                                     --------
True     True     String                                   System.Object

```
but doing the same reading contents of a file `b.txt` with a *multiple lines of text* creates `Array`:

```powershell

$y = get-content -path ( resolve-path 'b.txt').path

$y.geTType()

```
```text
IsPublic IsSerial Name                                     BaseType
-------- -------- ----                                     --------
True     True     Object[]                                 System.Array

```
##### Workarounds

* cast to an array:
```powershell
$x = @(get-content -path ( resolve-path 'a.txt').path)
$x.getType()
```

```text
IsPublic IsSerial Name                                     BaseType
-------- -------- ----                                     --------
True     True     Object[]                                 System.Array

```
```powershell
$y = @(get-content -path ( resolve-path 'b.txt').path)
$y.getType()
```

```text
IsPublic IsSerial Name                                     BaseType
-------- -------- ----                                     --------
True     True     Object[]                                 System.Array
```

* cast to aray through "leading comma unary operator in expression mode"
* NOTE: cryptic

```powershell
$x = (,(get-content -path ( resolve-path 'a.txt').path))
$x.getType()
```

or even

```powershell
$x = ,(get-content -path ( resolve-path 'a.txt').path)
$x.getType()
```

```text
IsPublic IsSerial Name                                     BaseType
-------- -------- ----                                     --------
True     True     Object[]                                 System.Array
```


### Next Steps

convert to .net Core

Pure C# logic using only System namespaces (e.g., System.Security.Cryptography).

No UI, no WinForms/WPF, no COM, no embedded browser — all major Core blockers are absent.

Only changes needed:

Convert the .csproj to SDK-style (simpler, cross-platform).

Update using directives if necessary (minor, Core is mostly compatible).


one can defer test  subproject conversion if it is hard


### Running Benchmarks

```cmd
cd Benchmarks\bin\Debug
.\Benchmarks.exe
```
```text

// ***** BenchmarkRunner: Start   *****
// Found benchmarks:

// Validating benchmarks:
// ***** BenchmarkRunner: Finish  *****

// * Export *
  BenchmarkDotNet.Artifacts\results\BenchmarkRun-001-2025-08-13-04-07-20-report.csv
  BenchmarkDotNet.Artifacts\results\BenchmarkRun-001-2025-08-13-04-07-20-report-github.md
  BenchmarkDotNet.Artifacts\results\BenchmarkRun-001-2025-08-13-04-07-20-report.html

// * Detailed results *
Total time: 00:00:00 (0 sec)

// * Summary *

Host Process Environment Information:
BenchmarkDotNet=v0.9.8.0
OS=Microsoft Windows NT 6.2.9200.0
Processor=11th Gen Intel(R) Core(TM) i5-11300H 3.10GHz, ProcessorCount=8
Frequency=10000000 ticks, Resolution=100.0000 ns, Timer=UNKNOWN
CLR=MS.NET 4.0.30319.42000, Arch=32-bit DEBUG
GC=Concurrent Workstation
JitModules=clrjit-v4.8.9310.0

There are no benchmarks found

```	


NOTE: cannot open in ildasm.exe __.Net 3.5__:

```cmd
"c:\Program Files\Microsoft SDKs\Windows\v7.0A\bin\ildasm.exe" /text  packages\BenchmarkDotNet.0.9.8\lib\net45\BenchmarkDotNet.dll

```
 - no output
```text
//  Microsoft (R) .NET Framework IL Disassembler.  Version 3.5.30729.1
//  Copyright (c) Microsoft Corporation.  All rights reserved.

```
need to use
```cmd
"c:\Program Files\Microsoft SDKs\Windows\v7.0A\bin\NETFX 4.0 Tools\ildasm.exe" /text packages\BenchmarkDotNet.0.9.8\lib\net45\BenchmarkDotNet.dll
```


### Porting to .Net Core

  * navigate to `https://dotnet.microsoft.com/en-us/download/dotnet/6.0` to download the SDK archive, pick per Linux platform / CPU
or directly to `https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-6.0.428-linux-x64-binaries`

![downloads](https://github.com/sergueik/powershell_samples/blob/master/csharp/pbkdf2-csharp/screenshots/downloads.png)

```sh
 curl  https://builds.dotnet.microsoft.com/dotnet/Sdk/6.0.428/dotnet-sdk-6.0.428-linux-x64.tar.gz -o ~/Downloads/dotnet-sdk-6.0.428-linux-x64.tar.gz
```

* purge the possibly installed - it is prone to lack the `host/fxr`
```sh
sudo apt remove --purge dotnet-sdk-6.0
file ~/Downloads/dotnet-sdk-6.0.428-linux-x64.tar.gz
```
```text
/home/sergueik/Downloads/dotnet-sdk-6.0.428-linux-x64.tar.gz: gzip compressed data, from Unix, original size modulo 2^32 515799040
```
  * explore the archive into `/usr/share/dotnet`
```sh
sudo tar -xzvf ~/Downloads/dotnet-sdk-6.0.428-linux-x64.tar.gz -C /usr/share/dotnet
```
  * verify the presence of SDK:

```sh
export PATH=$PATH:/usr/share/dotnet
```
    
```sh
dotnet --list-sdks
```
```text
6.0.428 [/usr/share/dotnet/sdk]
```
alternatibely use apt:

```sh
sudo apt install dotnet-sdk-6.0
```
  * generate SDK `*.csproj` files by hand in `Utils`, `Program`, `Test`.

```XML
<?xml version="1.0" encoding="utf-8"?>
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\Utils\Utils.csproj" />
    <ProjectReference Include="..\Program\Program.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.3" />
    <PackageReference Include="NUnit" Version="3.13.3" />
    <PackageReference Include="NUnit3TestAdapter" Version="4.3.1" />
  </ItemGroup>
</Project>
```
```text
```
  * generate solution file
```sh
dotnet new sln -n pbkdf2-csharp --force
```
```text
The template "Solution File" was created successfully.
```
  * add projects to solution
```sh
dotnet sln add Utils/Utils.csproj Test/Test.csproj Program/Program.csproj
```
```text
Project `Utils/Utils.csproj` added to the solution.
Project `Test/Test.csproj` added to the solution.
Project `Program/Program.csproj` added to the solution.`
```
  * compile and build solution
```sh
dotnet build pbkdf2-csharp.sln
```
```text
MSBuild version 17.3.4+a400405ba for .NET
  Determining projects to restore...
/home/sergueik/src/powershell_samples/csharp/pbkdf2-csharp/Test/Test.csproj : warning NU1603: Test depends on Microsoft.NET.Test.Sdk (>= 17.8.3) but Microsoft.NET.Test.Sdk 17.8.3 was not found. An approximate best match of Microsoft.NET.Test.Sdk 17.9.0 was resolved. [/home/sergueik/src/powershell_samples/csharp/pbkdf2-csharp/pbkdf2-csharp.sln]
  All projects are up-to-date for restore.
/home/sergueik/src/powershell_samples/csharp/pbkdf2-csharp/Test/Test.csproj : warning NU1603: Test depends on Microsoft.NET.Test.Sdk (>= 17.8.3) but Microsoft.NET.Test.Sdk 17.8.3 was not found. An approximate best match of Microsoft.NET.Test.Sdk 17.9.0 was resolved.
  Utils -> /home/sergueik/src/powershell_samples/csharp/pbkdf2-csharp/Utils/bin/Debug/net6.0/Utils.dll
  Program -> /home/sergueik/src/powershell_samples/csharp/pbkdf2-csharp/Program/bin/Debug/net6.0/Program.dll
  Test -> /home/sergueik/src/powershell_samples/csharp/pbkdf2-csharp/Test/bin/Debug/net6.0/Test.dll

Build succeeded.

```
  * compile and run tests
```sh
dotnet add Test package NUnit --version 3.13.3
dotnet add Test package NUnit3TestAdapter --version 4.3.1
dotnet add Test package Microsoft.NET.Test.Sdk --version 17.8.3
dotnet test Test
```
```text
Passed!  - Failed:     0, Passed:    16, Skipped:     0, Total:    16, Duration: 66 ms - /home/sergueik/src/powershell_samples/csharp/pbkdf2-csharp/Test/bin/Debug/net6.0/Test.dll (net6.0)
```
  * run console application:
```sh
./Program/bin/Debug/net6.0/Program -value=hello -password=secret -operation=encrypt -strong true  -debug false
```
```text
debug: true
password: secret
value: hello
use SHA512: True
salt: BDADAA085A7DF321373CC27D1606C698
key: 2730A531AC75617909DCE36C0CD83D697DE1CEAB01D9F803006C4E878A46A469
iv: C769F10EAF40525297FF706B0E7912D8
data: E3730FEBFA7AD1AF31EE17D098F8D0FE
encrypted: WLalBjEeam/Db0j03mUv0QpEDID6Dr90FyKjs1Yb/3DHG9PG4sbxDhC9mYRuGoes
encrypted: WLalBjEeam/Db0j03mUv0QpEDID6Dr90FyKjs1Yb/3DHG9PG4sbxDhC9mYRuGoes
```
```sh
./Program/bin/Debug/net6.0/Program -value=WLalBjEeam/Db0j03mUv0QpEDID6Dr90FyKjs1Yb/3DHG9PG4sbxDhC9mYRuGoes -password=secret -operation=decrypt -strong true  -debug false
```
```text
debug: true
password: secret
value: WLalBjEeam/Db0j03mUv0QpEDID6Dr90FyKjs1Yb/3DHG9PG4sbxDhC9mYRuGoes
use SHA512: True
salt: 58B6A506311E6A6FC36F48F4DE652FD1
iv: 0A440C80FA0EBF741722A3B3561BFF70
data: C71BD3C6E2C6F10E10BD99846E1A87AC
key: 43379493E5E1E32568D52B3BA6548BC8F5AA1BD4FC7CB74411211223FB4B94F7
decrypted: hello
```
### See Also

  * [PBKDF2](https://en.wikipedia.org/wiki/PBKDF2)
  * `Rfc2898DeriveBytes` class [documentation](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.rfc2898derivebytes?view=netframework-4.5) - need more undertanding of password-based key derivation function PBDKF2. ifthe same password and salt is used to derive keys two times, and the keys are used during encryption and decryptiion the original data should be decrypted successfully.
  * https://stackoverflow.com/questions/4329909/hashing-passwords-with-md5-or-sha-256-c-sharp
  * https://www.codeproject.com/Tips/1156169/Encrypt-Strings-with-Passwords-AES-SHA
  * https://fastapi.metacpan.org/source/ARODLAND/Crypt-PBKDF2-0.161520/lib/Crypt
  * [Request for Comments: 2898 PKCS #5: Password-Based Cryptography Specification Version 2.0](https://datatracker.ietf.org/doc/html/rfc2898)
  * [Password-Based Key Derivation Function 2 (PBKDF2) test vectors](https://www.rfc-editor.org/rfc/rfc6070.html)
  * Perl CPAN [Crypt::Rijndael](https://metacpan.org/dist/Crypt-Rijndael) module
  * Perl CPAN [Crypt::PBE](https://metacpan.org/pod/Crypt::PBE) module
  * [basic exit options in .net c#](https://www.c-sharpcorner.com/UploadFile/c713c3/how-to-exit-in-C-Sharp/)
  * [cross Platform AES 256 GCM Encryption / Decryption](https://www.codeproject.com/Articles/1265115/Cross-Platform-AES-256-GCM-Encryption-Decryption) - uses  `BouncyCastle.Crypto.dll` in [c# .net core version](https://github.com/KashifMushtaq/AesGcm256) and in [Java version](https://github.com/KashifMushtaq/Aes256GCM_Java) (NOTE: the latter needs converson fron netbeans `build.xml` to maven)
  * [Symmetric Key Encryption by AES])(https://www.codeproject.com/Articles/1085427/Symmetric-Key-Encryption-by-AES)(Java)  
  * [Simple AES Encryption using C#](https://www.codeproject.com/Articles/1278566/Simple-AES-Encryption-using-Csharp) - uses static `iv`
  * [Using RSA and AES for File Encryption](https://www.codeproject.com/Tips/834977/Using-RSA-and-AES-for-File-Encryption) - lacks the PBE part, appears to use random bytes for `key` and `iv`
  * [Encryption Standard (AES) and Security Assertion Markup Language (SAML)](https://www.codeproject.com/Articles/1023379/Security-on-the-Web-by-Advanced-Encryption-Standar)
  * [CBC Stream Cipher in C# (With wrappers for two open source AES implementations in C# and C)](https://www.codeproject.com/Articles/7546/A-CBC-Stream-Cipher-in-C-With-wrappers-for-two-ope) - with Derived Key method
  * [How to Encrypt and Decrypt String using AES Algorithm](https://www.codeproject.com/Tips/839656/How-to-encrypt-and-decrypt-string-using-AES-algori)
  * [Windows / Android (C#/Java) Compatible Data Encryption with Compression](https://www.codeproject.com/Tips/5356513/Windows-Android-Csharp-Java-Compatible-Data-Encryp) - uses AES and HMACSHA256 - no source repo, only inline
  * [AES Encrypted Data Transmission between Arduino (ESP32) and C# (ASP.NET)](https://www.codeproject.com/Articles/5365769/AES-Encrypted-Data-Transmission-between-Arduino-ES)
  * [windows / Android (C#/Java) Compatible Data Encryption with Compression](https://www.codeproject.com/Tips/5356513/Windows-Android-Csharp-Java-Compatible-Data-Encryp) - code sample only
  * [protectedJson: Integrating ASP.NET Core Configuration and Data Protection](https://www.codeproject.com/Articles/5372873/ProtectedJson-Integrating-ASP-NET-Core-Configurati)
  * [comment](https://www.appsloveworld.com/csharp/100/101/how-do-i-use-sha-512-with-rfc2898derivebytes-in-my-salt-hash-code)
  * https://cdn3.iconfinder.com/data/icons/cryptocurrency-10/64/hash-function-power-cryptocurrency-256.png
  * https://www.iconfinder.com/search/icons?q=data
  * https://www.iconfinder.com/icons/2858157/data_lines_report_icon
  * https://www.iconfinder.com/icons/5309433/business_cryptocurrency_function_hash_security_icon
  * [BenchmarkDotNet](https://benchmarkdotnet.org/articles/guides/troubleshooting.html) 
  * [BenchmarkDotNet](https://www.nuget.org/packages/BenchmarkDotNet)  nuget reference. Note the signature of `BenchmarkRunner.Run(...)`.
  * [BCryptPbkdf.Net](https://github.com/Devolutions/BCryptPbkdf.Net/tree/maste)- A pure C# implementation of bcrypt_pbkdf, with Benchmark.net tests, on .Net  core.
  * [Why is a leading comma required when creating an array](https://stackoverflow.com/questions/42772083/why-is-a-leading-comma-required-when-creating-an-array)

### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)

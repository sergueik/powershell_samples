### Info

This directory contains the code from the 
[translate PBE Codes from Java to C#](https://cuteprogramming.wordpress.com/2015/02/20/translate-pbe-codes-from-java-to-c/) blog and 
[Emulating PBEWithMD5AndDES Encryption under .NET](https://www.codeproject.com/Articles/16450/Emulating-PBEWithMD5AndDES-Encryption-under-NET) codeproject article used to do [PBE](https://en.wikipedia.org/wiki/Password-based_cryptography)
encryption in way compatible with [jasypt](http://www.jasypt.org/howtoencryptuserpasswords.html)
### Usage

```powershell
.\Program\bin\Debug\Program.exe -value:something -password=abcdef -debug:true

```
```text
password: abcdef
value: something
DK: bc352f729f0097dd4e4df47887c12e05
encrypted: /SsSdC91HQs0B8ILFbvWcdHp8D2G6QHm
```

To reverse, currently have to Use Perl or Java version:
```sh
IMAGE=basic-perl-crypt-jasypt
docker image rm $IMAGE
docker build -t $IMAGE -f Dockerfile .
NAME=example-perl-jasypt
docker run --name $NAME -it $IMAGE sh
docker image prune -f
```
```sh
perl test.pl -password abcdef  -value '/SsSdC91HQs0B8ILFbvWcdHp8D2G6QHm' -operation decrypt
```
```text
password: abcdef
value: /SsSdC91HQs0B8ILFbvWcdHp8D2G6QHm
Salt: fd2b12742f751d0b
Encrypted: 3407c20b15bbd671d1e9f03d86e901e6
something
```
### Decrypting

```powershell
.\Program\bin\Debug\Program.exe -value:something -password=abcdefgh -debug:true -operation:encrypt -password: abcdefgh
```
```text
password: abcdefgh
value: something
DK: 6b397e8288089872321504f36f0f7d24
salt: orc]I>??
salt: f872a95dcc3e1919
result: orc]I>??
"ªruû"ó{ª-,IZ"d
encrypted: +HKpXcw+GRkKIqqudfuT83uqlrhJWpPw
```
```powershell
.\Program\bin\Debug\Program.exe -value:+HKpXcw+GRkKIqqudfuT83uqlrhJWpPw -password=abcdefgh -debug:true -operation:decrypt
```
```text
value: /SsSdC91HQs0Wxnl1ZqH/6LU63yUc5fX
value: /SsSdC91HQs0Wxnl1ZqH/6LU63yUc5fX
payload: y+?t/u??4[?åOs+ÿ¢Oë|"s-x
salt: y+?t/u??
salt: fd2b12742f751d0b
DK: bcfc32ee3ac9b338c081fbc36f18fb7a
decrypted: something
```
NOTE: the binary data is shown has few characters replaced


```
.\Program\bin\Debug\Program.exe -value:something -password=abcdefgh -debug:true -operation:encrypt -password: abcdefgh -salt:8F3F7F4F8F3F9F1F
password: true
value: something
DK: 63c149e1de0e1c77f9ec78ece653508b
salt: ??¦O??Y?
salt: 8f3f7f4f8f3f9f1f
result: ??¦O??Y?id~UX¬?dHn?_c   o?
encrypted: jz9/T48/nx9pZJjaWKwB8EhuP6+pCZwd
```

### Interability with Jasypt Java

* run in Docker Toolbox or Docker Desktop

```sh
docker build -t jasypt -f Dockerfile .
```
```text
Sending build context to Docker daemon    105kB
Step 1/7 : FROM openjdk:8-jre-alpine3.9
 ---> f7a292bbb70c
Step 2/7 : WORKDIR /tmp
 ---> Using cache
 ---> 2606fae84b0a
Step 3/7 : ARG VERSION=1.9.3
 ---> Using cache
 ---> 93c9946773f8
Step 4/7 : ENV VERSION=$VERSION
 ---> Using cache
 ---> 1678db9ed4ff
Step 5/7 : RUN apk add curl
 ---> Using cache
 ---> 66271bed3ee0
Step 6/7 : RUN curl -LOsk https://github.com/jasypt/jasypt/releases/download/jasypt-$VERSION/jasypt-$VERSION-dist.zip   && unzip jasypt-$VERSION-dist.zip > /dev/null   && sed -i 's|\r||g' ./jasypt-$VERSION/bin/encrypt.sh ./jasypt-$VERSION/bin/decrypt.sh ./jasypt-$VERSION/bin/listAlgorithms.sh   && chmod +x ./jasypt-$VERSION/bin/encrypt.sh ./jasypt-$VERSION/bin/decrypt.sh ./jasypt-$VERSION/bin/listAlgorithms.sh   && rm  jasypt-$VERSION-dist.zip
 ---> Running in 001eccffb210
Removing intermediate container 001eccffb210
 ---> f008c559155e
Step 7/7 : ENTRYPOINT ["/bin/sh", "-c", "/tmp/jasypt-$VERSION/bin/encrypt.sh $0 $@"]
 ---> Running in bbf6f2362e44
Removing intermediate container bbf6f2362e44
 ---> 671305888833
Successfully built 671305888833
Successfully tagged jasypt:latest
SECURITY WARNING: You are building a Docker image from Windows against a non-Windows Docker host. All files and directories added to build context will have '-rwxr-xr-x' permissions. It is recommended to double check and reset permissions for sensitive files and directories.
```
* encrypt via Java tool
```sh
docker run -it jasypt input=password password=secret
```
or (based on version of jasypt `Dockerfile`)
```sh
docker run -it jasypt encrypt input=password password=secret
```

```text
SVEOYtPGk5NZpEXYMZj/OAAOMivSCoFS
```
* decrypt with jasypt-csharp
```cmd
.\Program\bin\Debug\Program.exe -value:SVEOYtPGk5NZpEXYMZj/OAAOMivSCoFS -password=secret -debug:true -operation:decrypt
```
```text
password: secret
value: SVEOYtPGk5NZpEXYMZj/OAAOMivSCoFS
value: SVEOYtPGk5NZpEXYMZj/OAAOMivSCoFS
payload: IQ?bOÆ""Y¤EO1~ÿ8 ?2+O?R
salt: IQ?bOÆ""
salt: 49510e62d3c69393
DK: 384382eed3795f3e5f5e6797a7dd91fd
decrypted: password
```
### Note 

Currently we cover `PBEWithMD5AndDES` algorithm. This is the same encryption used by last __1.x__ release  of the jasypt available on author's [guthub](https://github.com/jasypt/jasypt/releases). The __3.x__ releases default to `PBEWithHmacSHA512AndAES_256` algorithm. The easies way to tweak the algorythm to different binary block sizes is to compare the two versions of `Crypt:PBE` Perl module in the [CPAN](https://metacpan.org/pod/Crypt::PBE)

### See Also
 
  * [hashing loop](https://metacpan.org/release/GDT/Crypt-PBE-0.102/source/lib/Crypt/PBE/PBKDF1.pm#L139) essential for jasypt [password, salt and iteration count based encryption](http://www.jasypt.org/howtoencryptuserpasswords.html) Iterate the hash function at least 1,000 times rule.
  * [equivalent Python code](https://github.com/fareliner/jasypt4py/blob/master/jasypt4py/generator.py#L162)
  * [cross Platform AES 256 GCM Encryption / Decryptio](https://www.codeproject.com/Articles/1265115/Cross-Platform-AES-256-GCM-Encryption-Decryption)
  * [convert a byte array to a hexadecimal string in C#](https://stackoverflow.com/questions/311165/how-do-you-convert-a-byte-array-to-a-hexadecimal-string-and-vice-versa)
  * [convert a hex string to bytes using Perl](https://stackoverflow.com/questions/2427527/how-can-i-convert-a-48-hex-string-to-bytes-using-perl)
  * Misc.
    + https://www.cryptosys.net/pki/dotnetpki/html/AllMembers_T_CryptoSysPKI_Pbe.htm
    + https://www.example-code.com/csharp/crypt2_pbes2.asp - requires Chilkat .NET Assemblies which is commercial - 30 day evaluation
    + https://www.nuget.org/packages/chilkat-x64#supportedframeworks-body-tab - works with .NET Framework net481, net451 and earlier - separate dlls for  net 451 through 472 in the archive: ChilkatDotNet45.dlland the ike

    + `PbeParameters Class` - not available for .NET Framework 4.8.1, only the .Net 7 https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.pbeparameters?view=net-7.0&viewFallbackFrom=netframework-4.8.1
  * Decrypt Utils Open Source PBX [example on java2s](http://www.java2s.com/Code/CSharp/Security/DecryptUtils.htm)
  * Encrypt Utils [example on java2s](http://www.java2s.com/Code/CSharp/Security/EncryptUtils.htm)
  * [C# AES 256 bits Encryption Library with Salt](https://www.codeproject.com/Articles/769741/Csharp-AES-bits-Encryption-Library-with-Salt)
  * [cross Platform AES 256 GCM Encryption / Decryption](https://www.codeproject.com/Articles/1265115/Cross-Platform-AES-256-GCM-Encryption-Decryption) - uses  `BouncyCastle.Crypto.dll` in [c# .net core version](https://github.com/KashifMushtaq/AesGcm256) and in [Java version](https://github.com/KashifMushtaq/Aes256GCM_Java) (NOTE: the latter needs converson fron netbeans `build.xml` to maven)
  
### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)

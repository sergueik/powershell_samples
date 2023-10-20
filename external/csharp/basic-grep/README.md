### Info

this drectory contains WIP
Powershell wrapper of the __C# Grep Application__ [codeproject](https://www.codeproject.com/Articles/1485/A-C-Grep-Application) project (project has both console and Windows Forms versons)
replacing ghe C# `Arguments` class with Powershell `param` for `Grep` class properties.

### Usage

* copy a typical `pom.xml` into the workpace directory for testing

```powershell
.\grep.ps1 -files pom.xml -regex 'spr[iIzz]ng\-'
```
```text
Grep results:

C:\developer\sergueik\powershell_ui_samples\external\csharp\basic-grep\pom.xml:
      <groupId>org.springframework.boot</groupId>
      <artifactId>spring-boot-starter-parent</artifactId>
        <groupId>org.springframework.boot</groupId>
        <artifactId>spring-boot-starter-web</artifactId>
        <groupId>org.springframework.boot</groupId>
        <artifactId>spring-boot-starter-test</artifactId>
          <groupId>org.springframework.boot</groupId>
          <artifactId>spring-boot-maven-plugin</artifactId>
```
```cmd
.\Program\bin\Debug\Program.exe /E:"spr[iIzz]ng\-" /F:pom.xml /n
```
```text
Grep results:

C:\developer\sergueik\powershell_ui_samples\external\csharp\basic-grep\pom.xml:
  12:     <artifactId>spring-boot-starter-parent</artifactId>
  38:       <artifactId>spring-boot-starter-web</artifactId>
  42:       <artifactId>spring-boot-starter-test</artifactId>
  52:         <artifactId>spring-boot-maven-plugin</artifactId>
```
### See Also

  * later version of the app on [codeplex repository](http://wingrep.codeplex.com/) (defunc)


### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)


### Info
 https://www.exemsi.com/documentation/sign-your-msi/
### Usage



* the original link command


```powershell
$cert = New-SelfSignedCertificate -DNSName "www.yourdomain.com" -CertStoreLocation Cert:\CurrentUser\My -Type CodeSigningCert -Subject “Example of Your Code Signing Certificate”
```


the `New-SelfSignedCertificate` cmdlet no longer accepts `Type` argument To create a self-signed certificate in earlier versions of Windows, use the Certificate Creation tool (MakeCert.exe). This  tool is included in the Microsoft .NET Framework SDK (versions 1.1 and later) and in the Microsoft Windows SDK.
### See Also
  * https://stackoverflow.com/questions/2718776/how-do-i-sign-exes-and-dlls-with-my-code-signing-certificate

   * [Signing MSI Installers with a Code Signing Certificate](https://dogschasingsquirrels.com/2020/01/27/signing-msi-installers-with-a-code-signing-certificate/)
   * [how to Generate a Self Signed Code Signing Certificate (And Why You Shouldn’t)](https://codesigningstore.com/how-to-generate-self-signed-code-signing-certificate)(incorrect)
   * [signing MSIX Packages](https://www.firegiant.com/wix/wep-documentation/msix/signing-msix-packages/)
   * https://stackoverflow.com/questions/17589754/wix-installer-msi-publisher-unknown 
   * https://wixtoolset.org/docs/tools/signing/
   * [](https://zhivye-oboi-windows.ru/kak-sozdat-samopodpisannyy-sertifikat-v-windows.html)(in Russian)
   * [self-signed SPC steps](https://translated.turbopages.org/proxy_u/en-ru.ru.33ab5b2f-6489065c-d0c826cb-74722d776562/https/stackoverflow.com/questions/84847/how-do-i-create-a-self-signed-certificate-for%20-code-signing-on-windows) (Russian translation)


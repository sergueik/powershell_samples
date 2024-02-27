#Copyright (c) 2022 Serguei Kouzmine
#
#Permission is hereby granted, free of charge, to any person obtaining a copy
#of this software and associated documentation files (the "Software"), to deal
#in the Software without restriction, including without limitation the rights
#to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
#copies of the Software, and to permit persons to whom the Software is
#furnished to do so, subject to the following conditions:
#
#The above copyright notice and this permission notice shall be included in
#all copies or substantial portions of the Software.
#
#THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
#IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
#FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
#AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
#LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
#OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
#THE SOFTWARE.

# https://support.microsoft.com/en-us/help/4026814/windows-accessing-credential-manager
# Control Panel -> User Accounts -> Credential Manager -> Windows Credential
# https://qna.habr.com/q/1184734

# see also: https://stackoverflow.com/questions/29103238/accessing-windows-credential-manager-from-powershell
# https://github.com/davotronic5000/PowerShell_Credential_Manager
<#
[Windows.Security.Credentials.PasswordVault,Windows.Security.Credentials,ContentType=WindowsRuntime]
(new-object Windows.Security.Credentials.PasswordVault).RetrieveAll() | % { $_.RetrievePassword(); $_ }


#>
  # https://github.com/davotronic5000/PowerShell_Credential_Manager/blob/master/PSCredentialManager.Api/Imports.cs
  # https://www.pinvoke.net/default.aspx/advapi32/CredRead.html
  # https://gist.github.com/meziantou/10311113   
  $shared_assemblies = @(
    'CredentialManagement.dll',
    'nunit.framework.dll'
  )

  $selenium_drivers_path = $shared_assemblies_path = "c:\Users\${env:USERNAME}\Downloads"

  if (($env:SHARED_ASSEMBLIES_PATH -ne $null) -and ($env:SHARED_ASSEMBLIES_PATH -ne '')) {
    $shared_assemblies_path = $env:SHARED_ASSEMBLIES_PATH
  }

  pushd $shared_assemblies_path

  $shared_assemblies | ForEach-Object { Unblock-File -Path $_; Add-Type -Path $_ }
  popd
  # https://www.c-sharpcorner.com/forums/windows-credential-manager-with-c-sharp
  # one can install wth nuget

  # https://www.nuget.org/packages/CredentialManagement/

  # cmdkey can create, list, and deletes stored user names and passwords or credentials.
  # Passwords will not be displayed once they are stored
  # https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/cmdkey

add-type -TypeDefinition @"
// "
using System;
using CredentialManagement;

public class Helper {
  private String password = null;
  private String userName = null;

  public string UserName {
    get { return userName; }
    set { userName = value; }
  }
  public string Password {
    set { password = value; }
  }

  public void SavePassword() {
    try {
      using (var cred = new Credential()) {
        cred.Password = password;
        cred.Target = userName;
        cred.Type = CredentialType.Generic;
        cred.PersistanceType = PersistanceType.LocalComputer;
        cred.Save();
      }
    } catch(Exception ex){
      Console.Error.WriteLine("Exception (ignored) " + ex.ToString());
    }
  }

  public String GetPassword() {
    try {
      using (var cred = new Credential()) {
        cred.Target = userName;
        cred.Load();
        return cred.Password;
      }
    } catch (Exception ex) {
      Console.Error.WriteLine("Exception (ignored) " + ex.ToString());
    }
    return "";
  }
}

"@  -ReferencedAssemblies 'System.Security.dll', "c:\Users\${env:USERNAME}\Downloads\CredentialManagement.dll"
$o = new-object Helper

<#
Typically, to create a PSCredential object, you'd use the Get-Credential cmdlet. The Get-Credential cmdlet is the most common way that PowerShell receives input to create the PSCredential object like the username and password. The Get-Credential cmdlet works fine and all but it's interactive.
#>
# https://adamtheautomator.com/powershell-get-credential/#:~:text=Typically,%20to%20create%20a%20PSCredential,like%20the%20username%20and%20password.&text=The%20Get-Credential%20cmdlet%20works%20fine%20and%20all%20but%20it's%20interactive.
<#
$username = 'root'
$plaintext_password = '...'
[System.Security.SecureString]$securestring = convertto-securestring $plaintext_password -asplaintext -force
[System.Management.Automation.PSCredential]$credential = new-object System.Management.Automation.PSCredential($username, $securestring)

$credential.GetNetworkCredential()

UserName Domain
-------- ------
root
$credential.GetNetworkCredential().Password
$plaintext_password will be printed

#>

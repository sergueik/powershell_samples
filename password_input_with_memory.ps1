#Copyright (c) 2014,2018,2020,2021,2022 Serguei Kouzmine
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

param (
  [string]$user = 'demouser',
  [switch]$store,
  [switch]$clipboard,
  [switch]$allow_automatic, # probably not useful, keep as an option to demonstrate
  [switch]$debug
)

function measure_width{

  param(
    [System.Windows.Forms.Control]$control,
    [System.Drawing.Font]$font,
    # both options are not precise
    [switch]$allow_automatic
  )
 $text_width = ($control.CreateGraphics().MeasureString($control.Text, $font).Width)
 if ($text_width -lt $control.Size.Width) {
  write-host ('Width: automatic {0}' -f $control.Size.Width)
  if ([bool]$PSBoundParameters['allow_automatic'].IsPresent) {
    $result = $control.Size.Width
  } else {
    $result = $text_width
  }
} else {
  $result = $text_width
  write-host ('Width: calculated {0}' -f $result)
}
 return $result
}

  $RESULT_OK = 0
$RESULT_CANCEL = 2


function PromptPassword {
  param(
    [string]$title,
    [string]$user,
    [object]$caller,
    [switch]$clipboard,
    [switch]$allow_automatic
  )
  $clipboard_flag = [bool]$PSBoundParameters['clipboard'].IsPresent

  if ([bool]$PSBoundParameters['allow_automatic'].IsPresent) {
  $allow_automatic_flag = '-allow_automatic'
} else {
  $allow_automatic_flag = $null
}

  @( 'System.Drawing','System.Windows.Forms') | ForEach-Object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }

  $f = new-object System.Windows.Forms.Form
  $f.MaximizeBox = $false
  $f.MinimizeBox = $false
  $f.Text = $title
  $f.size = new-object System.Drawing.Size(344,182)

  $l1 = new-object System.Windows.Forms.Label
  $l1.Location = new-object System.Drawing.Size (10,20)
  $l1.Size = new-object System.Drawing.Size (100,20)
  $l1.Text = 'Username'
  $f.Controls.Add($l1)

  $f.Font = new-object System.Drawing.Font ('Microsoft Sans Serif',10,[System.Drawing.FontStyle]::Regular,[System.Drawing.GraphicsUnit]::Point,0)

  $t1 = new-object System.Windows.Forms.TextBox
  $t1.Location = new-object System.Drawing.Point (120,20)
  $t1.Size = new-object System.Drawing.Size (140,20)
  $t1.Text = $user
  $t1.Name = 'txtUser'
  $f.Controls.Add($t1)

  $l2 = new-object System.Windows.Forms.Label
  $l2.Location = new-object System.Drawing.Size (10,50)
  $l2.Size = new-object System.Drawing.Size (100,20)
  $l2.Text = 'Password'
  $f.Controls.Add($l2)

  $t2 = new-object System.Windows.Forms.TextBox
  $t2.Location = new-object System.Drawing.Point (120,50)
  $t2.Size = new-object System.Drawing.Size (140,20)
  $t2.Text = ''
  $t2.Name = 'txtPassword'
  $t2.PasswordChar = '*'
  $f.Controls.Add($t2)

  $bOK = new-object System.Windows.Forms.Button

  $bOK.Text = 'OK'
  $bOK.Name = 'btnOK'
  $right_margin = 60
  $margin_y = 16
  $left_margin = 24
  $y = ($t2.Location.Y +  $t2.Size.Height + $margin_y)
  $bOK.Location = new-object System.Drawing.Point($left_margin, $y)
  $f.Controls.Add($bOK)
  $f.AcceptButton = $bOK
  # https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.control.creategraphics
  # https://docs.microsoft.com/en-us/dotnet/api/system.drawing.graphics.measurestring

  $bCancel = new-object System.Windows.Forms.Button
  $bCancel.Text = 'Cancel'
  $bCancel.Name = 'btnCancel'
  $bCancel.AutoSize = $true
  $w = measure_width  -font $f.Font -control $bCancel $allow_automatic_flag
  write-host ('measure_width: {0}' -f $w)
  $bCancel.Location = new-object System.Drawing.Point(($f.Size.Width - $w - $right_margin), $bOK.Location.y)
  $f.Controls.Add($bCancel)
<#

  $bE1 = new-object System.Windows.Forms.Button
  $bE1.Text = 'Really Cancel'
  $bE1.Name = 'bE1'
  $bE1.AutoSize = $true
  $right_margin = 60
  $w = measure_width  -font $f.Font -control $bE1 $allow_automatic_flag
  write-host ('measure_width: {0}' -f $w)

  $bE1.Location = new-object System.Drawing.Point(($f.Size.Width - $w - $right_margin), ($bCancel.Location.y + $bCancel.Size.Height + $margin_y))
  $f.Controls.Add($bE1)

  $bE2 = new-object System.Windows.Forms.Button
  $bE2.Text = 'Really Really Cancel'
  $bE2.Name = 'bE2'
  $bE2.AutoSize = $true
  $right_margin = 60
  $w = measure_width  -font $f.Font -control $bE2 $allow_automatic_flag
  write-host ('measure_width: {0}' -f $w)

  $bE2.Location = new-object System.Drawing.Point(($f.Size.Width - $w - $right_margin), ($bE1.Location.y + $bE1.Size.Height + $margin_y))
  $f.Controls.Add($bE2)
#>
<#
  $f.SuspendLayout()
  $f.Controls.AddRange(@(
    $l1,
    $t1,
    $l2,
    $t2,
    $bOK,
    $bCancel,
    ))
  $f.ResumeLayout($true)
  $f.PerformLayout()
#>

  $bCancel.add_click({
    if ($clipboard_flag) {
      [System.Windows.Forms.Clipboard]::Clear()
    } else {
      $caller.txtPassword = $null
      $caller.txtUser = $null
    }
    $f.Close()
  })
  $bOK.add_click({
    if ($clipboard_flag) {
      $Password = $t2.Text
      $user = $t1.Text

      [String]$cred = "${user}:${Password}"
      write-host ('Encoding {0}' -f $cred)
      $encodedCred = [System.Convert]::ToBase64String([System.Text.Encoding]::ASCII.GetBytes($cred))

      # $basicAuthValue = "Basic $encodedCred"
      #
      # $Headers = @{
      #  Authorization = $basicAuthValue
      # }

      # ContentType = 'application/json'
      # $uri = 'https://www.github.com
      # Invoke-WebRequest -Uri $uri -ContentType 'text/json' -Headers $Headers

      [System.Windows.Forms.Clipboard]::SetText($encodedCred)
    } else {
      $caller.Data = $RESULT_OK
      $caller.txtPassword = $t2.Text
      $caller.txtUser = $t1.Text
    }
    $f.Close()
  })

  $f.Controls.Add($l)
  $f.Topmost = $true

  if ($clipboard_flag) {
      [System.Windows.Forms.Clipboard]::Clear()
  } else {
    $caller.Data = $RESULT_CANCEL
  }
  $f.Add_Shown({
    $f.ActiveControl = $t2
    $f.Activate()
  })
  $f.KeyPreview = $True
  $f.Add_KeyDown({
    if ($_.KeyCode -eq 'Escape') {
      if ($clipboard_flag) {
        [System.Windows.Forms.Clipboard]::Clear()
      } else {
        $caller.Data = $RESULT_CANCEL
      }
    }
    else { return }
    $f.Close()
  })

  if ($clipboard_flag) {
    [void]$f.ShowDialog()
  } else {
    [void]$f.ShowDialog([win32window]($caller))
  }
  $f.Dispose()
}

Add-Type -TypeDefinition @"
using System;
using System.Windows.Forms;
public class Win32Window : IWin32Window
{
    private IntPtr _hWnd;
    private int _data;
    private string _txtUser;
    private string _txtPassword;

    public int Data
    {
        get { return _data; }
        set { _data = value; }
    }


    public string TxtUser
    {
        get { return _txtUser; }
        set { _txtUser = value; }
    }
    public string TxtPassword
    {
        get { return _txtPassword; }
        set { _txtPassword = value; }
    }

    public Win32Window(IntPtr handle)
    {
        _hWnd = handle;
    }

    public IntPtr Handle
    {
        get { return _hWnd; }
    }
}

"@ -ReferencedAssemblies 'System.Windows.Forms.dll'

[void][System.Reflection.Assembly]::LoadWithPartialName('System.Windows.Forms')
[void][System.Reflection.Assembly]::LoadWithPartialName('System.Drawing')

$store_flag = [bool]$PSBoundParameters['store'].IsPresent
if ($store_flag){
}
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
$plaintext_password = '...'
$secure_password = convertto-securestring $plaintext_password -AsPlainText -Force
$credential = new-object System.Management.Automation.PSCredential ('root', $secure_password)

$credential.GetNetworkCredential()

UserName Domain
-------- ------
root
$credential.GetNetworkCredential().Password
$plaintext_password will be shown

#>
$clipboard_flag = [bool]$PSBoundParameters['clipboard'].IsPresent
if ([bool]$PSBoundParameters['allow_automatic'].IsPresent) {
  $allow_automatic_flag = '-allow_automatic'
} else {
  $allow_automatic_flag = $null
}
if ($debug){
  $DebugPreference = 'Continue'
}
$title = 'Enter credentials'
<#
NOTE: launch powershell window
[System.Diagnostics.Process]::GetCurrentProcess().MainWindowHandle
788284
within powershell window, launch powershell again
- this is often done when debugging c# code embedded in Powershell script via add-type
 - this will lose MainWindowHandle
powershell.exe
Windows PowerShell
Copyright (C) 2015 Microsoft Corporation. All rights reserved.

[System.Diagnostics.Process]::GetCurrentProcess().MainWindowHandle
0
#>

if (-not $clipboard_flag) {
  write-output '1'
  $window_handle = [System.Diagnostics.Process]::GetCurrentProcess().MainWindowHandle
  write-output ('Using current process handle {0}' -f $window_handle)
  if ($window_handle -eq 0) {
    $processid = [System.Diagnostics.Process]::GetCurrentProcess().Id
    $parent_process_id = get-wmiobject win32_process | where-object {$_.processid -eq  $processid } | select-object -expandproperty parentprocessid

    $window_handle = get-process -id $parent_process_id | select-object -expandproperty MainWindowHandle $allow_automatic_flag
    write-output ('Using current process parent process {0} handle {1}' -f $parent_process_id, $window_handle)
  }

  $caller = new-object Win32Window -ArgumentList ($window_handle)
  PromptPassword -Title $title -user $user -caller $caller
  if ($caller.Data -ne $RESULT_CANCEL) {
    if ($debug){
      Write-Debug ('Original username/password was: {0} / {1}' -f $caller.txtUser,$caller.txtPassword)
    }
    if ($store_flag) {
      $o.UserName = $caller.txtUser
      $o.Password = $caller.txtPassword
      $o.SavePassword()
      write-output 'Password is stored in the vault'
    } else {
    if ([system.String]::Compare($o.GetPassword(), $caller.txtPassword) -eq 0 ){
      $result = 'valid'
    } else {
      $result = 'invalid'
    }
    write-output ('Password is ' + $result)
    if ($debug){
      write-debug ('Password loaded from Vault: {0}' -f $o.GetPassword())
    }
    }
  }
} else {
  PromptPassword -Title $title -user $user -clipboard $allow_automatic_flag
  $data = [System.Windows.Forms.Clipboard]::GetText()
  if ($debug){
  [System.Text.Encoding]::ASCII.GetString([System.Convert]::FromBase64String( $data))

  } else {
    write-output ('clipboard: {0}' -f $data)
  }
  [System.Windows.Forms.Clipboard]::Clear()

}

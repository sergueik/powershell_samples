### Elevate Credentials ###
param([switch]$Elevated)
function Check-Admin {
$currentUser = New-Object Security.Principal.WindowsPrincipal $([Security.Principal.WindowsIdentity]::GetCurrent())
$currentUser.IsInRole([Security.Principal.WindowsBuiltinRole]::Administrator) }
if ((Check-Admin) -eq $false)  { 
    if ($elevated){ # Could not elevate, quit
} else { 
    Start-Process powershell.exe -Verb RunAs -ArgumentList ('-noprofile -noexit -file "{0}" -elevated' -f ( $myinvocation.MyCommand.Definition ))
    } exit
}
### Elevate Credentials ###
###Test Registry Value##
function Test-RegistryValue {
    param (
     [parameter(Mandatory=$true)]
     [ValidateNotNullOrEmpty()]$Path,
     [parameter(Mandatory=$true)]
     [ValidateNotNullOrEmpty()]$Value
    )
     try {
     Get-ItemProperty -Path $Path | Select-Object -ExpandProperty $Value -ErrorAction Stop | Out-Null
     return $true
     }
     catch {
     return $false
     }
     }
  
###Test Registry Value##

### Switch Encoding to  UTF-8 ###
[Console]::outputEncoding = [System.Text.Encoding]::GetEncoding('UTF-8')
### Switch Encoding to  UTF-8 ###

#########################Select Registry Tweaks######################
Write-Host "---------------------------------"-ForegroundColor Yellow
Write-Host "Registry Tweaks" -ForegroundColor Yellow
Write-Host "---------------------------------"-ForegroundColor Yellow
Write-Host "1.Add Owner to context menu" -ForegroundColor Green
Write-Host "2.Add Run as Invoker to context menu" -ForegroundColor Green
Write-Host "3.Add Run Powershell as Admin to context menu" -ForegroundColor Green
Write-Host "4.Exit" -ForegroundColor Green
Write-Host "---------------------------------" -ForegroundColor Yellow

$Tweaks = Read-Host "Select Applying Tweak "
Switch($Tweaks){
 1{Write-Host '---------------------------------' -ForegroundColor Yellow
 Write-Host "Owner Registry Tweak" -ForegroundColor Yellow
 Write-Host '---------------------------------'-ForegroundColor Yellow
 Write-Host '1. Add to context menu' -ForegroundColor Green
 Write-Host '2. Remove from context menu' -ForegroundColor Green
 Write-Host "---------------------------------" -ForegroundColor Yellow
$Owner = Read-Host 'Action';
Switch($Owner){
    1{New-PSDrive -PSProvider registry -Root HKEY_CLASSES_ROOT -Name HKCR | Out-Null; `
     Set-Location -LiteralPath "HKCR:\*\shell\"; `
     If (!(Test-Path -LiteralPath HKCR:\*\shell\takeownership)){New-Item -Path HKCR:\`*\shell\takeownership -Force | Out-Null}; `
     New-ItemProperty -LiteralPath HKCR:\*\shell\takeownership -Name HasLUAShield -Value "" -PropertyType String -Force | Out-Null; `
     New-ItemProperty -LiteralPath HKCR:\*\shell\takeownership -Name NoWorkingDirectory -Value "" -PropertyType String -Force | Out-Null; `
         Set-ItemProperty -LiteralPath HKCR:\*\shell\takeownership -Name '(default)' -Value "Стать Владельцем" -Force | Out-Null; `
         If (!(Test-Path -LiteralPath HKCR:\*\shell\takeownership\command)){New-Item -Path HKCR:\`*\shell\takeownership\command -Force | Out-Null}; `
         Set-ItemProperty -LiteralPath HKCR:\*\shell\takeownership\command -Name '(default)' -Value 'cmd.exe /c takeown /f \"%1\" && icacls \"%1\" /grant administrators:F' -Force | Out-Null; `
         New-ItemProperty -LiteralPath HKCR:\*\shell\takeownership\command -Name IsolatedCommand -Value 'cmd.exe /c takeown /f \"%1\" && icacls \"%1\" /grant administrators:F' -PropertyType String -Force | Out-Null; `
         If (!(Test-Path -LiteralPath HKCR:\exefile\shell\takeownership)){New-Item -Path HKCR:\exefile\shell\takeownership -Force | Out-Null}; `
         New-ItemProperty -LiteralPath HKCR:\exefile\shell\takeownership -Name HasLUAShield -Value "" -PropertyType String -Force | Out-Null; `
     New-ItemProperty -LiteralPath HKCR:\exefile\shell\takeownership -Name NoWorkingDirectory -Value "" -PropertyType String -Force | Out-Null; `
         Set-ItemProperty -LiteralPath HKCR:\exefile\shell\takeownership -Name '(default)' -Value "Стать Владельцем" -Force | Out-Null; `
         If (!(Test-Path -LiteralPath HKCR:\exefile\shell\takeownership\command)){New-Item -Path HKCR:\exefile\shell\takeownership\command -Force | Out-Null}; `
         Set-ItemProperty -LiteralPath HKCR:\exefile\shell\takeownership\command -Name '(default)' -Value 'cmd.exe /c takeown /f \"%1\" && icacls \"%1\" /grant administrators:F' -Force | Out-Null; `
         New-ItemProperty -LiteralPath HKCR:\exefile\shell\takeownership\command -Name IsolatedCommand -Value 'cmd.exe /c takeown /f \"%1\" && icacls \"%1\" /grant administrators:F' -PropertyType String -Force | Out-Null; `
         If (!(Test-Path -LiteralPath HKCR:\dllfile\shell\takeownership)){New-Item -Path HKCR:\dllfile\shell\takeownership -Force | Out-Null}; `
         New-ItemProperty -LiteralPath HKCR:\dllfile\shell\takeownership -Name HasLUAShield -Value "" -PropertyType String -Force | Out-Null; `
     New-ItemProperty -LiteralPath HKCR:\dllfile\shell\takeownership -Name NoWorkingDirectory -Value "" -PropertyType String -Force | Out-Null; `
         Set-ItemProperty -LiteralPath HKCR:\dllfile\shell\takeownership -Name '(default)' -Value "Стать Владельцем" -Force | Out-Null; `
         If (!(Test-Path -LiteralPath HKCR:\dllfile\shell\takeownership\command)){New-Item -Path HKCR:\dllfile\shell\takeownership\command -Force | Out-Null}; `
         Set-ItemProperty -LiteralPath HKCR:\dllfile\shell\takeownership\command -Name '(default)' -Value 'cmd.exe /c takeown /f \"%1\" && icacls \"%1\" /grant administrators:F' -Force | Out-Null; `
         New-ItemProperty -LiteralPath HKCR:\dllfile\shell\takeownership\command -Name IsolatedCommand -Value 'cmd.exe /c takeown /f \"%1\" && icacls \"%1\" /grant administrators:F' -PropertyType String -Force | Out-Null; `
         If (!(Test-Path -LiteralPath HKCR:\Directory\shell\takeownership)){New-Item -Path HKCR:\Directory\shell\takeownership -Force | Out-Null}; `
         New-ItemProperty -LiteralPath HKCR:\Directory\shell\takeownership -Name HasLUAShield -Value "" -PropertyType String -Force | Out-Null; `
     New-ItemProperty -LiteralPath HKCR:\Directory\shell\takeownership -Name NoWorkingDirectory -Value "" -PropertyType String -Force | Out-Null; `
         Set-ItemProperty -LiteralPath HKCR:\Directory\shell\takeownership -Name '(default)' -Value "Стать Владельцем" -Force | Out-Null; `
         If (!(Test-Path -LiteralPath HKCR:\Directory\shell\takeownership\command)){New-Item -Path HKCR:\Directory\shell\takeownership\command -Force | Out-Null}; `
         Set-ItemProperty -LiteralPath HKCR:\Directory\shell\takeownership\command -Name '(default)' -Value 'cmd.exe /c takeown /f \"%1\" && icacls \"%1\" /grant administrators:F' -Force | Out-Null; `
         New-ItemProperty -LiteralPath HKCR:\Directory\shell\takeownership\command -Name IsolatedCommand -Value 'cmd.exe /c takeown /f \"%1\" && icacls \"%1\" /grant administrators:F' -PropertyType String -Force | Out-Null; `
         Write-Host "Set Owner button was added successfully"}
         2{New-PSDrive -PSProvider registry -Root HKEY_CLASSES_ROOT -Name HKCR | Out-Null; `
             Remove-Item -LiteralPath HKCR:\`*\shell\takeownership -recurse -Force | Out-Null; `
             Remove-Item -LiteralPath HKCR:\exefile\shell\takeownership -recurse -Force | Out-Null; `
             Remove-Item -LiteralPath HKCR:\dllfile\shell\takeownership -recurse -Force | Out-Null; `
             Remove-Item -LiteralPath HKCR:\Directory\shell\takeownership -recurse -Force | Out-Null; `
             Write-Host "Set Owner button was removed successfully"}
     }
 }
 2{Write-Host '-------------- ------------------' -ForegroundColor Yellow;
 Write-Host "Invoker Registry Tweak" -ForegroundColor Yellow;
 Write-Host '---------------------------------'-ForegroundColor Yellow;
 Write-Host '1. Add to context menu' -ForegroundColor Green;
 Write-Host '2. Remove from context menu' -ForegroundColor Green;
 Write-Host "---------------------------------" -ForegroundColor Yellow;
 $Invoker = Read-Host 'Action';
 Switch($Invoker){
 1{If (!(Test-RegistryValue -Path 'HKCU:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers' -Value 'C:\Program Files\ConEmu\ConEmu64.exe')){New-ItemProperty -LiteralPath 'HKCU:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers' `
 -Name 'C:\Program Files\ConEmu\ConEmu64.exe' -Value "RUNASINVOKER" -PropertyType String -Force | Out-Null}; `
 Write-Host "Inoker mode enabled for ConEmu64"
}
 2{Remove-ItemProperty -Path 'HKCU:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers' `
 -Name 'C:\Program Files\ConEmu\ConEmu64.exe' -Force | Out-Null; `
 Write-Host "Inoker mode disabled for ConEmu64"}
}
 }
 3{Write-Host '---------------------------------' -ForegroundColor Yellow;
 Write-Host "Powershell Admin Session Registry Tweak" -ForegroundColor Yellow;
 Write-Host '---------------------------------'-ForegroundColor Yellow;
 Write-Host '1. Add to context menu' -ForegroundColor Green;
 Write-Host '2. Remove from context menu' -ForegroundColor Green;
 Write-Host "---------------------------------" -ForegroundColor Yellow;
 $PSAdmin = Read-Host 'Action';
 Switch($PSAdmin){
 1{New-PSDrive -PSProvider registry -Root HKEY_CLASSES_ROOT -Name HKCR | Out-Null; `
    If (!(Test-Path -LiteralPath HKCR:\Microsoft.PowerShellScript.1\Shell\runas\command)){New-Item -Path HKCR:\Microsoft.PowerShellScript.1\Shell\runas\command -Force | Out-Null}; `
    New-ItemProperty -LiteralPath HKCR:\Microsoft.PowerShellScript.1\Shell\runas\ -Name HasLUAShield -Value '' -PropertyType String -Force | Out-Null; `
    New-ItemProperty -LiteralPath HKCR:\Microsoft.PowerShellScript.1\Shell\runas\command -Name 'Default' -Value 'powershell.exe –NoExit "-Command" "if((Get-ExecutionPolicy ) -ne 'AllSigned') { Set-ExecutionPolicy -Scope Process Bypass }; & '%1'\"' -PropertyType String -Force | Out-Null; `
    }
 2{}
 }
 
}
}
 #4{Write-Host 'Exit'; exit}
 #   default {Write-Host 'Wrong choice, try again' -ForegroundColor Red}

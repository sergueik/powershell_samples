# https://www.cyberforum.ru/powershell/thread2845542.html
Add-Type @'
  using System;
  using System.Runtime.InteropServices;
  public class SFW {
     [DllImport("user32.dll")]
     [return: MarshalAs(UnmanagedType.Bool)]
     public static extern bool SetForegroundWindow(IntPtr hWnd);
  }
'@
 
Add-Type -AssemblyName System.Windows.Forms
$IP = '192.168.0.29'
$Wait = 1
$Loggin=
(Get-Content ($MyInvocation.MyCommand).Source)[17] 
<#
user
#>
$Pass =
(Get-Content ($MyInvocation.MyCommand).Source)[22]
<#
123456789
#> 
$ps = Start-Process -FilePath 'C:\Program Files\TightVNC\tvnviewer.exe' -PassThru
Start-Sleep -Seconds $Wait
[void][SFW]::SetForegroundWindow($ps.MainWindowHandle)
Start-Sleep -Seconds $Wait
[void][System.Windows.Forms.SendKeys]::SendWait($IP)
$wshell = New-Object -ComObject wscript.shell;
$wshell.AppActivate('title of the application window')
$wshell.SendKeys('~')
Start-Sleep -Seconds $Wait
[void][System.Windows.Forms.SendKeys]::SendWait($Loggin)
Start-Sleep -Seconds $Wait
[void][System.Windows.Forms.SendKeys]::SendWait($Pass)
$wshell = New-Object -ComObject wscript.shell;
$wshell.AppActivate('title of the application window')
Sleep 1
$wshell.SendKeys('~')



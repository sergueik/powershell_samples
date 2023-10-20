@echo OFF
REM origin: http://forum.oszone.net/thread-349309.html
@set ".trayfrms.t_f=%LocalAppData%\Temp\trayfrms"
  @call :source.ps1.systray-menu >"%.trayfrms.t_f%.ps1"
  @call :script.ps1 "%.trayfrms.t_f%.ps1"
@goto END.

:script.ps1
  @call "%WinDir%\System32\WindowsPowerShell\v1.0\PowerShell.EXE" -ExecutionPolicy ByPass -File %*
@goto EXIT

:source.ps1.systray-menu
  @echo [void][System.Reflection.Assembly]::LoadWithPartialName("System.windows.forms")
    @echo. 
  @echo $STForm = New-Object System.Windows.Forms.form
  @echo $NotifyIcon = New-Object System.Windows.Forms.NotifyIcon
  @echo $ContextMenu = New-Object System.Windows.Forms.ContextMenu
  @echo $MenuItem = New-Object System.Windows.Forms.MenuItem
  @echo $MenuItem2 = New-Object System.Windows.Forms.MenuItem
  @echo $MenuItem3 = New-Object System.Windows.Forms.MenuItem
  @echo $Timer = New-Object System.Windows.Forms.Timer
  @echo $HealthyIcon = New-Object System.Drawing.Icon("%~dp0\sample.ico")
  @echo $UnhealthyIcon = New-Object System.Drawing.Icon("%~dp0\sample.ico")
    @echo. 
  @echo $STForm.ShowInTaskbar = $false
  @echo $STForm.WindowState = "minimized"
    @echo. 
  @echo $NotifyIcon.Icon = $HealthyIcon
  @echo $NotifyIcon.ContextMenu = $ContextMenu
  @echo $NotifyIcon.ContextMenu.MenuItems.AddRange($MenuItem)
  @echo $NotifyIcon.ContextMenu.MenuItems.AddRange($MenuItem2)
  @echo $NotifyIcon.ContextMenu.MenuItems.AddRange($MenuItem3)
  @echo $NotifyIcon.Visible = $True
    @echo. 
  @echo # We need to avoid using Start-Sleep as this freezes the GUI. Instead, we'll utilitse the .NET forms timer, it repeats a function at a set interval.
  @echo $Timer.Interval = 300000 # (5 min)
  @echo $Timer.add_Tick({ Load-Config })
  @echo $Timer.start()
    @echo. 
  @echo # This will appear as a right click option on the system tray icon
  @echo $MenuItem.Index = 0
  @echo $MenuItem.Text = "Exit"
  @echo $MenuItem.add_Click({
  @echo         $Timer.Stop()
  @echo         $NotifyIcon.Visible = $True
  @echo         $STForm.close()
  @echo     })
    @echo. 
  @echo $MenuItem2.Index = 0
  @echo $MenuItem2.Text = "Open notepad.exe"
  @echo $MenuItem2.add_Click({
  @echo         $Timer.Stop()
  @echo         $NotifyIcon.Visible = $True
  @echo         Start-Process notepad.exe
  @echo         $STForm.close()
  @echo     })
    @echo. 
  @echo $MenuItem3.Index = 0
  @echo $MenuItem3.Text = "Open notepad.exe (no exit)"
  @echo $MenuItem3.add_Click({
  @echo         $Timer.Stop()
  @echo         $NotifyIcon.Visible = $True
  @echo         Start-Process notepad.exe
  @echo     })
    @echo. 
  @echo function Load-Config
  @echo {
  @echo     #Get-Content some Data from a file here
  @echo     if ($warn)
  @echo     {
  @echo         $NotifyIcon.Icon = $UnhealthyIcon
  @echo         $NotifyIcon.ShowBalloonTip(30000, "Attention!", "Some data from a file here...", [system.windows.forms.ToolTipIcon]"Warning")
  @echo         Remove-Variable warn
  @echo     }
  @echo     else
  @echo     {
  @echo         $NotifyIcon.Icon = $HealthyIcon
  @echo     }
  @echo }
    @echo. 
  @echo Load-Config
  @echo [void][System.Windows.Forms.Application]::Run($STForm)
@goto EXIT

:END.
  @if NOT defined .trayfrms.t_f (@goto EXIT)
    @del /q /f "%LocalAppData%\Temp\trayfrms*" >NUL 2>NUL.
  @set ".trayfrms.t_f="
:EXIT

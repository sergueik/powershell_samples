strComputer = "."
Set objWMIService = GetObject("winmgmts:{impersonationLevel=impersonate}!\\" & strComputer & "\root\cimv2")

Set colOperatingSystems = objWMIService.ExecQuery ("select * from Win32_OperatingSystem")

For Each objOperatingSystem in colOperatingSystems
Wscript.echo objOperatingSystem.Caption & " " & objOperatingSystem.Version, 0 + 32,"Window Version"
Next
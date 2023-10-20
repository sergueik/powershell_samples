' orgin: http://forum.oszone.net/thread-255310-2.html
Option Explicit

Const SW_NORMAL = 1

Dim objSWbemObjectEx_Win32_Process
Dim objSWbemObjectEx_Win32_ProcessStartup
Dim lngProcessID

Dim strCommandLine1
Dim strCommandLine2

strCommandLine1 = "ping.exe -t ya.ru"
strCommandLine2 = "ping.exe -t google.com"

With WScript.CreateObject("WbemScripting.SWbemLocator")
	With .ConnectServer(".", "root\cimv2")
		Set objSWbemObjectEx_Win32_Process        = .Get("Win32_Process")
		Set objSWbemObjectEx_Win32_ProcessStartup = .Get("Win32_ProcessStartup").SpawnInstance_
		
		With objSWbemObjectEx_Win32_ProcessStartup
			.ShowWindow = SW_NORMAL
			.CreateFlags = 16
			
			.X =  25
			.Y = 100
			
			.XSize = 600
			.YSize = 300
			
			.XCountChars = 40
			.YCountChars = 3000
			
			.Title = "Ping 1"
			.FillAttribute = 30
		End With
		
		If objSWbemObjectEx_Win32_Process.Create(strCommandLine1, Empty, objSWbemObjectEx_Win32_ProcessStartup, lngProcessID) <> 0 Then
			WScript.Echo "Can't create process [" & strCommandLine1 & "]"
			WScript.Quit 1
		End If
		
		With objSWbemObjectEx_Win32_ProcessStartup
			.X = 650
			
			.Title = "Ping 2"
			.FillAttribute = 91
		End With
		
		If objSWbemObjectEx_Win32_Process.Create(strCommandLine2, Empty, objSWbemObjectEx_Win32_ProcessStartup, lngProcessID) <> 0 Then
			WScript.Echo "Can't create process [" & strCommandLine2 & "]"
			WScript.Quit 2
		End If
	End With
End With

WScript.Quit 0
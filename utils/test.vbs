c = "Powershell -noprotile -File ""C:\path\to\My PS File.ps1"""
Set o = CreateObject("WScript.Shell")
Set e = o.Exec(c)
d = e.StdOut.ReadAll
Msgbox d
	
' o.Run "%comspec% /c echo|set /p=" & Mid(MyIP, 2) & "|clip", 0


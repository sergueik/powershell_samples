'
' ((New-Object -Com Shell.Application).NameSpace('shell:::{4234d49b-0245-4df3-b780-3893943456e1}').Items() | ?{$_.Name -eq "Windows PowerShell"}).Verbs() | ?{$_.Name.replace('&','') -match 'Pin to Start'}
' https://answers.microsoft.com/en-us/windows/forum/all/windows-10-1909-why-are-pin-to-starttaskbar-verbs/72194716-9d24-4bdd-bb51-3f7f5ae64047
' origin: https://stackoverflow.com/questions/31720595/pin-program-to-taskbar-using-ps-in-windows-10
' see also: 
' https://docs.microsoft.com/en-us/windows/configuration/configure-windows-10-taskbar#sample-taskbar-configuration-added-to-start-layout-xml-file
'
' see also (obsolete): 
' https://docs.microsoft.com/en-us/answers/questions/87376/powershell-command-to-pin-apps-to-windows-taskbar.html
' see also
' https://superuser.com/questions/1617185/how-to-pin-lnk-files-to-taskbar-in-powershell
If WScript.Arguments.Count < 1 Then WScript.Quit
'----------------------------------------------------------------------
Set objFSO = CreateObject("Scripting.FileSystemObject")
objFile    = WScript.Arguments.Item(0)
sKey1      = "HKCU\Software\Classes\*\shell\{:}\\"
sKey2      = Replace(sKey1, "\\", "\ExplorerCommandHandler")
'----------------------------------------------------------------------
With WScript.CreateObject("WScript.Shell")
    KeyValue = .RegRead("HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer" & _
        "\CommandStore\shell\Windows.taskbarpin\ExplorerCommandHandler")

    .RegWrite sKey2, KeyValue, "REG_SZ"

    With WScript.CreateObject("Shell.Application")
        With .Namespace(objFSO.GetParentFolderName(objFile))
            With .ParseName(objFSO.GetFileName(objFile))
                .InvokeVerb("{:}")
            End With
        End With
    End With

    .Run("Reg.exe delete """ & Replace(sKey1, "\\", "") & """ /F"), 0, True
End With
'----------------------------------------------------------------------

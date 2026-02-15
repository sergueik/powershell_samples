Option Explicit

Dim filename : filename = CreateObject("WScript.Shell").ExpandEnvironmentStrings("%USERPROFILE%\Desktop\test.xls") 
Dim fso: set fso = CreateObject("Scripting.FileSystemObject")
If fso.FileExists(filename) = False Then 
  WScript.Echo "File does not exist " & filename 
  WScript.Quit 1
end if
' detect owner / lock file - hidden file named ~$test.xls
Dim lockFile : lockFile = fso.GetParentFolderName(filename) & "\~$" & fso.GetFileName(filename)
If fso.FileExists(lockFile) Then
  WScript.Echo "Lock file exists (Excel crash or open instance): " & lockFile
  WScript.Quit 3
End If
Dim sheetName : sheetName = "Test"
Dim cellAddress : cellAddress = "A2"

Dim text, runCount

Dim excel
Set excel = CreateObject("Excel.Application")

If Err.Number<>0 Then 
  WScript.Echo "Lacking Excel Application": WScript.Quit 2
end if

excel.Visible = False
excel.DisplayAlerts = False

' Open workbook
Dim workbook
Set workbook = excel.Workbooks.Open(filename)

' Deal with Excel crash/lock recovery mechanism.
' 
If Err.Number<>0 Then 
  WScript.Echo "Cannot open " & filename
  WScript.Quit 4
end if

If workbook.ReadOnly Then 
  WScript.Echo "workbook is ReadOnly: " & filename
  'There is no safe API to unlock this from script.
  WScript.Quit 4
end if

Dim worksheet
Set worksheet = workbook.Worksheets(sheetName)
Dim range
Set range = worksheet.Range(cellAddress)

' Try..Catch
' NOTE: not
' If Not range.Comment Is Nothing Then
'
runCount = 0
text = ""
On Error Resume Next
text = range.Comment.Text
If Err.Number <> 0 Then
    ' cell has no comment
    text = ""
    Err.Clear
End If
On Error Goto 0

If text <> "" Then
    Dim regexp : Set regexp = New RegExp : regexp.Pattern = "Run\s*#(\d+)" : regexp.IgnoreCase = True : regexp.Global = False
    Dim match: Set match = regexp.Execute(text)
    If match.Count > 0 Then
        runCount = CInt(match(0).SubMatches(0))
    End If
End If

runCount = runCount + 1

' Try..Catch
On Error Resume Next
range.Comment.Delete
On Error Goto 0

text = "Run #" & runCount

' NOTE: AddComment returns a COM object, beware of implicit output
range.AddComment text

WScript.Echo "Updated comment in " & cellAddress & ":" & vbCrLf & range.Comment.Text

workbook.Save
workbook.Close(True)
excel.Quit

Set range = Nothing
Set worksheet = Nothing
Set workbook = Nothing
Set excel = Nothing


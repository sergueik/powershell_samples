Option Explicit

Dim excelFile, sheetName, cellAddress
Dim excel, wb, sheet, rng
Dim oldText, newText, runCount
Dim c

' Path to Excel file
excelFile = CreateObject("WScript.Shell").ExpandEnvironmentStrings("%USERPROFILE%\Desktop\test.xls")
sheetName = "Test"
cellAddress = "A2"

' Create Excel COM object
Set excel = CreateObject("Excel.Application")
excel.Visible = False
excel.DisplayAlerts = False

' Open workbook
Set wb = excel.Workbooks.Open(excelFile)
Set sheet = wb.Worksheets(sheetName)
Set rng = sheet.Range(cellAddress)

' -----------------------------
' OLD BAD ATTEMPT (kept commented for reference)
' -----------------------------
' If Not rng.Comment Is Nothing Then
'     MsgBox "Existing comment: " & rng.Comment.Text
' Else
'     MsgBox "No comment in cell " & cellAddress
' End If
' -----------------------------

' Read existing comment safely
runCount = 0
oldText = ""
On Error Resume Next
oldText = rng.Comment.Text
If Err.Number <> 0 Then
    ' No comment exists
    oldText = ""
    Err.Clear
End If
On Error Goto 0

' Try to extract Run #N
If oldText <> "" Then
    Dim re, matches
    Set re = New RegExp
    re.Pattern = "Run\s*#(\d+)"
    re.IgnoreCase = True
    re.Global = False
    Set matches = re.Execute(oldText)
    If matches.Count > 0 Then
        runCount = CInt(matches(0).SubMatches(0))
    End If
End If

runCount = runCount + 1

' Remove old comment if present
On Error Resume Next
rng.Comment.Delete
On Error Goto 0

' Build new comment text
newText = "Run #" & runCount ' & vbCrLf & "Hello from VBScript COM automation at " & Now

' Add new comment (this returns a COM object, beware of implicit output)
rng.AddComment newText

' Display updated comment
MsgBox "Updated comment in " & cellAddress & ":" & vbCrLf & rng.Comment.Text

' Save and close
wb.Save
wb.Close
excel.Quit

' Release objects
Set rng = Nothing
Set sheet = Nothing
Set wb = Nothing
Set excel = Nothing


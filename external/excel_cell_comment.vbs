Option Explicit

Dim excelFile : excelFile = CreateObject("WScript.Shell").ExpandEnvironmentStrings("%USERPROFILE%\Desktop\test.xls") 
Dim sheetName : sheetName = "Test"
Dim cellAddress : cellAddress = "A2"

' Deal with Excel crash/lock recovery mechanism.
' check for
Dim text, runCount

Dim excel
Set excel = CreateObject("Excel.Application")
excel.Visible = False
excel.DisplayAlerts = False

' Open workbook
Dim workbook
Set workbook = excel.Workbooks.Open(excelFile)
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
workbook.Close
excel.Quit

Set range = Nothing
Set worksheet = Nothing
Set workbook = Nothing
Set excel = Nothing


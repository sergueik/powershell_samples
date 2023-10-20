Imports System.IO
Imports System.Reflection
Public Class EditLineList
    Implements IEnumerable

    Private Const NO_FILE_PASSED As String = "\\\NoFile\\\"
    Private Const NO_LINE_NUMBER_PASSED_UPPER As Integer = -1
    Private Const NO_LINE_NUMBER_PASSED_LOWER As Integer = 0

    Private m_lineList As New ArrayList()
    Private m_lineNumber As Integer = 0
    Private m_fileName As String = String.Empty

    Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return New ArrayListEnumerator(m_lineList)
    End Function

    Public ReadOnly Property Count() As Integer
        Get
            Return m_lineList.Count
        End Get
    End Property

    Public Property fileName() As String
        Get
            Return m_fileName
        End Get
        Set(ByVal value As String)
            m_fileName = value
        End Set
    End Property

    Public Property lineNumber() As Integer
        Get
            Return m_lineNumber
        End Get
        Set(ByVal value As Integer)
            If value = 0 OrElse value < m_lineList.Count Then
                m_lineNumber = value
            Else
                Throw New Exception(String.Format("lineNumber: {0} is out of range; the upper limit is {1}.", value, m_lineList.Count - 1))
            End If
        End Set
    End Property

    Public Function Add(ByVal itm As EditLine) As Boolean
        Dim rVal As Boolean = True
        Try
            m_lineList.Add(itm)
            Me.lineNumber = m_lineList.Count - 1
        Catch ex As Exception
            rVal = False
        End Try
        Return rVal
    End Function

    Public Function Delete(Optional ByVal rangeStart As Integer = NO_LINE_NUMBER_PASSED_UPPER, Optional ByVal rangeEnd As Integer = NO_LINE_NUMBER_PASSED_UPPER) As Boolean
        If rangeStart = NO_LINE_NUMBER_PASSED_UPPER Then
            rangeStart = Me.lineNumber
        End If
        If rangeEnd = NO_LINE_NUMBER_PASSED_UPPER Then
            rangeEnd = Me.lineNumber
        End If
        Dim rVal As Boolean = True
        Try
            m_lineList.RemoveRange((rangeStart), (rangeEnd - rangeStart) + 1)
            If Me.lineNumber > m_lineList.Count Then
                Me.lineNumber = m_lineList.Count - 1
            End If
        Catch ex As Exception
            rVal = False
        End Try
        Return rVal

    End Function

    Public Function Insert(ByVal index As Integer, ByVal itm As EditLine) As Boolean
        Dim rVal As Boolean = True
        Try
            If index < m_lineList.Count Then
                m_lineList.Insert(index, itm)
                Me.lineNumber = index
            Else
                Throw New Exception(String.Format("Insert: {0} is out of range; the upper limit is {1}.", index, m_lineList.Count - 1))
            End If
        Catch ex As Exception
            rVal = False
        End Try
        Return rVal
    End Function

    Default Public ReadOnly Property this(ByVal i As Integer) As EditLine
        Get
            If i < m_lineList.Count Then
                Dim od As EditLine = CType(m_lineList(i), EditLine)
                'Dim od As LineEditorLine = Me(i)
                Return od
            Else
                Throw New Exception(String.Format("this: {0} is out of range; the upper limit is {1}.", i, m_lineList.Count - 1))
            End If
        End Get
    End Property

    ''' <summary>
    ''' ToString
    ''' For our implementation, return the current EditLine
    ''' </summary>
    Public Overrides Function ToString() As String
        Return Me(Me.lineNumber).ToString()
    End Function

    Public Function insertLines(Optional ByVal lineNumber = NO_LINE_NUMBER_PASSED_UPPER) As Boolean
        Dim rVal As Boolean = True
        Dim isAppendMode As Boolean = False
        Dim lineNum As Integer = lineNumber
        Try

            If lineNumber = NO_LINE_NUMBER_PASSED_UPPER Then
                lineNumber = Me.lineNumber
                lineNum = lineNumber
            End If
            If Me.Count = 0 OrElse lineNumber = Me.Count Then
                isAppendMode = True
            Else
                Me.lineNumber = lineNumber
            End If
            'Console.CursorTop

            Dim ecode As editExitCode
            Dim thisLine As New EditLine
            Do
                Console.Write("   {0}:*", lineNum + 1)
                ecode = thisLine.edit()

                Select Case ecode
                    Case editExitCode.normal
                        ' The user presses ENTER:
                        ' add this line to the list and allow them
                        ' to add another
                        Dim newLine As New EditLine(thisLine.ToString())
                        ' Insert doesn't work if there isn't any lines in the list
                        If isAppendMode = True Then
                            Me.Add(newLine)
                            'Me.lineNumber = 0
                        Else
                            Me.Insert(Me.lineNumber, newLine)
                            Me.lineNumber += 1
                        End If
                        lineNum += 1
                        Console.WriteLine()
                        thisLine = New EditLine
                    Case editExitCode.escape
                        ' The user presses ESCAPE
                        ' Ignore this line and try again
                        Console.WriteLine("\")
                    Case editExitCode.ctlc
                        ' The user presses CONTROL+C:
                        ' End inserting (ignore this line)
                        Console.WriteLine("^C")
                End Select

            Loop Until ecode = editExitCode.ctlc
            If isAppendMode = True Then
                Me.lineNumber = Me.Count - 1
            End If

        Catch ex As Exception
            rVal = False
        End Try
        Return rVal
    End Function

    Public Function edit(Optional ByVal lineNumber = NO_LINE_NUMBER_PASSED_UPPER) As Boolean
        Dim rVal As Boolean = True
        Try

            If lineNumber = NO_LINE_NUMBER_PASSED_UPPER Then
                lineNumber = Me.lineNumber
            End If
            Me.lineNumber = lineNumber

            Dim ecode As editExitCode

            Dim thisLine As EditLine = Me(Me.lineNumber)
            Console.WriteLine()
            Console.WriteLine("   {0}:*{1}", Me.lineNumber + 1, thisLine.ToString())
            Do
                Console.Write("   {0}:*", Me.lineNumber + 1)
                ecode = thisLine.edit()

                Select Case ecode
                    Case editExitCode.normal
                        Console.WriteLine()
                    Case editExitCode.escape
                        Console.WriteLine("\")
                    Case editExitCode.ctlc
                        Console.WriteLine("^C")
                End Select

            Loop Until ecode = editExitCode.normal OrElse ecode = editExitCode.ctlc
        Catch E As Exception
            rVal = False
        End Try
        Return rVal
    End Function

    ''' <summary>
    ''' loadFile
    ''' Read a text file into the list
    ''' </summary>
    ''' <param name="fileName">The filename of the file to open</param>
    ''' <returns>true: success; false: thrown exception</returns>
    ''' <remarks></remarks>
    Public Function loadFile(Optional ByVal fileName As String = NO_FILE_PASSED) As Boolean
        Dim rVal As Boolean = True
        Try
            ' If the user doesn't name a fileName, we want to use the previous filename
            If (fileName = NO_FILE_PASSED) Then
                fileName = Me.fileName
            End If
            Me.fileName = fileName

            ' Create an instance of StreamReader to read from a file.
            Using sr As StreamReader = New StreamReader(fileName)
                Dim line As String
                ' Read and display the lines from the file until the end 
                ' of the file is reached.
                Do
                    line = sr.ReadLine()
                    If Not line Is Nothing Then
                        Me.Add(New EditLine(line))
                    End If
                Loop Until line Is Nothing
                sr.Close()
                Me.lineNumber = 0
            End Using
        Catch E As Exception
            rVal = False
            ' Let the user know what went wrong.
            System.Diagnostics.Debug.WriteLine("The file '" & fileName & "' could not be read:")
            System.Diagnostics.Debug.WriteLine(E.Message)
        End Try
        Return rVal
    End Function

    ''' <summary>
    ''' saveFile
    ''' Write the lines to a text file
    ''' </summary>
    ''' <param name="fileName"></param>
    ''' <returns>true: success; false: thrown exception</returns>
    ''' <remarks></remarks>
    Public Function saveFile(Optional ByVal fileName As String = NO_FILE_PASSED) As Boolean
        Dim rVal As Boolean = True
        Try
            ' If the user doesn't name a fileName, we want to use the previous filename
            If (fileName = NO_FILE_PASSED) Then
                fileName = Me.fileName
            End If
            Me.fileName = fileName

            Using sw As StreamWriter = New StreamWriter(fileName)
                ' Add some text to the file.
                Dim thisLine As EditLine
                For Each thisLine In Me
                    sw.WriteLine(thisLine.ToString())
                Next
                sw.Close()
                Console.WriteLine(String.Format("{0} written", Me.fileName))
            End Using

        Catch E As Exception
            rVal = False
            ' Let the user know what went wrong.
            System.Diagnostics.Debug.WriteLine("The file '" & fileName & "' could not be written:")
            System.Diagnostics.Debug.WriteLine(E.Message)
        End Try

        Return rVal
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="rangeStart"></param>
    ''' <param name="rangeEnd"></param>
    ''' <returns>true: success; false: thrown exception</returns>
    ''' <remarks></remarks>
    Public Function listLines(Optional ByVal rangeStart As Integer = NO_LINE_NUMBER_PASSED_LOWER, Optional ByVal rangeEnd As Integer = NO_LINE_NUMBER_PASSED_UPPER) As Boolean

        Dim rVal As Boolean = True
        Try
            ' Since an optional paramter must be a constant, and I want it to be 
            ' the last possible line, so I give it an invalid default value and
            ' test for it here and set it to what I really want.
            ' Since the lower bound is always 0, I can just use the constant
            If rangeEnd = NO_LINE_NUMBER_PASSED_UPPER Then
                rangeEnd = m_lineList.Count
            End If

            Dim i As Integer
            For i = rangeStart To rangeEnd
                Dim thisLine As EditLine = Me(i)
                Console.Write("   {0}:", i + 1)
                Console.Write(IIf(i = Me.lineNumber, "*", " "))
                Console.Write(thisLine.ToString())
                Console.WriteLine()
            Next i
        Catch ex As Exception
            rVal = False
            ' Let the user know what went wrong.
            System.Diagnostics.Debug.WriteLine("Error listing lines")
        End Try
        Return rVal

    End Function

    ''' <summary>
    ''' listLinesPage
    ''' write a page of lines to the console; the current page at the middle of the list 
    ''' (if not close to either end of the list
    ''' </summary>
    ''' <returns>true: success; false: thrown exception</returns>
    ''' <remarks></remarks>
    Public Function listLinesPage() As Boolean
        Dim firstLine As Integer
        Dim lastLine As Integer
        If Me.lineNumber < 7 Then
            firstLine = 0
        Else
            firstLine = Me.lineNumber - 7
        End If
        lastLine = firstLine + 15
        If lastLine > m_lineList.Count Then
            lastLine = m_lineList.Count
        End If

        Return listLines(firstLine, lastLine)
    End Function

    ''' <summary>
    ''' editFile
    ''' Main command mode processing here
    ''' Gets and handles user commands when not in edit mode
    ''' This sub uses little new console functionality
    ''' </summary>
    ''' <param name="fileName">The fully qualified name of the file we are editing</param>
    ''' <returns>true: success; false: error thrown</returns>
    ''' <see>EditLine.edit() for more advanced keyboard and cursor handling</see>
    ''' <remarks></remarks>
    Public Function editFile(ByVal fileName As String) As Boolean
        Dim rVal As Boolean
        Try
            Me.fileName = fileName
            If File.Exists(fileName) Then
                Me.loadFile()
                Console.WriteLine("End of input file")
            Else
                Console.WriteLine("New file")
            End If

            ' You can set the title of the Console window!
            Console.Title = "Line Editor " & fileName

            Dim inputValue As String
            Dim commandValue As String
            Dim firstLine As Integer
            Dim lastLine As Integer
            Dim ontainsRange As Boolean
            'Console.TreatControlCAsInput = True
            Do
                Console.Write("*")
                inputValue = Console.ReadLine()
                If inputValue.Length > 0 Then
                    commandValue = getCommandName(inputValue)

                    getLineRange(inputValue, commandValue, Me, firstLine, lastLine, ontainsRange)

                    Select Case commandValue.ToUpper()
                        Case "W" ' Write
                            Me.saveFile()
                        Case "E" ' Exit (save first)
                            Me.saveFile()
                            Exit Do
                        Case "D" ' Delete
                            If ontainsRange = True Then
                                If inputValue.IndexOf(",") >= 0 Then
                                    Me.Delete(firstLine - 1, lastLine - 1)
                                Else
                                    Me.Delete(firstLine - 1, firstLine - 1)
                                End If
                            Else
                                Me.Delete()
                            End If
                        Case "Q" ' Quit (no save)
                            If getAbortYN() = True Then
                                Exit Do
                            End If
                        Case "L" ' List lines
                            If ontainsRange = False Then
                                Me.listLinesPage()
                            Else
                                Me.listLines(firstLine - 1, lastLine - 1)
                            End If
                        Case "?" ' Help
                            Console.WriteLine(getHelpText())
                        Case "I" ' Insert
                            If firstLine <= 0 Then
                                Me.insertLines()
                            Else
                                Me.insertLines(firstLine - 1)
                            End If
                        Case Else
                            ' If you entered a number, edit that line
                            ' otherwise, just ignore it and move along
                            Dim hasError As Boolean = True
                            If IsNumeric(inputValue) Then
                                hasError = Not (Me.edit(Integer.Parse(inputValue) - 1))
                            End If
                            If hasError = True Then
                                Console.WriteLine("Entry error")
                            End If
                    End Select
                End If
            Loop
            rVal = True
        Catch ex As Exception
            rVal = False
        End Try
        Return rVal
    End Function

    ''' <summary>
    ''' getAbortYN
    ''' When the user asks to quit, we want to make sure that they are sure
    ''' We ask to the user over and over again until we get a "Y" or a "N"
    ''' </summary>
    ''' <returns>true, "Y", false "N"</returns>
    Private Function getAbortYN() As Boolean
        Dim theKey As ConsoleKeyInfo
        Dim keyValue As String
        Dim rVal As Boolean = False
        Do
            Console.Write("Abort edit (Y/N)?")
            ' Get the key code
            theKey = Console.ReadKey()
            keyValue = theKey.KeyChar.ToString().ToUpper()
            Console.WriteLine()
            Select Case keyValue
                Case "Y"
                    rVal = True
                    Exit Do
                Case "N"
                    rVal = False
                    Exit Do
            End Select
        Loop
        Return rVal
    End Function

    ''' <summary>
    ''' getCommandName
    ''' Get the command from the command string
    ''' If we want to support all of edlin's commands, we will have to get
    ''' fancier that this
    ''' </summary>
    ''' <param name="commandString">The raw string that the user entered</param>
    ''' <returns>The command character</returns>
    ''' <remarks></remarks>
    Private Function getCommandName(ByVal commandString As String) As String
        Return commandString.Substring(commandString.Trim().Length - 1)
    End Function

    ''' <summary>
    ''' getLineRange
    ''' Get first and last line # from a raw command string and a commandValue
    ''' in a command like "1,7l", the range is "1,7" and the command is "l" (for list).
    ''' </summary>
    ''' <param name="commandString">The raw string that the user entered</param>
    ''' <param name="commandVal">
    ''' The char that represents the command (parsed earlier)
    ''' The line range is the part of the command string before the 
    ''' command name
    ''' </param>
    ''' <param name="lineList">We use the lineList to get the default top value</param>
    ''' <param name="firstLine">Output: the first line in the range</param>
    ''' <param name="lastLine">Output: the last line in the range</param>
    ''' <param name="containsRange">Output: Did we have an actual range in the command string</param>
    ''' <returns>true: success; false: failure</returns>
    Public Function getLineRange(ByVal commandString As String, ByVal commandVal As String, ByRef lineList As EditLineList, ByRef firstLine As Integer, ByRef lastLine As Integer, ByRef containsRange As Boolean) As Boolean
        Dim rVal As Boolean = True
        Dim parseParts As String()
        Try
            'commandString = commandString.Replace(commandVal, String.Empty)
            parseParts = commandString.Split(commandVal)
            If parseParts(0) = String.Empty Then
                containsRange = False
                firstLine = 0
                lastLine = lineList.Count() - 1
            Else
                containsRange = True
                Dim numbers As String() = Split(parseParts(0), ",")
                If (numbers.Length > 0) AndAlso numbers(0) <> String.Empty Then
                    firstLine = Integer.Parse(numbers(0))
                Else
                    firstLine = 0
                End If
                If (numbers.Length > 1) AndAlso numbers(1) <> String.Empty Then
                    lastLine = Integer.Parse(numbers(1))
                Else
                    lastLine = lineList.Count() - 1
                End If
            End If
        Catch ex As Exception
            rVal = False
        End Try
        Return rVal
    End Function

    ''' <summary>
    ''' getHelpText
    ''' Gets the help text from embedded resources
    ''' </summary>
    ''' <returns>The help string</returns>
    Private Function getHelpText() As String
        Dim Asm As Assembly = Assembly.GetExecutingAssembly()

        ' Resources are named using a fully qualified name.
        Dim strm As Stream = Asm.GetManifestResourceStream("LineEditor.Net.help.txt")

        ' Reads the contents of the embedded file.
        Dim reader As New StreamReader(strm)
        Return reader.ReadToEnd()

    End Function

End Class

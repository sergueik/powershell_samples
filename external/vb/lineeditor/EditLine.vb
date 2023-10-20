
Public Enum editExitCode
    normal = 1
    escape = 2
    ctlc = 3
End Enum

Public Class EditLine


    'Private m_cancel As Boolean = False
    Private m_line As String = String.Empty

    Public Sub New()
    End Sub

    Public Sub New(ByVal newLine As String)
        Me.line = newLine
    End Sub

    Public Overrides Function ToString() As String
        Return Me.line
    End Function

    Public Property line() As String
        Get
            Return m_line
        End Get
        Set(ByVal value As String)
            m_line = value
        End Set
    End Property

    Public Sub replace(ByVal oldString As String, ByVal newString As String)
        Me.line = Me.line.Replace(oldString, newString)
    End Sub
    Public Function edit() As editExitCode

        Dim rVal As editExitCode = editExitCode.ctlc
        Dim pontentialBuffer As String = Me.line
        Dim potentialPosPtr As UInteger = 0
        Dim excepted As String = String.Empty
        Dim thePosition As UInteger = 0

        Dim theKey As ConsoleKeyInfo
        ' Turn off Control+C tracking, we use the key sequence to
        ' end editing for 
        Console.TreatControlCAsInput = True

        ' This loop is the keyboard reading loop.  Here we read all of the
        ' key strokes and process it
        Do
            theKey = Console.ReadKey(True)
            Select Case theKey.Key
                Case ConsoleKey.Enter
                    ' Enter ends line editing
                    rVal = editExitCode.normal
                    Exit Do
                Case ConsoleKey.Escape
                    ' So does escape
                    excepted = Me.line
                    rVal = editExitCode.escape
                    Exit Do
                Case ConsoleKey.End, ConsoleKey.F3
                    ' Move cursor to the end
                    Console.Write(pontentialBuffer)
                    excepted += pontentialBuffer
                    potentialPosPtr += pontentialBuffer.Length
                    thePosition = excepted.Length
                    pontentialBuffer = String.Empty
                Case ConsoleKey.Backspace, ConsoleKey.LeftArrow
                    ' Move the cursor to the left, if it is Backspace,
                    ' the character goes away
                    If thePosition > 0 Then
                        Console.CursorLeft -= 1
                        Console.Write(" ")
                        Console.CursorLeft -= 1
                        thePosition -= 1
                        ' if it is LeftArrow, the character in
                        ' pontentialBuffer
                        If theKey.Key = ConsoleKey.LeftArrow Then
                            If excepted.Length > 0 Then
                                pontentialBuffer = excepted.Substring(excepted.Length - 1) & pontentialBuffer
                            End If
                        End If
                        excepted = excepted.Substring(0, excepted.Length - 1)
                        If potentialPosPtr > 0 Then
                            potentialPosPtr -= 1
                        End If
                    End If
                Case ConsoleKey.RightArrow
                    ' Move the cursor one character to the right and 
                    ' show the next character in the pontentialBuffer
                    If pontentialBuffer <> String.Empty Then
                        If potentialPosPtr < Me.line.Length Then
                            potentialPosPtr += 1
                        End If
                        Dim itemSt = pontentialBuffer.Substring(0, 1)
                        pontentialBuffer = Me.line.Substring(potentialPosPtr)
                        Console.Write(itemSt)
                        excepted &= itemSt
                        thePosition += 1
                    End If
                Case ConsoleKey.Delete
                    ' Remove the next character from potentialPosPtr
                    If potentialPosPtr < Me.line.Length Then
                        potentialPosPtr += 1
                    End If
                    If pontentialBuffer <> String.Empty Then
                        pontentialBuffer = Me.line.Substring(potentialPosPtr)
                    End If
                Case ConsoleKey.UpArrow, ConsoleKey.DownArrow _
                    , ConsoleKey.F1 To ConsoleKey.F12
                    ' Ignore theses keys.  To make a real app, we need to 
                    ' ignore more keys.  There are probably some dangerous
                    ' keys that can get to Case Else
                Case Else
                    ' Everything else is echoed
                    If Not (theKey.Key = ConsoleKey.C And theKey.Modifiers = ConsoleModifiers.Control) Then
                        Console.Write(theKey.KeyChar)
                        excepted &= theKey.KeyChar
                        If potentialPosPtr < Me.line.Length Then
                            potentialPosPtr += 1
                        End If
                        If pontentialBuffer <> String.Empty Then
                            pontentialBuffer = Me.line.Substring(potentialPosPtr)
                        End If
                        thePosition += 1
                    End If
            End Select
            ' Control+c breaks us out of the loop
            ' We don't handle this in the Select Case
        Loop Until theKey.Key = ConsoleKey.C And theKey.Modifiers = ConsoleModifiers.Control
        ' Turn Control+C tracking back on.
        Console.TreatControlCAsInput = False

        Me.line = excepted
        Return rVal

    End Function

End Class

' Copyright (c) 2006, Jack Stephens
' 
' Redistributions of source code must retain the above copyright notice, 
' and the following disclaimer.
'
' THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
' AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
' ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE 
' FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES 
' (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
' LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND 
' ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
' (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, 
' EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
'
Imports System.io
Imports System.Reflection
Module MainModule

    ''' <summary>
    ''' Sub Main
    ''' The entry point of the program
    ''' </summary>
    ''' <remarks></remarks>
    Sub Main()

        Dim lineList As New EditLineList

        Dim fileName As String = String.Empty
        If My.Application.CommandLineArgs.Count > 0 Then
            fileName = My.Application.CommandLineArgs(0)
            Dim fullPath As String = Path.GetDirectoryName(fileName)
            If fullPath Is Nothing OrElse fullPath = String.Empty Then
                fileName = Path.Combine(CurDir(), fileName)
            End If
            lineList.editFile(fileName)
        Else
            Console.WriteLine("File name must be specified")
        End If

    End Sub

End Module
' For Replace, use Split(inputValue, Chr(26))
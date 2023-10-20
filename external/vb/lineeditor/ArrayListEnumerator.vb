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
''' <summary>
''' ArrayListEnumerator
''' This is a standard class that I often use to implement a GetEnumerator
''' on a list class.
''' </summary>
''' <remarks></remarks>
Public Class ArrayListEnumerator
    Implements IEnumerator

    ' The current position within the list
    Private m_currentPos As Integer = -1
    ' The container of the objects that we are going to enumerate
    Private m_itemList As ArrayList

    ''' <summary>
    ''' New
    ''' Called by the GetEnumerator method
    ''' </summary>
    ''' <param name="ar">The ArrayList from the calling List class</param>
    ''' <remarks></remarks>
    Public Sub New(ByRef ar As ArrayList)
        m_itemList = ar
    End Sub

    ''' <summary>
    ''' Current 
    ''' Gets object in m_itemList pointed to by m_currentPos
    ''' </summary>
    ''' <returns>The current object</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Current() As Object Implements System.Collections.IEnumerator.Current
        Get
            Return m_itemList(m_currentPos)
        End Get
    End Property

    ''' <summary>
    ''' MoveNext
    ''' Increments m_currentPos and checks to see that it doesn't point past the end of m_itemList
    ''' </summary>
    ''' <returns>
    ''' True: success, we can increment m_currentPos and not pass the end of m_itemList;
    ''' False: failure, this increment would force m_currentPos pass the end of m_itemList.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function MoveNext() As Boolean Implements System.Collections.IEnumerator.MoveNext
        If m_currentPos < m_itemList.Count - 1 Then
            System.Math.Min(System.Threading.Interlocked.Increment(m_currentPos), m_currentPos - 1)
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Reset
    ''' Rewinds the list by setting m_currentPos to 1 spot before the beginning of m_itemList
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Reset() Implements System.Collections.IEnumerator.Reset
        m_currentPos = -1
    End Sub
End Class

Public Class Form1

	' origin: https://www.codeproject.com/articles/Line-Numbering-of-RichTextBox-in-NET-2-0
	' https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.richtextbox?view=netframework-4.5
	' https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/richtextbox-control-overview-windows-forms
 
   Private Sub DrawRichTextBoxLineNumbers(ByRef g As Graphics)
    'calculate font heigth as the difference in Y coordinate between line 2 and line 1
      'note that the RichTextBox text must have at least two lines. So the initial Text property of the RichTextBox should not be an empty string. It could be something like vbcrlf & vbcrlf & vbcrlf 
      Dim font_height As Single = MyRichTextBox.GetPositionFromCharIndex(MyRichTextBox.GetFirstCharIndexFromLine(2)).Y - MyRichTextBox.GetPositionFromCharIndex(MyRichTextBox.GetFirstCharIndexFromLine(1)).Y
      If font_height = 0 Then Exit Sub

      'Get the first line index and location
      Dim firstIndex As Integer = MyRichTextBox.GetCharIndexFromPosition(New Point(0, g.VisibleClipBounds.Y + font_height / 3))
      Dim firstLine As Integer = MyRichTextBox.GetLineFromCharIndex(firstIndex)
      Dim firstLineY As Integer = MyRichTextBox.GetPositionFromCharIndex(firstIndex).Y

      'Print on the PictureBox the visible line numbers of the RichTextBox
      g.Clear(Control.DefaultBackColor)
      Dim i As Integer = firstLine
      Dim y As Single
      Do While y < g.VisibleClipBounds.Y + g.VisibleClipBounds.Height
         y = firstLineY + 2 + font_height * (i - firstLine - 1)
         g.DrawString((i).ToString, MyRichTextBox.Font, Brushes.DarkBlue, MyPictureBox.Width - g.MeasureString((i).ToString, MyRichTextBox.Font).Width, y)
         i += 1
      Loop
      'Debug.WriteLine("Finished: " & firstLine + 1 & " " & i - 1)
   End Sub

   Private Sub r_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyRichTextBox.Resize
      MyPictureBox.Invalidate()
   End Sub

   Private Sub r_VScroll(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyRichTextBox.VScroll
      MyPictureBox.Invalidate()
   End Sub

   Private Sub p_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles MyPictureBox.Paint
      DrawRichTextBoxLineNumbers(e.Graphics)
   End Sub

   Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		MyRichTextBox.Rtf = "{\rtf1\ansi\deff0 " & vbCrLf & _
			"{\fonttbl{\f0\fswiss\fcharset0 Arial;}} " & vbCrLf  & _
			"\pard\sa200\sl276\slmult1\f0\fs24 " & vbCrLf & _
			"This is visible text. This is also visible text.  " & vbCrLf & _
			"} "
		MyRichTextBox.Rtf = "{\rtf1\ansi\deff0 " & vbCrLf & _
			"{\fonttbl{\f0\fswiss\fcharset0 Arial;}} " & vbCrLf & _
			"\pard\sa200\sl276\slmult1\f0\fs24  " & vbCrLf & _
			"This is visible text. {\v This text is hidden.} This is also visible text. " & vbCrLf & _
			"} "
   End Sub
End Class

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
      Me.MyRichTextBox = New System.Windows.Forms.RichTextBox
      Me.MyPictureBox = New System.Windows.Forms.PictureBox
      CType(Me.MyPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
      Me.SuspendLayout()
      '
      'MyRichTextBox
      '
      Me.MyRichTextBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                  Or System.Windows.Forms.AnchorStyles.Left) _
                  Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
      Me.MyRichTextBox.Font = New System.Drawing.Font("Courier New", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
      Me.MyRichTextBox.Location = New System.Drawing.Point(49, 7)
      Me.MyRichTextBox.Name = "MyRichTextBox"
      Me.MyRichTextBox.Size = New System.Drawing.Size(230, 247)
      Me.MyRichTextBox.TabIndex = 0
      Me.MyRichTextBox.Text = "" & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(10)
      Me.MyRichTextBox.WordWrap = False
      '
      'MyPictureBox
      '
      Me.MyPictureBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                  Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
      Me.MyPictureBox.Location = New System.Drawing.Point(2, 7)
      Me.MyPictureBox.Name = "MyPictureBox"
      Me.MyPictureBox.Size = New System.Drawing.Size(41, 247)
      Me.MyPictureBox.TabIndex = 2
      Me.MyPictureBox.TabStop = False
      '
      'Form1
      '
      Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
      Me.ClientSize = New System.Drawing.Size(291, 261)
      Me.Controls.Add(Me.MyPictureBox)
      Me.Controls.Add(Me.MyRichTextBox)
      Me.Name = "Form2"
      Me.Text = "Form2"
      CType(Me.MyPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
      Me.ResumeLayout(False)

   End Sub
   Friend WithEvents MyRichTextBox As System.Windows.Forms.RichTextBox
   Friend WithEvents MyPictureBox As System.Windows.Forms.PictureBox

End Class

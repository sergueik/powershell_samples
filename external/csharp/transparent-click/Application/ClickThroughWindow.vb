' This example implements a "click-through" window using Win32's built-in
' Extended Style window attributes.
'
' To make the form transparent to the mouse, you must set the window's
' "Transparent" and "Layered" attributes.  Once these are set, any mouse clicks
' are ignored by this form and sent to the window behind this one.
'
' At the start of this application, the window while be shown in "Transparent" mode.
' Any mouse clicks in this window will not fire events in this application.  If you 
' click anywhere in the window in "Transparent" mode, the window, and controls, behind
' this one will get the input focus.
'
' To restore the input focus to this window, you'll have to click on it's icon in
' TaskBar, or switch to it using Alt-Tab.
'
' While this window has the focus, you can hold down the Shift and Ctrl keys to
' get the window to turn off it's "Transparency" to the mouse.  This will allow
' you to play with the trackbars that control the windows Alpha in both Transparent
' and Opaque modes, which, BTW, shows that Alpha has nothing to do with the windows
' visibility to the mouse
'
' Try setting the Transparent Alpha to 100%, let go of the Shift-Ctrl keys and start
' clicking on, and dragging around, the window.

Imports Window_Library.WindowLibrary.User32Wrappers

Public Class ClickThroughWindow
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents waaValue As System.Windows.Forms.Label
    Friend WithEvents wtaValue As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents WindowTransparentAlpha As System.Windows.Forms.TrackBar
    Friend WithEvents WindowActiveAlpha As System.Windows.Forms.TrackBar
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.WindowActiveAlpha = New System.Windows.Forms.TrackBar
        Me.WindowTransparentAlpha = New System.Windows.Forms.TrackBar
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.waaValue = New System.Windows.Forms.Label
        Me.wtaValue = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        CType(Me.WindowActiveAlpha, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WindowTransparentAlpha, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'WindowActiveAlpha
        '
        Me.WindowActiveAlpha.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.WindowActiveAlpha.LargeChange = 10
        Me.WindowActiveAlpha.Location = New System.Drawing.Point(8, 72)
        Me.WindowActiveAlpha.Maximum = 100
        Me.WindowActiveAlpha.Minimum = 10
        Me.WindowActiveAlpha.Name = "WindowActiveAlpha"
        Me.WindowActiveAlpha.Size = New System.Drawing.Size(320, 45)
        Me.WindowActiveAlpha.TabIndex = 0
        Me.WindowActiveAlpha.TickFrequency = 10
        Me.WindowActiveAlpha.Value = 90
        '
        'WindowTransparentAlpha
        '
        Me.WindowTransparentAlpha.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.WindowTransparentAlpha.LargeChange = 10
        Me.WindowTransparentAlpha.Location = New System.Drawing.Point(8, 200)
        Me.WindowTransparentAlpha.Maximum = 100
        Me.WindowTransparentAlpha.Minimum = 10
        Me.WindowTransparentAlpha.Name = "WindowTransparentAlpha"
        Me.WindowTransparentAlpha.Size = New System.Drawing.Size(320, 45)
        Me.WindowTransparentAlpha.TabIndex = 1
        Me.WindowTransparentAlpha.TickFrequency = 10
        Me.WindowTransparentAlpha.Value = 60
        '
        'Label1
        '
        Me.Label1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(8, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(320, 40)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "This slider controls this windows Alpha value for when the window is ""not transpa" & _
        "rnet"", or clickable."
        '
        'Label2
        '
        Me.Label2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(8, 136)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(320, 40)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "This slider controls this windows Alpha value for when the window is ""transparent" & _
        """ or click-throughable."
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(8, 56)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(208, 16)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Window Active Alpha"
        '
        'Label4
        '
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(8, 184)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(208, 16)
        Me.Label4.TabIndex = 5
        Me.Label4.Text = "Window Transparent Alpha"
        '
        'waaValue
        '
        Me.waaValue.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.waaValue.Location = New System.Drawing.Point(264, 56)
        Me.waaValue.Name = "waaValue"
        Me.waaValue.Size = New System.Drawing.Size(64, 16)
        Me.waaValue.TabIndex = 6
        Me.waaValue.Text = "0"
        Me.waaValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'wtaValue
        '
        Me.wtaValue.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.wtaValue.Location = New System.Drawing.Point(264, 184)
        Me.wtaValue.Name = "wtaValue"
        Me.wtaValue.Size = New System.Drawing.Size(64, 16)
        Me.wtaValue.TabIndex = 7
        Me.wtaValue.Text = "0"
        Me.wtaValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label7
        '
        Me.Label7.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(8, 256)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(320, 48)
        Me.Label7.TabIndex = 8
        Me.Label7.Text = "Hold down the Ctrl and Shift keys to make this window ""clickable"", though, this w" & _
        "indow has to have the focus for the keys to work!"
        '
        'TransparentWindow
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(336, 310)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.wtaValue)
        Me.Controls.Add(Me.waaValue)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.WindowTransparentAlpha)
        Me.Controls.Add(Me.WindowActiveAlpha)
        Me.KeyPreview = True
        Me.Name = "TransparentWindow"
        Me.Text = "ClickThrough Window"
        CType(Me.WindowActiveAlpha, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WindowTransparentAlpha, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

#End Region

    ' Used to store the Extended Style information of our window, before we start
    ' making any modifications to it.  This will be used to create two different
    ' styles for our window.
    Private m_InitialStyle As Integer


    Private Sub Form_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Grab the Extended Style information for this window and store it.
        m_InitialStyle = GetWindowLong(Me.Handle, GWL.ExStyle)

        ' Set our window to "transparent", or invisible to the mouse.
        SetFormToTransparent()

        ' Update the lables over our track bar controls with the numbers they currently represent.
        UpdateLabelValues()

        ' Make our window the top-most form, system-wide.
        Me.TopMost = True
    End Sub


    Private Sub Form_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        ' NOTE: Notice that the form's KeyPreview property has to be turned ON for this to work!
        ' If both the Shift and Ctrl keys are being held down, set our window
        ' to be "non-transparent", or visible to the mouse.
        If e.Control And e.Shift Then
            SetFormToOpaque()
        End If
    End Sub


    Private Sub Form_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyUp
        ' NOTE: Notice that the form's KeyPreview property has to be turned ON for this to work!
        ' If both the Shift and Ctrl keys are no longer held down, set our window
        ' to be "transparent", or invisible to the mouse.  This has the effect of
        ' allowing mouse clicks to go straight through our window, like it doesn't
        ' even exist.
        If Not (e.Control And e.Shift) Then
            SetFormToTransparent()
        End If
    End Sub


    Private Sub SetFormToTransparent()
        ' This creates a new extended style for our window, which takes effect immediately
        ' upon being set, that combines the initial style of our window and adds the ability
        ' to be transparent to the mouse.  Both Layered and Transparent must be turned on for
        ' this to work and the window to render properly!
        SetWindowLong(Me.Handle, GWL.ExStyle, m_InitialStyle Or WS_EX.Layered Or WS_EX.Transparent)

        ' Set the Alpha for our window to the percentage specified by our TransparentAlpha trackbar.
        ' Note: This has NOTHING to do with making the form transparent to the mouse!  This is solely
        ' for visual effect!
        SetLayeredWindowAttributes(Me.Handle, 0, 255 * (WindowTransparentAlpha.Value / 100), LWA.Alpha)
    End Sub


    Private Sub SetFormToOpaque()
        ' This resets our window's "Transparent" attribute to what it was when the application
        ' was launched.  We're still keeping the "Layered" attribute turned on so our window's
        ' Alpha is maintained and renders properly.
        SetWindowLong(Me.Handle, GWL.ExStyle, m_InitialStyle Or WS_EX.Layered)

        ' Set the Alpha for our window to the percentage specified by our ActiveAlpha trackbar.
        ' Note: This has NOTHING to do with making the form transparent to the mouse!  This is solely
        ' for visual effect!
        SetLayeredWindowAttributes(Me.Handle, 0, 255 * (WindowActiveAlpha.Value / 100), LWA.Alpha)
    End Sub


    Private Sub AlphaScrollers_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles WindowActiveAlpha.Scroll, WindowTransparentAlpha.Scroll
        ' When either of the trackbar's are moved, we need to update the corresponding
        ' label controls with the new values.  Note that this method handles the Scrool
        ' event for BOTH trackbar controls!  See the "Handles" clause at the end of the
        ' function header.
        UpdateLabelValues()
    End Sub

    Private Sub UpdateLabelValues()
        ' Update the text property of both label controls (over the right end of their
        ' corresponding trackbar controls), with the values of both trackbars.
        waaValue.Text = Format(WindowActiveAlpha.Value / 100, "#0%")
        wtaValue.Text = Format(WindowTransparentAlpha.Value / 100, "#0%")
    End Sub
End Class

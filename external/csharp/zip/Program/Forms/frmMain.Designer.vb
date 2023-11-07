<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuFileNew = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuFileOpen = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuFileFavorites = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuFileClose = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator
        Me.mnuFileExit = New System.Windows.Forms.ToolStripMenuItem
        Me.ActionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuActionsAdd = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuActionsDelete = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuActionsExtract = New System.Windows.Forms.ToolStripMenuItem
        Me.HelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuHelpAbout = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip
        Me.btnNew = New System.Windows.Forms.ToolStripButton
        Me.btnOpen = New System.Windows.Forms.ToolStripButton
        Me.btnFavorites = New System.Windows.Forms.ToolStripButton
        Me.btnAdd = New System.Windows.Forms.ToolStripButton
        Me.btnExtract = New System.Windows.Forms.ToolStripButton
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip
        Me.lblFeedback = New System.Windows.Forms.ToolStripStatusLabel
        Me.imgLstTsMenus = New System.Windows.Forms.ImageList(Me.components)
        Me.imgLstMsMenu = New System.Windows.Forms.ImageList(Me.components)
        Me.ListView1 = New System.Windows.Forms.ListView
        Me.colName = New System.Windows.Forms.ColumnHeader
        Me.colType = New System.Windows.Forms.ColumnHeader
        Me.colModified = New System.Windows.Forms.ColumnHeader
        Me.imgLstFiles = New System.Windows.Forms.ImageList(Me.components)
        Me.MenuStrip1.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.ActionsToolStripMenuItem, Me.HelpToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(707, 24)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuFileNew, Me.mnuFileOpen, Me.mnuFileFavorites, Me.mnuFileClose, Me.ToolStripMenuItem1, Me.mnuFileExit})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(35, 20)
        Me.FileToolStripMenuItem.Text = "&File"
        '
        'mnuFileNew
        '
        Me.mnuFileNew.Name = "mnuFileNew"
        Me.mnuFileNew.Size = New System.Drawing.Size(150, 22)
        Me.mnuFileNew.Text = "&New Archive"
        '
        'mnuFileOpen
        '
        Me.mnuFileOpen.Name = "mnuFileOpen"
        Me.mnuFileOpen.Size = New System.Drawing.Size(150, 22)
        Me.mnuFileOpen.Text = "&Open Archive"
        '
        'mnuFileFavorites
        '
        Me.mnuFileFavorites.Name = "mnuFileFavorites"
        Me.mnuFileFavorites.Size = New System.Drawing.Size(150, 22)
        Me.mnuFileFavorites.Text = "&Favorites"
        '
        'mnuFileClose
        '
        Me.mnuFileClose.Name = "mnuFileClose"
        Me.mnuFileClose.Size = New System.Drawing.Size(150, 22)
        Me.mnuFileClose.Text = "&Close Archive"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(147, 6)
        '
        'mnuFileExit
        '
        Me.mnuFileExit.Name = "mnuFileExit"
        Me.mnuFileExit.Size = New System.Drawing.Size(150, 22)
        Me.mnuFileExit.Text = "E&xit"
        '
        'ActionsToolStripMenuItem
        '
        Me.ActionsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuActionsAdd, Me.mnuActionsDelete, Me.mnuActionsExtract})
        Me.ActionsToolStripMenuItem.Name = "ActionsToolStripMenuItem"
        Me.ActionsToolStripMenuItem.Size = New System.Drawing.Size(54, 20)
        Me.ActionsToolStripMenuItem.Text = "&Actions"
        '
        'mnuActionsAdd
        '
        Me.mnuActionsAdd.Name = "mnuActionsAdd"
        Me.mnuActionsAdd.Size = New System.Drawing.Size(120, 22)
        Me.mnuActionsAdd.Text = "&Add"
        '
        'mnuActionsDelete
        '
        Me.mnuActionsDelete.Name = "mnuActionsDelete"
        Me.mnuActionsDelete.Size = New System.Drawing.Size(120, 22)
        Me.mnuActionsDelete.Text = "&Delete"
        '
        'mnuActionsExtract
        '
        Me.mnuActionsExtract.Name = "mnuActionsExtract"
        Me.mnuActionsExtract.Size = New System.Drawing.Size(120, 22)
        Me.mnuActionsExtract.Text = "E&xtract"
        '
        'HelpToolStripMenuItem
        '
        Me.HelpToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuHelpAbout})
        Me.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
        Me.HelpToolStripMenuItem.Size = New System.Drawing.Size(40, 20)
        Me.HelpToolStripMenuItem.Text = "&Help"
        '
        'mnuHelpAbout
        '
        Me.mnuHelpAbout.Name = "mnuHelpAbout"
        Me.mnuHelpAbout.Size = New System.Drawing.Size(152, 22)
        Me.mnuHelpAbout.Text = "&About"
        '
        'ToolStrip1
        '
        Me.ToolStrip1.AutoSize = False
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnNew, Me.btnOpen, Me.btnFavorites, Me.btnAdd, Me.btnExtract})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 24)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(707, 64)
        Me.ToolStrip1.TabIndex = 1
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'btnNew
        '
        Me.btnNew.AutoSize = False
        Me.btnNew.Image = CType(resources.GetObject("btnNew.Image"), System.Drawing.Image)
        Me.btnNew.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnNew.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.btnNew.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnNew.Name = "btnNew"
        Me.btnNew.Size = New System.Drawing.Size(60, 48)
        Me.btnNew.Text = "New"
        Me.btnNew.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnNew.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'btnOpen
        '
        Me.btnOpen.AutoSize = False
        Me.btnOpen.Image = CType(resources.GetObject("btnOpen.Image"), System.Drawing.Image)
        Me.btnOpen.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnOpen.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.btnOpen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnOpen.Name = "btnOpen"
        Me.btnOpen.Size = New System.Drawing.Size(60, 48)
        Me.btnOpen.Text = "Open"
        Me.btnOpen.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnOpen.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'btnFavorites
        '
        Me.btnFavorites.AutoSize = False
        Me.btnFavorites.Image = CType(resources.GetObject("btnFavorites.Image"), System.Drawing.Image)
        Me.btnFavorites.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnFavorites.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.btnFavorites.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnFavorites.Name = "btnFavorites"
        Me.btnFavorites.Size = New System.Drawing.Size(60, 48)
        Me.btnFavorites.Text = "Favorites"
        Me.btnFavorites.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnFavorites.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'btnAdd
        '
        Me.btnAdd.AutoSize = False
        Me.btnAdd.Enabled = False
        Me.btnAdd.Image = CType(resources.GetObject("btnAdd.Image"), System.Drawing.Image)
        Me.btnAdd.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnAdd.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.btnAdd.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(60, 48)
        Me.btnAdd.Text = "Add"
        Me.btnAdd.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnAdd.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'btnExtract
        '
        Me.btnExtract.AutoSize = False
        Me.btnExtract.Enabled = False
        Me.btnExtract.Image = CType(resources.GetObject("btnExtract.Image"), System.Drawing.Image)
        Me.btnExtract.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnExtract.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.btnExtract.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnExtract.Name = "btnExtract"
        Me.btnExtract.Size = New System.Drawing.Size(60, 48)
        Me.btnExtract.Text = "Extract"
        Me.btnExtract.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnExtract.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblFeedback})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 551)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(707, 22)
        Me.StatusStrip1.TabIndex = 2
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'lblFeedback
        '
        Me.lblFeedback.Name = "lblFeedback"
        Me.lblFeedback.Size = New System.Drawing.Size(265, 17)
        Me.lblFeedback.Text = "Choose ""New"" to open an create or ""Open"" to archive"
        '
        'imgLstTsMenus
        '
        Me.imgLstTsMenus.ImageStream = CType(resources.GetObject("imgLstTsMenus.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imgLstTsMenus.TransparentColor = System.Drawing.Color.Transparent
        Me.imgLstTsMenus.Images.SetKeyName(0, "NewFolder.png")
        Me.imgLstTsMenus.Images.SetKeyName(1, "OpenFolder.png")
        Me.imgLstTsMenus.Images.SetKeyName(2, "Favorites.png")
        Me.imgLstTsMenus.Images.SetKeyName(3, "Add.png")
        Me.imgLstTsMenus.Images.SetKeyName(4, "Extract.png")
        '
        'imgLstMsMenu
        '
        Me.imgLstMsMenu.ImageStream = CType(resources.GetObject("imgLstMsMenu.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imgLstMsMenu.TransparentColor = System.Drawing.Color.Transparent
        Me.imgLstMsMenu.Images.SetKeyName(0, "NewFolder.png")
        Me.imgLstMsMenu.Images.SetKeyName(1, "OpenFolder.png")
        Me.imgLstMsMenu.Images.SetKeyName(2, "Favorites.png")
        Me.imgLstMsMenu.Images.SetKeyName(3, "Add.png")
        Me.imgLstMsMenu.Images.SetKeyName(4, "Remove.png")
        Me.imgLstMsMenu.Images.SetKeyName(5, "Extract.png")
        '
        'ListView1
        '
        Me.ListView1.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colName, Me.colType, Me.colModified})
        Me.ListView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListView1.FullRowSelect = True
        Me.ListView1.Location = New System.Drawing.Point(0, 88)
        Me.ListView1.Name = "ListView1"
        Me.ListView1.Size = New System.Drawing.Size(707, 463)
        Me.ListView1.SmallImageList = Me.imgLstFiles
        Me.ListView1.TabIndex = 3
        Me.ListView1.UseCompatibleStateImageBehavior = False
        Me.ListView1.View = System.Windows.Forms.View.Details
        '
        'colName
        '
        Me.colName.Text = "Name"
        Me.colName.Width = 250
        '
        'colType
        '
        Me.colType.Text = "Type"
        Me.colType.Width = 250
        '
        'colModified
        '
        Me.colModified.Text = "Modified"
        Me.colModified.Width = 150
        '
        'imgLstFiles
        '
        Me.imgLstFiles.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit
        Me.imgLstFiles.ImageSize = New System.Drawing.Size(16, 16)
        Me.imgLstFiles.TransparentColor = System.Drawing.Color.White
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(707, 573)
        Me.Controls.Add(Me.ListView1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Zip Demo"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents lblFeedback As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ActionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnFavorites As System.Windows.Forms.ToolStripButton
    Friend WithEvents imgLstTsMenus As System.Windows.Forms.ImageList
    Friend WithEvents imgLstMsMenu As System.Windows.Forms.ImageList
    Friend WithEvents btnNew As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnOpen As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnAdd As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnExtract As System.Windows.Forms.ToolStripButton
    Friend WithEvents HelpToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuHelpAbout As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFileNew As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFileOpen As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFileFavorites As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFileClose As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuFileExit As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuActionsAdd As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuActionsDelete As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuActionsExtract As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ListView1 As System.Windows.Forms.ListView
    Friend WithEvents colName As System.Windows.Forms.ColumnHeader
    Friend WithEvents colType As System.Windows.Forms.ColumnHeader
    Friend WithEvents colModified As System.Windows.Forms.ColumnHeader
    Friend WithEvents imgLstFiles As System.Windows.Forms.ImageList

End Class

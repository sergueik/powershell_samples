
Imports System.IO
Imports System.IO.Packaging


Public Class frmMain


    '*** First - Add a reference to the WindowsBase.dll
    '
    '  Project Menu | Add Reference...
    '
    '    On the .Net tab, look for System.IO.Packing
    '
    '      I was not able to find it on the .Net tab, so I searched my c: for "WindowsBase.dll",
    '      and found it in the following location:
    '
    '        C:\Program Files\Reference Assemblies\Microsoft\Framework\v3.0\WindowsBase.dll



#Region " Declarations"

    Private mCurrentZip As String = String.Empty

#End Region    'Declarations


    'This sub demonstrates adding files to a zip
    Private Function AddFileToZip(ByVal filePath As String, _
                             Optional ByVal uri As String = "") As ArchiveFile

        'ArchiveFile is a custom class that stores the File Name, Type, Modified, Uri,
        '  and gets the correct system icon.
        Dim archFile As New ArchiveFile(filePath)

        'Open the zip file if it exists, else create a new one
        Dim zip As Package = ZipPackage.Open(mCurrentZip, IO.FileMode.OpenOrCreate, IO.FileAccess.ReadWrite)

        'If no Uri was provided, then create one from the existing file path
        '   An optional route would be to just use the file name as the Uri, but then
        '   it will extract to the root directory. 
        If uri <> "" Then
            'Change all backward slashes to forward slashes
            uri = uri.Replace("\", "/")
        Else
            'Uri was not provided, so use the name of the file:
            uri = String.Concat("/", IO.Path.GetFileName(filePath))

            'Spaces cannot appear in the file name, so replace them with underscores.
            uri = uri.Replace(" ", "_")
        End If

        Dim partUri As New Uri(uri, UriKind.Relative)
        Dim contentType As String = Net.Mime.MediaTypeNames.Application.Zip   'constant: "application/zip"

        'The PackagePart contains the information:
        '   Where to extract the file when it's extracted (partUri)
        '   The type of content stream (MIME type) - (contentType)
        '   The type of compression to use (CompressionOption.Normal)
        Dim pkgPart As PackagePart = _
            zip.CreatePart(partUri, contentType, CompressionOption.Normal)

        'Read all of the bytes from the file to add to the zip file
        Dim bites As Byte() = File.ReadAllBytes(filePath)

        'Compress and write the bytes to the zip file
        pkgPart.Package.PackageProperties.Modified = archFile.Modified
        pkgPart.GetStream().Write(bites, 0, bites.Length)

        'store the Uri in the Custom ArchiveFile
        archFile.Uri = uri

        zip.Close()  'Close the zip file

        Return archFile

    End Function


    'This sub demonstrates retrieving the contents of a zip file
    Private Sub LoadArchive(ByVal path As String)

        'Clear ListView
        Me.ListView1.Items.Clear()

        mCurrentZip = path

        'Open the zip file
        Dim zip As Package = ZipPackage.Open(mCurrentZip, IO.FileMode.OpenOrCreate, IO.FileAccess.ReadWrite)

        For Each pkgPart As PackagePart In zip.GetParts()

            'Gets the complete path without the leading "/"
            Dim fileName As String = pkgPart.Uri.OriginalString.Substring(1)

            'The psmdcp is a file containing meta-data for the package properties
            '  The _rels file contains the package relationships
            '  skip both of them.
            If IO.Path.GetExtension(fileName) = ".psmdcp" OrElse _
                fileName.IndexOf("_rels") > -1 Then _
                    Continue For

            Dim archFile As New ArchiveFile(fileName)

            Dim item As New ListViewItem(fileName)

            'Get Icon and add to image list
            Dim ico As System.Drawing.Icon = Shell32.GetIcon(fileName)

            'Add the image to the list if it's not already in it
            If Me.imgLstFiles.Images.ContainsKey(archFile.Type) = False Then _
                Me.imgLstFiles.Images.Add(archFile.Type, ico)

            'Set the image key and store the Uri in the Tag
            item.ImageKey = archFile.Type
            item.Tag = pkgPart.Uri.ToString()  'Store Uri in ListView Tag

            'Add the Type and Modified date
            With item.SubItems
                .Add(archFile.Type)
                .Add(pkgPart.Package.PackageProperties.Modified.ToString())
            End With

            'Add the ListViewItem to the ListView
            Me.ListView1.Items.Add(item)

        Next

        zip.Close()

        EnableControls(True)

    End Sub


#Region " Infrastructure"

#Region " Events"

#Region " Form Events"

    Private Sub Form1_Load(ByVal sender As System.Object, _
                           ByVal e As System.EventArgs) Handles MyBase.Load

        'Show Eula
        Dim frm As New frmEula()
        frm.ShowDialog()

        frm.Dispose()

        SetImages()

    End Sub

#End Region    'Form Events


#Region " Control Events"


#Region " MenuStrip"

    Private Sub mnuFileNew_Click(ByVal sender As System.Object, _
                                 ByVal e As System.EventArgs) Handles mnuFileNew.Click

        NewArchive()

    End Sub


    Private Sub mnuFileOpen_Click(ByVal sender As System.Object, _
                                  ByVal e As System.EventArgs) Handles mnuFileOpen.Click

        OpenArchive()

    End Sub


    Private Sub mnuFileFavorites_Click(ByVal sender As System.Object, _
                                       ByVal e As System.EventArgs) Handles mnuFileFavorites.Click

        Favorites()

    End Sub


    Private Sub mnuFileClose_Click(ByVal sender As System.Object, _
                                   ByVal e As System.EventArgs) Handles mnuFileClose.Click

        mCurrentZip = String.Empty
        Me.ListView1.Items.Clear()

        EnableControls(False)

    End Sub


    Private Sub mnuFileExit_Click(ByVal sender As System.Object, _
                                  ByVal e As System.EventArgs) Handles mnuFileExit.Click

        Application.Exit()

    End Sub


    Private Sub mnuActionsAdd_Click(ByVal sender As System.Object, _
                                    ByVal e As System.EventArgs) Handles mnuActionsAdd.Click

        AddFileToArchive()

    End Sub


    Private Sub mnuActionsDelete_Click(ByVal sender As System.Object, _
                                       ByVal e As System.EventArgs) Handles mnuActionsDelete.Click

        DeleteFileFromArchive()

    End Sub


    Private Sub mnuActionsExtract_Click(ByVal sender As System.Object, _
                                        ByVal e As System.EventArgs) Handles mnuActionsExtract.Click

        ExtractArchive()

    End Sub


    Private Sub mnuHelpAbout_Click(ByVal sender As System.Object, _
                                   ByVal e As System.EventArgs) Handles mnuHelpAbout.Click

        Dim frm As New frmAbout()
        frm.ShowDialog()

        frm.Dispose()

    End Sub

#End Region    'MenuStrip


#Region " ToolStrip"

    Private Sub FileToolStripMenuItem_DropDownOpening(ByVal sender As System.Object, _
                                                  ByVal e As System.EventArgs) Handles FileToolStripMenuItem.DropDownOpening

        'If no zip archive is open, then disable the close menu item,
        '  else, enable it
        Me.mnuFileClose.Enabled = _
            Not (Me.mCurrentZip = String.Empty AndAlso Me.ListView1.Items.Count = 0)

    End Sub


    Private Sub btnNew_Click(ByVal sender As System.Object, _
                         ByVal e As System.EventArgs) Handles btnNew.Click

        NewArchive()

    End Sub


    Private Sub btnOpen_Click(ByVal sender As System.Object, _
                              ByVal e As System.EventArgs) Handles btnOpen.Click

        OpenArchive()

    End Sub


    Private Sub btnFavorites_Click(ByVal sender As System.Object, _
                                   ByVal e As System.EventArgs) Handles btnFavorites.Click

        Favorites()

    End Sub


    Private Sub btnAdd_Click(ByVal sender As System.Object, _
                             ByVal e As System.EventArgs) Handles btnAdd.Click

        AddFileToArchive()

    End Sub


    Private Sub btnExtract_Click(ByVal sender As System.Object, _
                                 ByVal e As System.EventArgs) Handles btnExtract.Click

        ExtractArchive()

    End Sub

#End Region    'ToolStrip


    Private Sub ListView1_SelectedIndexChanged(ByVal sender As System.Object, _
                                           ByVal e As System.EventArgs) Handles ListView1.SelectedIndexChanged
        Dim s As String = "s"
        If Me.ListView1.SelectedItems.Count = 1 Then _
            s = ""

        Me.lblFeedback.Text = String.Format("Selected {0} file{1}", Me.ListView1.SelectedItems.Count, s)

    End Sub


#End Region    'Control Events

#End Region    'Events


#Region " Methods"

    Private Sub NewArchive()

        Dim diag As SaveFileDialog = Dialogs.GetSaveFileDialog()
        With diag

            Dim result As DialogResult = .ShowDialog()
            If result = Windows.Forms.DialogResult.OK Then

                'Clear items from ListView
                Me.ListView1.Items.Clear()

                'Delete the selected file if it exits.
                If My.Computer.FileSystem.FileExists(.FileName) = True Then _
                    My.Computer.FileSystem.DeleteFile(.FileName)

                'Store the selected file.
                mCurrentZip = .FileName

                EnableControls(True)

            End If

        End With

        diag.Dispose()

        Me.lblFeedback.Text = ""

    End Sub


    Private Sub AddFileToArchive()

        Dim archFile As ArchiveFile = Nothing
        Dim diag As OpenFileDialog = Dialogs.GetOpenFileDialog(False)

        With diag

            .ShowDialog()

            For Each filePath As String In .FileNames

                'Add to Zip Archive
                archFile = AddFileToZip(filePath)

                'Get Icon and add to image list
                Dim ico As System.Drawing.Icon = Shell32.GetIcon(archFile.Name)

                'Add the image to the list if it's not already in it
                If Me.imgLstFiles.Images.ContainsKey(archFile.Type) = False Then _
                    Me.imgLstFiles.Images.Add(archFile.Type, ico)

                'Add to ListView
                Dim item As New ListViewItem(archFile.Name)
                item.ImageKey = archFile.Type
                item.Tag = archFile.Uri.ToString()  'store the Uri in the Tag.

                With item.SubItems
                    .Add(archFile.Type)
                    .Add(archFile.Modified.ToString())
                End With

                Me.ListView1.Items.Add(item)

            Next

        End With

    End Sub


    Private Sub DeleteFileFromArchive()

        'Items cannot be removed from a collection when in a iteration, 
        '  so build a list for all items to be removed
        Dim itemsToRemove As New List(Of ListViewItem)

        'Open the zip file
        Dim zip As Package = ZipPackage.Open(mCurrentZip, IO.FileMode.OpenOrCreate, IO.FileAccess.ReadWrite)

        'Loop through each item in the ListView
        For Each item As ListViewItem In Me.ListView1.Items

            'If the item is selected, then remove it
            If Me.ListView1.SelectedItems.Contains(item) = True Then
                itemsToRemove.Add(item)  'Add to removal list

                Dim partUri As New Uri(item.Tag.ToString(), System.UriKind.Relative)

                zip.DeletePart(partUri)

            End If

        Next

        'Remove items from ListView
        For Each item As ListViewItem In itemsToRemove
            Me.ListView1.Items.Remove(item)
        Next

        'Close Zip
        zip.Close()

    End Sub


    Private Sub EnableControls(ByVal enable As Boolean)

        Me.btnAdd.Enabled = enable
        Me.btnExtract.Enabled = enable
        Me.mnuActionsAdd.Enabled = enable
        Me.mnuActionsDelete.Enabled = enable
        Me.mnuActionsExtract.Enabled = enable

    End Sub


    Private Sub ExtractArchive()

        Dim folder As String = String.Empty

        Dim diag As New FolderBrowserDialog()
        With diag
            .Description = "Please select a folder to extract the current archive to."
            .RootFolder = Environment.SpecialFolder.MyComputer
            .ShowNewFolderButton = True

            Dim result As DialogResult = .ShowDialog()
            If result = Windows.Forms.DialogResult.OK Then _
                folder = diag.SelectedPath

        End With

        If folder <> String.Empty Then

            'extract archive to folder
            'Open the zip file
            Dim zip As Package = ZipPackage.Open(mCurrentZip, IO.FileMode.OpenOrCreate, IO.FileAccess.ReadWrite)

            Dim pkgPart As PackagePart = Nothing


            'Only Export selected items
            For Each item As ListViewItem In Me.ListView1.Items

                'If the item is selected, then export it.
                '  If no items are selected, then export all of them.
                If item.Selected = True OrElse _
                    Me.ListView1.SelectedItems.Count = 0 Then

                    'The Uri is stored in the Tag of the ListViewItem,
                    '  so retrieve it now and use it to get the part
                    Dim partUri As Uri = New Uri(item.Tag.ToString(), System.UriKind.Relative)
                    pkgPart = zip.GetPart(partUri)

                    'Gets the complete path without the leading "/"
                    Dim fileName As String = pkgPart.Uri.OriginalString.Substring(1)

                    'The psmdcp is a file containing meta-data for the package properties
                    '  The _rels file contains the package relationships
                    '  skip both of them.
                    If IO.Path.GetExtension(fileName) = ".psmdcp" OrElse _
                        fileName.IndexOf("_rels") > -1 Then _
                            Continue For

                    Dim stream As Stream = pkgPart.GetStream()

                    'Read all of the bytes from the file to add to the zip file
                    Dim bites(CInt(stream.Length - 1)) As Byte
                    stream.Read(bites, 0, bites.Length)

                    fileName = fileName.Replace("_", " ")  'replace underscore with space
                    File.WriteAllBytes(String.Concat(folder, "\", fileName), bites)

                End If

            Next

            zip.Close()

        End If


    End Sub


    Private Sub Favorites()

        Dim frm As New frmFavorites()
        AddHandler frm.LoadFavorite, AddressOf LoadArchive
        frm.ShowDialog()

        frm.Dispose()

    End Sub


    Private Sub OpenArchive()

        Dim diag As OpenFileDialog = Dialogs.GetOpenFileDialog(True)

        With diag

            Dim result As DialogResult = .ShowDialog()

            If result = Windows.Forms.DialogResult.OK AndAlso _
                .FileName <> "" Then

                LoadArchive(.FileName)

            End If

        End With

        'select 1st item
        If Me.ListView1.Items.Count > 0 Then _
            Me.ListView1.Items(0).Selected = True

    End Sub


    'Set the images for all controls
    Private Sub SetImages()

        'Set ToolStrip Button Images
        Me.btnNew.Image = Me.imgLstTsMenus.Images("NewFolder.png")
        Me.btnOpen.Image = Me.imgLstTsMenus.Images("OpenFolder.png")
        Me.btnFavorites.Image = Me.imgLstTsMenus.Images("Favorites.png")
        Me.btnAdd.Image = Me.imgLstTsMenus.Images("Add.png")
        Me.btnExtract.Image = Me.imgLstTsMenus.Images("Extract.png")

        'Set MenuStrip Button Images
        Me.mnuFileNew.Image = Me.imgLstMsMenu.Images("NewFolder.png")
        Me.mnuFileOpen.Image = Me.imgLstMsMenu.Images("OpenFolder.png")
        Me.mnuFileFavorites.Image = Me.imgLstMsMenu.Images("Favorites.png")
        Me.mnuActionsAdd.Image = Me.imgLstMsMenu.Images("Add.png")
        Me.mnuActionsDelete.Image = Me.imgLstMsMenu.Images("Remove.png")
        Me.mnuActionsExtract.Image = Me.imgLstMsMenu.Images("Extract.png")

    End Sub

#End Region

#End Region    'Infrastructure


End Class

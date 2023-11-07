Public Class frmFavorites

    Public Event LoadFavorite(ByVal path As String)

    Private mDsFavs As New DsFavorites()


#Region " Events"

#Region " Form Events"

    Private Sub frmFavorites_Load(ByVal sender As System.Object, _
                              ByVal e As System.EventArgs) Handles MyBase.Load

        mDsFavs.Zips.LoadFavorites()

        LoadListView()

    End Sub


    Private Sub frmFavorites_FormClosing(ByVal sender As Object, _
                                     ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        'Save the list and exit
        mDsFavs.Zips.SaveFavorites()
        mDsFavs.Dispose()

    End Sub

#End Region    'Form Events


#Region " Control Events"

    Private Sub btnAdd_Click(ByVal sender As System.Object, _
                         ByVal e As System.EventArgs) Handles btnAdd.Click

        Dim diag As OpenFileDialog = Dialogs.GetOpenFileDialog(True)
        With diag

            If .ShowDialog() = Windows.Forms.DialogResult.OK Then

                Dim fi As New IO.FileInfo(.FileName)

                'Add a new row using the selected file
                Dim row As DataRow = mDsFavs.Zips.NewRow()
                row.Item("Name") = fi.Name
                row.Item("Modified") = fi.LastWriteTime.ToString()
                row.Item("Folder") = fi.DirectoryName

                mDsFavs.Zips.Rows.Add(row)

                fi = Nothing

                LoadListView()

            End If

        End With
    End Sub


    Private Sub btnRemove_Click(ByVal sender As System.Object, _
                                ByVal e As System.EventArgs) Handles btnRemove.Click

        Dim items2Remove As New List(Of ListViewItem)

        'Remove each selected item from the table
        For Each item As ListViewItem In Me.ListView1.SelectedItems

            items2Remove.Add(item)

            'retrieve the row reference from the tag
            Dim row As DataRowView = item.Tag

            'remove the row from the table
            Me.mDsFavs.Zips.Rows.Remove(row.Row)

        Next

        'Remove all selected listview items
        For Each item As ListViewItem In items2Remove

            Me.ListView1.Items.Remove(item)

        Next



    End Sub


    Private Sub btnOpen_Click(ByVal sender As System.Object, _
                              ByVal e As System.EventArgs) Handles btnOpen.Click

        Dim row As DataRowView = DirectCast(Me.ListView1.SelectedItems(0).Tag, DataRowView)

        Dim path As String = _
            String.Concat(row.Item("Folder").ToString(), "\", row.Item("Name").ToString())

        RaiseEvent LoadFavorite(path)

        Me.Close()

    End Sub


    Private Sub btnCancel_Click(ByVal sender As System.Object, _
                                ByVal e As System.EventArgs) Handles btnCancel.Click

        'Discard changes and exit
        Me.Close()

    End Sub

#End Region    'Control Events

#End Region    'Events


#Region " Methods"

    Private Sub LoadListView()

        Me.ListView1.Items.Clear()

        Dim view As DataView = mDsFavs.Zips.DefaultView
        view.Sort = "Name ASC"

        Dim rowItems(2) As String

        'Loop through each row in the table and add it to the ListView
        For Each row As DataRowView In view

            row.Row.ItemArray.CopyTo(rowItems, 0)

            Dim item As New ListViewItem(rowItems)
            item.Tag = row                  'store a reference to the row in the tag
            Me.ListView1.Items.Add(item)

        Next

    End Sub

#End Region    'Methods


End Class
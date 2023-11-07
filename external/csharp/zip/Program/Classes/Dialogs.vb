Public Class Dialogs

    Public Shared Function GetOpenFileDialog(ByVal isZip As Boolean) As OpenFileDialog

        Dim diag As New OpenFileDialog()

        With diag
            .AddExtension = True
            .AutoUpgradeEnabled = True
            .CheckFileExists = True
            .CheckPathExists = True
            .RestoreDirectory = True

            If isZip = True Then
                .Filter = "Zip Files (*.zip)|*.zip|All Files (*.*)|*.*"
                .DefaultExt = ".zip"
                .Multiselect = False
                .Title = "Open"
            Else
                .Filter = "All Files (*.*)|*.*"
                .Multiselect = True
                .Title = "Add"
            End If

        End With

        Return diag

    End Function


    Public Shared Function GetSaveFileDialog() As SaveFileDialog

        Dim diag As New SaveFileDialog()
        With diag
            .AddExtension = True
            .AutoUpgradeEnabled = True
            .CheckFileExists = False
            .CheckPathExists = True
            .DefaultExt = ".zip"
            .Filter = "Zip Files (*.zip)|*.zip|All Files (*.*)|*.*"
            .OverwritePrompt = True
            .RestoreDirectory = True
            .Title = "New Archive"
        End With

        Return diag

    End Function

End Class

﻿Public NotInheritable Class frmAbout

    Private Sub frmAbout_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Set the title of the form.
        Dim ApplicationTitle As String
        If My.Application.Info.Title <> "" Then
            ApplicationTitle = My.Application.Info.Title
        Else
            ApplicationTitle = System.IO.Path.GetFileNameWithoutExtension(My.Application.Info.AssemblyName)
        End If
        Me.Text = String.Format("About {0}", ApplicationTitle)
        ' Initialize all of the text displayed on the About Box.
        ' TODO: Customize the application's assembly information in the "Application" pane of the project 
        '    properties dialog (under the "Project" menu).
        Me.LabelProductName.Text = My.Application.Info.ProductName
        Me.LabelVersion.Text = String.Format("Version {0}", My.Application.Info.Version.ToString)
        Me.LabelCopyright.Text = My.Application.Info.Copyright
        Me.LabelCompanyName.Text = My.Application.Info.CompanyName
        Me.TextBoxDescription.Text = My.Application.Info.Description
    End Sub

    Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.Close()
    End Sub


    Private Sub btnEula_Click(ByVal sender As System.Object, _
                              ByVal e As System.EventArgs) Handles btnEula.Click

        Dim frm As New frmEula()
        frm.ShowDialog()

        frm.Dispose()

    End Sub

    Private Sub LabelProductName_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LabelProductName.Click

    End Sub
    Private Sub TextBoxDescription_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBoxDescription.TextChanged

    End Sub
    Private Sub LabelVersion_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LabelVersion.Click

    End Sub
    Private Sub LabelCopyright_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LabelCopyright.Click

    End Sub
    Private Sub LabelCompanyName_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LabelCompanyName.Click

    End Sub
End Class

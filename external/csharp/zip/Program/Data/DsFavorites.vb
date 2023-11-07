Partial Class DsFavorites
    Partial Class ZipsDataTable

        Private mFavoritesPath As String = String.Concat(Application.StartupPath, "\Favorites.xml")

        Public Sub LoadFavorites()

            'Only attempt the read the xml file if it exists.
            If My.Computer.FileSystem.FileExists(mFavoritesPath) = True Then _
                Me.ReadXml(mFavoritesPath)

        End Sub


        Public Sub SaveFavorites()

            'Write the table out to an xml file in the same folder as the application.
            Me.WriteXml(mFavoritesPath, System.Data.XmlWriteMode.WriteSchema)

        End Sub

    End Class

End Class

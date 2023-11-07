
Imports System
Imports System.Runtime.InteropServices

Public Class Shell32

    'This function get the icon for the specified file path extension
    Public Shared Function GetIcon(ByVal filePath As String) As System.Drawing.Icon

        'Get the extension
        Dim ext As String = IO.Path.GetExtension(filePath)

        Dim shellFileInfo As New Shell32.SHFILEINFO()

        Shell32.SHGetFileInfo(ext, 128, shellFileInfo, _
                    Convert.ToUInt32( _
                        Runtime.InteropServices.Marshal.SizeOf(shellFileInfo)), _
                    Convert.ToUInt32(256I Or 1I Or 16I))

        Dim ico As System.Drawing.Icon = System.Drawing.Icon.FromHandle(shellFileInfo.hIcon)

        Return ico

    End Function


    <StructLayout(LayoutKind.Sequential)> _
        Private Structure SHFILEINFO

        Public hIcon As IntPtr
        Public iIcon As Integer
        Public dwAttributes As System.UInt32
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=260)> _
        Public szDisplayName As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=80)> _
        Public szTypeName As String

    End Structure


    <DllImport("Shell32.dll")> _
        Private Shared Function SHGetFileInfo( _
        ByVal pszPath As String, _
        ByVal dwFileAttributes As System.UInt32, _
        ByRef psfi As SHFILEINFO, _
        ByVal cbFileInfo As System.UInt32, _
        ByVal uFlags As System.UInt32) As IntPtr
    End Function

End Class


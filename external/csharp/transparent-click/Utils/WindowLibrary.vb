Imports System.Runtime.InteropServices

Namespace WindowLibrary

    Public Class User32Wrappers

        Public Enum GWL As Integer
            ExStyle = -20
        End Enum

        Public Enum WS_EX As Integer
            Transparent = &H20
            Layered = &H80000
        End Enum

        Public Enum LWA As Integer
            ColorKey = &H1
            Alpha = &H2
        End Enum

        <DllImport("user32", EntryPoint:="GetWindowLong")> _
        Public Shared Function GetWindowLong( _
            ByVal hWnd As IntPtr, _
            ByVal nIndex As GWL _
                ) As Integer
        End Function

        <DllImport("user32", EntryPoint:="SetWindowLong")> _
        Public Shared Function SetWindowLong( _
            ByVal hWnd As IntPtr, _
            ByVal nIndex As GWL, _
            ByVal dsNewLong As WS_EX _
                ) As Integer
        End Function

        <DllImport("user32.dll", EntryPoint:="SetLayeredWindowAttributes")> _
        Public Shared Function SetLayeredWindowAttributes( _
            ByVal hWnd As IntPtr, _
            ByVal crKey As Integer, _
            ByVal alpha As Byte, _
            ByVal dwFlags As LWA _
                ) As Boolean
        End Function
    End Class
End Namespace
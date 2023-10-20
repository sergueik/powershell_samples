
using System.Runtime.InteropServices;

public class Data {
	enum KnownFolder {
    Documents,
    Downloads,
    Music,
    Pictures,
    SavedGames,
    // ...
	};

public static void test ()  { 
		for (KnownFolder knownFolder in Enum.GetValues(typeof(KnownFolder))) {
    try {
        Console.Write(String.Format("Folder: {0}" , knownFolder ));
        Console.WriteLine(KnownFolders.GetPath(knownFolder));
    } catch (Exception e) {
        Console.WriteLine(String.Format("Exception {0}", e.Message));
    }
    Console.WriteLine();

}
}
static class KnownFolders
{
    private static readonly Dictionary<KnownFolder, Guid> _knownFolderGuids = new()
    {
        [KnownFolder.Documents] = new("FDD39AD0-238F-46AF-ADB4-6C85480369C7"),
        [KnownFolder.Downloads] = new("374DE290-123F-4565-9164-39C4925E467B"),
        [KnownFolder.Music] = new("4BD8D571-6D19-48D3-BE97-422220080E43"),
        [KnownFolder.Pictures] = new("33E28130-4E1E-4676-835A-98395C3BC3BB"),
        [KnownFolder.SavedGames] = new("4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4"),
    };
// https://www.codeproject.com/Articles/878605/Getting-All-Special-Folders-in-NET
// NOTE: the article code uses forward version of c# 9
 
/*
    public static string? GetPath(KnownFolder folder)
    {
        return SHGetKnownFolderPath(_knownFolderGuids[folder], 0);  
  }
    [DllImport("shell32", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
    private static extern string SHGetKnownFolderPath(
        [MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, nint hToken = 0);
        */
// see also: 
// https://www.pinvoke.net/default.aspx/shell32.shgetknownfolderpath
//

    [DllImport("shell32.dll")]
static extern int SHGetKnownFolderPath( [MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr ppszPath);
    
        
public static string? GetKnownFolderPath(Guid folderGuid) {
    IntPtr ppszPath = default;
    try {
        int hr = SHGetKnownFolderPath(folderGuid, 0, IntPtr.Zero, out ppszPath);
        Marshal.ThrowExceptionForHR(hr); // alternatively, check success with hr >= 0
        return Marshal.PtrToStringUni(ppszPath);
    } finally {
        Marshal.FreeCoTaskMem(ppszPath);
    }
	}
}
// see also (VB.Net):
// https://www.codeproject.com/Articles/24308/Accessing-All-of-Windows-Special-Folders
/*
 <DllImport("shell32.dll")> _
Shared Function SHGetKnownFolderPath(
    <MarshalAs(UnmanagedType.LPStruct)> ByVal rfid As Guid,
    ByVal dwFlags As UInteger,
    ByVal hToken As IntPtr,
    ByRef ppszPath As IntPtr ' must be freed with Marshal.FreeCoTaskMem
    ) As Integer
End Function 

    Public Function GetSpecialFolderPath _
          (ByVal aSpecialFolder As enuCSIDLPhysical _
          ) As String
      ' String for our WinAPI call.
      Dim xStr As String = Space(260)
      ' Working int32.
      Dim xInt As Int32
      ' Get the special folder path null-terminated-string.
      xInt = SHELL32_GetFolderPath(0, aSpecialFolder, 0, 0, xStr)
      ' Check HResult.
      If xInt <> 0 Then
        ' Error, return blank.
        Return ""
      Else
        ' No error, got a path. Return characters prior to null value.
        xInt = InStr(xStr, vbNullChar)
        If xInt > 0 Then
          Return Left(xStr, xInt - 1)
        Else
          Return xStr
        End If
      End If
    End Function

' VB.NET
Public Shared Function GetKnownFolderPath(ByVal folderGuid As Guid) As String
    Dim ppszPath As IntPtr
    Try
        Dim hr As Integer = SHGetKnownFolderPath(folderGuid, 0, IntPtr.Zero, ppszPath)
        Marshal.ThrowExceptionForHR(hr) ' alternatively, check success with hr >= 0
        Return Marshal.PtrToStringUni(ppszPath)
    Finally
        Marshal.FreeCoTaskMem(ppszPath)
    End Try
End Function
 */ 

}

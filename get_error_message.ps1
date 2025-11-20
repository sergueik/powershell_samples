param(
  [string] $code
)
function helper {
    param([Parameter(Mandatory=$true)] [string]$Code)

    # Normalize input
    $hr = $null
    if ($Code -match '^0x') {
        $hr = [int][convert]::ToInt32($Code,16)
    } elseif ($Code -match '^-?\d+$') {
        $hr = [int]$Code
    } else {
        throw "Unrecognized format: $Code"
    }

    $source = @"
using System;
using System.Runtime.InteropServices;

public static class HResultHelper
{
    private const int FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x100;
    private const int FORMAT_MESSAGE_FROM_SYSTEM     = 0x1000;
    private const int FORMAT_MESSAGE_IGNORE_INSERTS  = 0x200;

    [DllImport("kernel32.dll", SetLastError=true, CharSet=CharSet.Auto)]
    private static extern int FormatMessage(
        int dwFlags,
        IntPtr lpSource,
        int dwMessageId,
        int dwLanguageId,
        out IntPtr lpBuffer,
        int nSize,
        IntPtr Arguments);

    [DllImport("kernel32.dll")]
    private static extern IntPtr LocalFree(IntPtr hMem);

    public static string Msg(int hr)
    {
        IntPtr lpMsgBuf;
        int ret = FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
                                IntPtr.Zero, hr, 0, out lpMsgBuf, 0, IntPtr.Zero);
        if (ret == 0) return String.Format("Unknown HRESULT 0x{0:X8}", hr);
        string message = Marshal.PtrToStringAuto(lpMsgBuf);
        LocalFree(lpMsgBuf);
        return message.Trim();
    }
}
"@

    Add-Type -TypeDefinition $source -PassThru | Out-Null
    [HResultHelper]::Msg($hr)
}
helper -code $code
<#
./get_error_message.ps1 0x80070035
The network path not found
./get_error_message.ps1 -2147286785
Invalid flag error.
./get_error_message.ps1 0x80070001
Incorrect function.
./get_error_message.ps1  0x80041318
The task XML contains a value which is incorrectly formatted or out of range.
./get_error_message.ps1 0x8003000F
Unknown HRESULT 0x8003000F

#>
# detecting “pseudo-terminal” execution environment introduced in Windows 11 
# where is no real Win32 window to attach modals to
#
Add-Type -TypeDefinition @"
using System;
using System.Runtime.InteropServices;

public static class ConsoleInfo
{
	[DllImport("kernel32.dll")]
	public static extern IntPtr GetConsoleWindow();

	[DllImport("kernel32.dll")]
	public static extern bool GetConsoleMode(IntPtr hConsoleHandle, out int lpMode);

	[DllImport("kernel32.dll")]
	public static extern IntPtr GetStdHandle(int nStdHandle);

	public static string DetectEnvironment()
	{
		// Stdout handle
		IntPtr handle = GetStdHandle(-11); // STD_OUTPUT_HANDLE
		int mode;
		bool res = GetConsoleMode(handle, out mode);
		Console.Error.WriteLine(String.Format("res: {0}", res));
		if (!res) {
			// No console mode → must be a pseudoterminal (WSL, mintty, git-bash, ConPTY)
			IntPtr hwnd = GetConsoleWindow();
			if (hwnd != IntPtr.Zero)
				return "ConPTY-backed console (Windows Terminal)";

			return "True PTY (mintty / git-bash / WSL) — no Win32 console exists";
		}

		// We have a Win32 console (conhost.exe)
		return "Classic Win32 console (conhost.exe)";
	}
}
"@

Add-Type -TypeDefinition @"
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public static class ConsoleHelper {
    [DllImport("kernel32.dll")]
    public static extern IntPtr GetConsoleWindow();

    // Returns the HWND of the console, or IntPtr.Zero if none exists
    public static IntPtr GetConsoleHwnd()
    {
        return GetConsoleWindow();
    }

    // Returns a string describing the console environment
    public static string DetectConsoleType()
    {
        IntPtr hwnd = GetConsoleWindow();
        if (hwnd != IntPtr.Zero)
            return "Classic Win32 console (conhost.exe)";

        return "ConPTY-backed console (Windows Terminal) — no HWND available";
    }
}
"@ -ReferencedAssemblies 'System.Windows.Forms.dll'


# non-working
[ConsoleInfo]::DetectEnvironment()

$hwnd = [ConsoleHelper]::GetConsoleHwnd()
$consoleType = [ConsoleHelper]::DetectConsoleType()

Write-Host "Console type: $consoleType"
Write-Host "HWND: $hwnd"


Add-Type -TypeDefinition @"
using System;
using System.Runtime.InteropServices;
public static class ConsoleInfo2 {
    [DllImport("kernel32.dll")]
    public static extern IntPtr GetConsoleWindow();
}
"@

$hwnd = [ConsoleInfo2]::GetConsoleWindow()
$process = if ($hwnd -ne [IntPtr]::Zero) {
    # write-host ('looking for process with MainWindowHandle {0}' -f $hwnd ) 
    $pid3 = (Get-Process | Where-Object { $_.MainWindowHandle -eq $hwnd }).Id
    if ($pid3 -ne $null ) {
      Get-Process -Id $pid3
    } else { $null }
} else { $null }

if ($process -eq $null) {
    Write-Host "No Win32 console window found - probably Windows Terminal / ConPTY."
    $isClassicConsole = $false
} elseif ($process.ProcessName -eq "conhost") {
    Write-Host "Classic Win32 console detected."
    $isClassicConsole = $true
} else {
    Write-Host "Unknown console host: $($process.ProcessName)"
    $isClassicConsole = $false
}


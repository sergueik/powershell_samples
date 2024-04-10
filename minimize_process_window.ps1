#Copyright (c) 2023,2024 Serguei Kouzmine
#
#Permission is hereby granted, free of charge, to any person obtaining a copy
#of this software and associated documentation files (the "Software"), to deal
#in the Software without restriction, including without limitation the rights
#to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
#copies of the Software, and to permit persons to whom the Software is
#furnished to do so, subject to the following conditions:
#
#The above copyright notice and this permission notice shall be included in
#all copies or substantial portions of the Software.
#
#THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
#IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
#FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
#AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
#LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
#OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
#THE SOFTWARE.

# origin: https://gist.github.com/lalibi/3762289efc5805f8cfcf
# https://gist.github.com/Nora-Ballard/11240204
# https://www.codeproject.com/Articles/2286/Window-Hiding-with-C
# see also: https://qna.habr.com/q/1287504 (probably will remain unanswered)
# https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow
# NOTE: https://www.pinvoke.net is down
# https://www.reddit.com/r/csharp/comments/1an971f/what_happened_to_pinvokenet/

param(
  # NOTE: will not work with chrome - too many processes started, unclear how to find the correct one
  # 'C:\Program Files (x86)\Google\Chrome\Application\chrome.exe',
  # NOTE: will not work with soffice - the process that has main window is named soffice.bin
  # [string]$command = 'C:\Program Files\LibreOffice\program\soffice.exe',
  [string]$command = 'C:\Program Files (x86)\Notepad++\notepad++.exe',
  [string] $state = 'MINIMIZE',
  [int]$delay = 3  
)
start-process $command
start-sleep -seconds $delay

$name = ( $command -replace '^.*\\', '') -replace '\.exe$', ''
# TODO: trouble with 'C:\Program Files (x86)\Notepad++\notepad++.exe'
$name = (( $command -replace '^.*\\', '') -replace '\.exe$', '' ) -replace '\+', '\+'
write-host('name: {0}' -f $name )
$p = get-process  | where-object {$_.name -match ($name + '*') } | select-object -first 1
$handle = $p.MainWindowHandle

if ($handle -eq $null) {
  write-host ('unable to find window of the process {0}' -f $name )
  exit 
}
write-host ('handle: {0}' -f $handle)
$WindowStates = @{
  'FORCEMINIMIZE'    = 11;
  'HIDE'             = 0;
  'MAXIMIZE'         = 3;
  'MINIMIZE'         = 6;
  'RESTORE'          = 9;
  'SHOW'             = 5;
  'SHOWDEFAULT'      = 10;
  'SHOWMAXIMIZED'    = 3;
  'SHOWMINIMIZED'    = 2;
  'SHOWMINNOACTIVE'  = 7;
  'SHOWNA'           = 8;
  'SHOWNOACTIVATE'   = 4;
  'SHOWNORMAL'       = 1;
  }

$methods = Add-Type -MemberDefinition @'
[DllImport("user32.dll")]
public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
[DllImport("user32.dll", SetLastError = true)]
public static extern bool SetForegroundWindow(IntPtr hWnd);
'@ -name 'Win32ShowWindowAsync' -namespace Win32Functions -PassThru
write-host('calling method {0}' -f $methods)
# https://www.p-invoke.net/user32/showwindowasync
$methods::ShowWindowAsync($handle, $WindowStates[$State])|out-null
if ($SetForegroundWindow) {
  $methods::SetForegroundWindow($handle) | out-null
}

write-host ('set the Window State to "{0}" for "{1}"' -f $state, $name)

	
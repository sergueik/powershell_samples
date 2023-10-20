#Copyright (c) 2021 Serguei Kouzmine
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


param(
  [int]$handle = 0
)

function close_window {
  param(
    [int]$handle = 0
  )
  # https://stackoverflow.com/questions/1694451/cannot-use-pinvoke-to-send-wm-close-to-a-windows-explorer-window
  # see also http://poshcode.org/2049
  if (-not ('user32.helper' -as [type])) {
    Add-Type -name 'helper' -namespace 'user32' -MemberDefinition @'
  [DllImport("user32.Dll")]
  public static extern int PostMessage(IntPtr hWnd, UInt32 msg, int wParam, int lParam);
'@
  }
  # http://pinvoke.net/default.aspx/Constants.WM
  # http://pinvoke.net/default.aspx/Enums.WindowsMessages
  [user32.helper]::PostMessage($handle,0x10, 0, 0) | out-null

}

function find_window {
  param([String]$caption
)
# http://www.cyberforum.ru/powershell/thread1502608.html
# http://www.pinvoke.net/default.aspx/user32.getwindowrect
# http://www.pinvoke.net/default.aspx/user32.findwindow
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32 {
  [DllImport("user32.dll", SetLastError = true)]
  public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

  [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
  public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly /* IntPtr.Zero should be the first parameter. */, string lpWindowName);
}
"@
  # NOTE: FindWindowByCaption is not defined in jna:
  # https://github.com/java-native-access/jna/search?q=FindWindowByCaption
  # You can also call FindWindow(default(string), lpWindowName) or FindWindow((string)null, lpWindowName)

  <#
     HWND hWnd = User32.INSTANCE.FindWindowEx(hwndShell, null, "Start", "Start");
     User32.INSTANCE.PostMessage(hWnd, WinUser.WM_CLOSE, null, null);
  #>
  # need exact caption match
  $handle =[Win32]::FindWindowByCaption([IntPtr]::Zero, $caption )

  if ($handle -eq [IntPtr]::Zero) {
    write-error ('wnindow handle with caption "{0}" not found'  -f $caption 	)
    return  $null
  }
  return $handle
}



if ($handle  -eq 0 ) {
  $document = 'test.txt'
  new-item -path $document -force | out-null
  start notepad.exe $document
  sleep -Milliseconds 50
  $caption =  ('{0} - Notepad' -f $document)
  $handle = find_window -caption $caption
  write-output ('handle: {0}' -f $handle)
  $caption_search = $caption

  # NOTE: if the window caption search string contains special characters e.g. parenthesis, needs to be escaped and
  # the following exercise
  # does not work:
  # $caption = 'example (more info in parenthesis)'
  # $caption_search = $caption -replace '(\(|\))', "\$1"
  # the match(es) $1 will evauate to empty
  # only works in single quotes:
  # $caption_search = $caption -replace '(\(|\))', '\$1'
  # an ugly-looking non-capturing replace is another possible alternative
  # $caption_search = $caption -replace '\(', '\(' -replace '\(' , '\('
  $target_process = Get-Process | where {$_.MainWindowTitle -match $caption_search } | select-object -first 1
  if ($target_process -ne $null) {
    $handle = $target_process.MainWindowHandle
    write-output ('handle: {0}' -f $handle)
  }
}
if ($handle  -ne  0 ) {
  write-output ('Closing handle {0}' -f $handle)
  close_window -handle $handle
}

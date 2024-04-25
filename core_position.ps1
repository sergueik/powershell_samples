#Copyright (c) 2021,2024 Serguei Kouzmine
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



param (
  [String]$name = 'calc',
  [String]$title = 'Calculator',
  [int]$left = 300,
  [int]$top  = 300,
  [int]$rx = 0,
  [int]$ry = 230,
  [int]$delay = 1
)
# http://www.cyberforum.ru/powershell/thread1502608.html
# http://www.pinvoke.net/default.aspx/user32.getwindowrect
# https://www.p-invoke.net/user32/getwindowrect

# http://www.pinvoke.net/default.aspx/user32.findwindow
# https://www.p-invoke.net/user32/findwindow-1
# https://pinvokeisalive.gitbook.io/pinvoke/desktopfunctions/user32/findwindowa
# https://stackoverflow.com/questions/9668872/how-to-get-windows-position
# see also:
# https://stackoverflow.com/questions/13520705/move-mouse-to-position-and-left-click
Add-Type @"
using System;
using System.Runtime.InteropServices;

public class Win32 {
	[DllImport("user32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
	[DllImport("user32.dll", SetLastError = true)]
	public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

	[DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
	public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly /* IntPtr.Zero should be the first parameter. */, string lpWindowName);

	// You can also call FindWindow(default(string), lpWindowName) or FindWindow((string)null, lpWindowName)
}
// outside of the class
[StructLayout(LayoutKind.Sequential)]
public struct RECT {
	public int Left;
	public int Top;
	public int Right;
	public int Bottom;
}
"@

# https://stackoverflow.com/questions/12870535/moving-mouse-in-c-sharp-coordinate-units
Add-Type @"
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public static class VirtualMouse {
	[DllImport("user32.dll")]
	static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

	private const int MOUSEEVENTF_MOVE = 0x0001;
	private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
	private const int MOUSEEVENTF_LEFTUP = 0x0004;
	private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
	private const int MOUSEEVENTF_RIGHTUP = 0x0010;
	private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
	private const int MOUSEEVENTF_MIDDLEUP = 0x0040;
	private const int MOUSEEVENTF_ABSOLUTE = 0x8000;


	// positive values indicate movement right, down
	public static void Move(int xDelta, int yDelta) {
		mouse_event(MOUSEEVENTF_MOVE, xDelta, yDelta, 0, 0);
	}

	public static void MoveToAbsolute(int x, int y) {
		mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, x, y, 0, 0);
	}

	public static void LeftClick() {
		mouse_event(MOUSEEVENTF_LEFTDOWN, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
		mouse_event(MOUSEEVENTF_LEFTUP, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
	}

	public static void MoveTo(int X, int Y) {
		var screenBounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
		var normalizedX = X * 65536 / screenBounds.Width + 1;
		var normalizedY = Y * 65536 / screenBounds.Height + 1;
		MoveToAbsolute(normalizedX, normalizedY);
	}
}
"@ -ReferencedAssemblies 'System.Windows.Forms.dll','System.Drawing.dll'



# https://stackoverflow.com/questions/2969321/how-can-i-do-a-screen-capture-in-windows-powershell
[Reflection.Assembly]::LoadWithPartialName('System.Drawing') |out-null

# see also:
# https://codesteps.com/2018/10/09/c-sharp-graphics-draw-an-image-on-other-applications-window/
function drawbounds {
  param (
    [System.Collections.Hashtable]$rect,
    [IntPtr]$handle
  )

  if ($rect -eq $null) {
    [System.Drawing.Rectangle]$rect = new-object System.Drawing.Rectangle(100, 100, 24, 24)
  } else { 
    [System.Drawing.Rectangle]$rect = new-object System.Drawing.Rectangle($rect.left, $rect.top, $rect.width, $rect.height)
  }
  [System.Drawing.Pen] $pen = new-object System.Drawing.Pen([System.Drawing.Color]::Black, 3)
  [System.Drawing.Graphics] $graphics = [System.Drawing.Graphics]::FromHwnd($handle)
  write-host ('Draw rectangle to screen: {0}' -f $rect)
  # Draw rectangle to screen.
  $graphics.DrawRectangle($pen, $rect)
  $graphics.Dispose()
}

function screenshot {
  param (
    [System.Drawing.Rectangle]$rect,
    $path
  )
             
  if ($rect -eq $null) {
    # Create rectangle
    $rect = new-object Rectangle(0, 0, 200, 200)
  }

  $bitmap = new-object Drawing.Bitmap $rect.width, $rect.height
  $graphics = [Drawing.Graphics]::FromImage($bitmap)
  write-host ('Took screenshot bitmap from screen: {0}' -f $rect)
  $graphics.CopyFromScreen($rect.Location, [Drawing.Point]::Empty, $rect.size)
  # https://docs.microsoft.com/en-us/dotnet/api/system.drawing.bitmap.getpixel?view=netframework-4.5
  [System.Drawing.Color]$color = $bitmap.getPixel(1,1)
  # https://docs.microsoft.com/en-us/dotnet/api/system.drawing.color.b?view=netframework-4.5
  write-host ('Pixel at (1,2) Color: R={0} G={1} B={2} A={3} ({4})'-f $color.R, $color.G,  $color.B, $color.A, $color.ToString())
  $bitmap.Save($path)

  $graphics.Dispose()
  $bitmap.Dispose()
}


start $name
sleep -Milliseconds 500
# Alternative calls

$handle = [Win32]::FindWindow($title, [IntPtr]::Zero)
write-output ('MainWindowHandle: {0}' -f $handle)

$handle = [Win32]::FindWindowByCaption([IntPtr]::Zero, $title)
write-output ('MainWindowHandle: {0}' -f $handle)
$handle = get-process | where-object { $_.name -match $name } | select-object -expandproperty MainWindowHandle

write-output ('MainWindowHandle: {0}' -f $handle)

$handle = (Get-Process | where {$_.MainWindowTitle -match $title}).MainWindowHandle
write-output ('MainWindowHandle: {0}' -f $handle)

if ($handle -eq [IntPtr]::Zero) {
  return 'Cannot determine main window handle. Aborting'
  return
}
$rect = new-object RECT
[void][Win32]::GetWindowRect($handle,[ref]$rect)
$width  = $rect.Right - $rect.Left
$height = $rect.Bottom -$rect.Top
write-output ('width = {0},$height = {1}' -f $width,$height)

[void][Win32]::MoveWindow($handle, $left, $top, $width, $height, $true)

# recalculate
[void][Win32]::GetWindowRect($handle,[ref]$rect)
$region = @{
  Left = 100;
  Top = 100;
  Width = 24;
  Height = 24;
}

if ($rx -eq 0 -or $rx -eq $null) {
  $rx = $width / 2
}
if ($ry -eq 0 -or $ry -eq $null) {
  $ry = $height /2
}

$scrap = @{ 
  left = 0;
  top = 0;
  right = 0;
  bottom = 0;
}

# https://docs.microsoft.com/en-us/dotnet/api/system.drawing.rectangle.fromltrb?view=netframework-4.5
if ($region -eq $null -or $region.Width -eq 0) {
  $scrap = @{ 
    left = $rect.Left; 
    top = $rect.Top;
    right = $rect.Right; 
    bottom = $rect.Bottom 
  }
} else {
  if ($region.Left -eq 0) {
    $scrap = @{ 
      left = $rect.Left; 
      top = $rect.Top;
      right = $rect.Left + $region.Width; 
      bottom = $rect.Top + $region.Height
    }
  } else {
    $scrap = @{ 
      left = $rect.Left + $region.Left; 
      top = $rect.Top + $region.Top;
      right = $rect.Left + $region.Width + $region.Left; 
      bottom = $rect.Top + $region.Height + $region.Top
    }
  }
}

$bounds = [System.Drawing.Rectangle]::FromLTRB($scrap.Left, $scrap.Top, $scrap.Right, $scrap.Bottom)

write-output ('move mouse relative to window to {0} {1}' -f $rx, $ry)

[VirtualMouse]::MoveTo($left + $rx , $top + $ry )
start-sleep -seconds $delay
write-output ('left mouse button click on {0} {1}' -f ($left + $rx) , ($top + $ry))
[VirtualMouse]::LeftClick()
screenshot -rect $bounds ((resolve-path '.').Path + '\'+ 'screenshot.png')
drawbounds -rect $region -handle $handle
start-sleep -seconds 3

get-process | where-object { $_.name -match $name } | stop-process

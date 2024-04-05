#Copyright (c) 2022,2024 Serguei Kouzmine
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

# frequently asked to position the window on second display:
# https://stackoverflow.com/questions/1363374/showing-a-windows-form-on-a-secondary-monitor
# https://social.msdn.microsoft.com/Forums/windows/en-US/3fbaa012-b6f5-4461-b4b5-1b320adbd9ee/open-a-winform-on-another-display-or-monitor-in-c?forum=winforms

param (
  [string]$screen = 1,
  [switch]$debug
)
$RESULT_OK = 0
$RESULT_CANCEL = 2

function show_bounds {
  param(
    [System.Windows.Forms.Form]$form
  )
  # https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.form.desktopbounds?view=netframework-4.0
  [System.Drawing.Point]$p = $f.DesktopLocation
  write-host ('X: {0} Y: {1}' -f $p.X , $p.Y)

  [System.Drawing.Rectangle]$b = $f.DesktopBounds 
  write-host ('Width: {0} Height: {1}' -f $b.Width , $b.Height)
  write-host ('X: {0} Y: {1}' -f $b.Location.X , $b.Location.Y)

  write-host ('Width: {0} Height: {1}' -f $f.Size.Width, $f.Size.Height)
  write-host ('X: {0} Y: {1}' -f $f.Location.X , $f.Location.Y)
	

}
function move_to_screen{
  param(
    [int]$number,
    [System.Windows.Forms.Screen[]] $screens,
    [System.Windows.Forms.Form]$form
  )
    [System.Windows.Forms.Screen]$screen = $screens[$number]
    [System.Drawing.Rectangle] $bounds = $screen.Bounds
    # https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.control.setbounds?view=netframework-4.0
    # https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.form.desktoplocation?view=netframework-4.0
    # https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.control.move?view=netframework-4.0
    $form.SetBounds($bounds.X, $bounds.Y, $form.Size.Width, $form.Size.Height)
    # for pinvoke see

    # https://stackoverflow.com/questions/3961249/display-window-on-top-of-other-windows-but-not-the-task-bar/3961447#3961447
    # http://www.pinvoke.net/default.aspx/user32.setwindowpos
    # https://www.p-invoke.net/coredll/setwindowpos
    # https://pinvokeisalive.gitbook.io/pinvoke/desktopfunctions/user32/setwindowpos
    # https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos
    # https://stackoverflow.com/questions/53012896/using-setwindowpos-with-multiple-monitors

}

function select_screen {
  param(
    [string]$title,
    [object]$caller
  )
  @( 'System.Drawing','System.Windows.Forms') | ForEach-Object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }

  $f = New-Object System.Windows.Forms.Form
  $f.MaximizeBox = $false
  $f.MinimizeBox = $false
  $f.Text = $title

  $l1 = New-Object System.Windows.Forms.Label
  $l1.Location = New-Object System.Drawing.Size (10,20)
  $l1.Size = New-Object System.Drawing.Size (100,20)
  $l1.Text = 'Screen'
  $f.Controls.Add($l1)

  $f.Font = New-Object System.Drawing.Font ('Microsoft Sans Serif',10,[System.Drawing.FontStyle]::Regular,[System.Drawing.GraphicsUnit]::Point,0)

  $t1 = New-Object System.Windows.Forms.TextBox
  $t1.Location = New-Object System.Drawing.Point (120,20)
  $t1.Size = New-Object System.Drawing.Size (140,20)

  # https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.screen?view=netframework-4.0
  [System.Windows.Forms.Screen[]] $screens = [System.Windows.Forms.Screen]::AllScreens
  # dislay the number of screens. 
  # TODO: disable when only one
  $t1.Text = $screens.Length
  $t1.Name = 'txtUser'
  $f.Controls.Add($t1)

  $btnOK = New-Object System.Windows.Forms.Button
  $x2 = 20
  $y1 = ($t1.Location.Y + $t1.Size.Height + + $btnOK.Size.Height + 20)
  $btnOK.Location = New-Object System.Drawing.Point ($x2,$y1)
  $btnOK.Text = 'OK'
  $btnOK.Name = 'btnOK'
  $f.Controls.Add($btnOK)
  $btnCancel = New-Object System.Windows.Forms.Button
  $x1 = (($f.Size.Width - $btnCancel.Size.Width) - 20)

  $btnCancel.Location = New-Object System.Drawing.Point ($x1,$y1)
  $btnCancel.Text = 'Cancel'
  $btnCancel.Name = 'btnCancel'
  $f.Controls.Add($btnCancel)

  $s1 = ($f.Size.Width - $btnCancel.Size.Width) - 20
  $y2 = ($t1.Location.Y + $t1.Size.Height + $btnOK.Size.Height)

  $f.Size = New-Object System.Drawing.Size ($f.Size.Width,(($btnCancel.Location.Y +
        $btnCancel.Size.Height + 40)))

  $btnCancel.add_click({
      $f.Close()
    })
  $btnOK.add_click({
    $number = [int] $t1.Text
    $number = $number - 1 
    move_to_screen -form $f -screens $screens -number $number
    show_bounds -form $f
  })

  $f.Controls.Add($l)
  $f.Topmost = $true

  $f.Add_Shown({
  $f.Activate() 
  show_bounds -form $f
})
  $f.KeyPreview = $True
  $f.Add_KeyDown({
    if ($_.KeyCode -eq 'Escape') {
      $f.Close()  
    } else { 
      $number = [int] $t1.Text
      $number = $number - 1 
      move_to_screen -form $f -screens $screens -number $number
      show_bounds -form $f
      return 
    }
  })

  [void]$f.ShowDialog()

  $f.Dispose()
}

Add-Type -TypeDefinition @'
using System;
using System.Windows.Forms;
public class Win32Window : IWin32Window {
    private IntPtr _hWnd;
    private int _data;

    public int Data {
        get { return _data; }
        set { _data = value; }
    }


    public Win32Window(IntPtr handle) {
        _hWnd = handle;
    }

    public IntPtr Handle {
        get { return _hWnd; }
    }
}

'@ -ReferencedAssemblies 'System.Windows.Forms.dll'



[void][System.Reflection.Assembly]::LoadWithPartialName('System.Windows.Forms')
[void][System.Reflection.Assembly]::LoadWithPartialName('System.Drawing')

$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent
if ($debug){
  $DebugPreference = 'Continue'
}
$title = 'Choose Screen'

select_screen -Title $title

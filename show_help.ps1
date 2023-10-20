#Copyright (c) 2022,2023 Serguei Kouzmine
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

# a shorter version of simple_browser_localfile.ps1
param (
  [string]$filename = '' # = 'help.html'
)

function show_help {
  param(
    [string]$file_url = $null,
    [object]$caller = $null
  )
  @( 'System.Drawing','System.Collections','System.ComponentModel','System.Windows.Forms','System.Data') | ForEach-Object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }

  $f = new-object System.Windows.Forms.Form
  $f.Text = $title

  $timer1 = new-object System.Timers.Timer
  $label1 = new-object System.Windows.Forms.Label

  $f.SuspendLayout()
  $components = new-object System.ComponentModel.Container

  $browser = new-object System.Windows.Forms.WebBrowser
  $f.SuspendLayout();

  $browser.Dock = [System.Windows.Forms.DockStyle]::Fill
  $browser.Location = new-object System.Drawing.Point (0,0)
  $browser.Name = 'webBrowser1'
  $browser.Size = new-object System.Drawing.Size (600,600)
  $browser.TabIndex = 0

  $f.AutoScaleDimensions = new-object System.Drawing.SizeF (6,13)
  $f.AutoScaleMode = [System.Windows.Forms.AutoScaleMode]::Font
  $f.ClientSize = new-object System.Drawing.Size (600,600)
  $f.Controls.Add($browser)
  $f.Text = 'Show File'
  $f.ResumeLayout($false)
  $pageContent = @'
<html>
  <body>
<h3>Usage:</h3>
<pre>. ./show_help.ps1 -filename [HTML FILE]</pre>
to display the HELP FILEin the decorated browser window</body>
</html>
'@
  $f.add_Load({
      param([object]$sender,[System.EventArgs]$eventArgs)
      if (($file_url -eq $null ) -or ($file_url -eq '' )){
        $browser.DocumentText = $pageContent
      } else {
        $browser.Navigate($file_url)
      }
    })
  $f.ResumeLayout($false)
  $f.Topmost = $True

  $f.Add_Shown({ $f.Activate() })
  # NOTE: cannot use 
  # [void]$f.ShowDialog([IWin32Window]($caller))
  # Unable to find type [IWin32Window].
  # NOTE: cannot use $caller.GetType() explicitly (?) in the cast
  # [void]$f.ShowDialog($caller.GetType()$caller)
  [void]$f.ShowDialog($caller)
  $browser.Dispose() 

}
$helper_class = 'win32window_helper'
Add-Type -TypeDefinition @"

using System;
using System.Windows.Forms;
public class ${helper_class} : IWin32Window {
    private IntPtr _hWnd;
    private int _data;

    public int Data {
        get { return _data; }
        set { _data = value; }
    }

    public ${helper_class}(IntPtr handle) {
        _hWnd = handle;
    }

    public IntPtr Handle {
      get { return _hWnd; }
    }
}

"@ -ReferencedAssemblies 'System.Windows.Forms.dll'

$DebugPreference = 'Continue'

$caller = new-object -typename $helper_class -ArgumentList ([System.Diagnostics.Process]::GetCurrentProcess().MainWindowHandle)

if ($filename -ne '') {
  show_help ('file:///{0}' -f ((resolve-path '.').Path + '\'+ $filename))
} else {
  show_help -caller $caller
}

# Console colors
# NOTE: Get-PSReadlineOption, Set-PSReadlineOption are not available in Powershelll 5.1.14409.1005 (Windows 7) - works with 5.1.19041.1 (Windows 10) - check $psversiontable.PSVersion
# will need to use $host.PrivateData, $host.ui.rawui ForegroundColor, BackgroundColor
# see also: https://www.pdq.com/blog/change-powershell-colors/

# about using Set-PSReadlineOption see: https://4sysops.com/wiki/change-powershell-console-syntax-highlighting-colors-of-psreadline/

# red: 1
# green: 36
# blue: 86

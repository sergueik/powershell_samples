#Copyright (c) 2014,2022,2023 Serguei Kouzmine
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
# converted from http://www.java2s.com/Tutorial/CSharp/0460__GUI-Windows-Forms/AsimpleBrowser.htm
# http://www.java2s.com/Code/CSharpAPI/System.Windows.Forms/TabControlControlsAdd.htm
# with sizes adjusted to run the focus demo
# see also:
# https://stackoverflow.com/questions/17926197/open-local-file-in-system-windows-forms-webbrowser-control
# http://www.java2s.com/Tutorial/CSharp/0460__GUI-Windows-Forms/AsimpleBrowser.htm
# https://www.c-sharpcorner.com/UploadFile/mahesh/webbrowser-control-in-C-Sharp-and-windows-forms/
param (
  [string]$filename
)
Add-Type -TypeDefinition @'

using System;
using System.Text;
using System.Net;
using System.Windows.Forms;

using System.Runtime.InteropServices;

public class Win32Window : IWin32Window {
    private IntPtr _hWnd;

    public Win32Window(IntPtr handle) {
        _hWnd = handle;
    }

    public IntPtr Handle {
        get { return _hWnd; }
    }
}

'@ -ReferencedAssemblies 'System.Windows.Forms.dll'


function showLocalFile {
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
  <head>
    <!-- "head" tag is not understood by IE based WebBrowser control, anything here will be displayed -->
  </head>
  <body><h3>Usage:</h3><pre>. ./simple_browser_localfile.ps1 -filename [HTML FILE]</pre><br/>where the <code>[HTML FILE]</code> is looked in the current directory</body>
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

  [void]$f.ShowDialog([win32window]($caller))
  $browser.Dispose() 
}

$caller = new-object Win32Window -ArgumentList ([System.Diagnostics.Process]::GetCurrentProcess().MainWindowHandle)
if (($filename -eq $null ) -or ($filename -eq '' )){
  showLocalFile -caller $caller
} else {
  $prefix = 'file://'
  $filepath = ( resolve-path '.' ).Path + '\' + $filename
  $file_url = ('{0}/{1}' -f $prefix, $filepath)
  showLocalFile $file_url $caller
}

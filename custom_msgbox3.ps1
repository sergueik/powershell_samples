#Copyright (c) 2020,2022 Serguei Kouzmine
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

# $DebugPreference = 'Continue'
# Dialog Message in C# for .NET Framework
# https://www.codeproject.com/Articles/5264875/Dialog-Message-in-Csharp-for-NET-Framework-4-5
# https://github.com/chris-mackay/DialogMessage/tree/master/DialogMessage

param (
  [string]$user = 'demouser',
  [string]$systemicon,
  [String]$mainInstruction = 'please verify the stuff',
  [String]$detailedInstruction = 'do step 1, step 2 and step 3' + [char]10 + 'then step 4 and step 5',
  [switch]$debug
)
function DialogMessage{
  param(
    [string]$title,
    [string]$systemicon,
    [string]$mainInstruction,
    [string]$content_text = '',
    [object]$caller
  )
    @( 'System.Drawing','System.Windows.Forms') | ForEach-Object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }

  $f = new-object System.Windows.Forms.Form
  $f.Height = 157

  $components = new-object System.ComponentModel.Container

  $f.MaximizeBox = $false
  $f.MinimizeBox = $false
  $f.Text = $title
  $panel = new-object System.Windows.Forms.Panel
  $content = new-object System.Windows.Forms.Label
  $content.Text = $content_text

  $m = new-object System.Windows.Forms.Label
  $m.Text = $mainInstruction
  $tl = new-object System.Windows.Forms.TableLayoutPanel
  $b = new-object System.Windows.Forms.Button
  $icon = new-object System.Windows.Forms.PictureBox
  $panel.SuspendLayout()
  $tl.SuspendLayout()
  # ((System.ComponentModel.ISupportInitialize)($icon)).BeginInit()
  $f.SuspendLayout()
  # whiteSpace panel
  $panel.Anchor = [System.Windows.Forms.AnchorStyles]::Top -bor [System.Windows.Forms.AnchorStyles]::Bottom -bor [System.Windows.Forms.AnchorStyles]::Left -bor [System.Windows.Forms.AnchorStyles]::Right
  $panel.BackColor = [System.Drawing.Color]::White
  $panel.Controls.Add($content)
  $panel.Controls.Add($m)
  $panel.Location = new-object System.Drawing.Point(0, 0)
  $panel.Name = 'whiteSpace'
  $panel.Size = new-object System.Drawing.Size(383, 79)
  $panel.TabIndex = 1

  $content.AutoSize = $true
  $content.Font = new-object System.Drawing.Font('Microsoft Sans Serif' , 8.25, [System.Drawing.FontStyle]::Regular, [System.Drawing.GraphicsUnit]::Point, 0)
  $content.Location = new-object System.Drawing.Point(54, 51)
  $content.MaximumSize = new-object System.Drawing.Size(305, 0)
  $content.Name = 'content'
  $content.Size = new-object System.Drawing.Size(44, 13)
  $content.TabIndex = 1
  $content.Text = $detailedInstruction

  $m.AutoSize = $true
  $m.Font = new-object System.Drawing.Font('Segoe UI', 12, [System.Drawing.FontStyle]::Regular, [System.Drawing.GraphicsUnit]::Point, 0)
  $m.ForeColor = [System.Drawing.Color]::FromArgb(0, 51, 188 )
  $m.Location = new-object  System.Drawing.Point(54, 13)
  $m.Margin = new-object System.Windows.Forms.Padding(5, 0, 3, 18)
  $m.MaximumSize = new-object System.Drawing.Size(305, 0)
  $m.Name = 'mainInstruction'
  $m.Size = new-object System.Drawing.Size(123, 21)
  $m.TabIndex = 0
  $m.Text = $mainInstruction

  $tl.Anchor = [System.Windows.Forms.AnchorStyles]::Bottom -bor [System.Windows.Forms.AnchorStyles]::Right
  $tl.ColumnCount = 2
  $x1 = new-object System.Windows.Forms.ColumnStyle([System.Windows.Forms.SizeType]::Percent, 50)
  $tl.ColumnStyles.Add($x1) | out-null
  $x2 = new-object System.Windows.Forms.ColumnStyle([System.Windows.Forms.SizeType]::Percent, 50)
  [void]$tl.ColumnStyles.Add($x2)
  $tl.Controls.Add($b, 1, 0)

  $tl.Location = new-object System.Drawing.Point(232, 83)
  $tl.Name = 'tablePanelLayout'
  $tl.RowCount = 1
  $x3 = new-object System.Windows.Forms.RowStyle([System.Windows.Forms.SizeType]::Percent, 50)
  $tl.RowStyles.Add($x3)  | out-null
  $tl.Size = new-object System.Drawing.Size(146, 29)
  $tl.TabIndex = 2

  $b.Anchor = [System.Windows.Forms.AnchorStyles]::None
  $b.Location = new-object System.Drawing.Point(76, 3)
  $b.Name = 'button2'
  $b.Size = new-object System.Drawing.Size(67, 23)
  $b.TabIndex = 1
  $b.Text = 'Button 2'

  # Dialog Icon
  $icon.BackColor = [System.Drawing.Color]::White
  $icon.Location = new-object System.Drawing.Point(14, 14)
  $icon.Name = 'msgIcon'
  $icon.Size = new-object System.Drawing.Size(32, 32)
  $icon.TabIndex = 3
  $icon.TabStop = $false

  # https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.dialogresult?view=netframework-4.0
  $b.DialogResult = [System.Windows.Forms.DialogResult]::OK
  $b.Text = 'OK'
  $f.AcceptButton = $b
  $b.TabIndex = 0

  try {
    $f.ActiveControl = $b
  } catch [Exception] {
    # Exception setting "ActiveControl": "Invisible or disabled control cannot be activated"
  }
  # https://docs.microsoft.com/en-us/dotnet/api/system.drawing.systemicons?view=netframework-4.0
  $icon.Image = [System.Drawing.SystemIcons]::Exclamation.ToBitmap()
  if ($debug){
    write-host ( 'choosing system icon for argument {0}' -f $systemicon )
  }
  [System.Drawing.Icon] $si = $null
  if (( $systemicon -ne $null ) -and ($systemicon -ne '' )){
    switch ($systemicon) {
      'Application' {  $si = [System.Drawing.SystemIcons]::Application }
      'Asterisk' { $si = [System.Drawing.SystemIcons]::Asterisk }
      'Error' { $si = [System.Drawing.SystemIcons]::Error }
      'Exclamation' { $si = [System.Drawing.SystemIcons]::Exclamation }
      'Hand' {  $si = [System.Drawing.SystemIcons]::Hand }
      'Information' { $si = [System.Drawing.SystemIcons]::Information }
      'Question' { $si = [System.Drawing.SystemIcons]::Question }
      'Shield' { $si = [System.Drawing.SystemIcons]::Shield }
      'Warning' { $si = [System.Drawing.SystemIcons]::Warning }
      'WinLogo' { $si = [System.Drawing.SystemIcons]::WinLogo }
    }
    if ($debug){
      write-host $si.toString()
    }
    $icon.Image = $si.ToBitmap()
  } else {
    $iconBase64 = 'iVBORw0KGgoAAAANSUhEUgAAAD8AAABBCAYAAABmd3xuAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAAEnQAABJ0Ad5mH3gAAAUISURBVHhe7VltcxNVFPbP+QKIKKAItLS8VBheHWR8GTqjDgj1AzBUHVCcQSrOCO0HLTgqfuITH5RhwAGl1pLdvJGwSdskbbK7SR7PububtMvNDG13Y7e5Z+aZbdLkJs+5z3nuOZvn0MGhyHdqKPKdGoq8LGKbnl9RML464zJrhiIvC9kCUYYi7wtFXhayBaIMRd4XirwsZAtEGYq8LxR5WcgWiDLaT/7NF1y8iNhmAl8bzxFk7wkJ7SW/ZRX0HesRf38v4gP9SF84i+TgSegfHUH87e3Qtq11E9CeJLSVvLbjNRhXvkE5qcOczMOeKcEqTMPM51B4cA/pi19AP7ANWteatqigvTvfvQbp08dQ1h+hZlZQr1absC3YpSKm/7iF5IeHEdu6OvQEhE9+bi1TjWs9ryB95hhmx/9GzbZRr9VQr9cd0N8100Tx7m0kjh6gBKx6er0AES55Iq1RjWvdLzcTwNeu1Uif+hizj8YpAVaTvAtWxdStm9D6Xm++LwSER55dnGo8f+M6MpfOQ+tdR4b3kvM/VsD2V50ETPxDkicFEGkOkYBaFfb0FBKnj1PiqP79aweE8Mizs/cfROVxCuaTDDJDX0LfvclJAO8mH3PsAedOwSKi8+TPu18pI//bT9DeekO+fgAIjzwRSw2egD2ZEztrkbsbw0PQ9mwm4pQA3v2d65H97mvY5PhMnqORAMtCaewB9CN98vUDQGjkuc5Tn38Ke2rSIUPkzEyaEvAt9L1bBLJD51FJJcjtHdnPA3kBewKrR7Z+EAiRPO382U9o5/PivV4CLEqGMXoF2auXYBpZUd9zg18nrkR+ZmIM+gf7pOsHgdBr3sw+nr+jdKbbxQJhGjU+33213oBlonDnd2gHe+TrB4DwyHNNk8GV/ryDGtWvR8oL/2MvvOe55rnhiR/e2TRJ2ecsAeGRZ1CTItw8Z7Te4VagcqhSx5e/NiL8oXFMBohwybu7b4wO07lNxuee588M9gjyhdzoVcSp5+dSClIB4ZJnULOjH+pF/pcfxXEn+nhWwbMqgV7HHpGjBGp9G51jMqAEhE+exlONhhR931ZkaGqbGX8o5Mw9vEgAqYE9QTx2jzyOuVd+XZUMMvfrNafpYQVIP2thaAN5F7RbPNTE392DFM3xkzdvoHj/Lgr3bsP4+Qdkvr+IcjzmmKOrCo7G1fWAHHlAfH93ICbYPvIM/rLc1tIX5xsXPN9zj8/zu5j2Bk+irE1QAkgFvON+8DFJ3SB7gL6/a8km2F7yfoidc8HK2LUBaeoKedgRCZD5Aj1nGU9IAcOilJZSAv8veT84GaQIVkAlnXTG3RYJYAUY10fEaSLUtIgSWF7kGawAGn8TA/2o0CzQqgS4OxR9gDf5LUIBy488gxNACuDZoBz7t2GCHPOSIBQw5fQB7AGsANl6LbA8yTO8EvhsALPCBJ++4+MkwDHB7OULiJFpLkT+y5c8gxWwayOZ4ICrAIkJ8gkwU4IxcpnIr1tB5BmeAgZPCBMULbKXALrWymUUafpL9h9aQbKfC1ZAz1oywaOUgISY+Ii9uNFZenhf/AiymHt90SDP4AT0Oo3Q7NhfsPKG2PHGLe4FyN1DdMgz3BJIHn9PzAnxd/rcLm/hxBnRIs/gBNCgJH7XW+KIGz3yAUKR94UiLwvZAlGGIu8LRV4WsgWiDEXeF4q8LGQLRBmKvC9aku+EUOQ7NRT5To0OJg/8B2e5AyQxI1LwAAAAAElFTkSuQmCC'
    $iconBytes = [Convert]::FromBase64String($iconBase64)
    $stream = New-Object IO.MemoryStream($iconBytes, 0, $iconBytes.Length)
    $stream.Write($iconBytes, 0, $iconBytes.Length);
    $icon.image = [System.Drawing.Image]::FromStream($stream, $true)
  }

  $f.AutoScaleDimensions = new-object System.Drawing.SizeF(6, 13)
  $f.AutoScaleMode = [System.Windows.Forms.AutoScaleMode]::Font
  $f.ClientSize = new-object System.Drawing.Size(383, 118)
  $f.Controls.Add($icon)
  $f.Controls.Add($tl)
  $f.Controls.Add($panel)
  $f.FormBorderStyle = [System.Windows.Forms.FormBorderStyle]::FixedDialog
  $f.MaximizeBox = $false
  $f.MinimizeBox = $false
  $f.MinimumSize = new-object System.Drawing.Size(360, 157)
  $f.Name = 'MainForm'
  $f.ShowIcon = $false
  $f.ShowInTaskbar = $false
  $f.StartPosition = [System.Windows.Forms.FormStartPosition]::CenterParent
  $f.Text = 'Window Title'
  # $f.Load += new-objectSystem.EventHandler($f.DialogMessage_Load)
  $panel.ResumeLayout($false)
  $panel.PerformLayout()
  $tl.ResumeLayout($false)
  # ((System.ComponentModel.ISupportInitialize)($icon)).EndInit()
  $f.ResumeLayout($false)

  [void]$f.ShowDialog([System.Windows.Forms.IWin32window]($caller))

  $f.Dispose()

}

$helper_class = 'win32windowhelper'
if ( -not ($helper_class -as [type])){
  Add-Type -TypeDefinition @"
  using System;
  using System.Windows.Forms;
  public class $helper_class : IWin32Window {
      private IntPtr _hWnd;
      public $helper_class(IntPtr handle) {
          _hWnd = handle;
      }
  
      public IntPtr Handle {
          get { return _hWnd; }
      }
  }
  
"@ -ReferencedAssemblies 'System.Windows.Forms.dll'
# NOTE: no indent with closing HEREDOC delimiter
}

[void][System.Reflection.Assembly]::LoadWithPartialName('System.Windows.Forms')
[void][System.Reflection.Assembly]::LoadWithPartialName('System.Drawing')


$window_handle = [System.Diagnostics.Process]::GetCurrentProcess().MainWindowHandle
write-debug ('Using current process handle {0}' -f $window_handle)
if ($window_handle -eq 0) {
  $processid = [System.Diagnostics.Process]::GetCurrentProcess().Id
  $parent_process_id = get-wmiobject win32_process | where-object {$_.processid -eq  $processid } | select-object -expandproperty parentprocessid

  $window_handle = get-process -id $parent_process_id | select-object -expandproperty MainWindowHandle
  write-debug ('Using current process parent process {0} handle {1}' -f $parent_process_id, $window_handle)
}

$caller = new-object -typename $helper_class -ArgumentList $window_handle


dialogMessage -caller $caller -systemicon $systemicon -mainInstruction $mainInstruction -detailedInstruction $detailedInstruction

# Usage:
#  . .\custom_msgbox3.ps1 -systemicon exclamation

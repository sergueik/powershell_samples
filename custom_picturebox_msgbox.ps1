#Copyright (c) 2023 Serguei Kouzmine
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

# origin: https://github.com/sergueik/powershell_ui_samples/blob/master/custom_msgbox.ps1

# reduced from generic to just yes/no messagebox.Exploring the text clipping matter

function dialog {
  param (
    [string]$mesage1
  )

  @( 'System.Drawing','System.Windows.Forms') | ForEach-Object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }

  $f = new-object System.Windows.Forms.Form
  $by = new-object System.Windows.Forms.Button
  $bn = new-object System.Windows.Forms.Button
  $i = new-object System.Windows.Forms.PictureBox
  $p = new-object System.Windows.Forms.Panel
  $p.Dock = [System.Windows.Forms.DockStyle]::Fill
  # https://learn.microsoft.com/en-us/dotnet/api/system.drawing.systemicons?view=netframework-4.0
  $i.Image = ([System.Drawing.SystemIcons]::Warning).ToBitmap()

  $i.Height = 36
  $i.Width = 40
  $i.Location = new-object System.Drawing.Point (10,11)

  $l = new-object System.Windows.Forms.Label

  $f.Controls.Add($p)
  $p.Dock = [System.Windows.Forms.DockStyle]::Fill

  $l.Location = new-object System.Drawing.Point (($i.Location.X + $i.Width + 8),$i.Location.Y)
  $l.Text = ( 'Warning!' + [char]10+ 'something something something something' + [char]10 + 'Do You like to Proceed?' )

  $l.Height = 130
  $l.Width = 200

  # NOTE: without specifying the dimentions explicitly the text will be badly clipped
  # set BorderStyle below to reveal
  # $l.BorderStyle = [System.Windows.Forms.BorderStyle]::FixedSingle
  # https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.borderstyle?view=netframework-4.0
  # https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.borderstyle?view=netframework-4.0

  $l.Font = new-object System.Drawing.Font ('Microsoft Sans Serif',11,[System.Drawing.FontStyle]::Regular,[System.Drawing.GraphicsUnit]::Point,0)
  # With realistic dimensions the text will be wrapped over automatically

  $p.Controls.Add($i)
  $p.Controls.Add($l)

  $bn.Location = new-object System.Drawing.Point(141,144)
  $bn.Text = 'No'
  $bn.add_click.Invoke({
    param(
      [object]$sender,
      [System.EventArgs]$eventargs
    )
    $script:Result = $false
    $f.close()
  })
  $p.Controls.Add($bn)

  $by.Text = 'Yes'
  $by.add_click.Invoke({
      param(
        [object]$sender,
        [System.EventArgs]$eventargs
      )
    $script:Result = $true
    $f.close()
  })
  $by.Width = 80
  $by.Height = 24
  $by.Location = new-object System.Drawing.Point (($bn.Location.X - $bn.Width - 12), $bn.Location.y)

  $p.Controls.Add($by)

  [void]$f.ShowDialog()

  Write-Host ('$script:Result = ' + $script:Result)
  return $script:Result

}

dialog -message1 'something something something something' -message2 'Do You like to Proceed?'
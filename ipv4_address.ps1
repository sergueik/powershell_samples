#Copyright (c) 2014,2022 Serguei Kouzmine
#Permission is hereby granted, free of charge, to any person obtaining a copy
#of this software and associated documentation files (the "Software"), to deal
#in the Software without restriction, including without limitation the rights
#to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
#copies of the Software, and to permit persons to whom the Software is
#furnished to do so, subject to the following conditions:
#The above copyright notice and this permission notice shall be included in
#all copies or substantial portions of the Software.
#THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
#IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
#FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
#AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
#LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
#OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
#THE SOFTWARE.

# WARNING: fragile under DTW.PS.PrettyPrinterV1.psm1
# home-grown ternary ?:
# http://blogs.msdn.com/b/powershell/archive/2006/12/29/dyi-ternary-operator.aspx
# http://scriptinghell.blogspot.com/2012/10/ternary-operator-support-in-powershell.html
#

function ipv4_address () {
  param(
    [object]$ip1,
    [object]$ip2,
    [object]$ip3,
    [object]$ip4
  )
  $r = @()
  @( $ip1, $ip2, $ip3, $ip4) | foreach-object {
    $q = $_
    if ($q.Length -gt 0) { $u = $q } else { $u = '0' }
    $d = [int]::Parse($u).ToString()
    $r += $d
  }
  # $result = [System.Net.IPAddress]::Parse( ($r -join '.') )
  $result = try_parse -address ($r -join '.')

  return $result.IPAddressToString
}


function try_parse {
  param(
    $address
  )
  [bool]$status = $false
  [System.Net.IPAddress] $result = $null
  $status = ($address.Split('.').Where{
    try {
      [int]::Parse($_) -in (0..255)
    } catch {
      $null
    }}).Count -eq 4
  if ($status) {
    #  https://docs.microsoft.com/en-us/dotnet/api/system.net.ipaddress.tryparse?view=netframework-4.5
    # $result = ([System.Net.IPAddress]::TryParse($address,[ref][ipaddress]::Loopback))
    [System.Net.IPAddress]::TryParse($address,([ref] $result ))
  }
  return $result
}

#
function dialogForContinueAuto {
  param(
    $title,
    $message,
    $owner
  )

  @( 'System.Drawing','System.Windows.Forms') | foreach-object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }

  $f = new-object System.Windows.Forms.Form
  $f.Text = $title
  $f.Size = new-object System.Drawing.Size (400,130)
  $f.Owner = $owner
  $f.StartPosition = [System.Windows.Forms.FormStartPosition]::CenterScreen
  $f.Topmost = $True
  $p = new-object System.Windows.Forms.Panel
  $ip1 = new-object System.Windows.Forms.TextBox
  $ip2 = new-object System.Windows.Forms.TextBox
  $ip3 = new-object System.Windows.Forms.TextBox
  $ip4 = new-object System.Windows.Forms.TextBox

  function text_changed () {
    param(
      [object]$sender,
      [System.EventArgs]$eventargs
    )
    [int]$box_type = 0

    [System.Globalization.CultureInfo]$ci = new-object System.Globalization.CultureInfo('en-US')

    [double]$d = 0
    # see also:
    # https://stackoverflow.com/questions/7924000/ip-address-in-a-maskedtextbox
    # https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms?view=netframework-4.5
    switch ($sender) {

      $ip1 {
        if (($ip1.Text.Length -gt 0) -and ($ip1.Text.ToCharArray()[$ip1.Text.Length - 1] -eq '.'))  {
          $ip1.Text = $ip1.Text.TrimEnd('.')

          if ($ip1.Text.Length -gt 0) {
            $ip1.Text = [int]::Parse($ip1.Text).ToString()
          } else {
            $ip1.Text = '0'
          }
          $ip2.Focus()
          return
        }
        # integer validation
        if ([double]::TryParse( $ip1.Text, [System.Globalization.NumberStyles]::Integer, $ci, ([ref]$d)) -eq $false ) {
          $ip1.Text = $ip1.Text.Remove(0,$ip1.Text.Length)
          return
        }
        # change focus to the next textbox if fully inserted
        if ($ip1.Text.Length -eq 3) {
          if ([int]::Parse($ip1.Text) -ge 255) {
            $ip1.Text = '255'
          } else {
            $ip1.Text = [int]::Parse($ip1.Text).ToString()
          }
          $ip2.Focus()
        }
      }

      $ip2 {
        if (($ip2.Text.Length -gt 0) -and ($ip2.Text.ToCharArray()[$ip2.Text.Length - 1] -eq '.')) {
          $ip2.Text = $ip2.Text.TrimEnd('.')

          if ($ip2.Text.Length -gt 0) {
            $ip2.Text = [int]::Parse($ip2.Text).ToString()
          } else {
            $ip2.Text = '0'
          }
          $ip3.Focus()
          return
        }

        # integer validation
        if ([double]::TryParse( $ip2.Text, [System.Globalization.NumberStyles]::Integer, $ci, ([ref]$d)) -eq $false ) {
          $ip2.Text = $ip2.Text.Remove(0,$ip2.Text.Length)
          return
        }

        # change focus to the next textbox if fully inserted
        if ($ip2.Text.Length -eq 3) {
          if ([int]::Parse($ip2.Text) -ge 255) {
            $ip2.Text = '255'
          } else {
            $ip2.Text = [int]::Parse($ip2.Text).ToString()
          }
          $ip3.Focus()
        }
      }

      $ip3 {
        if (($ip3.Text.Length -gt 0) -and ($ip3.Text.ToCharArray()[$ip3.Text.Length - 1] -eq '.')) {
          $ip3.Text = $ip3.Text.TrimEnd('.')

          if ($ip3.Text.Length -gt 0) {
            $ip3.Text = [int]::Parse($ip3.Text).ToString()
          } else {
            $ip3.Text = '0'
          }
          $ip4.Focus()
          return
        }
        # integer validation
        if ([double]::TryParse( $ip3.Text, [System.Globalization.NumberStyles]::Integer, $ci, ([ref]$d)) -eq $false ) {
          $ip3.Text = $ip3.Text.Remove(0,$ip3.Text.Length)
          return
        }
        # change focus to the next textbox if fully inserted
        if ($ip3.Text.Length -eq 3) {
          if ([int]::Parse($ip3.Text) -ge 255) {
            $ip3.Text = '255'
          } else {
            $ip3.Text = [int]::Parse($ip3.Text).ToString()
          }
          $ip4.Focus()
        }
      }

      $ip4 {
        # integer validation
        if ([double]::TryParse( $ip4.Text, [System.Globalization.NumberStyles]::Integer, $ci, ([ref]$d)) -eq $false ) {
          $ip4.Text = $ip4.Text.Remove(0,$ip4.Text.Length)
          return
        }
        if ($ip4.Text.Length -eq 3) {
          if ([int]::Parse($ip4.Text) -ge 255) {
            $ip4.Text = '255'
          } else {
            $ip4.Text = [int]::Parse($ip4.Text).ToString()
          }
        }

      }

    }
    # Write-Debug $box_type
  }

  $dot_label_1 = new-object System.Windows.Forms.Label
  $dot_label_2 = new-object System.Windows.Forms.Label
  $dot_label_3 = new-object System.Windows.Forms.Label

  $p.SuspendLayout()

  $ip1.BorderStyle = [System.Windows.Forms.BorderStyle]::None
  $ip1.Location = new-object System.Drawing.Point (2,3)
  $ip1.MaxLength = 3
  $ip1.Name = 'ip1'
  $ip1.Size = new-object System.Drawing.Size (20,13)
  $ip1.TabIndex = 0
  $ip1.Text = '0'
  $ip1.TextAlign = [System.Windows.Forms.HorizontalAlignment]::Center
  $ip1_text_changed = $ip1.add_TextChanged
  $ip1_text_changed.Invoke({
    param(
      [object]$sender,
      [System.EventArgs]$eventargs
    )
    text_changed ($sender,$eventargs)
  })

  $ip2.BorderStyle = [System.Windows.Forms.BorderStyle]::None
  $ip2.Location = new-object System.Drawing.Point (28,3)
  $ip2.MaxLength = 3
  $ip2.Name = 'ip2'
  $ip2.Size = new-object System.Drawing.Size (20,13)
  $ip2.TabIndex = 1
  $ip2.Text = '0'
  $ip2.TextAlign = [System.Windows.Forms.HorizontalAlignment]::Center
  $ip2_text_changed = $ip2.add_TextChanged
  $ip2_text_changed.Invoke({
    param(
      [object]$sender,
      [System.EventArgs]$eventargs
    )
    text_changed ($sender,$eventargs)
  })

  $ip3.BorderStyle = [System.Windows.Forms.BorderStyle]::None
  $ip3.Location = new-object System.Drawing.Point (56,3)
  $ip3.MaxLength = 3
  $ip3.Name = 'ip3'
  $ip3.Size = new-object System.Drawing.Size (20,13)
  $ip3.TabIndex = 2
  $ip3.Text = '0'
  $ip3.TextAlign = [System.Windows.Forms.HorizontalAlignment]::Center
  $ip3_text_changed = $ip3.add_TextChanged
  $ip3_text_changed.Invoke({
    param(
      [object]$sender,
      [System.EventArgs]$eventargs
    )
    text_changed ($sender,$eventargs)
  })

  $ip4.BorderStyle = [System.Windows.Forms.BorderStyle]::None
  $ip4.Location = new-object System.Drawing.Point (84,3)
  $ip4.MaxLength = 3
  $ip4.Name = 'ip4'
  $ip4.Size = new-object System.Drawing.Size (20,13)
  $ip4.TabIndex = 3
  $ip4.Text = '0'
  $ip4.TextAlign = [System.Windows.Forms.HorizontalAlignment]::Center
  $ip4_text_changed = $ip4.add_TextChanged
  $ip4_text_changed.Invoke({
    param(
      [object]$sender,
      [System.EventArgs]$eventargs
    )
    text_changed ($sender,$eventargs)
  })

  $dot_label_1.BackColor = [System.Drawing.Color]::White
  $dot_label_1.Location = new-object System.Drawing.Point (25,-5) # centered
  $dot_label_1.Name = 'dotLabel1'
  $dot_label_1.Size = new-object System.Drawing.Size (1,25)
  $dot_label_1.Text = '.'
  $dot_label_1.TextAlign = [System.Drawing.ContentAlignment]::MiddleCenter
  $dot_label_2.BackColor = [System.Drawing.Color]::White
  $dot_label_2.Location = new-object System.Drawing.Point (53,-5)
  $dot_label_2.Name = 'dotLabel2'
  $dot_label_2.Size = new-object System.Drawing.Size (1,25)
  $dot_label_2.Text = '.'
  $dot_label_2.TextAlign = [System.Drawing.ContentAlignment]::MiddleCenter
  $dot_label_3.BackColor = [System.Drawing.Color]::White
  $dot_label_3.Location = new-object System.Drawing.Point (81,-5)
  $dot_label_3.Name = 'dotLabel3'
  $dot_label_3.Size = new-object System.Drawing.Size (1,25)
  $dot_label_3.Text = '.'
  $dot_label_3.TextAlign = [System.Drawing.ContentAlignment]::MiddleCenter

  $p.BackColor = [System.Drawing.Color]::White
  $p.BorderStyle = [System.Windows.Forms.BorderStyle]::Fixed3D
  $p.Controls.AddRange(@($ip1, $ip2, $ip3, $ip4, $dot_label_1, $dot_label_2, $dot_label_3))
  $p.Location = new-object System.Drawing.Point (0,0)
  $p.Name = 'IP Panel'
  $p.Size = new-object System.Drawing.Size (112,25)
  $f.Controls.Add($p)
  $b1 = new-object System.Windows.Forms.Button
  $b1.Location = new-object System.Drawing.Size (50,40)
  $b1.Size = new-object System.Drawing.Size (75,23)
  $b1.Text = 'Done!'
  $b1.add_click({
    write-host (ipv4_address -ip1 $ip1.Text -ip2 $ip2.Text -ip3 $ip3.Text -ip4 $ip4.Text )
    $f.Close();
  })

  $f.Controls.Add($b1)
  $p.Name = 'IpBox'
  $p.Size = new-object System.Drawing.Size (112,26)
  $p.ResumeLayout($false)
  $f.ResumeLayout($false)

  $f.Add_Shown({ $f.Activate() })
  [void]$f.ShowDialog()
  # Write-Debug $result
}

dialogForContinueAuto


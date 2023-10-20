param(
  [string]$test_path = 'c:\temp\b'
)

[String]$success_message = 'Found' + [char]10 + 'The item little green bag'
[String]$error_message = 'Got to find just the kind' + [char] 10 + "or I'm losin' my mind" + [char] 10 + 'But Out of sight'
[bool]$status = $false

if  ( -not ( test-path -path $test_path  -pathtype container ))  {
  $text = $error_message
  $status = $false
}  else {
  $text = $success_message
  $status = $true
}

# NOTE: can abbreviate to just [Regex]
[int]$lines = [System.Text.RegularExpressions.Regex]::Matches($text,'\r?\n').Count + 1
$text += ' ' + $lines + ' lines'
add-type -assembly 'System.Windows.Forms'
add-type -assembly 'System.Drawing'

$f = new-object Windows.Forms.Form
$f.Autosize = $true
$f.AutosizeMode = [Windows.Forms.AutosizeMode]::GrowAndShrink
$f.FormBorderStyle = [Windows.Forms.FormBorderStyle]::FixedToolWindow
$f.ControlBox = $false
$f.SizeGripStyle = [Windows.Forms.SizeGripStyle]::Hide
$f.showintaskbar = $false
$f.Topmost = $true
$f.StartPosition = 'CenterScreen'
$l = new-object System.Windows.Forms.Label
$l.Font = new-object System.Drawing.Font ('Microsoft Sans Serif',10,[System.Drawing.FontStyle]::Bold,[System.Drawing.GraphicsUnit]::Point,0)
if ( $status -eq $false ) {
   $l.text = $text
   $l.ForeColor = [System.Drawing.Color]::Red
}  else {
   $l.text = $text
   $l.ForeColor = [System.Drawing.Color]::Green
}

$l.Location = new-object System.Drawing.Point(3,3)
$l.AutoSize = $true
$f.Controls.add($l)
 
$b = new-object System.Windows.Forms.Button
$b.Text = 'OK'
# NOTE: need the parentesis, or receive an error
# new-object : Cannot find an overload for "Point" and the argument count: "3".
$b.Location = new-object System.Drawing.Point(3, (10 + 20 * $lines ))
$b.add_click({
   $f.close()
})
$f.Controls.add($b)
$f.ShowDialog() | out-null

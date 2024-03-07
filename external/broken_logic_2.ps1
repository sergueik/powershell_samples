#Copyright (c) 2024 Serguei Kouzmine
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
# origin: https://qna.habr.com/q/1339318
Add-Type -assembly System.Windows.Forms

$f = new-object System.Windows.Forms.Form
$f.Text = 'form'
$f.Width = 250
$f.Height = 380
$f.AutoSize = $true

$g1 = new-object System.Windows.Forms.GroupBox
$g1.Text = 'x x x x x x'
$g1.AutoSize = $true
$g1.Width = 240
$g1.Height = 80
$g1.Location = new-object System.Drawing.Point(5,5)
$r1 = new-object System.Windows.Forms.RadioButton
$r1.Text = 'show'
# NOTE:
# $r1.Add_Click({}) will be wrong event here
# https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.radiobutton.checkedchanged?view=netframework-4.5
$r1.Add_CheckedChanged({
param(
  [Object]$sender,
  [System.EventArgs]$e
)
  if ($r1.Checked -eq $true) {
    $c.Visible = $true
} else { 
    $c.Visible = $false
}

})
$r1.Location = new-object System.Drawing.Point(10,20)
$r1.AutoSize = $true
$r2 = new-object System.Windows.Forms.RadioButton
$r2.Text = 'hide'
$r2.Location = new-object System.Drawing.Point(10,50)
$r2.AutoSize = $true
$g1.Controls.Add($r1)
$g1.Controls.Add($r2)


$groupBox2 = new-object System.Windows.Forms.GroupBox
$groupBox2.Text = 'y y y y y y'
$groupBox2.AutoSize = $true
$groupBox2.Width = 240
$groupBox2.Height = 80
$groupBox2.Location = new-object System.Drawing.Point(5,100)
$radioButton3 = new-object System.Windows.Forms.RadioButton
$radioButton3.Text = 'zzzz'
$radioButton3.Location = new-object System.Drawing.Point(10,20)
$radioButton3.AutoSize = $true
$radioButton4 = new-object System.Windows.Forms.RadioButton
$radioButton4.Text = 'yyyy'
$radioButton4.Location = new-object System.Drawing.Point(10,50)
$radioButton4.AutoSize = $true
$groupBox2.Controls.Add($radioButton3)
$groupBox2.Controls.Add($radioButton4)
$groupBox2.Enabled = $false

$richTextBox = new-object System.Windows.Forms.RichTextBox
$richTextBox.Text = '.................'
$richTextBox.Width = 240
$richTextBox.Height = 65
$richTextBox.BorderStyle = 'None'
$richTextBox.Location  = new-object System.Drawing.Point(5,190)
$richTextBox.Enabled = $false

$c = new-object System.Windows.Forms.ComboBox
$c.Text = 'aa aa aa'
$c.Width = 240
$c.Location = new-object System.Drawing.Point(5,260)
$c.Visible = $false
if ($r1.Checked -eq $true) {
    $c.Visible = $true
}

$b1 = new-object System.Windows.Forms.Button
$b1.Text = 'OK'
$b1.Location = new-object System.Drawing.Point(50,290)
$b2 = new-object System.Windows.Forms.Button
$b2.Text = 'Cancel'
$b2.Location = new-object System.Drawing.Point(130,290)

$f.Controls.Add($g1)
$f.Controls.Add($groupBox2)
$f.Controls.Add($richTextBox)
$f.Controls.Add($c)
$f.Controls.Add($b1)
$f.Controls.Add($b2)
$f.ShowDialog()




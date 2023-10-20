#Copyright (c) 2022 Serguei Kouzmine
#
#Permission is hereby granted, free of charge, to any person obtaining a copy
#of this software and associated documentation files (the 'Software'), to deal
#in the Software without restriction, including without limitation the rights
#to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
#copies of the Software, and to permit persons to whom the Software is
#furnished to do so, subject to the following conditions:
#
#The above copyright notice and this permission notice shall be included in
#all copies or substantial portions of the Software.
#
#THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
#IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
#FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
#AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
#LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
#OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
#THE SOFTWARE.

# based on
# http://www.codeproject.com/Articles/16186/DialogBoxes
# see the discussion in
# https://www.cyberforum.ru/powershell/thread3020792.html#post16450769
@( 'System.Drawing','System.Windows.Forms') | ForEach-Object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }

function TextInputBox {
  param(
    $prompt_message = 'Enter the Value',
    $caption = 'Inputbox Test'
  )
  $script:result = @{ 'text' = ''; 'status' = $null; }
  $form = New-Object System.Windows.Forms.Form
  $label_prompt = New-Object System.Windows.Forms.Label
  $button_ok = New-Object System.Windows.Forms.Button
  $text_input = New-Object System.Windows.Forms.TextBox
  $form.SuspendLayout()
  $label_prompt.Anchor = [System.Windows.Forms.AnchorStyles]::Top -bor [System.Windows.Forms.AnchorStyles]::Bottom -bor [System.Windows.Forms.AnchorStyles]::Left -bor [System.Windows.Forms.AnchorStyles]::Right
  $label_prompt.BackColor = [System.Drawing.SystemColors]::Control
  $label_prompt.Font = New-Object System.Drawing.Font ('Arial',10,[System.Drawing.FontStyle]::Regular,[System.Drawing.GraphicsUnit]::Point,0)
  $label_prompt.Location = New-Object System.Drawing.Point (12,9)
  $label_prompt.Name = 'lblPrompt'
  $label_prompt.Size = New-Object System.Drawing.Size (302,82)
  $label_prompt.TabIndex = 3
  $label_prompt.Font = New-Object System.Drawing.Font ('Arial',10,[System.Drawing.FontStyle]::Bold,[System.Drawing.GraphicsUnit]::Point,0)
  $button_ok.DialogResult = [System.Windows.Forms.DialogResult]::OK
  $button_ok.FlatStyle = [System.Windows.Forms.FlatStyle]::Standard
  $button_ok.Location = New-Object System.Drawing.Point (326,8)
  $button_ok.Name = 'button_ok'
  $button_ok.Size = New-Object System.Drawing.Size (64,24)
  $button_ok.TabIndex = 1
  $button_ok.Text = '&OK'
  $script:result.status = [System.Windows.Forms.DialogResult]::None
  $button_ok.Add_Click({
    param([object]$sender,[System.EventArgs]$e)
    $script:result.status = [System.Windows.Forms.DialogResult]::OK
    $script:result.Text = $text_input.Text
    $form.Dispose()
  })
  $text_input.Location = New-Object System.Drawing.Point (8,100)
  $text_input.Name = 'text_input'
  $text_input.Size = New-Object System.Drawing.Size (379,20)
  $text_input.TabIndex = 0
  $text_input.Text = ''
  $text_input.Font = New-Object System.Drawing.Font ('Arial',10,[System.Drawing.FontStyle]::Regular,[System.Drawing.GraphicsUnit]::Point,0)
  $form.AutoScaleBaseSize = New-Object System.Drawing.Size (5,13)
  $form.ClientSize = New-Object System.Drawing.Size (398,128)
  $form.Controls.AddRange(@($text_input,$button_cancel,$button_ok,$label_prompt))
  $form.FormBorderStyle = [System.Windows.Forms.FormBorderStyle]::FixedDialog
  $form.MaximizeBox = $false
  $form.MinimizeBox = $false
  $form.Name = 'InputBoxDialog'
  $form.ResumeLayout($false)
  $form.AcceptButton = $button_ok
  $form.ShowInTaskbar = $false
  $response = [System.Windows.Forms.DialogResult]::Ignore
  $result = ''
  $text_input.Text = ''
  $label_prompt.Text = $prompt_message
  $form.Text = $caption
  $form.StartPosition = [System.Windows.Forms.FormStartPosition]::CenterScreen
  [void]$form.ShowDialog()
  $form.Dispose()
  $form = $null
  return $script:result
}


$prompt_message = 'Enter the Value'
$caption = 'Inputbox test'
$o = TextInputBox -caption $caption -prompt_message $prompt_message
if ($o.status -match 'OK') {
  write-output $o.Text
}

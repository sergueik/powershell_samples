#Copyright (c) 2023 Serguei Kouzmine
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


# origin: https://qna.habr.com/q/1282726?e=13786724#clarification_1741778

@( 'System.Drawing','System.Windows.Forms') | ForEach-Object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }
Add-Type -TypeDefinition @"
// "
using System;
using System.Windows.Forms;
public class Win32Window : IWin32Window
{
    private IntPtr _hWnd;
    private string _data;

    public string Data
    {
        get { return _data; }
        set { _data = value; }
    }

    public Win32Window(IntPtr handle)
    {
        _hWnd = handle;
    }

    public IntPtr Handle
    {
        get { return _hWnd; }
    }
}

"@ -ReferencedAssemblies 'System.Windows.Forms.dll'

function ComboInputBox {
  $form = New-Object System.Windows.Forms.Form
  $label_prompt = New-Object System.Windows.Forms.Label
  $projectBrowseButton = New-Object System.Windows.Forms.Button
  $projectTextBox = New-Object System.Windows.Forms.TextBox


  $closeButton = New-Object System.Windows.Forms.Button
  $eCombobox = New-Object System.Windows.Forms.ComboBox
  $dcCombobox = New-Object System.Windows.Forms.ComboBox
  $rCombobox = New-Object System.Windows.Forms.ComboBox


  $form.SuspendLayout()
  $closeButton.DialogResult = [System.Windows.Forms.DialogResult]::Cancel
  $closeButton.FlatStyle = [System.Windows.Forms.FlatStyle]::Standard
  $closeButton.Location = New-Object System.Drawing.Point (90,130)
  $closeButton.Name = 'btnCancel'
  $closeButton.Size = New-Object System.Drawing.Size (64,24)
  $closeButton.TabIndex = 2
  $closeButton.Text = 'C&lose'
  $closeButton.add_click({
      param([object]$sender,[System.EventArgs]$e)
      $script:result.status = [System.Windows.Forms.DialogResult]::Cancel
      $script:result.Text = ''
      $form.Dispose()
    })
  $closeButton.Font = New-Object System.Drawing.Font ('Arial',10,[System.Drawing.FontStyle]::Bold,[System.Drawing.GraphicsUnit]::Point,0)

  $eCombobox.Location = New-Object System.Drawing.Point (8,60)
  $eCombobox.Name = 'CmBxComboBox'
  $eCombobox.Size = New-Object System.Drawing.Size (60,20)
  $eCombobox.TabIndex = 0
  $eCombobox.Text = ''
  $eCombobox.Font = New-Object System.Drawing.Font ('Arial',10,[System.Drawing.FontStyle]::Regular,[System.Drawing.GraphicsUnit]::Point,0)
  $eCombobox.Add_TextChanged({
      param(
        [object]$sender,[System.EventArgs]$e)
      # no op
    })

  $eCombobox.Add_KeyPress({
      param(
        [object]$sender,[System.Windows.Forms.KeyPressEventArgs]$e)
      # no op
    })
  $eCombobox.Add_TextChanged({
      param(
        [object]$sender,[System.EventArgs]$e)
      # no op
    })

  $dcCombobox.Location = New-Object System.Drawing.Point (75,60)
  $dcCombobox.Name = 'CmBxComboBox'
  $dcCombobox.Size = New-Object System.Drawing.Size (50,20)
  $dcCombobox.TabIndex = 0
  $dcCombobox.Text = ''
  $dcCombobox.Font = New-Object System.Drawing.Font ('Arial',10,[System.Drawing.FontStyle]::Regular,[System.Drawing.GraphicsUnit]::Point,0)
  $dcCombobox.Add_TextChanged({
      param(
        [object]$sender,[System.EventArgs]$e
      )
    })

  $dcCombobox.Add_KeyPress({
      param(
        [object]$sender,[System.Windows.Forms.KeyPressEventArgs]$e
      )
    })
  $dcCombobox.Add_TextChanged({
      param(
        [object]$sender,[System.EventArgs]$e
      )
    })

  $rCombobox.Location = New-Object System.Drawing.Point (135,60)
  $rCombobox.Name = 'CmBxComboBox'
  $rCombobox.Size = New-Object System.Drawing.Size (220,20)
  $rCombobox.TabIndex = 0
  $rCombobox.Text = ''
  $rCombobox.Font = New-Object System.Drawing.Font ('Arial',10,[System.Drawing.FontStyle]::Regular,[System.Drawing.GraphicsUnit]::Point,0)
  $rCombobox.Add_TextChanged({
      param([object]$sender,[System.EventArgs]$e)
    })

  $rCombobox.Add_KeyPress({
      param(
        [object]$sender,[System.Windows.Forms.KeyPressEventArgs]$e)

    })
  $rCombobox.Add_TextChanged({
      param(
        [object]$sender,[System.EventArgs]$e)
    })

  $form.AutoScaleBaseSize = New-Object System.Drawing.Size (5,13)
  $form.ClientSize = New-Object System.Drawing.Size (438,158)
  $form.Controls.AddRange(@( $eCombobox,$dcCombobox,$rCombobox,$closeButton))
  $form.FormBorderStyle = [System.Windows.Forms.FormBorderStyle]::FixedDialog
  $form.MaximizeBox = $false
  $form.MinimizeBox = $false
  $form.Name = 'ComboBoxDialog'
  $form.ResumeLayout($false)
  $form.AcceptButton = $launchButton
  $script:result.status = [System.Windows.Forms.DialogResult]::Ignore
  $form.Name = 'Form1'
  $form.ResumeLayout($false)

  $form.Topmost = $true

  $form.Add_Shown({ $form.Activate() })
  $form.Add_Closing({
      param(
        [object]$sender,[System.ComponentModel.CancelEventArgs]$e
      )
      # prevent the Closing event from closing the form
      # The 'Cancel' button will still do
      $e.Cancel = $true
    })
  [void]$form.ShowDialog($caller)

  $form.Dispose()
  $form = $null
  return $script:result

}
$o = ComboInputBox 
<#
private void comboBoxMySqlFilter(object sender, EventArgs e)
        {
            
            ComboBox comboBox = (ComboBox)sender;
            string selectedValue = comboBox.SelectedItem?.ToString();

            List<(ComboBox comboBox, string propertyName)> filterMappingsMySql = new List<(ComboBox, string)>
            {
                (cbFindServerMySql, "server"),
                (cbFindModelMySql, "model"),
                (cbFindOperatorMySql, "mobileoperator"),
            };

            foreach (var mapping in filterMappingsMySql)
            {
                if (sender is ComboBox cb && cb == mapping.comboBox)
                {
                    List<deviceMySql> filteredDevices = deviceMySql.dgvDevices
                        .Where(d => d.GetType().GetProperty(mapping.propertyName)?
                        .GetValue(d)?.ToString() == cb.Text)
                        .ToList();
                    dgvMySql.DataSource = filteredDevices;
                }
                else
                {
                    mapping.comboBox.Text = null;
                }
            }
        }
#>

function shared_handler{
      param(
        [object]$sender,[System.EventArgs]$e
      )
      [System.Windows.Forms.ComboBox] $comboBox = (System.Windows.Forms.ComboBox) $sender
      [String] $selectedValue = $comboBox.SelectedItem.ToString()
      # prevent the Closing event from closing the form
      # The 'Cancel' button will still do
      $e.Cancel = $true
    }
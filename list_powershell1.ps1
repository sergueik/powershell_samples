#Copyright (c) 2014,2021 Serguei Kouzmine
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
Add-Type -TypeDefinition @"

// "
using System;
using System.Windows.Forms;

public class Win32Window : IWin32Window
{
    private IntPtr _hWnd;

    private  String _data;

    public String Data
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
"@ -ReferencedAssemblies "System.Windows.Forms.dll"


[void][System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms")
[void][System.Reflection.Assembly]::LoadWithPartialName("System.Drawing")
$title = 'Select master host'
$objForm = New-Object System.Windows.Forms.Form
$objForm.Text = $title
$objForm.Size = New-Object System.Drawing.Size (300,200)
$objForm.StartPosition = 'CenterScreen'

$objForm.KeyPreview = $True
$objForm.Add_KeyDown({ if ($_.KeyCode -eq 'Enter')
    { $x = $objListBox.SelectedItem; $objForm.Close() } })
$objForm.Add_KeyDown({ if ($_.KeyCode -eq 'Escape')
    { $objForm.Close() } })
$CancelButton = New-Object System.Windows.Forms.Button
$CancelButton.Location = New-Object System.Drawing.Size (150,120)
$CancelButton.Size = New-Object System.Drawing.Size (75,23)
$CancelButton.Text = 'Cancel'
$CancelButton.add_click({ $owner.Data = 0; $objForm.Close() })
$objForm.Controls.Add($CancelButton)

$OKButton = New-Object System.Windows.Forms.Button
$OKButton.Location = New-Object System.Drawing.Size (75,120)
$OKButton.Size = New-Object System.Drawing.Size (75,23)
$OKButton.Text = 'OK'
$OKButton.add_click({ $owner.Data = $objListBox.SelectedItem; $objForm.Close(); return [System.Windows.Forms.DialogResult]::OK })
$objForm.Controls.Add($OKButton)


$objLabel = New-Object System.Windows.Forms.Label
$objLabel.Location = New-Object System.Drawing.Size (10,20)
$objLabel.Size = New-Object System.Drawing.Size (280,20)
$objLabel.Text = 'Please select a computer: '
$objForm.Controls.Add($objLabel)

$objListBox = New-Object System.Windows.Forms.ListBox
$objListBox.Location = New-Object System.Drawing.Size (10,40)
$objListBox.Size = New-Object System.Drawing.Size (260,20)
$objListBox.Height = 80

[void]$objListBox.Items.Add("node1")
[void]$objListBox.Items.Add("node2")
[void]$objListBox.Items.Add("node3")
# https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.listbox.setselected
$objListBox.setSelected(1, $true)
$objForm.Controls.Add($objListBox)

$objForm.Topmost = $True

$owner = New-Object Win32Window -ArgumentList ([System.Diagnostics.Process]::GetCurrentProcess().MainWindowHandle)
$owner.Data = 42;

$objForm.Add_Shown({ $objForm.Activate() })
$x = $objForm.ShowDialog($owner)

$selected_host = $owner | Select-Object -ExpandProperty Data


Write-Host "Result is : ${selected_host}"




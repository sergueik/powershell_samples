#Copyright (c) 2021 Serguei Kouzmine
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

# see https://www.cyberforum.ru/powershell/thread2877339.html

param (
  [switch]$debug
)
$num_entries = 20
$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent

function is_blank{
  param (
    [string]$value
  )
  return $value -eq $null -or $value -eq ''
}
# sample data - registry query
$l = new-object System.collections.ArrayList
$l.addRange((get-itemproperty -path 'HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*' | 
where-object { 
( -not (is_blank($_.DisplayName))) -and ( -not (is_blank($_.Publisher ))) -and ( -not (is_blank($_.'DisplayVersion'))) } |
select-object -first $num_entries |
select-object DisplayName, DisplayVersion, Publisher, @{Name="InstallDate"; Expression={[DateTime]::ParseExact($_.InstallDate, 'yyyyMMdd', $null)}}) )

# sort items in a DataGrid https://msdn.microsoft.com/en-us/library/dd833072%28v=vs.95%29.aspx

@('System.Drawing','System.Windows.Forms') | ForEach-Object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }


$f = new-object System.Windows.Forms.Form

$y = new-object System.Windows.Forms.DataGridView
$y.AllowUserToAddRows = $false
$y.AllowUserToDeleteRows = $false
$y.AllowUserToResizeColumns = $true
# https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.datagridviewautosizecolumnsmode?view=netframework-4.0
$y.AutoSizeColumnsMode = [System.Windows.Forms.DataGridViewAutoSizeColumnsMode]::AllCells
$y.AllowUserToResizeRows = $false
$y.Anchor = [System.Windows.Forms.AnchorStyles]::Top -bor [System.Windows.Forms.AnchorStyles]::Bottom -bor [System.Windows.Forms.AnchorStyles]::Left-bor [System.Windows.Forms.AnchorStyles]::Right
$y.BorderStyle = [System.Windows.Forms.BorderStyle]::None
$y.CellBorderStyle = [System.Windows.Forms.DataGridViewCellBorderStyle]::None
$y.SelectionMode = [System.Windows.Forms.DataGridViewSelectionMode]::FullRowSelect
$y.RowTemplate.DefaultCellStyle.SelectionBackColor = [System.Drawing.Color]::Gray
$y.Location = new-object System.Drawing.Point (0,0)
$y.MultiSelect = $false
$y.Name = 'DataGridView'
$y.ReadOnly = $true
$y.RowHeadersVisible = $false
$y.Size = new-object System.Drawing.Size(500,320)

$d = new-object System.Data.DataSet('Installed Applications')
[void]$d.Tables.Add('Registry')
[System.Data.DataColumnCollection]$c = $d.Tables['Registry'].Columns
$c.AddRange(@('Dislay Version','Install Date','Display Name', 'Publisher'))
$c['Install Date'].DataType = [System.DateTime]
# NOTE: datatype affects sorting

0..($l.Count - 1) | foreach-object {
  $cnt = $_
  $lrow = $l.Item($cnt)
  if ($debug_flag){
    write-host ('Adding row # {0}'  -f $cnt)
  }
  [DateTime] $ldate = new-object DateTime
  [void]([DateTime]::TryParse($lrow.InstallDate, [ref]$ldate) )
  # $ldate = (get-date $lrow.InstallDate)
  # need DateTime in this column
  [void]$d.Tables['Registry'].Rows.Add(
    @(
      $lrow.'DisplayVersion',
      [DateTime]($ldate),
      $lrow.DisplayName,
      $lrow.Publisher
    )
  )
}
[System.Windows.Forms.BindingSource]$b = new-object System.Windows.Forms.BindingSource ($d,'Registry')
$y.DataSource = $b

#  NOTE: The property 'SortMode' cannot be found on this object.
# $y.Columns['Display Name'].SortMode = [System.Windows.Forms.DataGridViewColumnSortMode]::Automatic

# $f.AutoScaleDimensions = new-object System.Drawing.SizeF (6.0,13.0)
$f.AutoScaleMode = [System.Windows.Forms.AutoScaleMode]::Font
$f.Controls.Add($y)
$f.Name = 'Installed Applications'
$f.Size = new-object System.Drawing.Size (828,346)
$f.ResumeLayout($false)
$f.PerformLayout()
$f.Topmost = $true
$f.Add_Shown({ $f.Activate() })

[void]$f.ShowDialog()

$f.Dispose()

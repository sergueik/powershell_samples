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

@( 'System.Drawing','System.Windows.Forms') | ForEach-Object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }

$f = New-Object System.Windows.Forms.Form

# sort items in a DataGrid https://msdn.microsoft.com/en-us/library/dd833072%28v=vs.95%29.aspx



$f.SuspendLayout()
$l = new-object System.collections.ArrayList
$l.addRange((get-itemproperty -path 'HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*' | where-object { $_.DisplayName  -ne $null} | Select-Object DisplayName, DisplayVersion, Publisher, @{Name="InstallDate"; Expression={[DateTime]::ParseExact($_.InstallDate, 'yyyyMMdd', $null)}}) )

# NOTE: default minimal constructor no interactive sorting
# see https://www.cyberforum.ru/powershell/thread2877339.html
$y = New-Object System.Windows.Forms.DataGridView -Property @{
  Size=New-Object System.Drawing.Size(728,320)
  ColumnHeadersVisible = $true
  DataSource = $l
}

$f.Controls.Add($y)
$f.Name = 'Installed Applications'
$f.Size = New-Object System.Drawing.Size (728,346)
$f.ResumeLayout($false)
$f.PerformLayout()
$f.Topmost = $true
$f.Add_Shown({ $f.Activate() })

[void]$f.ShowDialog()

$f.Dispose()

$f = $null

$f = New-Object System.Windows.Forms.Form
$f.Name = 'Installed Applications'

$y = New-Object System.Windows.Forms.DataGridView
$y.AllowUserToAddRows = $false
$y.AllowUserToDeleteRows = $false
$y.AllowUserToResizeColumns = $true
$y.AllowUserToResizeRows = $false
$y.Anchor = [System.Windows.Forms.AnchorStyles]::Top -bor [System.Windows.Forms.AnchorStyles]::Bottom -bor [System.Windows.Forms.AnchorStyles]::Left-bor [System.Windows.Forms.AnchorStyles]::Right
$y.AutoSizeColumnsMode = [System.Windows.Forms.DataGridViewAutoSizeColumnsMode]::Fill
$y.BorderStyle = [System.Windows.Forms.BorderStyle]::None
$y.CellBorderStyle = [System.Windows.Forms.DataGridViewCellBorderStyle]::None
$y.ColumnHeadersHeightSizeMode = [System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode]::AutoSize
$y.SelectionMode = [System.Windows.Forms.DataGridViewSelectionMode]::FullRowSelect
$y.RowTemplate.DefaultCellStyle.SelectionBackColor = [System.Drawing.Color]::Transparent
$y.AutoResizeRows([System.Windows.Forms.DataGridViewAutoSizeRowsMode]::AllCellsExceptHeaders)
$y.Location = new-object System.Drawing.Point (0,26)
$y.MultiSelect = $false
$y.Name = 'DataGridView'
$y.ReadOnly = $true
$y.RowHeadersVisible = $false
$y.Size = New-Object System.Drawing.Size (728,320)

$data_source = new-object System.Data.DataSet('Installed Applications')
[void]$data_source.Tables.Add('Registry')
$t = $data_source.Tables['Registry']
[void]$t.Columns.Add('Dislay Version')
[void]$t.Columns.Add('Install Date')
# NOTE: Exception calling "Add" with "1" argument(s): 
# Unable to cast object of type 'System.Management.Automation.PSObject' to type 'System.IConvertible'.
# Couldn't store <7/15/2018 12:00:00 AM> in Install Date Column.  Expected type is DateTime."
# $t.Columns['Install Date'].DataType = [System.DateTime]
[void]$t.Columns.Add('Display Name')
[void]$t.Columns.Add('Publisher')

$rows = @();
$l | foreach-object { 
  $row  = @{ }
  if ($_.InstallDate -ne $null) {
    $row = @{
      'Display Version' = $_.DisplayVersion;
      'Publisher' = $_.Publisher;
      'ReplacementStrings' = $_.DisplayVersion;
      'Install Date' = (get-date $_.InstallDate);
    }
  } else  {
    $row =      @{
      'Display Version' = $_.DisplayVersion;
      'Publisher' = $_.Publisher;
      'Name' = $_.DisplayName;
    }
  }   
  $rows +=$row
}

$event_set_mock['Entries'] = $rows;
$event_set = $event_set_mock

$MAX_EVENTLOG_ENTRIES = 100
$max_cnt = $event_set.Entries.Count - 1
if ($max_cnt -gt $MAX_EVENTLOG_ENTRIES) {
  $max_cnt = $MAX_EVENTLOG_ENTRIES
}

0..$max_cnt | foreach-object {
  $cnt = $_
  $event_item = $event_set.Entries[$cnt]
  [void]$data_source.Tables['Registry'].Rows.Add(
    @(
      $event_item.'Display Version',
      $event_item.'Install Date',
      $event_item.Name,
      $event_item.Publisher
    )
  )
  # Write-output ($event_item | format-list)

}
[System.Windows.Forms.BindingSource]$b = new-object System.Windows.Forms.BindingSource ($data_source,'Registry')
$y.DataSource = $b

#  NOTE: The property 'SortMode' cannot be found on this object.

<#
$y.Columns['Name'].SortMode = [System.Windows.Forms.DataGridViewColumnSortMode]::Automatic 
$y.Columns['Display Version'].SortMode = [System.Windows.Forms.DataGridViewColumnSortMode]::Automatic 
$y.Columns['Publisher'].SortMode = [System.Windows.Forms.DataGridViewColumnSortMode]::Automatic 
$y.Columns['Install Date'].SortMode = [System.Windows.Forms.DataGridViewColumnSortMode]::Automatic 
#>
#
#EventLogViewer
#
$f.AutoScaleDimensions = New-Object System.Drawing.SizeF (6.0,13.0)
$f.AutoScaleMode = [System.Windows.Forms.AutoScaleMode]::Font
$f.Controls.Add($y)
$f.Name = 'Installed Applications'
$f.Size = New-Object System.Drawing.Size (728,346)
$f.ResumeLayout($false)
$f.PerformLayout()
$f.Topmost = $true
$f.Add_Shown({ $f.Activate() })

[void]$f.ShowDialog()


$f.Dispose()



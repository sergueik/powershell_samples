# sample data - registry
$l = new-object System.collections.ArrayList
$l.addRange((get-itemproperty -path 'HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*' | where-object { $_.DisplayName  -ne $null} | Select-Object DisplayName, DisplayVersion, Publisher, @{Name="InstallDate"; Expression={[DateTime]::ParseExact($_.InstallDate, 'yyyyMMdd', $null)}}) )

@('System.Drawing','System.Windows.Forms') | ForEach-Object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }


$f = new-object System.Windows.Forms.Form
$f.Name = 'Installed Applications'

$y = new-object System.Windows.Forms.DataGridView
$y.AllowUserToAddRows = $false
$y.AllowUserToDeleteRows = $false
$y.AllowUserToResizeColumns = $true
$y.AllowUserToResizeRows = $false
$y.Anchor = [System.Windows.Forms.AnchorStyles]::Top -bor [System.Windows.Forms.AnchorStyles]::Bottom -bor [System.Windows.Forms.AnchorStyles]::Left-bor [System.Windows.Forms.AnchorStyles]::Right
$y.AutoSizeColumnsMode = [System.Windows.Forms.DataGridViewAutoSizeColumnsMode]::Fill
$y.BorderStyle = [System.Windows.Forms.BorderStyle]::Fixed3D
$y.CellBorderStyle = [System.Windows.Forms.DataGridViewCellBorderStyle]::None
$y.ColumnHeadersHeightSizeMode = [System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode]::AutoSize
$y.SelectionMode = [System.Windows.Forms.DataGridViewSelectionMode]::FullRowSelect
$y.RowTemplate.DefaultCellStyle.SelectionBackColor = [System.Drawing.Color]::Gray
$y.AutoResizeRows([System.Windows.Forms.DataGridViewAutoSizeRowsMode]::AllCellsExceptHeaders)
$y.Location = new-object System.Drawing.Point (0,0)
$y.MultiSelect = $false
$y.Name = 'DataGridView'
$y.ReadOnly = $true
$y.RowHeadersVisible = $false
$y.Size = new-object System.Drawing.Size (400,320)
$y.ColumnCount = 4
$y.columns.Item(0).HeaderText = 'Display Name'
$y.columns.Item(1).HeaderText = 'Version'
$y.columns.Item(2).HeaderText = 'Publisher'
$y.columns.Item(3).HeaderText = 'Install Date'
0..($l.Count - 1) | foreach-object {
  $cnt = $_
  $lrow = $l.Item($cnt)
  # not needed
  if ($lrow.InstallDate -ne $null) {
    $ldate = (get-date $lrow.InstallDate)
  } else {
    $ldate = ''
  }
  $y.Rows.Add($lrow.DisplayName, $lrow.DisplayVersion, $lrow.Publisher, $lrow.InstallDate) | out-null
}
$f.AutoScaleDimensions = new-object System.Drawing.SizeF (6.0,13.0)
$f.AutoScaleMode = [System.Windows.Forms.AutoScaleMode]::Font
$f.Controls.Add($y)
$f.Name = 'Installed Applications'
$f.Size = new-object System.Drawing.Size (1028,346)
$f.ResumeLayout($false)
$f.PerformLayout()
$f.Topmost = $true
$f.Add_Shown({ $f.Activate() })

[void]$f.ShowDialog()

$f.Dispose()

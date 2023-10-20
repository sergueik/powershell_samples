param (
  [switch]$debug
)

$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent


function is_blank{
  param (
    [string]$value
  )
  return $value -eq $null -or $value -eq ''
}

Add-Type -Assembly 'System.Windows.Forms'

$form = [Windows.Forms.Form]@{
  Text = 'Installed Applications'
  AutoSize = $true
}
$grid = [Windows.Forms.DataGridView]@{
  Size = [Drawing.Size]::new(0, 777)
  AutoSizeColumnsMode = 6    
  Parent = $form
}
$autoSizer = {
  $w = 20+$grid.columns.GetColumnsWidth(0)
  $grid.Width = $w + $grid.RowHeadersWidth
  $form.Activate()
}

'DisplayName', 'DisplayVersion', 'Publisher', 'InstallDate'|
foreach-object{
  $grid.Columns.Add($_,$_)
}|out-null

get-itemproperty -path 'HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*' |
where-object { 
  -not (is_blank($_.DisplayName)) 
} | 
select-object -first 20 |
foreach-object {
  $grid.Rows.Add(
    @(
      $_.DisplayName,
      $_.DisplayVersion,
      $_.Publisher,
      $(try{[DateTime]::ParseExact($_.InstallDate, 'yyyyMMdd', $null) } catch{$null})
    )
  ) | out-null
}
$form.add_shown($autoSizer)
$form.showdialog()|out-null

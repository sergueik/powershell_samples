add-type -assembly System.Windows.Forms
$form = New-Object System.Windows.Forms.Form
$treeView = New-Object System.Windows.Forms.TreeView
$treeView.Nodes.Add("Первый") | out-null
$treeView.Nodes.Add("Второй") | out-null
$form.Controls.add($treeView)
# https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.treeview.nodes?view=netframework-4.0
$text = 'Второй'
$treeNodes = $TreeView.Nodes.Find($text, $true) 
if ($treeNodes -ne $null) {
  $treeView.SelectedNode = $treeNodes[0]
  write-output 'found via API call'
} else { 
  $treeView.Nodes | where-object { $_.Text -eq $text } | select-object -first 1 | tee-object -variable $treeNode| format-list
  $treeView.SelectedNode = $treeNode
  write-output 'found via iteration'
}

$treeView.Focus()
$form.ShowDialog()
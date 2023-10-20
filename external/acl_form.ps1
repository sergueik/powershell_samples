# origin: https://qna.habr.com/q/1075778      
add-type -AssemblyName System.Windows.Forms
add-type -AssemblyName System.Drawing

function GetAclFolder {

  $path = $i.Text
  write-host ('Input: {0}' -f $path)
  $data_column_label = 'have access'
  $data = (get-acl -path $path).access | 
  select-object @{ expression = {$_.IdentityReference}; label = 'have access'}, @{ expression = {$_.IsInherited}; label = 'inherited'}, @{ expression = {$_.FileSystemRights}; label = 'access'}
  [System.Windows.Forms.ListViewItem]$item
  $data | foreach-object {
    $row = $_
    $item = new-object System.Windows.Forms.ListViewItem($row.'have access'.ToString())
    $item.SubItems.Add($row.'inherited'.ToString())
    $item.SubItems.Add($row.'access'.ToString())
    $v.Items.AddRange($item)
  }
}


$f = new-object System.Windows.Forms.Form
$f.Text = 'folder access'
$f.Size = new-object System.Drawing.Size(600,400)


$l = new-object System.Windows.Forms.Label
$l.Text = 'enter path'
$l.Location = new-object System.Drawing.Size(20,30)
$l.BackColor = 'Transparent'
$l.AutoSize = $true

$f.Controls.Add($l)

$i = new-object System.Windows.Forms.TextBox
$i.Location = new-object System.Drawing.Size(20,50)
$i.Size = new-object System.Drawing.Size(250,20)

$f.Controls.Add($i)


$o = new-object System.Windows.Forms.TextBox
$o.Location = new-object System.Drawing.Size(10,150)
$o.Size = new-object System.Drawing.Size(565,200)
$o.MultiLine = $True

$v = new-object System.Windows.Forms.ListView
$v.View = 'Details'
$v.Location = new-object System.Drawing.Size(10,150)
$v.Width = 565
$v.Height = 200

$v.Columns.Add('have access') | out-null
[void]$v.Columns.Add('inherited')
[void]$v.Columns.Add('access')
$f.Controls.Add($v)

$f.Controls.Add($o)

$b = new-object System.Windows.Forms.Button
$b.Location = new-object System.Drawing.Size(400,30)
$b.Size = new-object System.Drawing.Size(110,24)
$b.Text = 'run'

$b.Add_Click( {GetAclFolder} )


$f.Controls.Add($b)


[void] $f.ShowDialog()


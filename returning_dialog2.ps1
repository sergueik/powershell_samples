# based on
# https://www.cyberforum.ru/powershell/thread3020792.html#post16451106
# NOTE: Powershell 5.0+ (5.0.10586.117,5.1.14409.1005)
# supports this syntax

using namespace System.Drawing
using namespace System.Windows.Forms

@( 'System.Drawing','System.Windows.Forms') | forEach-Object {
   Add-Type -AssemblyName $_
}


function show_result(
# NOTE: not 'param'
[parameter(ValueFromPipeline)][String]$value) {
  end {
    [MessageBox]::Show(('selected: {0}' -f $value), 'selection')
  }
}

$l = [Label]@{
  Location = [Point]::new(13, 12)
  Text = 'select:'
  Width = 90
}

$c = [ComboBox]@{
  Location = [Point]::new(103, 13)
}
$c.Items.AddRange(@('foo', 'bar'))
$c.Add_SelectedIndexChanged({
  $this.SelectedItem | show_result
})

$f = [Form]@{
  ClientSize = [Size]::new(350, 100)
  Icon = ($ico = [Icon]::ExtractAssociatedIcon("${PSHOME}\powershell.exe"))
  StartPosition = [FormStartPosition]::CenterScreen
  Text = 'Example'
}
$f.Controls.AddRange(@($l, $c))
$f.Add_FormClosing({
  if ($ico) { $ico.Dispose() }
})
[void]$f.ShowDialog()
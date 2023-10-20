# https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.button.click?view=netframework-4.5.1
param(
  [string]$script = 'script.ps1',
  [switch]$debug
)
@( 'System.Drawing','System.Windows.Forms') | foreach-object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }
# alternatively add-type -AssemblyName System.Windows.Forms
$form = new-object Windows.Forms.Form
$button = new-object Windows.Forms.Button
$button.Text = 'Button1'
$form.Controls.Add($button)


$button.add_Click({
  if ($debug) {
    write-host $script
    write-host ((resolve-path -path '.').path + '\' + $script )
  }
  # https://stackoverflow.com/questions/50760616/use-powershell-command-in-add-click
  # powershell.exe -file ((resolve-path -path '.').path + '\' + $script )
  . ((resolve-path -path '.').path + '\' + $script )
})

$form.ShowDialog()

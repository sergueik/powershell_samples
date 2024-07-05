# based on: http://forum.oszone.net/thread-355453.html

add-type -name helper -memberDefinition '[DllImport("user32.dll")] public static extern bool ShowWindow(int handle, int state);' -namespace pinvoke
$p = [System.Diagnostics.Process]::GetCurrentProcess() | get-process
[pinvoke.helper]::ShowWindow($p.MainWindowHandle, 0)

Add-Type -AssemblyName System.Windows.Forms

$f = new-object System.Windows.Forms.Form
$f.Text = 'Example'
$f.Size = new-object System.Drawing.Size(800, 500)

$f.add_Resize({
  if ($f.WindowState -eq 'Minimized') {
    $f.Hide()
    $n.Visible = $true
    $n.ShowBalloonTip(1000)
  }
})

$f.add_SizeChanged({
  if ($f.WindowState -eq 'Minimized') {
    $f.Hide()
    $n.Visible = $true
    $n.ShowBalloonTip(1000)
  }
})

$f.add_FormClosing({
  $f.Hide()
  $n.Visible = $true
  $_.Cancel = $true
  $n.Visible = $false
  $n.Dispose()
  $p.CloseMainWindow()
  $p.Dispose()
})

$n = new-object System.Windows.Forms.NotifyIcon
$n.Icon = [System.Drawing.Icon]::ExtractAssociatedIcon($p.Path)
$n.Visible = $false
$n.BalloonTipIcon = [System.Windows.Forms.ToolTipIcon]::Info
$n.BalloonTipText = 'In Tray'
$n.BalloonTipTitle = 'PowerShell GUI'
$n.ShowBalloonTip(1000)

$n.add_Click({
  $f.Show()
  $f.WindowState = 'Normal'
  $n.Visible = $false
})

$c = new-object System.Windows.Forms.ContextMenu
$m = new-object System.Windows.Forms.MenuItem
$m.Text = 'Exit'
$m.add_Click({
  $f.Close()
  $n.Dispose()
})
$c.MenuItems.Add($m)
$n.ContextMenu = $c

$f.Show()
[System.Windows.Forms.Application]::Run()

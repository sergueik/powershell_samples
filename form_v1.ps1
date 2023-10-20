add-type -assembly 'System.Windows.Forms'
$count = 0
$form = new-object Windows.Forms.Form
$form.autosize = $true
$form.autosizemode = [Windows.Forms.AutosizeMode]::GrowAndShrink
$form.formborderstyle = [Windows.Forms.FormBorderStyle]::FixedToolWindow
$form.controlbox = $false
$form.sizegripstyle = [Windows.Forms.SizeGripStyle]::hide
$form.showintaskbar = $false
$form.topmost = $true
$form.startposition = 'CenterScreen'
 
$label = new-object windows.forms.label
$label.text = $count
$label.location = new-object System.Drawing.Point(3,3)
$label.autosize = $true
$form.controls.add($label)
 
$btn = new-object windows.forms.button
$btn.text = 'Counter +'
$btn.location = new-object System.Drawing.Point(3,30)
$btn.add_click({
    $script:count++
    $label.text = $count
    if ($count -gt 5){$form.close()}
})
$form.controls.add($btn)
$form.ShowDialog()
@( 'System.Drawing','System.Windows.Forms') | ForEach-Object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }
$f = new-object -TypeName 'System.Windows.Forms.Form'
$f.SuspendLayout()

$f.AutoScaleDimensions = new-object System.Drawing.SizeF (6.0,13.0)
$f.AutoScaleMode = [System.Windows.Forms.AutoScaleMode]::Font
$f.ClientSize = new-object System.Drawing.Size (100,45)

$b = new-object System.Windows.Forms.Button
$b.Location = new-object System.Drawing.Point(40, 10)
$b.text = 'ok'
$b.Add_Click({ $f.close() })
$f.Controls.Add($b)
$f.Name = 'Form1'
$f.Text = 'Dialog'
$f.ResumeLayout($false)
$f.Topmost = $Trues
$f.Add_Shown({ $f.Activate() })
[void]$f.ShowDialog()
$f.Dispose()

Add-Type -AssemblyName System.Windows.Forms
[System.Windows.Forms.Application]::EnableVisualStyles()

$Form = New-Object system.Windows.Forms.Form
$Form.ClientSize = New-Object System.Drawing.Point(541,338)
$Form.text = "Скрипт для отображения последней смены пароля пользователя"
$Form.TopMost = $false

$Button1 = New-Object system.Windows.Forms.Button
$Button1.text = "Вывести"
$Button1.width = 150
$Button1.height = 30
$Button1.location = New-Object System.Drawing.Point(161,56)
$Button1.Font = New-Object System.Drawing.Font('Microsoft Sans Serif',10)

$TextBox1 = New-Object system.Windows.Forms.TextBox
$TextBox1.multiline = $true
$TextBox1.width = 408
$TextBox1.height = 126
$TextBox1.location = New-Object System.Drawing.Point(67,188)
$TextBox1.Font = New-Object System.Drawing.Font('Microsoft Sans Serif',10)

$Label1 = New-Object system.Windows.Forms.Label
$Label1.text = "Нажмите для отображения последней смены пароля пользователя :"
$Label1.AutoSize = $true
$Label1.width = 25
$Label1.height = 10
$Label1.location = New-Object System.Drawing.Point(31,28)
$Label1.Font = New-Object System.Drawing.Font('Microsoft Sans Serif',10)

$Label2 = New-Object system.Windows.Forms.Label
$Label2.text = "Нажмите для смены пароля пользователя:"
$Label2.AutoSize = $true
$Label2.width = 25
$Label2.height = 10
$Label2.location = New-Object System.Drawing.Point(31,107)
$Label2.Font = New-Object System.Drawing.Font('Microsoft Sans Serif',10)

$Button2 = New-Object system.Windows.Forms.Button
$Button2.text = "Сменить"
$Button2.width = 148
$Button2.height = 30
$Button2.location = New-Object System.Drawing.Point(161,138)
$Button2.Font = New-Object System.Drawing.Font('Microsoft Sans Serif',10)

$Form.controls.AddRange(@($Button1,$TextBox1,$Label1,$Label2 ,$Button2))

$Button1.add_click({

$TextBox1.text= Get-LocalUser -Name "Человек" | Select-Object PasswordLastSet
})

$Button2.add_click({

$UserPassword = ConvertTo-SecureString $TextBox1.Text -AsPlainText -Force

Set-LocalUser -Name "Человек" -Password $UserPassword
[System.Windows.Forms.MessageBox]::Show("Пароль успешно сменён", "Смена пароля", "OK")


})

$Form.ShowDialog()
Denusbuble вне форума 
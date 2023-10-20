# origin: https://www.cyberforum.ru/powershell/thread3009578.html
[CmdletBinding()]
param (
    [string]$IPAddress='192.168.1.50'
    )
 
### Form Colors ###
    $FormBackColor = "#222222"
    $FormForeColor = "#c5c9ca"
    $TextboxBackColor = "#2b2b2b"
    $TextboxForeColor = "#c3803c"
 
    
### Load Assembies ###
   Add-Type -AssemblyName System.Windows.Forms
   [System.Windows.Forms.Application]::EnableVisualStyles()
   
    #### Form ###
   $Form = New-Object system.Windows.Forms.Form
   $Form.Text = "TESSSSSST"
   $Form.FormBorderStyle = "FixedDialog"
   $Form.TopMost  = $True
   $Form.MinimizeBox  = $False
   $Form.MaximizeBox  = $False
   $Form.StartPosition = "CenterScreen"
   $Font = New-Object System.Drawing.Font("Segoe UI Semibold",10)
   $Form.Font = $Font
   $Form.Width = 480
   $Form.Height = 585
   $Form.AutoScroll = $True
   $Form.AutoSize = $True
   $Form.AutoSizeMode = "GrowOnly"
   $Form.TopMost = $True
   $Form.Opacity = 1 #0.95
   $Form.BackColor = $FormBackColor
   $Form.ForeColor = $FormForeColor
   $Form.ShowInTaskbar = $False
   $Form.BackgroundImageLayout = "Center"
   
 
######################################################  
######################################################   
          ### IP Address ###
   $label = New-Object Windows.Forms.Label
   $label.Location = New-Object Drawing.Point 50,30
   $label.Size = New-Object Drawing.Point 125,20
   $label.BackColor = "Transparent"
   $label.text = "IPv4 Address"
   $Form.Controls.Add($label)
   $FormIPAddress = New-Object System.Windows.Forms.TextBox
   $FormIPAddress.Location = New-Object System.Drawing.Point 50,50
   $FormIPAddress.Size = New-Object System.Drawing.Point 125,20
   $FormIPAddress.text = $IPAddress
   
 
 
   $FormIPAddress.Add_TextChanged({
    if ($FormGateway.Text.Length -eq "0") {$FormIPAddress.BackColor="#FFF000"}
        else {$FormIPAddress.BackColor="#4fc8f0"}
        if ($FormIPAddress.Text -match '[^0-9.]') {
            $cursorPos = $FormIPAddress.SelectionStart
            $FormIPAddress.Text = $FormIPAddress.Text -replace '[^0-9.]',''
            $FormIPAddress.SelectionStart = $FormGateway.Text.Length
            $FormIPAddress.SelectionStart = $cursorPos - 1
            $FormIPAddress.SelectionLength = 0
        }
    })
   
   $FormIPAddress.TabIndex = 1
   $FormIPAddress.BackColor = $TextboxBackColor
   $FormIPAddress.ForeColor = $TextboxForeColor
   $FormIPAddress.BorderStyle = "FixedSingle"
   $Form.Controls.Add($FormIPAddress)
 
 
 
 
 
 
######################################################  
######################################################  
 
    ### Close Button ###
       $Closebutton = New-Object Windows.Forms.Button
       $Closebutton.text = "Close"
       $Closebutton.Location = New-Object Drawing.Point 195,15
       $Closebutton.TabIndex = 0
       $Closebutton.FlatStyle = "Flat"
       $Closebutton.BackColor = $ButtonBackColor
       $Form.Controls.Add($Closebutton)
       $CloseButton.Add_Click({$Form.Close()})
       
 
$Form.ShowDialog() | Out-Null
 
Remove-Variable IPAddress*
Remove-Variable Binary*
Remove-Variable Form*
Remove-Variable Label*
Remove-Variable Txt*
Remove-Variable Gateway*
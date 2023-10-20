
sl $PSScriptRoot
[Environment]::CurrentDirectory = gl
$eol = [Environment]::NewLine
cls

Add-Type -Name Window -Namespace Console -MemberDefinition '
[DllImport("Kernel32.dll")]
public static extern IntPtr GetConsoleWindow();

[DllImport("user32.dll")]
public static extern bool ShowWindow(IntPtr hWnd, Int32 nCmdShow);
'
function Hide-Console
{
    $consolePtr = [Console.Window]::GetConsoleWindow()
    #0 hide
    [Console.Window]::ShowWindow($consolePtr, 0)
}
# Hide-Console




Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing
[System.Windows.Forms.Application]::EnableVisualStyles()


$Form                             = New-Object System.Windows.Forms.Form
$Form.Text                        = "??????  $($env:USERNAME) `n ? ????  ??? ?? ?????? "
$Form.Size                        = New-Object System.Drawing.Size(550, 500)
$Form.StartPosition               = "CenterScreen"
$Form.FormBorderStyle             = "FixedToolWindow"
$Form.StartPosition               = "CenterScreen"
$Form.BackgroundImageLayout       = "Center"
$Form.AutoSize                    = $true
$Form.AutoScale                   = $false
$Form.ShowInTaskbar               = $True
$Form.Topmost                     = $True
$Form.Opacity                     = 0.95           #???????????? ???????? ???? ## 1 -??? ????????????
$Form.KeyPreview                  = $True
$Form.ShowInTaskbar               = $False
$Form.MinimumSize                 = New-object System.Drawing.Size(450, 500)#??????????? ?????? ????
$Form.MaximumSize                 = New-object System.Drawing.Size(600, 500)#???????????? ?????? ????
#$Form.FormBorderStyle             = [System.Windows.Forms.AutoSizeMode]::GrowOnly
$Form.add_paint(
    {
        $brush = new-object System.Drawing.Drawing2D.LinearGradientBrush(
    (new-object system.drawing.point 0, 0),
    (new-object system.drawing.point($this.clientrectangle.width, $this.clientrectangle.height)), "#A9D1CC", "#F7F2A8")
        $_.graphics.fillrectangle($brush, $this.clientrectangle)
    }
)
$Form.Add_KeyDown(
    {
        if ($_.KeyCode -eq "Enter") { $FormAcceptButton.PerformClick() }
        if ($_.KeyCode -eq "Escape") { $FormCloseButton.PerformClick() }
    }
)
$Form.AcceptButton                 = $FormAcceptButton
$Form.CancelButton                 = $FormCloseButton 


#####################
#Accept and Close
#####################

$global:FormAcceptButton           = New-Object System.Windows.Forms.Button
$FormAcceptButton.text             = "?????????"
$FormAcceptButton.width            = 110
$FormAcceptButton.height           = 50
$FormAcceptButton.location         = New-Object System.Drawing.Point(50,400)
$FormAcceptButton.Font             = 'Segoe UI Semibold,13'
$FormAcceptButton.ForeColor        = "Transparent"
$FormAcceptButton.BackColor        = "#2e4058"
$FormAcceptButton.DialogResult     = [System.Windows.Forms.DialogResult]::OK



$global:FormCloseButton           = New-Object System.Windows.Forms.Button
$FormCloseButton.text             = "Close"
$FormCloseButton.width            = 110
$FormCloseButton.height           = 50
$FormCloseButton.location         = New-Object System.Drawing.Point(170,400)
$FormCloseButton.Font             = 'Segoe UI Semibold,14'
$FormCloseButton.ForeColor        = "Transparent"
$FormCloseButton.BackColor        = "#2e4058"
$FormCloseButton.DialogResult     = [System.Windows.Forms.DialogResult]::Cancel

#####################
#Func Accept and Close
#####################

Function Form_MouseBehavior {
    param(
        $Action
    )
    Switch ($Action) {
        "MouseHover-?????????" { $FormAcceptButton.BackColor = "#f79d04"; $Form.Cursor = "Hand" }
        "MouseHover-Close" { $FormCloseButton.BackColor = "#f79d04"; $Form.Cursor = "Hand" }
        ##
        "MouseLeave-?????????" { $FormAcceptButton.BackColor = "#2e4058"; $Form.Cursor = "Default" }
        "MouseLeave-Close" { $FormCloseButton.BackColor = "#2e4058"; $Form.Cursor = "Default" }     
    }
    $Global:Action = "No"
}

####################################################################################

function Test-Subnet 
{
    param
    (
        [Parameter(Mandatory)] [Net.IPAddress] $ip1,
        [Parameter(Mandatory, ValueFromPipeline)] [Net.IPAddress[]] $ip2,
        [parameter()] [Net.IPAddress] $Mask = $MaskIspTextBox.text
    )
    process {
        foreach ($ip in $ip2) {
        [PSCustomObject]@{
                    Ip1           = $ip1
                    Mask          = $Mask
                    Ip2           = $ip
                    IsEqualSubnet = ($ip1.address -band $Mask.address) -eq
                    ($ip.address -band $Mask.address)
                }
            }
        }
}
function Update-ComboBox
{
    param
    (
    [Parameter(Mandatory = $true)]
    [ValidateNotNull()]
    [System.Windows.Forms.ComboBox]
    $ComboBox,
    [Parameter(Mandatory = $true)]
    [ValidateNotNull()]
    $Items,
    [Parameter(Mandatory = $false)]
    [string]$DisplayMember,
    [Parameter(Mandatory = $false)]
    [string]$ValueMember,
    [switch]
    $Append
    )

    if (-not $Append)
    {
    $ComboBox.Items.Clear()
}
if ($Items -is [Object[]])
{
    $ComboBox.Items.AddRange($Items)
}
elseif ($Items -is [System.Collections.IEnumerable])
{
    $ComboBox.BeginUpdate()
    foreach ($obj in $Items)
    {
    $ComboBox.Items.Add($obj)
    }
    $ComboBox.EndUpdate()
}
else
{  $ComboBox.Items.Add($Items) }
if ($DisplayMember)
{  $ComboBox.DisplayMember = $DisplayMember}
if ($ValueMember)
{  $ComboBox.ValueMember = $ValueMember}
}





Function Models_Menu {
    #####################
    #Models COMBO
    #####################
    $global:FgtModelBox = New-Object system.Windows.Forms.ComboBox
    $FgtModelBox.text = "?????? ?????? FGT!"
    $FgtModelBox.width = 200
    $FgtModelBox.height = 25
    $FgtModelBox.location = New-Object System.Drawing.Point(300, 40)
    $FgtModelBox.Font = 'Microsoft Sans Serif,10,style=Bold'  
    #$FgtModelBox.SelectedIndex = 0
    Update-ComboBox $FgtModelBox "FGT 40C", "FGT 30D", "FGT 30E", "FGT 40F"

    #********************
    $FgtModelBox_SelectedIndexChanged = {
        switch ( $FgtModelBox.SelectedItem.ToString()) {
            'FGT 40C' {
                Clean_TEST;
                Button1_Select;
                Drop_Dhcp;
                Drop_Ppoe;
                Drop_Static;
                $Wan1Button.Add_Click(
                    {
                        $ClickOn = "WAN1";
                        Clean_FormButton;
                        Create_FormButton -ClickOn $ClickOn  
                    }
                )
                $Wan2Button.Add_Click(
                    {
                        $ClickOn = "WAN2";
                        Clean_FormButton;
                        Create_FormButton -ClickOn $ClickOn  
                    }
                )
            };
            'FGT 30D' {
                Clean_TEST;
                Button2_Select;
                Drop_Dhcp;
                Drop_Ppoe;
                Drop_Static;
                $WanButton.Add_Click(
                    {
                        $ClickOn = "WAN";
                        Clean_FormButton;
                        Create_FormButton -ClickOn $ClickOn  
                    }
                )
                $Lan1Button.Add_Click(
                    {
                        $ClickOn = "LAN1";
                        Clean_FormButton;
                        Create_FormButton -ClickOn $ClickOn  
                    }
                )
            };
            'FGT 30E' {
                Clean_TEST;
                Button2_Select;
                Drop_Dhcp;
                Drop_Ppoe;
                Drop_Static;
                $WanButton.Add_Click(
                    {
                        $ClickOn = "WAN";
                        Clean_FormButton;
                        Create_FormButton -ClickOn $ClickOn  
                    }
                )
                $Lan1Button.Add_Click(
                    {
                        $ClickOn = "LAN1";
                        Clean_FormButton;
                        Create_FormButton -ClickOn $ClickOn  
                    }
                )
            };
            'FGT 40F' {
                Clean_TEST;
                Button2_Select;
                Drop_Dhcp;
                Drop_Ppoe;
                Drop_Static;
                $WanButton.Add_Click(
                    {
                        $ClickOn = "WAN";
                        Clean_FormButton;
                        Create_FormButton -ClickOn $ClickOn  
                    }
                )
                $Lan1Button.Add_Click(
                    {
                        $ClickOn = "LAN1";
                        Clean_FormButton;
                        Create_FormButton -ClickOn $ClickOn  
                    }
                )
            };
        }
    }
    $FgtModelBox.add_SelectedIndexChanged($FgtModelBox_SelectedIndexChanged)
    ###
    $Form.Add_Shown({ $FgtModelBox.Select()  })
    $Form.Controls.AddRange(@($FgtModelBox))
    
}


Function Switch_Menu {
    #####################
    #Swithc COMBO
    #####################

    $global:SwitchModsBox = New-Object system.Windows.Forms.ComboBox
    $SwitchModsBox.text = "????? ??????!"
    $SwitchModsBox.width = 170
    $SwitchModsBox.height = 20
    $SwitchModsBox.location = New-Object System.Drawing.Point(300, 5)
    $SwitchModsBox.Font = 'Microsoft Sans Serif,10,style=Bold'  
    #$SwitchesModsBox.SelectedIndex = 0
    Update-ComboBox $SwitchModsBox "Unmanaged Switch", "Managed Switch"

    #********************
    $SwitchModsBox_SelectedIndexChanged = {
        switch ( $SwitchModsBox.SelectedItem.ToString()) {
            'Unmanaged Switch' {
                Write-Host "333333"
                if ($FgtModelBox) { $FgtModelBox.Dispose() };
                Clean_TEST;
                Drop_Dhcp;
                Drop_Ppoe;
                Drop_Static;
                ###
                Models_Menu
            }
            'Managed Switch' {
                Write-Host "444444"
                if ($FgtModelBox) { $FgtModelBox.Dispose() };
                Clean_TEST;
                Drop_Dhcp;
                Drop_Ppoe;
                Drop_Static;
                ###
                Models_Menu
            }
        }
    }
    $SwitchModsBox.add_SelectedIndexChanged($SwitchModsBox_SelectedIndexChanged)
    ###
    $Form.Add_Shown({ $SwitchModsBox.Select()   })
    $Form.Controls.AddRange(@($SwitchModsBox))

}
#####################
#PORT COMBO
#####################

Function Ports_Menu {
    $global:PortModeBox = New-Object system.Windows.Forms.ComboBox
    $PortModeBox.text = "SELECT link mode"
    $PortModeBox.width = 170
    $PortModeBox.height = 20
    $PortModeBox.location = New-Object System.Drawing.Point(300, 110)
    $PortModeBox.Font = 'Microsoft Sans Serif,10,style=Bold'
    $PortModeBox.ForeColor = "#9b9b9b"
    Update-ComboBox $PortModeBox "DHCP", "STATIC", "PPPOe"
    $PortModeBox_SelectedIndexChanged = {
        switch ( $PortModeBox.SelectedItem.ToString())
        {
            "DHCP"
            {
                Drop_Ppoe;
                Drop_Static;
                ##
                Dhcp_Mode;
            }
            "STATIC"
            {
                Drop_Dhcp;
                Drop_Ppoe;
                ##
                Static_Mode;
            }
            "PPPOe"
            {
                Drop_Dhcp;
                Drop_Static;
                ##
                Ppoe_Mode;
            }
    
        }
    }
    $PortModeBox.add_SelectedIndexChanged($PortModeBox_SelectedIndexChanged)
    ###
    $Form.Controls.AddRange(@($PortModeBox))
}


#####################
#Functions MODS PORT
#####################

function Dhcp_Mode {
    #### DHCP rofl ####
    $globaL:labelDhcp = New-Object System.Windows.Forms.Label
    $labelDhcp.Location = New-Object System.Drawing.Point(300, 140)
    $labelDhcp.Size = New-Object System.Drawing.Size(200, 20)
    $labelDhcp.Font = 'Microsoft Sans Serif,12,style=Bold'
    $labelDhcp.Text = "--DHCP--"
    $labelDhcp.ForeColor = "#4fc8f0"
    $labelDhcp.BackColor = "Transparent"

    $Form.Controls.AddRange(@($labelDhcp))

}

function Ppoe_Mode {

    ### Login ###

    $globaL:LoginIspTextBox = New-Object System.Windows.Forms.TextBox
    $LoginIspTextBox.multiline = $false
    $LoginIspTextBox.location = New-Object System.Drawing.Point(300, 140)
    $LoginIspTextBox.Size = New-Object System.Drawing.Size(150, 20)
    $LoginIspTextBox.Font = 'Microsoft Sans Serif,12'
    $LoginIspTextBox.Text = "Login ISP"
    $LoginIspTextBox.BackColor = "#ff426b"
    $LoginIspTextBox.Add_TextChanged(
        {
            if ($LoginIspTextBox.Text.Length -eq "0") { $LoginIspTextBox.BackColor = "#ff426b" }
            else { $LoginIspTextBox.BackColor = "#4fc8f0" }
            if ($LoginIspTextBox.Text -match '[^a-zA-Z0-9@._-]') {
                $cursorPos = $LoginIspTextBox.SelectionStart
                $LoginIspTextBox.Text = $LoginIspTextBox.Text -replace '[^a-zA-Z0-9@._-]', ''
                $LoginIspTextBox.SelectionStart = $LoginIspTextBox.Text.Length
                $LoginIspTextBox.SelectionStart = $cursorPos - 1
                $LoginIspTextBox.SelectionLength = 0
            }
        }
    )

    ### Pass ###

    $globaL:PassIspTextBox = New-Object System.Windows.Forms.TextBox
    $PassIspTextBox.multiline = $false
    $PassIspTextBox.location = New-Object System.Drawing.Point(300, 170)
    $PassIspTextBox.Size = New-Object System.Drawing.Size(150, 20)
    $PassIspTextBox.Font = 'Microsoft Sans Serif,12'
    $PassIspTextBox.Text = "Password ISP"
    $PassIspTextBox.BackColor              ="#ff426b"
    $PassIspTextBox.Add_TextChanged(
        {
            if ($PassIspTextBox.Text.Length -eq "0") { $PassIspTextBox.BackColor = "#FFF000" }
            else { $PassIspTextBox.BackColor = "#4fc8f0" }
            if ($PassIspTextBox.Text -match '[^a-zA-Z0-9]') {
                $cursorPos = $PassIspTextBox.SelectionStart
                $PassIspTextBox.Text = $PassIspTextBox.Text -replace '[^a-zA-Z0-9]', ''
                $PassIspTextBox.SelectionStart = $PassIspTextBox.Text.Length
                $PassIspTextBox.SelectionStart = $cursorPos - 1
                $PassIspTextBox.SelectionLength = 0
            }
        }
    )
    $Form.Controls.AddRange(@($LoginIspTextBox,$PassIspTextBox))
}

function Static_Mode {

    #######   LABELS

    #### IP ISP ####
    $globaL:labelIpIsp = New-Object Windows.Forms.Label
    $labelIpIsp.Location = New-Object Drawing.Point (300, 140)
    $labelIpIsp.Size = New-Object Drawing.Point (150, 20)
    $labelIpIsp.BackColor = "Transparent"
    $labelIpIsp.Text = "IPv4 Address"
    $labelIpIsp.Font = New-Object System.Drawing.Font("Segoe UI Semibold", 10)
    #### Mask ISP ####
    $globaL:labelMaskIsp = New-Object Windows.Forms.Label
    $labelMaskIsp.Location = New-Object Drawing.Point (300, 190)
    $labelMaskIsp.Size = New-Object Drawing.Point (150, 20)
    $labelMaskIsp.BackColor = "Transparent"
    $labelMaskIsp.Text = "Mask ISP"
    $labelMaskIsp.Font = New-Object System.Drawing.Font("Segoe UI Semibold", 10)
    #### Gateway ISP ####
    $globaL:labelGatewayIsp = New-Object Windows.Forms.Label
    $labelGatewayIsp.Location = New-Object Drawing.Point (300, 240)
    $labelGatewayIsp.Size = New-Object Drawing.Point (150, 20)
    $labelGatewayIsp.BackColor = "Transparent"
    $labelGatewayIsp.Text = "Gateway"
    $labelGatewayIsp.Font = New-Object System.Drawing.Font("Segoe UI Semibold", 10)

    ### IP Address ###

    $globaL:IpAddressIspTextBox = New-Object System.Windows.Forms.TextBox
    $IpAddressIspTextBox.Location = New-Object System.Drawing.Point (300, 160)
    $IpAddressIspTextBox.Size = New-Object System.Drawing.Point (150, 20)
    $IpAddressIspTextBox.Font = New-Object System.Drawing.Font("Segoe UI Semibold", 11.5)
    $IpAddressIspTextBox.text = "192.168.1.50"
    $IpAddressIspTextBox.BorderStyle = "FixedSingle"
    $IpAddressIspTextBox.Add_TextChanged(
        {
            $IpAddressIspTextBox.Text = $IpAddressIspTextBox.Text -replace '[^0-9.]', ''
            # ???? ?? ????????????? ?????????
            if ($IpAddressIspTextBox.Text -notmatch '(^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$)')
            {
                $IpAddressIspTextBox.BackColor = [System.Drawing.Color]::White
                $IpAddressIspTextBox.ForeColor = [System.Drawing.Color]::Red
                $labelIpIsp.Text = "IP Adress is invalid!"
                $labelIpIsp.BackColor = [System.Drawing.Color]::FromArgb(250, 80, 101)
                # ?????  
            }
            else {
                $labelIpIsp.Text = "IPv4 Address    -valids!"
                $IpAddressIspTextBox.BackColor = "#4fc8f0"
                $IpAddressIspTextBox.ForeColor = [System.Drawing.Color]::Black
                $labelIpIsp.BackColor = "Transparent"
            }
            # ???? ?????
            if ($IpAddressIspTextBox.Text.Length -eq "0") { $labelIpIsp.Text = "Please enter ip Adress!" }
        }
    )

    ### IP Masks ISP ###

    $globaL:MaskIspTextBox = New-Object System.Windows.Forms.TextBox
    $MaskIspTextBox.Location = New-Object System.Drawing.Point 300, 210
    $MaskIspTextBox.Size = New-Object System.Drawing.Point 150, 20
    $MaskIspTextBox.Font = New-Object System.Drawing.Font("Segoe UI Semibold", 11.5)
    $MaskIspTextBox.text = "255.255.255.0"
    $MaskIspTextBox.BorderStyle = "FixedSingle"
    $MaskIspTextBox.Add_TextChanged(
        {
            $MaskIspTextBox.Text = $MaskIspTextBox.Text -replace '[^0-9.]', ''
            # ???? ?? ????????????? ?????????
            if ($MaskIspTextBox.Text -notmatch '(^(255|254|252|248|240|224|192|128|0+)\.(255|254|252|248|240|224|192|128|0+)\.(255|254|252|248|240|224|192|128|0+)\.(255|254|252|248|240|224|192|128|0+)$)') {
                $MaskIspTextBox.BackColor = [System.Drawing.Color]::White
                $MaskIspTextBox.ForeColor = [System.Drawing.Color]::Red
                $labelMaskIsp.Text = "Subnet Mask is invalid!"
                $labelMaskIsp.BackColor = [System.Drawing.Color]::FromArgb(250, 80, 101)
                # ?????  
            } 
            else {
                $labelMaskIsp.Text = "Subnet Mask    -valids!"
                $MaskIspTextBox.BackColor = "#4fc8f0"
                $MaskIspTextBox.ForeColor = [System.Drawing.Color]::Black
                $labelMaskIsp.BackColor = "Transparent"
            }
            # ???? ?????
            if ($MaskIspTextBox.Text.Length -eq "0") { $labelMaskIsp.Text = "Please enter Subnet Mask!" }
        }
    )

    ### IP Gateway ISP ###

    $globaL:GatewayIspTextBox = New-Object System.Windows.Forms.TextBox
    $GatewayIspTextBox.Location = New-Object System.Drawing.Point 300, 260
    $GatewayIspTextBox.Size = New-Object System.Drawing.Point 150, 20
    $GatewayIspTextBox.Font = New-Object System.Drawing.Font("Segoe UI Semibold", 11.5)
    $GatewayIspTextBox.text = "192.168.1.1"
    $GatewayIspTextBox.BorderStyle = "FixedSingle"
    $GatewayIspTextBox.Add_TextChanged(
        {
            $GatewayIspTextBox.Text = $GatewayIspTextBox.Text -replace '[^0-9.]', ''
            # ???? ?? ????????????? ?????????
            if ($GatewayIspTextBox.Text -notmatch '(^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$)') {
                $GatewayIspTextBox.BackColor = [System.Drawing.Color]::White
                $GatewayIspTextBox.ForeColor = [System.Drawing.Color]::Red
                $labelGatewayIsp.Text = "Gateway is invalid!"
                $labelGatewayIsp.BackColor = [System.Drawing.Color]::FromArgb(250, 80, 101)
                # ?????  
            } 
            #  ???????? ?? ??????????????, ?? ????? ?? ???? ????
            else {
                $Test = Test-Subnet -ip1 $IpAddressIspTextBox.text -ip2  $GatewayIspTextBox.text
                if ($Test.IsEqualSubnet -eq $true) { 
                    $labelGatewayIsp.Text = "Gateway          -valids!"
                    $GatewayIspTextBox.BackColor = "#4fc8f0"
                    $GatewayIspTextBox.ForeColor = [System.Drawing.Color]::Black
                    $labelGatewayIsp.BackColor = "Transparent"
                }
                # else { return $GatewayIspTextBox.text }
            }
            # ???? ?????
            if ($GatewayIspTextBox.Text.Length -eq "0") { $labelGatewayIsp.Text = "Please enter Gateway!" }
        }
    )
    $Form.Controls.AddRange(@($labelIpIsp,$labelMaskIsp,$labelGatewayIsp,$IpAddressIspTextBox,$MaskIspTextBox,$GatewayIspTextBox))
}
function Drop_Dhcp {
    if ($labelDhcp) { $labelDhcp.Dispose() }
}
function Drop_Static {
    if ($labelIpIsp) { $labelIpIsp.Dispose() }
    if ($IpAddressIspTextBox) { $IpAddressIspTextBox.Dispose() }
    if ($labelMaskIsp) { $labelMaskIsp.Dispose() }
    if ($MaskIspTextBox) { $MaskIspTextBox.Dispose() }
    if ($labelGatewayIsp) { $labelGatewayIsp.Dispose() }
    if ($GatewayIspTextBox) { $GatewayIspTextBox.Dispose() }
}
function Drop_Ppoe {
    if ($LoginIspTextBox) { $LoginIspTextBox.Dispose() }
    if ($PassIspTextBox) { $PassIspTextBox.Dispose() }
}

function Button1_Select {
        #### Port ISP ####
        $global:labelChek = New-Object System.Windows.Forms.Label
        $labelChek.Location = New-Object System.Drawing.Point(300, 65)
        $labelChek.Size = New-Object System.Drawing.Size(200, 20)
        $labelChek.Font = 'Microsoft Sans Serif,12,style=Bold'
        $labelChek.Text = "Select Curent port ISP"
        $labelChek.BackColor = "Transparent"
        $labelChek.ForeColor = "#a69d9d"
    
    #####################
    #RADIO BUTTONS
    #####################

        #######   WAN1
        $globaL:Wan1Button = New-Object system.Windows.Forms.RadioButton
        $Wan1Button.text = "WAN1"
        $Wan1Button.width = 70
        $Wan1Button.height = 20
        $Wan1Button.location = New-Object System.Drawing.Point(330, 85)
        $Wan1Button.Font = 'Microsoft Sans Serif,10'
        $Wan1Button.BackColor = "Transparent"
    
    
        #######   WAN2
        $globaL:Wan2Button = New-Object system.Windows.Forms.RadioButton
        $Wan2Button.text = "WAN2"
        $Wan2Button.width = 70
        $Wan2Button.height = 20
        $Wan2Button.location = New-Object System.Drawing.Point(430, 85)
        $Wan2Button.Font = 'Microsoft Sans Serif,10'
        $Wan2Button.BackColor = "Transparent"

        $Form.Controls.AddRange(@($labelChek,$Wan1Button, $Wan2Button));
}
function Button2_Select {

    #### Port ISP ####
    $global:labelChek = New-Object System.Windows.Forms.Label
    $labelChek.Location = New-Object System.Drawing.Point(300, 65)
    $labelChek.Size = New-Object System.Drawing.Size(200, 20)
    $labelChek.Font = 'Microsoft Sans Serif,12,style=Bold'
    $labelChek.Text = "Select Curent port ISP"
    $labelChek.BackColor = "Transparent"
    $labelChek.ForeColor = "#a69d9d"

#####################
#RADIO BUTTONS
#####################

    #######   WAN
    $global:WanButton = New-Object system.Windows.Forms.RadioButton
    $WanButton.text = "WAN"
    $WanButton.width = 70
    $WanButton.height = 20
    $WanButton.location = New-Object System.Drawing.Point(330, 85)
    $WanButton.Font = 'Microsoft Sans Serif,10'
    $WanButton.BackColor = "Transparent"   


    #######   LAN1
    $globaL:Lan1Button = New-Object system.Windows.Forms.RadioButton
    $Lan1Button.text = "LAN1"
    $Lan1Button.width = 70
    $Lan1Button.height = 20
    $Lan1Button.location = New-Object System.Drawing.Point(430, 85)
    $Lan1Button.Font = 'Microsoft Sans Serif,10'
    $Lan1Button.BackColor = "Transparent"


    $Form.Controls.AddRange(@($labelChek,$WanButton, $Lan1Button));
}


Function Create_FormButton {
    param(
        $ClickOn
    )
    Switch ($ClickOn) {
        "WAN1" {Write-Host "$ClickOn"; if ($PortModeBox) { $PortModeBox.Dispose() }; Ports_Menu; Drop_Dhcp; Drop_Ppoe; Drop_Static }
        "WAN2" {Write-Host "$ClickOn"; if ($PortModeBox) { $PortModeBox.Dispose() }; Ports_Menu; Drop_Dhcp; Drop_Ppoe; Drop_Static }
        "WAN" {Write-Host "$ClickOn"; if ($PortModeBox) { $PortModeBox.Dispose() }; Ports_Menu; Drop_Dhcp; Drop_Ppoe; Drop_Static }
        "LAN1" {Write-Host "$ClickOn"; if ($PortModeBox) { $PortModeBox.Dispose() }; Ports_Menu; Drop_Dhcp; Drop_Ppoe; Drop_Static }
    }
}

Function Clean_FormButton {
    switch ($ClickOn) {
        "WAN1" {  if ($WanButton) { $WanButton.Dispose() }; if ($Wan2Button) { $Wan2Button.Dispose() }; if ($Lan1Button) { $Lan1Button.Dispose() } }
        "WAN2" { if ($WanButton) { $WanButton.Dispose() }; if ($Wan1Button) { $Wan1Button.Dispose() }; if ($Lan1Button) { $Lan1Button.Dispose() } }
        "WAN" {  if ($Wan1Button) { $Wan1Button.Dispose() }; if ($Wan2Button) { $Wan2Button.Dispose() }; if ($Lan1Button) { $Lan1Button.Dispose() } }
        "LAN1" { if ($Wan1Button) { $Wan1Button.Dispose() }; if ($Wan2Button) { $Wan2Button.Dispose() }; if ($WanButton) { $WanButton.Dispose() } }
    }
}

Function Clean_TEST {
    if ($labelChek) { $labelChek.Dispose() };
    if ($WanButton) { $WanButton.Dispose() };
    if ($Wan1Button) { $Wan1Button.Dispose() };
    if ($Wan2Button) { $Wan2Button.Dispose() };
    if ($Lan1Button) { $Lan1Button.Dispose() } ;
    if ($PortModeBox) { $PortModeBox.Dispose() };
}




#########################
#########################
#************************************************** Mouse magic

$FormAcceptButton.Add_MouseHover({  
    $Action = "MouseHover-?????????"; Form_MouseBehavior -Action $Action
})
$FormAcceptButton.Add_MouseLeave({ 
    $Action = "MouseLeave-?????????"; Form_MouseBehavior -Action $Action
})

$FormCloseButton.Add_MouseHover({
    $Action = "MouseHover-Close"; Form_MouseBehavior -Action $Action
})
$FormCloseButton.Add_MouseLeave({
    $Action = "MouseLeave-Close"; Form_MouseBehavior -Action $Action
})
#**************************************************


# $global:SwichR = ($SwitchModsBox.SelectedItem)
# $global:FGmodel = ($FgtModelBox.SelectedItem)

$FormAcceptButton.Add_Click({
    Write-Host "$FGmodel"
    Write-Host "$SwichR"
    Write-Host "ALLAHHHHH"
    })
# #$FormAcceptButton.Add_Click({ All-NameValidate })
# #$FormAcceptButton.Add_Click({  [System.IO.File]::WriteAllLines($MyPath, $Myfile, $Utf8NoBomEncoding) })
$FormCloseButton.Add_Click({ Write-Host "GRRRRRR" })
#**************************************************

$Form.Controls.AddRange(@($FormCloseButton,$FormAcceptButton))
Switch_Menu
$Form.Refresh()
[void]$Form.showdialog()

<#
#>

remove-Variable  -Name Switch*
remove-Variable  -Name SwichR*
remove-Variable -Name PortMode*
remove-Variable -Name FgtModel*
remove-Variable -Name Wan*
remove-Variable -Name Inbox*
remove-Variable -Name label*
remove-Variable -Name Login*
remove-Variable -Name Pass*
remove-Variable -Name form*
#>

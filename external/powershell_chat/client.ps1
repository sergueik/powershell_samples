$rs = [runspacefactory]::CreateRunspace()
$rs.ApartmentState = "STA"
$rs.ThreadOptions = "ReuseThread"
$rs.Open()
$ps = [powershell]::Create()
$ps.Runspace = $rs
$ps.Runspace.SessionStateProxy.SetVariable("pwd",$pwd)
$handle = $ps.AddScript({
    Add-Type assemblyName PresentationFramework
    Add-Type assemblyName PresentationCore
    Add-Type assemblyName WindowsBase
    ##Functions
    function Open-PoshChatAbout {
      $rs = [runspacefactory]::CreateRunspace()
      $rs.ApartmentState = "STA"
      $rs.ThreadOptions = "ReuseThread"
      $rs.Open()
      $ps = [powershell]::Create()
      $ps.Runspace = $rs
      $ps.Runspace.SessionStateProxy.SetVariable("pwd",$pwd)
      [void]$ps.AddScript({
          [xml]$xaml = @"
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" x:Name="AboutWindow" Title="About" Height="170" Width="330" ResizeMode="NoResize" WindowStartupLocation="CenterScreen" ShowInTaskbar="False">
  <Window.Background>
    <LinearGradientBrush StartPoint="0,0" EndPoint="0,1">
      <LinearGradientBrush.GradientStops>
        <GradientStop Color="#C4CBD8" Offset="0"/>
        <GradientStop Color="#E6EAF5" Offset="0.2"/>
        <GradientStop Color="#CFD7E2" Offset="0.9"/>
        <GradientStop Color="#C4CBD8" Offset="1"/>
      </LinearGradientBrush.GradientStops>
    </LinearGradientBrush>
  </Window.Background>
  <StackPanel>
    <Label FontWeight="Bold" FontSize="20">PowerShell Chat </Label>
    <Label FontWeight="Bold" FontSize="16" Padding="0"> Version: 1.1 </Label>
    <Label FontWeight="Bold" FontSize="16" Padding="0"> Created By: Boe Prox </Label>
    <Label Padding="10">
      <Hyperlink x:Name="AuthorLink"> http://learn-powershell.net </Hyperlink>
    </Label>
    <Button x:Name="CloseButton" Width="100"> Close </Button>
  </StackPanel>
</Window>
"@
          #Load XAML
          $reader = (New-Object System.Xml.XmlNodeReader $xaml)
          $AboutWindow = [Windows.Markup.XamlReader]::Load($reader)


          #Connect to Controls
          $CloseButton = $AboutWindow.FindName("CloseButton")
          $AuthorLink = $AboutWindow.FindName("AuthorLink")

          #PsexecLink Event
          $AuthorLink.Add_Click({
              Start-Process "http://learn-powershell.net"
            })

          $CloseButton.Add_Click({
              $AboutWindow.Close()
            })

          #Show Window
          [void]$AboutWindow.showDialog()
        }).BeginInvoke()
    }
    function Script:Save-Transcript {
      $saveFile = New-Object Microsoft.Win32.SaveFileDialog
      $saveFile.Filter = "Text documents (.txt)|*.txt"
      $saveFile.DefaultExt = '.txt'
      $saveFile.FileName = ("{0:yyyyddmm_hhmmss}ChatTranscript" -f (Get-Date))
      $saveFile.OverwritePrompt = $True
      $return = $saveFile.showDialog()
      if ($return) {
        $Message = New-Object System.Windows.Documents.TextRange -ArgumentList $MainMessage.Document.ContentStart,$MainMessage.Document.ContentEnd
        $Message.text | Out-File $saveFile.FileName
      }
    }
    function Script:New-ChatMessage {
      [CmdletBinding()]
      param(
        [Parameter(ValueFromPipeline = $True)]
        [string]$Message,
        [Parameter()]
        [string]$Foreground,
        [Parameter()]
        [string]$Background,
        [Parameter()]
        [switch]$Bold
      )
      begin {
        $Run = New-Object System.Windows.Documents.Run
        $Run.Foreground = $Foreground
        if ($PSBoundParameters['Bold']) {
          $run.FontWeight = 'Bold'
        }
      }
      process {
        $Run.text = $Message
      }
      end {
        Write-Output $Run
      }
    }

    function Script:Invoke-FontDialog {
      [CmdletBinding()]
      param(
        $Control,
        [switch]$ShowColor,
        [switch]$FontMustExist,
        [switch]$HideEffects
      )
      begin {
        $Script:fontDialog = New-Object windows.forms.fontdialog
        $fontDialog.AllowScriptChange = $False
        if ($PSBoundParameters['ShowColor']) {
          $fontDialog.Showcolor = $True
          $fontDialog.Color = $colors[$Control.Foreground.Color.ToString()]
        }
        if ($PSBoundParameters['FontMustExist']) {
          $fontDialog.FontMustExist = $True
        }
        if ($PSBoundParameters['HideEffects']) {
          $fontDialog.ShowEffects = $False
        }
        $styles = New-Object System.Collections.Arraylist
        $textDecorations = $Control.TextDecorations | Select -Expand Location
        if ($textDecorations -contains "Underline") {
          $Styles.Add("Underline") | Out-Null
        }
        if ($textDecorations -contains "Strikethrough") {
          $Styles.Add("Strikeout") | Out-Null
        }
        if ($Inputbox_txt.FontStyle -eq "Italic") {
          $Styles.Add("Italic") | Out-Null
        }
        if ($Inputbox_txt.FontWeight -eq "Bold") {
          $Styles.Add("Bold") | Out-Null
        }
        if ($styles.count -eq 0) {
          $style = "Regular"
        } else {
          $style = $styles -join ","
        }
      }
      process {
        $fontDialog.Font = New-Object System.Drawing.Font -ArgumentList $Control.Fontfamily.source,$Control.FontSize,$Style,"Point"
        if ($fontDialog.showDialog() -eq "OK") {
          $Control.FontSize = $FontDialog.Font.Size
          if ($PSBoundParameters['ShowColor']) {
            $Control.Foreground = $colors[$FontDialog.Color.Name]
          }
          $Control.Fontfamily = $fontDialog.Font.Fontfamily.Name
          if ($fontDialog.Font.Bold) {
            $Control.FontWeight = "Bold"
          } else {
            $Control.FontWeight = "Regular"
          }
          if ($fontDialog.Font.Italic) {
            $Control.FontStyle = "Italic"
          } else {
            $Control.FontStyle = "Normal"
          }
          if (-not $PSBoundParameters['HideEffects']) {
            $textDecorationCollection = New-Object System.Windows.TextDecorationCollection
            if ($fontDialog.Font.Underline) {
              $underline = New-Object System.Windows.TextDecoration
              $underline.Location = 'Underline'
              $textDecorationCollection.Add($underline)
            }
            if ($fontDialog.Font.Strikeout) {
              $strikethrough = New-Object System.Windows.TextDecoration
              $strikethrough.Location = 'strikethrough'
              $textDecorationCollection.Add($strikethrough)
            }
            if ($textDecorationCollection.count -gt 0) {
              #Sometimes a control does not have a TextDecorations property
              if ($Control | Get-Member -Name TextDecorations) {
                $Control.TextDecorations = $textDecorationCollection
              }
            }
          }
        }
      }
    }
    [xml]$xaml = @"
<Window
    xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
    xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
    x:Name='Window' Title='PoshChat' Height = '400' Width = '550' WindowStartupLocation = 'CenterScreen' ShowInTaskbar = 'True'>    
    <Window.Background>
        <LinearGradientBrush StartPoint='0,0' EndPoint='0,1'>
            <LinearGradientBrush.GradientStops> <GradientStop Color='#C4CBD8' Offset='0' /> <GradientStop Color='#E6EAF5' Offset='0.2' /> 
            <GradientStop Color='#CFD7E2' Offset='0.9' /> <GradientStop Color='#C4CBD8' Offset='1' /> </LinearGradientBrush.GradientStops>
        </LinearGradientBrush>
    </Window.Background>    
    <Grid ShowGridLines = 'false'>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width ='175*'> </ColumnDefinition>
            <ColumnDefinition Width ='Auto'> </ColumnDefinition>
            <ColumnDefinition Width ='75*'> </ColumnDefinition>
        </Grid.ColumnDefinitions>
        <Grid.RowDefinitions>
            <RowDefinition Height = 'Auto'/>
            <RowDefinition Height = '*'/>
            <RowDefinition Height = '10'/>
            <RowDefinition Height = '80'/>
        </Grid.RowDefinitions>   
       <Menu Width = 'Auto' HorizontalAlignment = 'Stretch' Grid.Row = '0' Grid.ColumnSpan = '3'>
        <Menu.Background>
            <LinearGradientBrush StartPoint='0,0' EndPoint='0,1'>
                <LinearGradientBrush.GradientStops> <GradientStop Color='#C4CBD8' Offset='0' /> <GradientStop Color='#E6EAF5' Offset='0.2' /> 
                <GradientStop Color='#CFD7E2' Offset='0.9' /> <GradientStop Color='#C4CBD8' Offset='1' /> </LinearGradientBrush.GradientStops>
            </LinearGradientBrush>
        </Menu.Background>
            <MenuItem x:Name = 'FileMenu' Header = '_File'>           
                <MenuItem x:Name = 'SaveTranscript' Header = '_Save Transcript' ToolTip = 'Saves the chat transcript.' InputGestureText ='Ctrl+S'> </MenuItem>
                <Separator />
                <MenuItem x:Name = 'ExitMenu' Header = 'E_xit' ToolTip = 'Exits the client.' InputGestureText ='Ctrl+E'> </MenuItem>
            </MenuItem>
            <MenuItem x:Name = 'EditMenu' Header = '_Edit'>
                <MenuItem x:Name = 'FontMenu' Header = '_Font'>
                    <MenuItem x:Name = 'MessageWindowFont' Header = '_MessageWindow'/>
                    <MenuItem x:Name = 'OnlineUsersFont' Header = '_OnlineUsers'/>
                    <MenuItem x:Name = 'InputBoxFont' Header = '_InputBox'/>
                </MenuItem>         
            </MenuItem>           
            <MenuItem x:Name = 'HelpMenu' Header = '_Help'>
                <MenuItem x:Name = 'AboutMenu' Header = '_About' ToolTip = 'Show the current version and other information.'> </MenuItem>
            </MenuItem>            
        </Menu>          
        <RichTextBox x:Name = 'MainMessage_txt' Grid.Column = '0' Grid.Row = '1' IsReadOnly = 'True' VerticalScrollBarVisibility='Visible'>   
            <FlowDocument>
                <Paragraph x:Name = 'Paragraph'/>
            </FlowDocument>
        </RichTextBox>
        <Label Grid.Column='1' Grid.Row = '1' Width='8' Grid.RowSpan = '3' HorizontalAlignment = 'Center' VerticalAlignment = 'Stretch'
        Background = 'LightGray'/>
        <Label Grid.Column = '0' Grid.Row = '2' Grid.ColumnSpan = '3' Background = 'LightGray'/>
        <ListView x:Name = 'OnlineUsers' Grid.Column = '2' Grid.Row = '1' />
        <Grid Grid.Column = '0' Grid.Row = '3' ShowGridLines = 'false'>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width ='175*'> </ColumnDefinition>
            <ColumnDefinition Width ='Auto'> </ColumnDefinition>
        </Grid.ColumnDefinitions>          
            <TextBox x:Name = 'Input_txt' AcceptsReturn = 'True' VerticalScrollBarVisibility='Visible' TextWrapping = 'Wrap'
            Grid.Column = '0' HorizontalAlignment = 'Stretch'/>
            <Button x:Name = 'Send_btn' Width = '50' Height = '25' Content = 'Send' Grid.Column = '1'/>
        </Grid>   
        <Grid Grid.Column = '2' Grid.Row = '3' ShowGridLines = 'false'>  
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width ='Auto'> </ColumnDefinition>
                <ColumnDefinition Width ='5'> </ColumnDefinition>
                <ColumnDefinition Width ='*'> </ColumnDefinition>
                <ColumnDefinition Width ='Auto'> </ColumnDefinition>
            </Grid.ColumnDefinitions>
            <Grid.RowDefinitions>
                <RowDefinition Height = 'Auto'/>
                <RowDefinition Height = 'Auto'/>
                <RowDefinition Height = '*'/>
            </Grid.RowDefinitions>  
            <Label Content = 'UserName' HorizontalAlignment = 'Stretch' Grid.Column = '0' Grid.Row = '0'/>
            <TextBox x:Name = 'username_txt' HorizontalAlignment = 'Stretch' Grid.Column = '2' Grid.Row = '0'/>    
            <Label Content = 'Server' HorizontalAlignment = 'Stretch' Grid.Column = '0' Grid.Row = '1'/>
            <TextBox x:Name = 'servername_txt' HorizontalAlignment = 'Stretch' Grid.Column = '2' Grid.Row = '1'/>   
            <Button x:Name = 'Connect_btn' MaxWidth = '75' Height = '20' Content = 'Connect'
            Grid.Column = '0' Grid.Row = '2' HorizontalAlignment = 'stretch'/>
            <Button x:Name = 'Disconnect_btn' MaxWidth = '75' Height = '20' Content = 'Disconnect'
            Grid.Column = '2' Grid.Row = '2' HorizontalAlignment = 'stretch' IsEnabled = 'False'/>    
            <Label Grid.Column = '3' Grid.Row = '2' Width ='5'/>
        </Grid>   
    </Grid>
</Window>
"@
    #Load XAML
    $reader = (New-Object System.Xml.XmlNodeReader $xaml)
    $Window = [Windows.Markup.XamlReader]::Load($reader)

    ##Controls
    $MessageWindowFont = $Window.FindName("MessageWindowFont")
    $OnlineUsersFont = $Window.FindName("OnlineUsersFont")
    $InputBoxFont = $Window.FindName("InputBoxFont")
    $Script:Paragraph = $Window.FindName('Paragraph')
    $Script:OnlineUsers = $Window.FindName('OnlineUsers')
    $SendButton = $Window.FindName('Send_btn')
    $Script:ConnectButton = $Window.FindName('Connect_btn')
    $DisconnectButton = $Window.FindName('Disconnect_btn')
    $Username_txt = $Window.FindName('username_txt')
    $Server_txt = $Window.FindName('servername_txt')
    $Inputbox_txt = $Window.FindName('Input_txt')
    $Script:MainMessage = $Window.FindName('MainMessage_txt')
    $ExitMenu = $Window.FindName('ExitMenu')
    $SaveTranscript = $Window.FindName('SaveTranscript')
    $AboutMenu = $Window.FindName('AboutMenu')

    ##Events
    ##InputBox Font event
    $InputBoxFont.Add_Click({
        Invoke-FontDialog -Control $Inputbox_txt -ShowColor -FontMustExist
      })
    ##OnlineUser Font event
    $OnlineUsersFont.Add_Click({
        Invoke-FontDialog -Control $OnlineUsers -ShowColor -FontMustExist
      })
    ##MessageWindow Font event
    $MessageWindowFont.Add_Click({
        Invoke-FontDialog -Control $MainMessage -FontMustExist
      })

    #ExitMenu
    $ExitMenu.Add_Click({
        $Window.Close()
      })

    #SaveTranscriptMenu
    $SaveTranscript.Add_Click({
        Save-Transcript
      })

    #AboutMenu
    $AboutMenu.Add_Click({
        Open-PoshChatAbout
      })

    #Connect
    $ConnectButton.Add_Click({
        #Get Server IP
        $Server = $Server_txt.text

        #Get Username
        $Global:Username = $Username_txt.text
        if ($username -match "^[A-Za-z0-9_!]*$") {
          $ConnectButton.IsEnabled = $False
          $DisconnectButton.IsEnabled = $True
          if ($Server -and $Username) {
            $Message = "Connecting to {0} as {1}" -f $Server,$username
            $Paragraph.Inlines.Add((New-ChatMessage -Message ("{0} " -f (Get-Date).ToShortDateString()) -ForeGround Black -Bold))
            $Paragraph.Inlines.Add((New-Object System.Windows.Documents.LineBreak))
            $Paragraph.Inlines.Add((New-ChatMessage -Message $message -ForeGround Green))
            $Paragraph.Inlines.Add((New-Object System.Windows.Documents.LineBreak))

            #Connect to server
            $Endpoint = New-Object System.Net.IPEndpoint ([ipaddress]::any,$SourcePort)
            $TcpClient = [Net.Sockets.TCPClient]$endpoint
            try {
              $TcpClient.Connect($Server,15600)
              $Global:ServerStream = $TcpClient.GetStream()
              $data = [text.Encoding]::Ascii.GetBytes($Username)
              $ServerStream.Write($data,0,$data.length)
              $ServerStream.Flush()
              if ($TcpClient.Connected) {
                $Window.Title = ("{0}: Connected as {1}" -f $Window.Title,$Username)
                #Kick off a job to watch for messages from clients
                $newRunspace = [runspacefactory]::CreateRunspace()
                $newRunspace.Open()
                $newRunspace.SessionStateProxy.SetVariable("TcpClient",$TcpClient)
                $newRunspace.SessionStateProxy.SetVariable("MessageQueue",$MessageQueue)
                $newRunspace.SessionStateProxy.SetVariable("ConnectButton",$ConnectButton)
                $newPowerShell = [powershell]::Create()
                $newPowerShell.Runspace = $newRunspace
                $sb = {
                  #Code to kick off client connection monitor and look for incoming messages.
                  $client = $TCPClient
                  $serverstream = $Client.GetStream()
                  #While client is connected to server, check for incoming traffic
                  while ($client.Connected) {
                    try {
                      [byte[]]$inStream = New-Object byte[] 200KB
                      $buffSize = $client.ReceiveBufferSize
                      $return = $serverstream.Read($inStream,0,$buffSize)
                      if ($return -gt 0) {
                        $Messagequeue.Enqueue([System.Text.Encoding]::Ascii.GetString($inStream[0..($return - 1)]))
                      }
                    } catch {
                      #Connection to server has been closed                            
                      $Messagequeue.Enqueue("~S")
                      break
                    }
                  }
                  #Shutdown the connection as connection has ended
                  $client.Client.Disconnect($True)
                  $client.Client.Close()
                  $client.Close()
                  $ConnectButton.IsEnabled = $True
                  $DisconnectButton.IsEnabled = $False
                }
                $job = "" | Select Job,PowerShell
                $job.PowerShell = $newPowerShell
                $Job.job = $newPowerShell.AddScript($sb).BeginInvoke()
                $ClientConnection.$Username = $job
              }
            } catch {
              #Errors Connecting to server
              $Paragraph.Inlines.Add((New-ChatMessage -Message ("Unable to connect to {0}! Please try again later!" -f $RemoteServer) -ForeGround Red))
              $ConnectButton.IsEnabled = $True
              $TcpClient.Close()
              $ClientConnection.user.PowerShell.EndInvoke($ClientConnections.user.job)
              $ClientConnection.user.PowerShell.Runspace.Close()
              $ClientConnection.user.PowerShell.Dispose()
            }
          }
        } else {
          #Username is not in correct format
          $Paragraph.Inlines.Add((New-ChatMessage -Message ("`'{0}`' is not a valid username! Acceptable characters are 'A-Za-z0-9!_'. Spaces are not allowed!" -f $username) -ForeGround Red))
          $Paragraph.Inlines.Add((New-Object System.Windows.Documents.LineBreak))
        }
      })

    #Send message
    $SendButton.Add_Click({
        #Send message to server
        if (($Inputbox_txt.text).StartsWith("@")) {
          $Messagequeue.Enqueue(("~I{0}{1}{2}" -f $username,"~~",$Inputbox_txt.text))
        }
        $Message = "~M{0}{1}{2}" -f $username,"~~",$Inputbox_txt.text
        $data = [text.Encoding]::Ascii.GetBytes($Message)
        $ServerStream.Write($data,0,$data.length)
        $ServerStream.Flush()
        $Inputbox_txt.Clear()
      })

    #Load Window
    $Window.Add_Loaded({
        #Dictionary of colors for Fonts
        $script:colors = @{}
        $Color = [windows.media.colors] | Get-Member -Static -Type Property | Select -Expand Name | ForEach-Object {
          $colors["$([windows.media.colors]::$_)"] = $_
          $colors[$_] = "$([windows.media.colors]::$_)"
        }
        #Date placeholder for later use
        $Global:Date = Get-Date -Format ddMMyyyy
        #Used for managing the queue of messages in an orderly fashion
        $Global:MessageQueue = [System.Collections.Queue]::Synchronized((New-Object System.collections.queue))
        #Used for managing client connection
        $Global:ClientConnection = [hashtable]::Synchronized(@{})
        #Create Timer object
        $Global:timer = New-Object System.Windows.Threading.DispatcherTimer
        #Fire off every 1 seconds
        $timer.Interval = [timespan]"0:0:1.00"
        #Add event per tick
        $timer.Add_Tick({
            [Windows.Input.InputEventHandler]{ $Global:Window.UpdateLayout() }
            if ($Messagequeue.count -gt 0) {
              $Message = $Messagequeue.Dequeue()
              #If a different day then when client started
              if ($date -ne (Get-Date -Format ddMMyyyy)) {
                $date = (Get-Date -Format ddMMyyyy)
                $Paragraph.Inlines.Add((New-ChatMessage -Message ("{0} " -f (Get-Date).ToShortDateString()) -ForeGround Black -Bold))
                $Paragraph.Inlines.Add((New-Object System.Windows.Documents.LineBreak))
              }
              switch ($Message) {
                { $_.StartsWith("~B") } {
                  #Message
                  $data = ($_).Substring(2)
                  $split = $data -split ("{0}" -f "~~")
                  $Paragraph.Inlines.Add((New-ChatMessage -Message ("[{0}] " -f (Get-Date).ToLongTimeString()) -ForeGround Gray))
                  $Paragraph.Inlines.Add((New-ChatMessage -Message ("{0}: " -f $split[0]) -ForeGround Black -Bold))
                  $Paragraph.Inlines.Add((New-ChatMessage -Message ("{0}" -f $split[1]) -ForeGround Orange))
                }
                { $_.StartsWith("~I") } {
                  #Message
                  $data = ($_).Substring(2)
                  $split = $data -split ("{0}" -f "~~")
                  $Paragraph.Inlines.Add((New-ChatMessage -Message ("[{0}] " -f (Get-Date).ToLongTimeString()) -ForeGround Gray))
                  $Paragraph.Inlines.Add((New-ChatMessage -Message ("{0}: " -f $split[0]) -ForeGround Black -Bold))
                  $Paragraph.Inlines.Add((New-ChatMessage -Message ("{0}" -f $split[1]) -ForeGround Blue))
                }
                { $_.StartsWith("~M") } {
                  #Message
                  $data = ($_).Substring(2)
                  $split = $data -split ("{0}" -f "~~")
                  $Paragraph.Inlines.Add((New-ChatMessage -Message ("[{0}] " -f (Get-Date).ToLongTimeString()) -ForeGround Gray))
                  $Paragraph.Inlines.Add((New-ChatMessage -Message ("{0}: " -f $split[0]) -ForeGround Black -Bold))
                  $Paragraph.Inlines.Add((New-ChatMessage -Message ("{0}" -f $split[1]) -ForeGround Black))
                }
                { $_.StartsWith("~D") } {
                  #Disconnect
                  $Paragraph.Inlines.Add((New-ChatMessage -Message ("[{0}] " -f (Get-Date).ToLongTimeString()) -ForeGround Gray))
                  $Paragraph.Inlines.Add((New-ChatMessage -Message ("{0} has disconnected from the server" -f $_.Substring(2)) -ForeGround Green))
                  #Remove user from online list
                  $OnlineUsers.Items.Remove($_.Substring(2))
                }
                { $_.StartsWith("~C") } {
                  #Connect
                  $Message = ("{0} has connected to the server" -f $_.Substring(2))
                  $Paragraph.Inlines.Add((New-ChatMessage -Message ("[{0}] " -f (Get-Date).ToLongTimeString()) -ForeGround Gray))
                  $Paragraph.Inlines.Add((New-ChatMessage -Message $message -ForeGround Green))
                  ##Add user to online list       
                  if ($Username -ne $_.Substring(2)) {
                    $OnlineUsers.Items.Add($_.Substring(2))
                  }
                }
                { $_.StartsWith("~S") } {
                  #Server Shutdown
                  $Paragraph.Inlines.Add((New-ChatMessage -Message ("[{0}] " -f (Get-Date).ToLongTimeString()) -ForeGround Gray))
                  $Paragraph.Inlines.Add((New-ChatMessage -Message ("SERVER HAS DISCONNECTED.") -ForeGround Red))
                  $TcpClient.Close()
                  $ClientConnection.user.PowerShell.EndInvoke($ClientConnections.user.job)
                  $ClientConnection.user.PowerShell.Runspace.Close()
                  $ClientConnection.user.PowerShell.Dispose()
                  $ConnectButton.IsEnabled = $True
                  $DisconnectButton.IsEnabled = $False
                  $OnlineUsers.Items.Clear()
                }
                { $_.StartsWith("~Z") } {
                  #List of connected users
                  $online = (($_).Substring(2) -split "~~")
                  #Add online users to window
                  $Online | ForEach-Object {
                    $OnlineUsers.Items.Add($_)
                  }
                }
                Default {
                  $MainMessage.text += ("[{0}] {1}" -f (Get-Date).ToLongTimeString(),$_)
                }
              }
              $Paragraph.Inlines.Add((New-Object System.Windows.Documents.LineBreak))
              $MainMessage.ScrollToEnd()
            }
          })
        #Start timer
        $timer.Start()
        if (-not $timer.IsEnabled) {
          $Window.Close()
        }
      })
    #Close Window
    $Window.Add_Closed({
        if ($TcpClient) {
          $TcpClient.Close()
        }
        if ($ClientConnection.user) {
          $ClientConnection.user.PowerShell.EndInvoke($ClientConnection.user.job)
          $ClientConnection.user.PowerShell.Runspace.Close()
          $ClientConnection.user.PowerShell.Dispose()
        }
      })

    #Disconnect from server
    $DisconnectButton.Add_Click({
        $Paragraph.Inlines.Add((New-ChatMessage -Message ("[{0}] " -f (Get-Date).ToLongTimeString()) -ForeGround Gray))
        $Paragraph.Inlines.Add((New-ChatMessage -Message ("Disconnecting from server: {0}" -f $Server) -ForeGround Red))
        $Paragraph.Inlines.Add((New-Object System.Windows.Documents.LineBreak))
        #Shutdown client runspace and socket
        $TcpClient.Close()
        $ClientConnection.user.PowerShell.EndInvoke($ClientConnection.user.job)
        $ClientConnection.user.PowerShell.Runspace.Close()
        $ClientConnection.user.PowerShell.Dispose()
        $ConnectButton.IsEnabled = $True
        $DisconnectButton.IsEnabled = $False
        $OnlineUsers.Items.Clear()
      })

    $Window.Add_KeyDown({
        $key = $_.Key
        if ([System.Windows.Input.Keyboard]::IsKeyDown("RightCtrl") -or [System.Windows.Input.Keyboard]::IsKeyDown("LeftCtrl")) {
          switch ($Key) {
            "E" { $Window.Close() }
            "RETURN" {
              Write-Verbose ("Sending message")
              if (($Inputbox_txt.text).StartsWith("@")) {
                $Messagequeue.Enqueue(("~I{0}{1}{2}" -f $username,"~~",$Inputbox_txt.text))
              }
              $Message = "~M{0}{1}{2}" -f $username,"~~",$Inputbox_txt.text
              $data = [text.Encoding]::Ascii.GetBytes($Message)
              $ServerStream.Write($data,0,$data.length)
              $ServerStream.Flush()
              $Inputbox_txt.Clear()
            }
            "S" { Save-Transcript }
            Default { $Null }
          }
        }
      })

    [void]$Window.showDialog()

  }).BeginInvoke()

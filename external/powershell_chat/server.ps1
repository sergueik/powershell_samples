<#
To Do:
    * Graceful Exit of server while keeping console active
    * Better handling of message traffic
    * Message to client if at max connections    
#>

#Function to halt server and disconnect clients
[CmdletBinding()]
param(
  [Parameter()]
  [string]$EnableLogging # (Join-Path $pwd ChatLog.log)
)
if ($PSBoundParameters['EnableLogging']) {
  #Make global so it can be seen in event Action blocks
  $Global:EnableLogging = $EnableLogging
}
#Save previous prompt
$Global:old_Prompt = Get-Content Function:Prompt
#No Prompt
function Global:Prompt { [char]8 }
Clear-Host
##Create Globally synchronized hash tables and queue to share across runspaces
#Used for initial connections
$Global:sharedData = [hashtable]::Synchronized(@{})
#Used for managing client connections
$Global:ClientConnections = [hashtable]::Synchronized(@{})
#Used for managing the queue of messages in an orderly fashion
$Global:MessageQueue = [System.Collections.Queue]::Synchronized((New-Object System.collections.queue))
#Used to manage incoming client messages
$Global:ClientHash = [hashtable]::Synchronized(@{})
#Removal Queue
$Global:RemoveQueue = [System.Collections.Queue]::Synchronized((New-Object System.collections.queue))
#Listener
$Global:Listener = [hashtable]::Synchronized(@{})

#Set up timer
$Timer = New-Object Timers.Timer
$timer.Enabled = $true
$Timer.Interval = 1000

#Timer event to track client connections and remove disconnected clients
$NewConnectionTimer = Register-ObjectEvent -SourceIdentifier MonitorClientConnection -InputObject $Timer -EventName Elapsed -Action {
  while ($RemoveQueue.count -ne 0) {
    $user = $RemoveQueue.Dequeue()
    ##Close down the runspace
    $ClientConnections.$user.PowerShell.EndInvoke($ClientConnections.$user.Job)
    $ClientConnections.$user.PowerShell.Runspace.Close()
    $ClientConnections.$user.PowerShell.Dispose()
    $ClientConnections.Remove($User)
    $Messagequeue.Enqueue("~D{0}" -f $user)
  }
}
#Timer event to track for new incoming connections and to kick off seperate jobs to track messages 
$NewConnectionTimer = Register-ObjectEvent -SourceIdentifier NewConnectionTimer -InputObject $Timer -EventName Elapsed -Action {
  if ($ClientHash.count -lt $SharedData.count) {
    $sharedData.GetEnumerator() | ForEach-Object {
      if (-not ($ClientHash.Contains($_.Name))) {
        #Spin off new job and add to ClientHash
        $ClientHash[$_.Name] = $_.Value
        $User = $_.Name
        $Messagequeue.Enqueue(("~C{0}" -f $User))

        #Kick off a job to watch for messages from clients
        $newRunspace = [runspacefactory]::CreateRunspace()
        $newRunspace.Open()
        $newRunspace.SessionStateProxy.setVariable("shareddata",$shareddata)
        $newRunspace.SessionStateProxy.setVariable("ClientHash",$ClientHash)
        $newRunspace.SessionStateProxy.setVariable("User",$user)
        $newRunspace.SessionStateProxy.setVariable("MessageQueue",$MessageQueue)
        $newRunspace.SessionStateProxy.setVariable("RemoveQueue",$RemoveQueue)
        $newPowerShell = [powershell]::Create()
        $newPowerShell.Runspace = $newRunspace
        $sb = {
          #Code to kick off client connection monitor and look for incoming messages.
          $client = $ClientHash.$user
          $serverstream = $Client.GetStream()
          #While client is connected to server, check for incoming traffic
          while ($True) {
            [byte[]]$inStream = New-Object byte[] 200KB
            $buffSize = $client.ReceiveBufferSize
            $return = $serverstream.Read($inStream,0,$buffSize)
            if ($return -gt 0) {
              $Messagequeue.Enqueue([System.Text.Encoding]::ASCII.GetString($inStream[0..($return - 1)]))
            } else {
              $shareddata.Remove($User)
              $clienthash.Remove($User)
              $RemoveQueue.Enqueue($User)
              break
            }
          }
        }
        $job = "" | Select Job,PowerShell
        $job.PowerShell = $newPowerShell
        $Job.Job = $newPowerShell.AddScript($sb).BeginInvoke()
        $ClientConnections.$User = $job
      }
    }
  }
}

#Timer event to track for new incoming messages and broadcast message to all connected clients
$IncomingMessageTimer = Register-ObjectEvent -SourceIdentifier IncomingMessageTimer -InputObject $Timer -EventName Elapsed -Action {
  while ($MessageQueue.count -ne 0) {
    $Message = $MessageQueue.Dequeue()
    switch ($Message) {
      { $_.Startswith("~M") } {
        #Message
        $data = ($_).Substring(2)
        $split = $data -split ("{0}" -f "~~")
        if ($split[1].Startswith("@")) {
          if ($split[1] -match "(?<name>@\w+)(?<Message>.*)") {
            $directMessage = $matches.Name.Trim("@")
            $tempmessage = $matches.Message.Trim(" ")
            $message = ("~B{0}~~{1}" -f $split[0],$tempmessage)
          }
        } else {
          Write-Host ("{0} >> {1}: {2}" -f (Get-Date).ToString(),$split[0],$split[1])
          if ($EnableLogging) {
            Out-File -InputObject ("{0} >> {1}: {2}" -f (Get-Date).ToString(),$split[0],$split[1]) -FilePath $EnableLogging -Append
          }
        }
      }
      { $_.Startswith("~D") } {
        #Disconnect
        Write-Host ("{0} >> {1} has disconnected from the server" -f (Get-Date).ToString(),$_.Substring(2))
        if ($EnableLogging) {
          Out-File -InputObject ("{0} >> {1} has disconnected from the server" -f (Get-Date).ToString(),$_.Substring(2)) -FilePath $EnableLogging -Append
        }
      }
      { $_.Startswith("~C") } {
        #Connect
        Write-Host ("{0} >> {1} has connected to the server" -f (Get-Date).ToString(),$_.Substring(2))
        if ($EnableLogging) {
          Out-File -InputObject ("{0} >> {1} has connected to the server" -f (Get-Date).ToString(),$_.Substring(2)) -FilePath $EnableLogging -Append
        }
      }
      { $_.Startswith("~S") } {
        #Server Shutdown
        Write-Host ("{0} >> Server has shutdown." -f (Get-Date).ToString())
        if ($EnableLogging) {
          Out-File -InputObject ("{0} >> Server has shutdown." -f (Get-Date).ToString()) -FilePath $EnableLogging -Append
        }
      }
      Default {
        Write-Host ("{0} >> {1}" -f (Get-Date).ToString(),$_)
        if ($EnableLogging) {
          Out-File -InputObject ("{0} >> {1}" -f (Get-Date).ToString(),$_) -FilePath $EnableLogging -Append
        }
      }
    }
    #Broadcast message
    if ($directMessage) {
      $Broadcast = $Clienthash[$directMessage]
      $broadcastStream = $broadcast.GetStream()
      $string = $Message
      $broadcastbyte = ([text.encoding]::ASCII).GetBytes($String)
      $broadcastStream.Write($broadcastbyte,0,$broadcastbyte.Length)
      $broadcastStream.Flush()
      Remove-Variable directMessage -ErrorAction SilentlyContinue
    } else {
      $Clienthash.GetEnumerator() | ForEach-Object {
        $Broadcast = $Clienthash[$_.Name]
        $broadcastStream = $broadcast.GetStream()
        $string = $Message
        $broadcastbyte = ([text.encoding]::ASCII).GetBytes($String)
        $broadcastStream.Write($broadcastbyte,0,$broadcastbyte.Length)
        $broadcastStream.Flush()
      }
    }
  }
}

$Timer.Start()

#Initial runspace creation to set up server listener 
$Global:newRunspace = [runspacefactory]::CreateRunspace()
$newRunspace.Open()
$newRunspace.SessionStateProxy.setVariable("sharedData",$sharedData)
$newRunspace.SessionStateProxy.setVariable("Listener",$Listener)
$newRunspace.SessionStateProxy.setVariable("EnableLogging",$EnableLogging)
$Global:newPowerShell = [powershell]::Create()
$newPowerShell.Runspace = $newRunspace
$sb = {
  $Listener['listener'] = [System.Net.Sockets.TcpListener]15600
  $Listener['listener'].Start()
  [console]::WriteLine("{0} >> Server Started on port 15600" -f (Get-Date).ToString())
  if ($EnableLogging) {
    Write-Verbose ('Logging to file')
    Out-File -InputObject ("{0} >> Server Started on port 15600" -f (Get-Date).ToString()) -FilePath $EnableLogging
  }
  while ($true) {
    [byte[]]$byte = New-Object byte[] 5KB
    $client = $Listener['listener'].AcceptTcpClient()
    if ($client -ne $Null) {
      $stream = $client.GetStream()
      do {
        #Write-Host 'Processing Data'
        Write-Verbose ("Bytes Left: {0}" -f $Client.Available)
        $Return = $stream.Read($byte,0,$byte.Length)
        $String += [text.Encoding]::ASCII.GetString($byte[0..($Return - 1)])

      } while ($stream.DataAvailable)
      if ($SharedData.count -lt 30) {
        $SharedData[$String] = $client
        #Send list of online users to client
        $users = ("~Z{0}" -f ($shareddata.Keys -join "~~"))
        $broadcastStream = $client.GetStream()
        $broadcastbyte = ([text.encoding]::ASCII).GetBytes($users)
        $broadcastStream.Write($broadcastbyte,0,$broadcastbyte.Length)
        $broadcastStream.Flush()
        $String = $Null
      } else {
        #Too many clients, refuse connection
      }
    } else {
      #Connection to server closed
      break
    }
  } #End While
}
$Global:handle = $newPowerShell.AddScript($sb).BeginInvoke()

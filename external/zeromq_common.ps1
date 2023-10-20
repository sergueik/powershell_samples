# origin https://github.com/rohitjoshi/PowerShell-and-ZeroMQ 

#=======================================================================
# Purpose: Send logs to logging server using ZeroMQ
# Author: Rohit Joshi
# Date: 10/31/2012
#=======================================================================
#Global variables
$global:zmqSocket=$null
$enc = [system.Text.Encoding]::Unicode

#=======================================================================
# Function: Init ZeroMQ
# Arguments:
#=======================================================================
# Purpose: Initialize the zeromq
#=======================================================================
Function InitZeroMQ()
{

  $zmqCtx= [ZeroMQ.ZmqContext]::Create()
  $global:zmqSocket =([ZeroMQ.ZmqSocket]$zmqCtx.CreateSocket([ZeroMQ.SocketType]::PUSH));
  $global:zmqSocket.SendHighWatermark = 5000;
  $global:zmqSocket.Connect("tcp://localhost:10000");
}

#=======================================================================
# Function: SendMessage
# Arguments: string 
#=======================================================================
# Purpose:Send Message
#=======================================================================
Function SendMessage($msg)
{
 $data=$enc.GetBytes($msg) 
 $global:zmqSocket.Send($data, $data.Length, [ZeroMQ.SocketFlags]::None)
}

InitZeroMQ();
SendMessage("Sending data from PowerShell script");
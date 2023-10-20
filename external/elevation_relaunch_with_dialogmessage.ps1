param(
  [bool]$fullstop = $true,
  [switch]$message
)

# see also: https://www.cyberforum.ru/powershell/thread1794721.html#post9502065
# see also
# Wait-Event
# https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.utility/wait-event?view=powershell-7.2
# installed from Module: "Microsoft.PowerShell.Utility" with  Powershell 5.0 and ealier
# https://docs.microsoft.com/en-us/previous-versions/powershell/module/microsoft.powershell.utility/wait-event?view=powershell-5.0

function custom_pause {
  param(
    [bool]$fullstop,
    [string]$message = $null, # optional, e.g. 'Press any key to continue...'
    [int]$timeout = 1000
  )
  # Do not close Browser / Selenium when run from Powershell ISE
  if ($fullstop) {
    # write-host ('processing message "{0}"' -f $message )
    try {
      # NOTE: initializing string parameter with $null and testing 
      # if ($message -ne $null)
      # does not behave as expected
      if ($message -ne '') {
         write-host $message -nonewline
      } else {
        write-output 'Press any key to continue...'
      }
      [void]$host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown')
    } catch [exception]{}
  } else {
    Start-Sleep -Millisecond $timeout
  }
}



# based on:
# http://blogs.msdn.com/b/virtual_pc_guy/archive/2010/09/23/a-self-elevating-powershell-script.aspx

# Get the ID and security principal of the current user account
$myWindowsID=[System.Security.Principal.WindowsIdentity]::GetCurrent()
$myWindowsPrincipal=new-object System.Security.Principal.WindowsPrincipal($myWindowsID)

# Get the security principal for the Administrator role
$adminRole=[System.Security.Principal.WindowsBuiltInRole]::Administrator 

if ($myWindowsPrincipal.IsInRole($adminRole)) {
  # change the window title and background color to indicate that script is running "as Administrator"
  $Host.UI.RawUI.WindowTitle = $myInvocation.MyCommand.Definition + '(Elevated)'
  $Host.UI.RawUI.BackgroundColor = 'DarkBlue'
  clear-host
} else {
  # relaunch as administrator
  # Create a new process object that starts PowerShell
  # https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process.startinfo?view=netframework-4.0
  $newProcess = new-object System.Diagnostics.ProcessStartInfo 'PowerShell'
  # Specify the current script path and name as a parameter
  $newProcess.Arguments = $myInvocation.MyCommand.Definition;
  # pass down the copy of args - does not work as expected. No defined arguments fall to $args array, 
  # e.g. -fullstop $true won't be 
  # fall to copying the known
  $args_copy = $args
  if ([bool]$PSBoundParameters['message'].IsPresent) {
    $args_copy += '-message'
  }

  $newProcess.Arguments =  "& '" + $myinvocation.mycommand.definition + "'" + " " + ($args_copy -join " ")
  write-host ('Launching {0}' -f $newProcess.Arguments)
  custom_pause -fullstop $fullstop
  # Indicate that the process should be elevated
  $newProcess.Verb = 'runas'
  [System.Diagnostics.Process]::Start($newProcess) |out-null
  # see also 
  # https://www.cyberforum.ru/powershell/thread3004367.html
  # can add "noexit" argument to make it stay open

  # $arguments = "-NoExit & '" +$myinvocation.mycommand.definition + "'"
  # can also pack all attributes in the call
  # Start-Process powershell.exe -Verb runAs -ArgumentList $arguments
  # Exit from the current, unelevated, process
  exit
}
# Run your code that needs to be elevated here

$code = Add-Type -TypeDefinition @"
// https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.processstartinfo.arguments?view=netframework-4.0#System_Diagnostics_ProcessStartInfo_Arguments
// Place this code into a console project called ArgsEcho to build the argsecho.exe target

using System;

namespace StartArgs {
  public class ArgsEcho {
    public static void Show(string[] args) {
      Console.WriteLine("Received the following arguments:\n");
      for (var i = 0; i < args.Length; i++) {
        Console.WriteLine("[" + i + "] = " + args[i]);
      }
      Console.WriteLine("\nPress any key to exit");
      Console.ReadLine();
    }
  }
}
"@

# list the arguments (appears to not work as expected)
for ($i=0; $i -lt $args.length; $i++) {
  "arg $i is <$($args[$i])>"
}

$args_copy = $args

if ($args.Count -gt 0 ) {
  write-host ('processing {0} arguments' -f $args.Count)
  $status_continue = $true
  (0..$($args_copy.Count-1)) | foreach-object {
    $index = $_
    write-host ('{0} {1}' -f $index, $args_copy[$index])
  }
} else {
  write-host('no arguments provided')
}

# used in https://www.cyberforum.ru/powershell/thread3005977.html#post16343442
$message_switch = [bool]([bool]$PSBoundParameters['message'].IsPresent -bor $message.ToBool())
write-host ('message switch: "{0}"' -f $message_switch )
if (($message_switch -eq $true) -or (($test -ne '' ) -and ($test -ne $null))) {
   @( 'System.Drawing','System.Windows.Forms') | ForEach-Object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }
   # NOTE: may either double the quotes "" within double quoted string or escape with `
   $result = [System.Windows.Forms.MessageBox]::Show("We are running `"as Administrator.`"" + "`n" + "Continue Task?","Elevated", "YesNo" , "Information" , "Button1")
   $result = [System.Windows.Forms.MessageBox]::Show("We are running ""as Administrator.""" + "`n" + "Continue Task?","Elevated", "YesNo" , "Information" , "Button1")
}

custom_pause -fullstop $fullstop	
# origin: https://stackoverflow.com/questions/150161/waiting-for-user-input-with-a-timeout
# see also for a longer version:
# https://stackoverflow.com/questions/150161/waiting-for-user-input-with-a-timeout/52546471#52546471
# https://www.cyberforum.ru/powershell/thread1794721.html#post9502065
# see also
# Wait-Event
# https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.utility/wait-event?view=powershell-7.2
# installed from Module: "Microsoft.PowerShell.Utility" with  Powershell 5.0 and ealier
# https://docs.microsoft.com/en-us/previous-versions/powershell/module/microsoft.powershell.utility/wait-event?view=powershell-5.0

function custom_pause_timeout {
  param(
    [string]$message = 'Press any key to continue...',
    [int]$delay_counts = 10,
    [int]$timeout = 1000
  )
  try {
      write-host $message -nonewline
      $c = 0

      While ( -not ($host.UI.RawUI.KeyAvailable) -and ($c -lt $delay_counts)) {
      $c ++
      [Threading.Thread]::Sleep($timeout)
      <#
        # can read and exmine the key:
        if($host.UI.RawUI.KeyAvailable) {
           $key = $host.ui.RawUI.ReadKey('NoEcho,IncludeKeyUp')
      #>
  }
  } catch [exception]{}
}

custom_pause_timeout

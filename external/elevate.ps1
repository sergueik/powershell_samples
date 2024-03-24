# origin: http://www.cyberforum.ru/powershell/thread1719005.html
# NOTE: crafted by original author in "powershell-way"-heavy fashion
# considered a *good* practice  by original author
# a cleaner C# variant is possible

# see also:
# https://stackoverflow.com/questions/7985755/how-to-detect-if-cmd-is-running-as-administrator-has-elevated-privileges
# https://stackoverflow.com/questions/1894967/how-to-request-administrator-access-inside-a-batch-file

function Invoke-Sudo {
  begin {
    function private:ConvertTo-RegularString([Security.SecureString]$s) {
      return [Runtime.InteropServices.Marshal]::PtrToStringAuto(
        [Runtime.InteropServices.Marshal]::SecureStringToBSTR($s)
      )
    }
    
    function private:get-win32error([Int32]$e) {
      [PSObject].Assembly.GetType(
        'Microsoft.PowerShell.Commands.Internal.Win32Native'
      ).GetMethod(
        'GetMessage', [Reflection.BindingFlags]40
      ).Invoke($null, @($e))
    }
    
    if ((new-object Security.Principal.WindowsPrincipal(
      [Security.Principal.WindowsIdentity]::GetCurrent()
    )).IsInRole(
      [Security.Principal.WindowsBuiltInRole]::Administrator
    )) {
      get-win32error 12
      break
    }
  }
  process {
    # TODO: covert $domain, $user to method parameters
    [string]$domain = $env:computername
    [string]$username = 'user' 
    $usr = read-host ('{0}\{1}' -f $domain, $username  ) -AsSecureString
    if (($$ = convertto-regularstring $usr) -notmatch '\\') {
      get-win32error 10
      break
    }
    
    $domain, $username = $$.Split('\\')
    try {
      $process = new-object Diagnostics.Process
      $process.StartInfo.Domain = $domain
      $process.StartInfo.FileName = 'powershell'
      $process.StartInfo.LoadUserProfile = $false
      $process.StartInfo.Password = $(Read-Host 'Password' -AsSecureString)
      $process.StartInfo.UserName = $username
      $process.StartInfo.UseShellExecute = $false
      $process.StartInfo.WorkingDirectory = $HOME
      [void]$process.Start()
      
      start-sleep -Miliseconds 100
	  # NOTE: not tested
      stop-process -Id $PID 
    }
    catch [Management.Automation.MethodInvocationException] {
      write-host $_.Exception.InnerException
    }
  }
  end {}
}

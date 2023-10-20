# based on: https://www.cyberforum.ru/powershell/thread3116765.html
($ps = [PowerShell]::Create()).AddScript({
  $VerbosePreferense = 'Continue'

  function Test-VerboseRedirection {
    [CmdletBinding()]param()
    end {
    # NOTE: 
    # Write-Verbose 'Пример перенаправления потока Verbose.'
    # The string is missing the terminator: '.
      Write-Verbose 'line1'
      Write-Verbose 'line2'
      Write-Verbose 'line3'
    }
  }
}, $false).Invoke()
$ps.Commands.Clear()
$ps.AddScript({Test-VerboseRedirection -Verbose}).Invoke()
Write-output 'line1 example redirecing of write-Verbose.'
$data  = $ps.Streams.Verbose # "collected" the Verbose stream contents
write-output ($data -join ([char]10) )
$ps.Dispose()
<#
NOTE: 
powershell 3.x + supports collecting write-verbose outout via *> and cia | Tee-Object file.txt
#>

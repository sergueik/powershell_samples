write-output "wi-fi strength monitor"
$cnt = 4
@(1..$cnt) | % {

    (& C:\Windows\System32\netsh.exe wlan show interfaces) -Match '^\s+Signal' -Replace '^\s+Signal\s+:\s+',''
    start-sleep -second 2
}

echo ""
<#
NOTE: browser copypaste unicode issue:
The term 'Â' is not recognized as the name of a cmdlet, function, script
file, or operable program
Â Â  Â Function Get-InternetSpeed
#>
echo "internet speed test"


Function Get-InternetSpeed{
  $testfile = 'http://speedtest.tele2.net/10MB.zip'
  $tempfile = Join-Path -Path $env:TEMP -ChildPath 'testfile.tmp'
  $WebClient = new-object Net.WebClient
  $TimeTaken = Measure-Command { $WebClient.DownloadFile($testfile,$tempfile)} | Select-Object -ExpandProperty TotalSeconds
  $Speed = (10 / $TimeTaken) * 8
  $Message = "{0:N2} Mbit/Sec" -f ($Speed)
  remove-item -force -path $tempfile
  echo $Message
}

@(1..$cnt) | % {
  Get-InternetSpeed
  start-sleep -second 5
}

# start-sleep -second 120


# based on: https://qna.habr.com/q/1362552
param (
   $process_name = 'chrome',
   $threshold = .25
)

# $logFile = "${env:TEMP}\killer.log"

$cpu_cores = (Get-WMIObject Win32_ComputerSystem).NumberOfLogicalProcessors

$processes = get-process -Name $process_name
$cnt = 0
foreach ($process in $processes) {
  $counter = ('\Process({0}#{1})\% Processor Time' -f $Process.Name, $cnt)
  try {
    $data = Get-Counter $counter -erroraction stop
    $cpu = ($data.CounterSamples.CookedValue / $cpu_cores)
  } catch [Exception] {
    $cpu = 0
  }
  $timestamp = $GetProcessPayload.CounterSamples.Timestamp

  if ($cpu -ge $threshold) {
    $message = ("{0} : {1} : {2}: {3}" -f $process.Name , $process.Id, $cpu,  $timestamp)
    write-output $message
    #  Add-Content -Path $logFile -Value $message       
  }
  $cnt = $cnt + 1
}
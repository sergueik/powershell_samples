(1..10) |
foreach-object {
  $cnt = $_
  write-output ('test iteration {0}' -f $cnt) | out-file c:\temp\pstest.log -encoding ascii -append
  start-sleep -second 10
}
 write-host 'Done.' 
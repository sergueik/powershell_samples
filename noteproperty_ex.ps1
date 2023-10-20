$data = & net user $env:username | select-string -pattern '(Password last set)\s*([0-9].*)$' -allmatches
$date1 = get-date
$date2 = get-date($data.Matches.Groups[2].Captures.Value)
$data = $date1 - $date2
# the type of $data is Selected.System.TimeSpan
write-output ('The data of type {0}:' -f $data.getType().FullNAme )
$data | format-list

$result = @{
  days = 0 ;
  hours = 0; 
  minutes = 0;
}

write-output 'Loading properties "NoteProperty" (will fail):'
$data | 
get-member -membertype NoteProperty | 
foreach-object { 
  $name = $_.Name; 
  $result[$name] = $data.$name 
}
write-output 'Result:'
write-output $result
# all zeros
write-output 'Loading properties explicitly:'
$result['days'] = $data.Days
$result['hours'] = $data.Hours
$result['minutes'] = $data.Minutes

write-output 'Result:'
write-output $result
# exit
$data = $date1 - $date2 | select-object -property days,hours,minutes | select-object -property days,hours,minutes 
# the type of $data is Selected.System.Management.Automation.PSCustomObject:
write-output ('The data of type {0}:' -f $data.getType().FullNAme )
$data | format-list

$result = @{
  days = 0 ;
  hours = 0; 
  minutes = 0;
}


$data | 
get-member -membertype NoteProperty | 
foreach-object { 
  $name = $_.Name; 
  $result[$name] = $data.$name 
}
write-output $result


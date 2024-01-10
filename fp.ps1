$funcs = @{
  'x' = { $data = $args[0] ; $data};
  'y' = { ("'{0}'" -f $args[0]) };
  'z' = { param($data) ('"{0}"' -f $data )};
}
		
$data = @{ 'x' = 10; 'y' = 20; 'z' = 30; };

$data.keys |
foreach-object {
  $k = $_
  $res = $null
  $value = $data[$k]
  if ($funcs.containskey($k)) {
    $func = $funcs[$k]
    $res = $func.invoke($value)
  } else  { 
    $res = ''
  }
  # NOTE: print will produce something useless when composed this way
  # write-output ('{0}: {1}' -f $k, $res) 
  # x System.Collections.ObjectModel.Collection`1[System.Management.Automation.PSObject]
  # Workaround is
  write-output (('{0}: ' -f $k ) + $res) 
}

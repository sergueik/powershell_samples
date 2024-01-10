$funcs = @{
  'x' = { $value = $args[0] ; $value};
  'y' = { ("'{0}'" -f $args[0]) };
  'z' = { param($value) ('"{0}"' -f $value )};
}
function default_convertor {
  param (
    $value
  )
  return ( 'default_convertor: {0}' -f $value )
}
$data = @{ 'x' = 10; 'y' = 20; 'z' = 30; 't' = 101; };

$data.keys |
foreach-object {
  $k = $_
  $res = $null
  $value = $data[$k]
  if ($funcs.containskey($k)) {
    $func = $funcs[$k]
    $res = $func.invoke($value)
  } else  { 
    $res = default_convertor -value $value
  }
  # NOTE: print will produce something useless when composed this way
  # write-output ('{0}: {1}' -f $k, $res) 
  # x System.Collections.ObjectModel.Collection`1[System.Management.Automation.PSObject]
  # Workaround is
  write-output (('{0}: ' -f $k ) + $res) 
}

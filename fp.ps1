$funcs = @{
  'x' = { $key = $args[0]; $value = $args[1] ; $value};
  'y' = { ("args[0]='{0}' args[1]='{1}'" -f $args[0],$args[1]) };
  'z' = { param($key, $value) ('key="{0}" value="{1}"' -f $key, $value )};
}
function default_convertor {
  param (
    $key,
    $value
  )
  return ( 'default_convertor: key="{0}" value="{1}"' -f $key, $value )
}
$data = @{ 'x' = 10; 'y' = 20; 'z' = 30; 't' = 101; };

$data.keys |
foreach-object {
  $k = $_
  $res = $null
  $v = $data[$k]
  if ($funcs.containskey($k)) {
    $func = $funcs[$k]
    $res = $func.invoke($k,$v)
  } else  { 
    $res = default_convertor -key $k -value $v
  }
  # NOTE: print will produce something useless when composed this way
  # write-output ('{0}: {1}' -f $k, $res) 
  # x System.Collections.ObjectModel.Collection`1[System.Management.Automation.PSObject]
  # Workaround is
  write-output (('{0}: ' -f $k ) + $res) 
}

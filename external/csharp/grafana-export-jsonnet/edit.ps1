param(
# NOTE: `$input` is a bad name in Powershell -  assigning value to it leaves it empty
[string] $input_file = 'example.json',
[string] $output_file = 'new.json',
[switch] $debug
)
$content = get-content -literalpath $input_file
$root = $content | convertfrom-json

$root.'panels'[0].'type' = 'modified'
$root.'panels'[0].'targets'[0].'datasource'.'type' = 'new datasource type'
$panels = $root.'panels'

$panel_indexes = 0 .. ($panels.count -1 )

$panel_indexes | foreach-object {
  $cnt1 = $_
  $panel = $panels[$cnt1]
  write-output ('type: {0}' -f $panel.'type' )
  $targets =  $panel.'targets'
  $target_indexes = 0 .. ($targets.cout - 1 )
  $panel_indexes | foreach-object {
    $cnt2 = $_
    $target = $targets[$cnt2]
    write-output ('expr: {0}' -f $target.'expr' )
    write-output ('datasource type: {0}' -f $target.'datasource'.'type' )
  }
}
$depth = 20
# default depth is too low

[string]$newdata = $root | convertto-json -depth $depth

$newdata |out-file -literalpath $output_file -encoding ascii


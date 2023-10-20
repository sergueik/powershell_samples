param(
  [String]$datafile =  'chartdata_sample.json',
  [switch]$verbose
)
# origin: http://blogs.msdn.com/b/timid/archive/2013/03/05/converting-pscustomobject-to-from-hashtables.aspx
# https://www.powershellgallery.com/packages/ArcAdminTools/0.1.9/Content/Public%5CConvertTo-HashtableFromPsCustomObject.ps1
function ConvertTo-HashtableFromPsCustomObject {
  param(
    [Parameter(
      Position = 0,
      Mandatory = $true,
      ValueFromPipeline = $true,
      ValueFromPipelineByPropertyName = $true
    )] [object[]]$psCustomObject
  );

  process {
    foreach ($myPsObject in $psCustomObject) {
      $output = @{};
      $myPsObject | Get-Member -MemberType *Property | % {
        $output.($_.Name) = $myPsObject.($_.Name);
      }
      $output;
    }
  }
}

# the stock convertfrom-json produces type which is very uncomfortable to work with
$x = get-content -path (resolve-path -path $datafile) | convertfrom-json
$y = ConvertTo-HashtableFromPsCustomObject -psCustomObject $x
$z = $y |where-object { $_.target -eq 'test'} 
$w = $z['datapoints']
$t0   = $w[0][1]
$data = @()
0..($w.count-1) | foreach-object { 
$cnt = $_ ; 
$u =  $w[$cnt] ; 
$m =  $u[0]; 
$t = $u[1] - $t0; 
$data += @($t,$m)
}
$data | format-list
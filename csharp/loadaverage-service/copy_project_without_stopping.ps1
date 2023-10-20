param (
  [switch]$debug
)
# origin: https://stackoverflow.com/questions/18616581/how-to-properly-filter-multiple-strings-in-a-powershell-copy-script

$staging_dir = "${env:TEMP}/loadaverage-service"
$script_dir = 'C:\developer\sergueik\powershell_ui_samples\csharp\loadaverage-service'
if ($debug) {
  if ($PSScriptRoot -ne $null) {
    write-host  ('$PSScriptRoot: {0}' -f $PSScriptRoot)
  }
  if ($PSCommandPath -ne $null) {
    write-host  ('$PSCommandPath: {0}' -f $PSCommandPath)
  }
}
cd ..
$workspace_dir = (resolve-path 'loadaverage-service').path

# NOTE: without filtering will encounter the error
# copy-item: the process cannot access the file '*.exe' because it is being used by another process
# the files will be LoadAverageService.exe and Utils.dll
$exclude_masks  = @( '*.exe','*.dll','*.pdb','*.exe.config' )
copy-item -path "${staging_dir}\*" -destination $workspace_dir -recurse -force -exclude $exclude_masks 
if ($debug) {
  write-output ('Copied from {0} to {1}' -f $staging_dir, $script_dir) 
  get-childitem -path $workspace_dir -file -recurse -exclude $exclude_masks |
  foreach-object { write-output $_.fullname }
}

<#
  To confirm filtering works
  prune some non-critical directory e.g. Program\obj and confirm that no '*.exe' or '*.dll'
  files are there after the script run
#>
cd $script_dir
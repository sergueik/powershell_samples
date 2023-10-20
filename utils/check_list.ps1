# this script performs several checks and 'or's the status
# NOTE powershell script exitcode a.l.a $LASTEXITCODE 0 indicates a success
# however $status = $true
# [int]$status) is 1
[bool]$status= $false
@(1,2,3,4,5) | foreach-object {
  $tag = $_
  write-host ('perform step {0} check' -f $tag)
  ./check_single.ps1 $tag
  [bool]$step_status= ([bool]($LASTEXITCODE -eq 0))
  $status = $status -bor $step_status
  write-host ( 'step status: {0}' -f $step_status.ToString() )
  write-host ( 'overall status: {0}' -f $status.ToString() )
}
write-host ( 'status: {0}' -f $status.ToString() )
if ($status) {
  exit 0
} else {
  exit 1
}

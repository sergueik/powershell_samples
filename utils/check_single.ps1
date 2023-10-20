# this script performs several checks and 'or's the status
# NOTE powershell script exitcode a.l.a $LASTEXITCODE 0 indicates a success
# however $status = $true
# [int]$status) is 1
param(
  [int]$value = 0 
)
[bool]$status = [bool]($value -gt 2)
write-host ( 'check_single status: {0}' -f $status.ToString() )
if ($status) {
  exit 0
} else {
  exit 1
}

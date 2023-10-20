# illlustrates how sourcing the script which
# sets the default value of the parameter
# ruins that variable in the caller
$param =  '100'
write-host ('before 1st sourcing of callee, param = {0}' -f $param)
write-host ". ./callee.ps1 -param $param"
. ./callee.ps1 -param $param
write-host ('after 1st sourcing of callee, param = {0}' -f $param)
write-host ('param = {0}' -f $param)
write-host '. ./callee.ps1'
. ./callee.ps1
write-host ('after 2nd sourcing of callee, param = {0}' -f $param)
# lesson learned never use the same parameter name across sourced scripts

$param =  '200'
write-host ('before 1st call to callee, param = {0}' -f $param)
write-host ". ./callee.ps1 -param $param"
./callee.ps1 -param $param
write-host ('after 1st call to callee, param = {0}' -f $param)
write-host ('param = {0}' -f $param)
write-host '. ./callee.ps1'
./callee.ps1
write-host ('after 2nd call to callee, param = {0}' -f $param)

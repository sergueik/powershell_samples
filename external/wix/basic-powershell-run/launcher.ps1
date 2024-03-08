param(
  [parameter(position=0)]$logname,
  [parameter(position=1)]$message
)
# $logname = $args[0]
# $message = $args[1]
$message = $message.ToUpper()
. .\dependency.ps1 -message $message -logname $logname

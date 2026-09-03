# remove the system wide dotnet install dir from the PATH to favor
# USERPROFILE installed one
#
$p = [Environment]::GetEnvironmentVariable('PATH', 'User' )
$p = $p -replace 'C:\\Program Files\\dotnet;?',''
[Environment]::SetEnvironmentVariable('PATH', $p, 'User' )

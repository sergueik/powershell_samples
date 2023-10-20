# origin:
# https://tommymaynard.com/self-destruction-script-2018/

param(
  # switch_parameter System.Management.Automation.SwitchParameter
  [switch] $switch_parameter = $true,
  [switch] $debug
)

$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent
$script_dir = "${env:userprofile}\Desktop\scriptdir"
$script_path = $script_dir + '\' + 'self_destruct.ps1'
# [System.Management.Automation.InvocationInfo]
# [System.Management.Automation.ScriptInfo]


# NOTE: these are often empty
if ($debug_flag){
  write-host ('$MyInvocation.PSCommandPath  = {0}' -f $MyInvocation.PSCommandPath )
  write-host ('$PSCommandPath = {0}' -f $PSCommandPath )
  write-host ('$MyInvocation.PSScriptRoot  = {0}' -f $MyInvocation.PSScriptRoot )
  write-host ('$PSScriptRoot = {0}' -f $PSScriptRoot )
}
if ($PSScriptRoot -ne $null) {
  $script_dir = $PSScriptRoot
}
if ($PSCommandPath -ne $null) {
  $script_path = $PSCommandPath
}

cd $script_dir
$task_script = 'screen_resolution.ps1'
if (test-path -path $task_script ){
  write-host ('running script "{0}"' -f $task_script)
  # this does not work
  # & ".\${task_script}" -list
  $args = @('-list','-debug') -join ' '
  invoke-expression -command  ".\${task_script} $args"
}
if ($debug_flag){
  write-host ('Cleanup {0} and self-destruct in 3 seconds' -f $script_dir )
  
  3..1 | foreach-object {
    if ($_ -gt 1) {
      write-host ( "$_ seconds")
    } else {
      write-host ( "$_ second")
    } 
    start-sleep -seconds 1
  }
}
<#
get-childitem -path $script_dir -file |
where-object {$_.fullname -ne $script_path } |
foreach-object {
  select-object -inputobject $_ -property *
}
#>

if ($debug_flag){
  write-host ('Remove files from {0}' -f $script_dir )
}
# remove everything except self
get-childitem -file -path $script_dir |
where-object { $_.fullname -ne $script_path } | 
foreach-object {
  remove-item -Path $_.FullName -erroraction silentlycontinue
}

if ($debug_flag){
  write-host 'removing self'
}
# NOTE, using yet another "system property"
remove-item -path $MyInvocation.MyCommand.Source

# see also:
# https://github.com/sergueik/powershell_samples/blob/master/work_script.ps1

# wrapper around write-eventlog cmdlet with multi line messages
# see also:
# https://github.com/sergueik/powershell_samples/blob/master/log_eventlog.ps1

function log_eventlog {

  param(
    $message = $null,
    [int]$eventid = 1001,
    [String]$eventlog = 'application',
    [String]$source = 'EventSystem',    
    [bool]$debug

  )
  if ( -not ( [System.Diagnostics.EventLog]::Exists($eventlog) ) ) {
    write-debug ('The event Log does noe exist: {0}' -f $eventlog )
    return
  }
  # https://ss64.com/ps/write-eventlog.html
  # NOTE: Valid entryType are: Error (1), Warning (2), Information (4), SuccessAudit (8), FailureAudit (16)
  # https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.eventlogentrytype?view=netframework-4.5
  if ($source -ne '') {
    write-eventlog -source $source -logname $eventlog -eventid $eventid -message ($message -join ([char]10)) -entrytype Information
  } else {
    # NOTE: the source argument is required.
    # 
    # NOTE: can pick existing, no-offensive system known event log source to fallback to, e.g. 'Application'
    # https://learn.microsoft.com/en-us/windows/win32/eventlog/event-sources
    # https://stackoverflow.com/questions/5563585/can-i-list-all-registered-event-sources
    # NOTE, expect to observe the following in the event log created:
    # The description for Event ID 1001 from source Microsoft-Windows-EventSystem
    # cannot be found
    $source = 'Application'
    write-eventlog -source $source -logname $eventlog -eventid $eventid -message ($message -join ([char]10)) -entrytype Information
  }

}

# main

$processid = $pid
# NOTE: somewhat heavy interpolation. Cannot use ${...} in this command
# $wmiresult = get-wmiobject win32_process -filter "processid='${processid}'" | select-object -property parentprocessid,name

$wmiresult = get-wmiobject win32_process -filter "processid='$($processid)'" | select-object -property parentprocessid
$parentprocessid = $wmiresult.parentprocessid

$wmiresult = get-wmiobject win32_process -filter "processid='$($parentprocessid)'" | select-object -property name
$parentprocessname = $wmiresult.name
$date = get-date -format 'yyyy-MM-dd HH:mm'

$data = @{
  'username' = $env:USERNAME;
  'pid' = $processid;
  'parent' = $parentprocessname;
  'message' = 'test';
  'invoked' = $date;
}

# NOTE: windows line endings should be used in replacement
# $message = (((ConvertTo-Json -InputObject $data  -depth 10) -join '\n') -replace '\n' , '' )

$message = (((ConvertTo-Json -InputObject $data -depth 10) -join "`n") -replace '\r\n' , ' ' )

$eventlog = 'application'
log_eventlog -eventlog $eventlog -message $message
<#
Inspecting error code: 2147750692
The Task Scheduler service attempted to run the task, but the task did not run
due to one of the constraints in the task definition. (Exception from HRESULT:
0x80041324)

Task Scheduler did not launch task "\ATASK"  because computer is running on batteries. User Action: If launching the task on batteries is required, change the respective flag in the task configuration.
#>

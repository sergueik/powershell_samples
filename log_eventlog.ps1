# wrapper around write-eventlog cmdlet with multi line messages
function log_eventlog {

  param(
    $message = $null,
    [int]$eventid = 1001,
    [String]$eventlog = 'applog',
    [String]$source = '',    
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
    # Pick existing, no-offensive system known event log source
    # NOTE, expect to observe the following in the event log created:
    # The description for Event ID 1001 from source Microsoft-Windows-EventSystem
    # cannot be found
    write-eventlog -source EventSystem -logname $eventlog -eventid $eventid -message ($message -join ([char]10)) -entrytype Information
  }

}

# main
$eventlog = 'application'

log_eventlog -eventlog $eventlog -message $args

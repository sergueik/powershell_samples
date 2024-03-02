#Copyright (c) 2023 Serguei Kouzmine
#
#Permission is hereby granted, free of charge, to any person obtaining a copy
#of this software and associated documentation files (the "Software"), to deal
#in the Software without restriction, including without limitation the rights
#to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
#copies of the Software, and to permit persons to whom the Software is
#furnished to do so, subject to the following conditions:
#
#The above copyright notice and this permission notice shall be included in
#all copies or substantial portions of the Software.
#
#THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
#IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
#FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
#AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
#LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
#OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
#THE SOFTWARE.

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
$eventlog = 'application'

log_eventlog -eventlog $eventlog -message $args

# see also:
# https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_preference_variables?view=powershell-5.1
# https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.utility/write-verbose?view=powershell-5.1
# https://learn.microsoft.com/en-us/answers/questions/862166/understanding-write-verbose-and-write-debug
#
# NOTE: the explanation of non-terminating errors does not cover how to suppress stack trace fragment
#  https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.utility/write-error?view=powershell-5.1
#  using write-error for sole STDERR messaging is a topic in
#  https://stackoverflow.com/questions/38064704/how-can-i-display-a-naked-error-message-in-powershell-without-an-accompanying
#  and the recommendation is a wornaround
#  https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_preference_variables?view=powershell-5.1#errorview
#  Sadly, there is no single solution that works both from within PowerShell (across hosts) and from outside of it
# see also:
# https://intellitect.com/blog/powershell-write-error-without-writing-stack-trace/

function test1{ 
  param([string]$message)

  write-verbose 'message by test1'
  return 'test1 result'
}

function test2{ 
  param([string]$message)
  # the message will be lost
  write-output 'message by test2'
  # NOTE: when the function is being redirected the return value and write-output are colapsed into pipe
  return 'test2 result'
}

function test3{ 
  param([string]$message)
  # NOTE: Write-Error does not write to caller's stderr, unless when redirected
  # see also: https://stackoverflow.com/questions/4998173/how-do-i-write-to-standard-error-in-powershell/15669365#15669365
  write-error 'message by test3'
  return 'test3 result'
}

function test4{ 
  param([string]$message)
  [Console]::Error.WriteLine( 'message by test4' )
  return 'test4 result'
}

function test5{ 
  param([string]$message)
  # NOTE: Host.UI.WriteErrorLine() is a UI method (like Write-Host ) and can not be redirected to the file by some callers
  $host.ui.WriteErrorLine( 'message by test5' )
  return 'test5 result'
}

# setting to make verbose message display
$VerbosePreference = 'Continue'
write-verbose 'calling test1'
$result1 = test1
write-output ('test1 returned value {0}' -f $result1)

write-verbose 'calling test2'
$result2 = test2
write-output ('test2 returned value {0}' -f $result2)
write-output ('test2 returned value {0}' -f ($result2 -join ','))

write-verbose 'calling test3'
$result3 = test3
write-output ('test3 returned value {0}' -f $result3)

write-verbose 'calling test4'
$result4 = test4
write-output ('test4 returned value {0}' -f $result4)

write-verbose 'calling test5'
$result5 = test5
write-output ('test5 returned value {0}' -f $result5)

write-verbose 'piping test2'
test2| foreach-object {
  write-output ('test2 produced value: ' + $_)
}

write-verbose 'calling test2'
$result2 = test2
write-output ('test2 returned value lines:  {0}' -f $result2.count)

# NOTE: setting the value to the default simply makes all write-verbose messages disappear
$VerbosePreference = 'SilentlyContinue'

<#
Usage:
from Powershell console
$VerbosePreference="Continue"
.\stderr_ex.ps1

# call with redirection in Powershell console
. .\stderr_ex.ps1  | out-file a.log
# prints verbose message in console using colors
VERBOSE: calling test1
VERBOSE: message by test1
test1 returned value test1 result
VERBOSE: calling test2
test2 returned value message by test2
VERBOSE: calling test3
test3 : message by test3
At C:\developer\sergueik\powershell_samples\utils\stderr_ex.ps1:63 char:12
+ $result3 = test3
+            ~~~~~
    + CategoryInfo          : NotSpecified: (:) [Write-Error], WriteErrorExcep
   tion
    + FullyQualifiedErrorId : Microsoft.PowerShell.Commands.WriteErrorExceptio
   n,test3

test3 returned value test3 result
VERBOSE: calling test4
message by test4
test4 returned value test4 result
VERBOSE: calling test5
message by test5
test5 returned value test5 result
VERBOSE: piping test2
test2 produced value: message by test2
test2 produced value: test2 result
VERBOSE: calling test2
test2 returned value lines:  2

# redirection
. .\stderr_ex.ps1   |out-file a.log
VERBOSE: calling test1
VERBOSE: message by test1
VERBOSE: calling test2
VERBOSE: calling test3
test3 : message by test3
At C:\developer\sergueik\powershell_samples\utils\stderr_ex.ps1:63 char:12
+ $result3 = test3
+            ~~~~~
    + CategoryInfo          : NotSpecified: (:) [Write-Error], WriteErrorExcep
   tion
    + FullyQualifiedErrorId : Microsoft.PowerShell.Commands.WriteErrorExceptio
   n,test3

VERBOSE: calling test4
message by test4
VERBOSE: calling test5
message by test5
VERBOSE: piping test2
VERBOSE: calling test2

- the write-verbose messages are not redirected
get-content a.log

test1 returned value test1 result
test2 returned value message by test2
test2 returned value message by test2,test2 result
test3 returned value test3 result
test4 returned value test4 result
test5 returned value test5 result
test2 produced value: message by test2
test2 produced value: test2 result
test2 returned value lines:  2

from CMD console
powershell.exe -executionpolicy bypass -file "stderr_ex.ps1"

VERBOSE: calling test1
VERBOSE: message by test1
test1 returned value test1 result
VERBOSE: calling test2
test2 returned value message by test2
test2 returned value message by test2,test2 result
VERBOSE: calling test3
test3 : message by test3
At C:\developer\sergueik\powershell_samples\utils\stderr_ex.ps1:64 char:12
+ $result3 = test3
+            ~~~~~
    + CategoryInfo          : NotSpecified: (:) [Write-Error], WriteErrorExcep
   tion
    + FullyQualifiedErrorId : Microsoft.PowerShell.Commands.WriteErrorExceptio
   n,test3

test3 returned value test3 result
VERBOSE: calling test4
message by test4
test4 returned value test4 result
VERBOSE: calling test5
message by test5
test5 returned value test5 result
VERBOSE: piping test2
test2 produced value: message by test2
test2 produced value: test2 result
VERBOSE: calling test2
test2 returned value lines:  2
will use console colors

powershell.exe -executionpolicy bypass -file "stderr_ex.ps1" > a.log

message by test4

type a.log

VERBOSE: calling test1
VERBOSE: message by test1
test1 returned value test1 result
VERBOSE: calling test2
test2 returned value message by test2
test2 returned value message by test2,test2 result
VERBOSE: calling test3
test3 : message by test3
At C:\developer\sergueik\powershell_samples\utils\stderr_ex.ps1:64 char:12
+ $result3 = test3
+            ~~~~~
    + CategoryInfo          : NotSpecified: (:) [Write-Error], WriteErrorExcep
   tion
    + FullyQualifiedErrorId : Microsoft.PowerShell.Commands.WriteErrorExceptio
   n,test3

test3 returned value test3 result
VERBOSE: calling test4
test4 returned value test4 result
VERBOSE: calling test5
message by test5
test5 returned value test5 result
VERBOSE: piping test2
test2 produced value: message by test2
test2 produced value: test2 result
VERBOSE: calling test2
test2 returned value lines:  2

powershell.exe -executionpolicy bypass -file "stderr_ex.ps1" > a.log 2>&1
nothing printed to console

type a.log

VERBOSE: calling test1
VERBOSE: message by test1
test1 returned value test1 result
VERBOSE: calling test2
test2 returned value message by test2
test2 returned value message by test2,test2 result
VERBOSE: calling test3
test3 : message by test3
At C:\developer\sergueik\powershell_samples\utils\stderr_ex.ps1:64 char:12
+ $result3 = test3
+            ~~~~~
    + CategoryInfo          : NotSpecified: (:) [Write-Error], WriteErrorExcep
   tion
    + FullyQualifiedErrorId : Microsoft.PowerShell.Commands.WriteErrorExceptio
   n,test3

test3 returned value test3 result
VERBOSE: calling test4
message by test4
test4 returned value test4 result
VERBOSE: calling test5
message by test5
test5 returned value test5 result
VERBOSE: piping test2
test2 produced value: message by test2
test2 produced value: test2 result
VERBOSE: calling test2
test2 returned value lines:  2

#>


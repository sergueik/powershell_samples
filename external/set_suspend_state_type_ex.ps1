# based on:
# https://www.cyberforum.ru/powershell/thread2882812.html
# https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.application?view=netframework-4.5
# https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.application.setsuspendstate?view=netframework-4.5
# https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.powerstate?view=netframework-4.5

# the cryptic looking command
# [Reflection.Assembly]::LoadWithPartialName('System.Windows.Forms').GetType('System.Windows.Forms.Application')::SetSuspendState(0,$false,$false)
# is simply a cryptic way of calling (probably a tiny amount less typing)
<#
[System.Reflection.Assembly]::LoadWithPartialName('System.Windows.Forms') | out-null
[System.Windows.Forms.Application]::SetSuspendState([System.Windows.Forms.PowerState]::Suspend, $false, $false)
#>

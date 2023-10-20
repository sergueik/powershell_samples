# origin: https://www.cyberforum.ru/powershell/thread2795287.html
[reflection.assembly]::loadwithpartialname('System.Windows.Forms') | out-null
[reflection.assembly]::loadwithpartialname('System.Drawing') | out-null
$iconpath = (resolve-path '.').path + '\' + 'folder.ico'
[system.drawing.icon] $Icon = new-object System.Drawing.Icon($iconPath)

$notify=new-object system.windows.forms.notifyicon
$notify.icon=[System.Drawing.SystemIcons]::Information
$notify.icon=$icon
$notify.visible=$true
# Cannot convert argument "tipIcon", with value: "(Icon)", for "ShowBalloonTip" to type "System.Windows.Forms.ToolTipIcon"
$notify.showballoontip(10,'Defender','Need reboot',<# $icon #> [system.windows.forms.tooltipicon]::Warning)
$notify = $null
# }
# https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.notifyicon
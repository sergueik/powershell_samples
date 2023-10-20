# origin: https://www.cyberforum.ru/powershell/thread2795287.html
[reflection.assembly]::loadwithpartialname('System.Windows.Forms') | out-null
[reflection.assembly]::loadwithpartialname('System.Drawing') | out-null
$notify=new-object system.windows.forms.notifyicon
$notify.visible=$true
$result = $notify.showballoontip(10,'Defender','Need reboot',[system.windows.forms.tooltipicon]::Warning)

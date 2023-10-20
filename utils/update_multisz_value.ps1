# used in: http://forum.oszone.net/thread-352734.html
# NOTE:

# for reading and especially writing MULTI_SZ into Registry use one of the orphan Windows NT Service registry entries under the 
# "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services"
# e.g. 
# "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\WsAppService"
# one of the forcibly uninstalled rogue Application 
# a.k.a. "Wondershare Application Framework Service"
# for remove Wondershare steps see
# https://h30434.www3.hp.com/t5/Notebook-Software-and-How-To-Questions/how-to-uninstall-wondershare-app-service/td-p/6225611
#

$a = get-itemproperty -path 'HKLM:\SYSTEM\CurrentControlSet\Services\wscsvc'  -name DependOnService | select-object -expandproperty DependOnService
$a += 'noname'
# convert from [Object[]] to [String[]]
[String[]]$b = [String[]]($a)
$b.getType()

IsPublic IsSerial Name                                     BaseType
-------- -------- ----                                     --------
True     True     String[]                                 System.Array


set-itemproperty -path 'HKLM:\SYSTEM\CurrentControlSet\Services\wscsvc'  -name test -value $b

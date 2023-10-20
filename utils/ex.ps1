# based on: https://www.cyberforum.ru/powershell/thread3124761.html
$h = @{"a"= 1;"b" = 2;"c" = 3; }
$res = New-Object -TypeName PSObject
$h.GetEnumerator() | foreach {
$item = $_;
write-host ('{0}={1}' -f $item.Key, $item.Value);
$res | Add-Member -Name $item.Key -Value $item.Value -MemberType NoteProperty;
}
<#
c=3
b=2
a=1
#>

$res | format-list

<#
c : 3
b : 2
a : 1
#>


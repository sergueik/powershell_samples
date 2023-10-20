# see also: https://www.cyberforum.ru/powershell/thread3118155.html
$e = New-Object -ComObject Excel.Application
$w = $e.Workbooks.open('test.xls')
$i= $w.sheets.item('List1')
$r = $i.range('A:A')

<#
  https://learn.microsoft.com/en-us/office/vba/api/excel.range.find
  https://learn.microsoft.com/en-us/office/vba/api/excel.range.find#parameters
  expression.Find (What, After, LookIn, LookAt, SearchOrder, SearchDirection, MatchCase, MatchByte, SearchFormat)
  NOTE: prone to Type mismatch. (Exception from HRESULT: 0x80020005 (DISP_E_TYPEMISMATCH))
  see also: http://ovinitech.blogspot.com/2014/08/find-exact-text-in-excel-using.html
#>

<#
data in test.xls:
test4
test21
test3
test2 <= will find this one
test212
test4

#>

# https://stackoverflow.com/questions/51660982/do-i-need-to-define-default-values-in-excel-range-find-or-can-i-skip-some-param
# see also: https://stackoverflow.com/questions/5544844/how-to-call-a-complex-com-method-from-powershell
# and
# https://learn.microsoft.com/en-us/dotnet/visual-basic/misc/bc30804
$o = $r.Find('test2', [Type]::Missing, -4163, 1, 1, 1, $false)
$o.text
test2

$o = $null
$r = $null
$i = $null
$w.close()
$w = $null
$e = $null

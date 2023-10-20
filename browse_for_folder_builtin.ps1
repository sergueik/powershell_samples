# origin: https://4sysops.com/archives/how-to-create-an-open-file-folder-dialog-box-with-powershell/

# NOTE: format
$initial_directory = [Environment]::GetFolderPath('Desktop')
$filter_expression = 'SpreadSheet (*.xlsx)|*.xls|Documents (*.docx)|*.docx|All files (*.*)| *.*'
add-type -AssemblyName 'System.Windows.Forms'
$o = new-object System.Windows.Forms.OpenFileDialog -Property @{
  InitialDirectory = $initial_directory
  Filter = $filter_expression
  }
$null = $o.ShowDialog()
$filename = $o | select-object -expandproperty FileName
if ($filename -ne $null) {
  write-output ('selected filename: {0}' -f $filename)
}
# NOTE: cannot pass the costructor arguments by name
# can set via property assignment
$o = new-object System.Windows.Forms.OpenFileDialog
$o.Title = 'Select a file'
$o.InitialDirectory = $initial_directory
$o.Filter = $filter_expression
if ($o.ShowDialog() -eq 'OK'){
  $filename = $o.FileName
  if ($filename -ne $null) {
    write-output ('selected filename: {0}' -f $filename)
  }
}


$BIF_NONEWFOLDERBUTTON = 0x00000200
$BIF_BROWSEINCLUDEFILES  = 0x00004000

$BIF_NEWDIALOGSTYLE = 0x00000040 
$BIF_EDITBOX = 0x00000010
$VT_PTR = 26;

$s = new-object -com 'Shell.Application'
# Unexpected VarEnum VT_PTR
<# 
OverloadDefinitions
-------------------
Folder BrowseForFolder (int, string, int, Variant)

 #>
$f = $s.BrowseForFolder( [System.Diagnostics.Process]::GetCurrentProcess().MainWindowHandle,  'Choose Folder',   ($BIF_NEWDIALOGSTYLE -bor $BIF_NONEWFOLDERBUTTON),  ( resolve-path -path '.').path )

$file_path = $f.Self.Path
write-output $file_path
# https://csharp.hotexamples.com/examples/-/VarEnum/-/php-varenum-class-examples.html
# https://learn.microsoft.com/en-us/windows/win32/api/wtypes/ne-wtypes-varenum
# https://stackoverflow.com/questions/49323410/com-interop-and-marshaling-of-variantvt-ptr
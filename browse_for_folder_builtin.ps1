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
#
#
# https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.folderbrowserdialog?view=netframework-4.5
#
$o = new-object System.Windows.Forms.FolderBrowserDialog
# Set the help text description for the FolderBrowserDialog.
$o.Description = "Select the directory that you want to use as the default."

# Do not allow the user to create new files via the FolderBrowserDialog.
$o.ShowNewFolderButton = $false

# Set root Folder to the "My Documents" folder


$o.RootFolder = [System.Environment+SpecialFolder]::Personal

# NOTE if the next line is uncommented get exception revealing the needed Powrshell decoration details
# Exception setting "RootFolder":
# Cannot convert null to type "System.Environment+SpecialFolder" due to enumeration values that are not valid.
# Specify one of the following enumeration values and try again.
# The possible enumeration values are
# Desktop, Programs, MyDocuments, Personal, Favorites, Startup, Recent, SendTo, StartMenu, MyMusic, MyVideos, DesktopDirectory, MyComputer, NetworkShortcuts, Fonts, Templates, CommonStartMenu, CommonPrograms, CommonStartup, CommonDesktopDirectory, ApplicationData, PrinterShortcuts, LocalApplicationData, InternetCache, Cookies, History, CommonApplicationData, Windows, System, ProgramFiles, MyPictures, UserProfile, SystemX86, ProgramFilesX86, CommonProgramFiles, CommonProgramFilesX86, CommonTemplates, CommonDocuments, CommonAdminTools, AdminTools, CommonMusic, CommonPictures, CommonVideos, Resources, LocalizedResources, CommonOemLinks, CDBurning
# $o.RootFolder = 123445678889

# $o.RootFolder = 5
# https://learn.microsoft.com/en-us/dotnet/api/system.environment.specialfolder?view=netframework-4.5
# Personal	5 The directory that serves as a common repository for documents. This member is equivalent to MyDocuments.

[System.Windows.Forms.DialogResult] $result = $o.ShowDialog()
if ( $result -eq [System.Windows.Forms.DialogResult]::OK ) {
  $folderName = $o.SelectedPath
  write-host ('Selected folder name: {0}' -f $folderName)
}

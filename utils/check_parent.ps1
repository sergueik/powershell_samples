$shellApplication = new-object -com 'Shell.Application'
$windows = $shellApplication.windows()
write-output ('Found {0} windows' -f $windows.Count)
$cnt = 0
$windows | foreach-object { 
  write-host ('Windows {0}' -f $cnt)
  $cnt ++
  $window = $_
  write-output ('Window: ') 
  write-output $window | 
  select-object -property Name,HWND,Application,LocationName | 
  format-list
  write-output ('Parent: ') 
  $parent = $window.parent
  write-output $parent | 
  select-object -property Name,HWND,Application,LocatioName|
  format-list
  
  write-output ('Parent(2): ') 
  $parent2 = $parent.parent
  write-output $parent2 | 
  select-object -property Name,HWND,Application,LocatioName|
  format-list
}
<#
# does not get valuable information about the parent:
Found 1 windows
Windows 0
Window:

Name         : File Explorer
HWND         : 329002
Application  : System.__ComObject
LocationName : explorer_dialog_automator

Parent:
Name        : File Explorer
HWND        : 329002
Application : System.__ComObject
LocatioName :



Parent(2):


Name        : File Explorer
HWND        : 329002
Application : System.__ComObject
LocatioName :

#>

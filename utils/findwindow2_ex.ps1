# origin: https://stackoverflow.com/questions/25780138/how-to-find-a-desktop-window-by-window-name-in-windows-8-1-update-2-os-using
param (
  [string]$AppName = 'Documents',
  [switch]$launch = $false,
  [int]$x = 0, # 1350 for the desktop #2
  [int]$y = 0,
  [switch]$debug
)


$o = Add-Type -name dummy -PassThru  -MemberDefinition @'
  [DllImport("user32.dll", CharSet = CharSet.Unicode)]
  public static extern IntPtr FindWindow(IntPtr sClassName, String sAppName);
  [DllImport("kernel32.dll")]
  public static extern uint GetLastError();
'@
if ($launch) {
  $proc = [System.Diagnostics.Process]::Start("explorer.exe", "/separate, ${env:userprofile}\Documents")
  start-sleep -second 4
}



$result = $o::FindWindow([IntPtr]::Zero, $AppName ) 
$a = $o::GetLastError()
if ($a -ne 0){
  write-host ('Error: {0}' -f $a)
	# NOTE:
        # run screen_selector.ps1 in a separate process. it will  create a simple form with title 'Choose Screen'
        # try to find its HWND results in
	# Error: 203
	# The system could not find the environment option that was entered.
        # the HWND is still determined
} else {
  write-host ('handle: {0}' -f $result)
}

# NOTE: trick does not work with Me	mberDefinition

if (-not ('WinAPI' -as [type])) {
$WinApi = Add-Type -Name 'WinApi' -PassThru -MemberDefinition @'
  [DllImport("user32.dll", CharSet = CharSet.Auto)]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool repaint);  
'@
}
# TODO: measure the form Rectangle
$width = 400
$height = 170
$WinApi::MoveWindow($result, $x, $y, $width, $height, $true)
<#
$shell = New-Object -ComObject Shell.Application
$Windows = $shell.Windows()
write-host  ('inspecting {0} windows' -f $Windows.Count)
0..($Windows.Count -1) | foreach-object {
  $count = $_
  $window =  $Windows.item($count)
  write-output $window | select-object -property LocationName,HWND

  # $host.ui.RawUI.ReadKey(6).ch
}
#>	
<#
$proc = [System.Diagnostics.Process]::Start("explorer.exe", "/separate, $env:SystemDrive")
$shell = New-Object -ComObject Shell.Application
$shell.Windows() | select HWND,LocationName,LocationUrl,@{n='Path'; e={$_.Document.Folder.Self.Path}}
$shell = New-Object -ComObject Shell.Application
$Windows = $shell.Windows(); $Count = $Windows.Count
$shell.Open($env:SystemDrive)
while($Windows.Count -eq $Count){}
$hwnd = $Windows.item($Windows.Count - 1).HWND
#>

<#

get-wmiobject -class 'win32_process' -filter "parentprocessid=$PID" | select-object -property Processid
# will not be able to find explorer process this way
#>
<#
(Start explorer $env:SystemDrive -PassThru).WaitForExit()
$myExplorer = ps explorer|Sort StartTime|Select -Last 1
while(![Int64]$myExplorer.MainWindowHandle){
  start-sleep -millisecond 500
}
$myExplorer.MainWindowHandle

#>
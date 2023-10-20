# origin https://social.technet.microsoft.com/Forums/windowsserver/en-US/2f606c18-4fc9-4ba9-bc55-77173d42c058/using-lanmanserver-with-powershell?forum=winserverpowershell
# http://etutorials.org/Server+Administration/Active+directory/Part+III+Scripting+Active+Directory+with+ADSI+ADO+and+WMI/Chapter+22.+Manipulating+Persistent+and+Dynamic+Objects/22.3+Enumerating+Sessions+and+Resources/ 
$computer = $env:computername
$collection = @()
$netfile = [adsi]"WinNT://${computer}/LanmanServer"
$netfile.Invoke('Resources') | ForEach-Object {
  try {
    $collection += New-Object PsObject -Property @{
      Id = $_.GetType().InvokeMember('Name','GetProperty',$null,$_,$null)
      itemPath = $_.GetType().InvokeMember('Path','GetProperty',$null,$_,$null)
      UserName = $_.GetType().InvokeMember('User','GetProperty',$null,$_,$null)
      LockCount = $_.GetType().InvokeMember('LockCount','GetProperty',$null,$_,$null)
      Server = $computer
    }
  }
  catch {
    Write-Warning $error[0]
  }
}


$collection

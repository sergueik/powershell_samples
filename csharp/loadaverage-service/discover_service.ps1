$servicename = 'LoadAverageCounterService' ; 

sc.exe query $servicename | out-null

# NOTE: silentlycontinue wors for this commandlet
# $o =  get-service -name ($servicename +'a') -erroraction silentlycontinue
$o =  get-service -name $servicename -erroraction silentlycontinue
if ($o -ne $null) {
  $o =  sc.exe qc $servicename
  $o
  $o =  sc.exe qc $servicename | select-string -pattern 'BINARY_PATH_NAME' | select-object -first 1 | convertfrom-string
  $pathname = ($o.'P4') -replace '"' , ''
  write-host ('BINARY_PATH_NAME: {0}' -f $pathname)
  $executable = 'LoadAverageCounterService.exe'
  $configuration = $pathname -replace '^.*\\Program\\bin\\(.*)\\LoadAverageCounterService.exe', '$1'
  write-host ('Configuration: {0}' -f $configuration )
  <#
  [SC] QueryServiceConfig SUCCESS

  SERVICE_NAME: LoadAverageCounterService
          TYPE               : 10  WIN32_OWN_PROCESS
          START_TYPE         : 4   DISABLED
          ERROR_CONTROL      : 1   NORMAL
          BINARY_PATH_NAME   : "C:\developer\sergueik\powershell_samples\csharp\loadaverage-service\Program\bin\Release\LoadAverageCounterService.exe"
          LOAD_ORDER_GROUP   :
          TAG                : 0
          DISPLAY_NAME       : LoadAverageCounterService
          DEPENDENCIES       :
          SERVICE_START_NAME : LocalSystem
  #>
  get-service -name $servicename -erroraction silentlycontinue | 
  select-object -property * | 
  format-list

  write-host 'Query WMI (slow)'
  # https://stackoverflow.com/questions/24449113/how-can-i-extract-path-to-executable-of-all-services-with-powershell

  $o = (get-wmiobject -Query `
  "Select * from win32_service where Name like `"%${servicename}%`"") | 
  select-object Name, DisplayName, State, Pathname | select-object -expandproperty pathname
  # NOTE: format-custom does not help diplay the pathname:
  <# $o | format-custom -depth 1024

  still breaks the line in the display
  class ManagementObject
  {
    Name = LoadAverageCounterService
    DisplayName = LoadAverageCounterService
    State = Running
    Pathname = "C:\developer\sergueik\powershell_samples\csharp\loadaverage-servi
    ce\Program\bin\Release\LoadAverageCounterService.exe"
  }

  #> 
  # no information  when service is stopped
  $pathname = $o -replace '"',''
  write-host ('Pathname: {0}' -f $pathname)
  $executable = 'LoadAverageCounterService.exe'
  $configuration = $pathname -replace '^.*\\Program\\bin\\(.*)\\LoadAverageCounterService.exe', '$1'
  write-host ('Configuration: {0}' -f $configuration )

}
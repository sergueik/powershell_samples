if (($MyInvocation.InvocationName -ne $null) -and ($MyInvocation.InvocationName.length -ne 0 )) { 
  [System.IO.FileInfo] $fileinfo = new-object System.IO.FileInfo($MyInvocation.InvocationName)
  [string] $script_path = $fileinfo.Directory.FullName
   # alternatively 
   # $script_path = ($MyInvocation.InvocationName) -replace '\\[^\\]*$', ''
   write-host ('script_path: {0}' -f $script_path)

}
# TODO: else fail with exception
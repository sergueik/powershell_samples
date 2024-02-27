#Copyright (c) 2021,2022 Serguei Kouzmine
#
#Permission is hereby granted, free of charge, to any person obtaining a copy
#of this software and associated documentation files (the "Software"), to deal
#in the Software without restriction, including without limitation the rights
#to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
#copies of the Software, and to permit persons to whom the Software is
#furnished to do so, subject to the following conditions:
#
#The above copyright notice and this permission notice shall be included in
#all copies or substantial portions of the Software.
#
#THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
#IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
#FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
#AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
#LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
#OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
#THE SOFTWARE.

# Microsoft OpenSSH Server
# https://docs.microsoft.com/en-us/windows-server/administration/openssh/openssh_server_configuration
# https://winscp.net/eng/docs/guide_windows_openssh_server
# https://tunnelix.com/installing-openssh-on-windows-2012-r2-through-powershell/
# creates the environment variables SSH_CLIENT and SSH_CONNECTION in the client session
# which presence can be used for making the administration scripts switch between GUI and console for parameter input
param(
  [string]$user = 'dummy',
  [switch]$ssh,
  [switch]$debug
)

function private:ConvertTo-RegularString {
  param(
    [Security.SecureString]$securestring
  ) 
  return [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($securestring))
}
if ( [bool]$psboundparameters['ssh'].ispresent ) {
  $env_check = $true
} else {
  $env_name = 'SSH_CLIENT'
  $save_pwd = $pwd
  if ($debug) {
    write-host ('checking value of $env:{0}' -f $env_name)
  }
  chdir 'env:'
  $env_check = ((dir $env_name -erroraction silentlycontinue) -ne $null)
  cd $save_pwd
}
if ($debug) {
  write-host (' $env_check=  {0}'  -f $env_check )
}
$message = (' Enter password for user {0}' -f $user)
if ($env_check -eq $true ) {
  $securestring_password = read-host $message -assecurestring
  $plaintext_password = convertTo-RegularString $securestring_password
  $credential = new-object -TypeName System.Management.Automation.PSCredential -ArgumentList $User,  $securestring_password
  $plaintext_password2 = $credential.GetNetworkCredential().Password
} else {
  $credential = get-credential -username $user -message $message
  $plaintext_password = convertTo-RegularString $credential.password
  $plaintext_password2 = $credential.GetNetworkCredential().Password
}

write-host $plaintext_password
write-host $plaintext_password2
$env_check = $false
# clear leftover global var

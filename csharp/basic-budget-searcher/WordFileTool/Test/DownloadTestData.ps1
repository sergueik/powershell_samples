# download script is pure shell around the stock `invoke-webrequest` and created to illustrate bad practices
param (
  [string]$Url,
  [string]$OutFile
)
Invoke-WebRequest -Uri $Url -OutFile $OutFile
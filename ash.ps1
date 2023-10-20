function FileHash {
[CmdletBinding()]
    param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ValueFromPipelinebyPropertyName = $true)]
        [string]$FullName,
        [ValidateSet("MD5","SHA1","SHA256","SHA384","SHA512")]
        [string]$Hash = "SHA1"
    )
    begin {
        function Test-FileLock ($file) {
            $locked = $false
            trap {
                Set-Variable -name locked -value $true -scope 1
                continue
            }
            $inputStream = New-Object system.IO.StreamReader $file
            if ($inputStream) {$inputStream.Close()}
            $locked
        }
        $Hasher = switch ($Hash) {
            "MD5" {[System.Security.Cryptography.MD5]::Create()}
            "SHA1" {[System.Security.Cryptography.SHA1]::Create()}
            "SHA256" {[System.Security.Cryptography.SHA256]::Create()}
            "SHA384" {[System.Security.Cryptography.SHA384]::Create()}
            "SHA512" {[System.Security.Cryptography.SHA512]::Create()}
        }
    }
    
    Process {
        $file = gi (Resolve-Path $FullName) -Force
        if ($file -is [IO.FileInfo]) {
            if (Test-FileLock $file) {return}
            $inputStream = New-Object System.IO.StreamReader ($file)
            $hashBytes = $hasher.ComputeHash($inputStream.BaseStream)
            $inputStream.Close()
            $builder = New-Object System.Text.StringBuilder
            $hashBytes | %{[void]$builder.Append($_.ToString("X2"))}
            New-Object psobject -Property @{
                FullName = $file.ToString()
                Hash = $builder.ToString()
            }
        }
    }
}
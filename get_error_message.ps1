function Get-HRESULTMessage {
    param(
        [Parameter(Mandatory=$true)]
        [int]$hresult
    )

    try {
        $exception = [System.Runtime.InteropServices.Marshal]::GetExceptionForHR($hresult)
        return $exception.Message
    } catch {
        return "Unknown HRESULT: $hresult"
    }
}
# origin: https://www.cyberforum.ru/powershell/thread3222520.html
[CmdletBinding(DefaultParameterSetName = 'Toggle')]
param (
    [Parameter(ParameterSetName = 'Single')] [switch] $SingleClick,
    [Parameter(ParameterSetName = 'Double')] [switch] $DoubleClick
)
 
$HWND_BROADCAST        = [IntPtr] 0xFFFF
$WM_SETTINGCHANGE      = 0x001A
$SMTO_ABORTIFHUNG      = 0x0002
$API_TIMEOUT_MS        = 1500
$SHELLSTATE_CLICK_FLAG = 0x20
$REG_SYNC_DELAY_MS     = 200
$CLICK_MODE_SINGLE     = 0x1E
$CLICK_MODE_DOUBLE     = 0x3E
 
$path = 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer'
$state = @()
$stateIndex = 4
 
try {
    $GetItemPropertyParams = @{
        Path        = $path
        Name        = 'ShellState'
        ErrorAction = 'Stop'
    }
 
    $state = Get-ItemPropertyValue @GetItemPropertyParams
} catch {
    $errmsg = $_.Exception.Message
    Write-Warning "failed to read ShellState: $errmsg"
    return
}
 
if ($state.Count -lt $stateIndex) {
    Write-Warning "the capacity or ${path} too low : ShellState needs ${stateIndex} entries"
    return
}
 
if ($SingleClick -and $state[$stateIndex] -eq $CLICK_MODE_SINGLE) {
    Write-Information "already set to single click"
    return
}
 
if ($DoubleClick -and $state[$stateIndex] -eq $CLICK_MODE_DOUBLE) {
    Write-Information "already set to double click"
    return
}
 
$MemberDefinition = @'
    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern IntPtr SendMessageTimeoutW(
        IntPtr hWnd,
        uint Msg,
        UIntPtr wParam,
        IntPtr lParam,
        uint fuFlags,
        uint uTimeout,
        out UIntPtr lpdwResult);
'@
 
if (-not ([Management.Automation.PSTypeName]'Win32.User32').Type) {
    $AddTypeParams = @{
        MemberDefinition = $MemberDefinition
        Namespace        = 'Win32'
        Name             = 'User32'
        ErrorAction      = 'Stop'
    }
 
    try {
        [void] (Add-Type @AddTypeParams)
    } catch {
        $errmsg = $_.Exception.Message
        Write-Warning "error compiling WinAPI: $errmsg"
        return
    }
}
 
try {
    $state[$stateIndex] = $state[$stateIndex] -bxor $SHELLSTATE_CLICK_FLAG
 
    $SetItemPropertyParams = @{
        Path        = $path
        Name        = 'ShellState'
        Value       = $state
        Type        = 'Binary'
        Force       = $true
        ErrorAction = 'Stop'
    }
 
    Set-ItemProperty @SetItemPropertyParams
} catch {
    $errmsg = $_.Exception.Message
    Write-Warning "failed to save item propeties: $errmsg"
    return
}
 
$lParam = [Runtime.InteropServices.Marshal]::StringToHGlobalUni('ShellState')
$res = [UIntPtr]::Zero
 
try {
    [void] [Win32.User32]::SendMessageTimeoutW(
        $HWND_BROADCAST,
        $WM_SETTINGCHANGE,
        [UIntPtr]::Zero,
        $lParam,
        $SMTO_ABORTIFHUNG,
        $API_TIMEOUT_MS,
        [ref] $res)
} finally {
    [void] [Runtime.InteropServices.Marshal]::FreeHGlobal($lParam)
}
 
Start-Sleep -Milliseconds $REG_SYNC_DELAY_MS
 
$shell = New-Object -ComObject Shell.Application
$windows = $null
 
try {
    $windows = $shell.Windows()
    
    foreach ($win in $windows) {
        try {
            if ($win.FullName -like '*explorer.exe' -and $win.LocationURL) {
                $win.Navigate($win.LocationURL)
            }
        } catch {
            # ignore
        }
    }
} catch {
    $errmsg = $_.Exception.Message
    Write-Warning "failed to find window list: $errmsg"
    return
} finally {
    if ($shell) {
        [void] [Runtime.InteropServices.Marshal]::ReleaseComObject($shell)
    }
}

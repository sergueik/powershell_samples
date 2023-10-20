# based on: https://www.cyberforum.ru/vbfavorites.php?do=add&entry_type=post&entry_id=13707523
# see also: https://www.cyberforum.ru/powershell/thread2603020.html

Add-Type -AssemblyName PresentationCore,PresentationFramework
[xml]$xaml = @'
<?xml version="1.0"?>
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Name="main" Title="MainWindow" Height="250.456" Width="223.212">
  <Grid Margin="0,0,2,0">
    <Label HorizontalAlignment="Left" Margin="10,10,0,0" VerticalAlignment="Top" RenderTransformOrigin="-1.028,-1.353" Width="186" Height="38">
      <Grid Height="38" Margin="0,0,0,0" Width="176">
        <TextBlock HorizontalAlignment="Left" TextWrapping="Wrap" Text="Launcher" VerticalAlignment="Top" Height="28" Width="97" FontSize="18" FontFamily="Segoe Script" Margin="10,0,0,0"/>
        <TextBlock Name="Count" Text="" HorizontalAlignment="Right" TextWrapping="Wrap" VerticalAlignment="Top" Height="28" Width="48" FontSize="18" FontFamily="Segoe Script"/>
      </Grid>
    </Label>
    <Button Name="inc" Content="Run App" HorizontalAlignment="Left" Margin="10,53,0,0" VerticalAlignment="Top" Width="180" Height="45">
        </Button>
    <ProgressBar Minimum="0" Maximum="100" Margin="10,93,0,0" HorizontalAlignment="Left" Width="180" Height="45" Name="pbStatus"/>
  </Grid>
</Window>
'@
$reader = (new-object System.Xml.XmlNodeReader $xaml)
$Window = [Windows.Markup.XamlReader]::Load( $reader )
$xaml.SelectNodes("//*[@*[contains(translate(name(.),'n','N'),'Name')]]") | foreach-object {
    Set-Variable -Name ($_.Name) -Value $Window.FindName($_.Name) -Scope Global
}
$inc.add_click({
  $pbStatus.isIndeterminate=$true
  $runspace = [runspacefactory]::CreateRunspace()
  $runspace.Open()
  $runspace.SessionStateProxy.SetVariable('pbStatus',$pbStatus)
  $powershell = [powershell]::Create()
  $powershell.Runspace = $runspace
  $powershell.AddScript({
  sleep -Seconds 5
    $pbStatus.Dispatcher.invoke([action]{$pbStatus.isIndeterminate=$false})
        $pbStatus.Dispatcher.invoke([action]{$pbStatus.value=100})
    })
    $powershell.BeginInvoke()
})
$Window.showdialog()

<#
##  Script Exit Code
    [int32]$exitCode = 0

## Set the script execution policy for this process
Try { Set-ExecutionPolicy -ExecutionPolicy 'ByPass' -Scope 'Process' -Force -ErrorAction 'Stop' } Catch {}

If (Test-Path -LiteralPath 'variable:HostInvocation') { $InvocationInfo = $HostInvocation } Else { $InvocationInfo = $MyInvocation }
[string]$scriptDirectory = Split-Path -Path $InvocationInfo.MyCommand.Definition -Parent

[string]$modulePath = "$scriptDirectory\Modules"

## Dot source the required Functions

Try {
    ."$modulePath\Second.ps1" #Import Base module
}
Catch {
    ## Exit the script, returning the exit code
    [System.Windows.Forms.Messagebox]::Show([String]'')
    If (Test-Path -LiteralPath 'variable:HostInvocation') { $script:ExitCode = $exitCode; Exit } Else { Exit $exitCode }
}

#Log Vendor and model

[void][System.Reflection.Assembly]::LoadWithPartialName('presentationframework')
[xml]$XAML = '<Window
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Main form" Height="180" Width="280" ResizeMode="NoResize">
    <Grid>
        <Label Name="Label1" Content="Press button" HorizontalAlignment="Left" Margin="10,0,0,0" VerticalAlignment="Top" Height="25" Width="200" FontSize="12"/>
        <ProgressBar Name="ProgressBar" HorizontalAlignment="Left" Margin="10,30,0,0" VerticalAlignment="Top" Height="30" Width="250" IsIndeterminate="True" Visibility="Collapsed" />
        <Button Name="FirstButton" Content="Install" HorizontalAlignment="Left" Margin="20,90,0,0" VerticalAlignment="Top" Width="100" Height="50" FontSize="12" />
        <Button Name="SecondButton" Content="Collect" HorizontalAlignment="Left" Margin="150,90,0,0" VerticalAlignment="Top" Width="100" Height="50" FontSize="12"/>
    </Grid>
    </Window>'

try { Add-Type -AssemblyName PresentationFramework,System.Windows.Forms}
catch { throw "Failed to load Windows Presentation Framework assemblies." }

#Read XAML
$reader=(New-Object System.Xml.XmlNodeReader $xaml)
try{$Form=[Windows.Markup.XamlReader]::Load( $reader )}
catch{Write-Host "Unable to load Windows.Markup.XamlReader. Some possible causes for this problem include: .NET Framework is missing PowerShell must be launched with PowerShell -sta, invalid XAML code was encountered."; exit}

#$script:form = [Windows.Markup.XamlReader]::Load((New-Object System.Xml.XmlNodeReader $xaml))

#===========================================================================
# Store Form Objects In PowerShell
#===========================================================================
$xaml.SelectNodes("//*[@Name]") | ForEach-Object {Set-Variable -Name ($_.Name) -Value $Form.FindName($_.Name) -Scope Global}

$Global:syncHash = [hashtable]::Synchronized(@{})

    $processRunspace =[runspacefactory]::CreateRunspace()
    $processRunspace.ApartmentState = "STA"
    $processRunspace.ThreadOptions = "ReuseThread"
    $processRunspace.Open()
    $processRunspace.SessionStateProxy.SetVariable("syncHash",$syncHash)
    $syncHash.ScriptDirectory = $scriptDirectory
    $syncHash.FirstButton = $FirstButton
    $syncHash.SecondButton= $SecondButton
    $syncHash.Progressbar = $Progressbar

    $psCmd = [PowerShell]::Create().AddScript({

    # $SyncHash.GUIThread
    $Global:scriptDirectory = Split-Path -Path $InvocationInfo.MyCommand.Definition -Parent
    $Global:FirstButton = $syncHash.FirstButton
    $Global:SecondButton= $syncHash.SecondButton
    $Global:Progbar = $syncHash.Progressbar


    [string]$modulePath = "$($syncHash.ScriptDirectory)\Modules"
    ."$modulePath\Second-Module.ps1" #Import Base module
   #Call function from Second.ps1 module
   InstallConfiguration
})

$FirstButton.Add_Click({
    $ProgressBar.Visibility="Visible"
    $FirstButton.IsEnabled = $False
    $SecondButton.IsEnabled = $False
    $psCmd.Runspace = $processRunspace
    $data = $psCmd.BeginInvoke()
})

$form.ShowDialog() | Out-Null

#>

<#
# proposed invoker
[void][System.Reflection.Assembly]::LoadWithPartialName('presentationframework')
 
try { Add-Type -AssemblyName PresentationFramework,System.Windows.Forms} 
catch { throw "Failed to load Windows Presentation Framework assemblies." }
 
$scriptDirectory = Split-Path -parent $modulePath
 
function InstallConfiguration
{
#Some steps for configuring 
$FirstButton.IsEnabled = $true
$SecondButton.IsEnabled = $true
$ProgressBar.Visibility="Collapsed"
}
#>

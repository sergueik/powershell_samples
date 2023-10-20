# origin: http://www.cyberforum.ru/powershell/thread2480090.html
# Windows Platform support ?

Add-Type -AssemblyName PresentationCore,PresentationFramework
[xml]$xaml = @'
<Window Name="main"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="MainWindow" Height="250.456" Width="223.212">
    <Grid Margin="0,0,2,0">
        <Label HorizontalAlignment="Left" Margin="10,10,0,0" VerticalAlignment="Top" RenderTransformOrigin="-1.028,-1.353" Width="186" Height="38">
            <Grid Height="38" Margin="0,0,0,0" Width="176">
                <TextBlock HorizontalAlignment="Left"  TextWrapping="Wrap" Text="Launch script" VerticalAlignment="Top" Height="28" Width="97" FontSize="18" FontFamily="Segoe Script" Margin="10,0,0,0"/>
                <TextBlock Name="Count" Text="" HorizontalAlignment="Right"  TextWrapping="Wrap" VerticalAlignment="Top" Height="28" Width="48" FontSize="18" FontFamily="Segoe Script"/>
            </Grid>
        </Label>
        <Button Name="inc" Content="Script runner with progress"  HorizontalAlignment="Left" Margin="10,53,0,0" VerticalAlignment="Top" Width="180" Height="45">          
        </Button>
        <ProgressBar Minimum="0" Maximum="100" Margin="10,93,0,0" HorizontalAlignment="Left" Width="180" Height="45" Name="pbStatus" />
    </Grid> 
</Window>
'@
$reader=(New-Object System.Xml.XmlNodeReader $xaml)
$Window=[Windows.Markup.XamlReader]::Load( $reader )
$xaml.SelectNodes("//*[@*[contains(translate(name(.),'n','N'),'Name')]]") | %{
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
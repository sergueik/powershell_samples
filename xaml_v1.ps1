# origin: http://www.cyberforum.ru/powershell/thread2353875.html
# see also: https://learn-powershell.net/2012/09/13/powershell-and-wpf-introduction-and-building-your-first-window/
[xml]$xaml = @'
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Name="main" Title="MainWindow" Height="156.456" Width="223.212">
  <Grid Margin="0,0,2,0">
    <Label HorizontalAlignment="Left" Margin="10,10,0,0" VerticalAlignment="Top" Width="186" Height="38">
      <Grid Height="38" Margin="0,0,0,0" Width="176">
        <TextBlock HorizontalAlignment="Left" TextWrapping="Wrap" Text="Value" VerticalAlignment="Top" Height="28" Width="97"/>
        <TextBlock Name="Count" Text="0" HorizontalAlignment="Right" TextWrapping="Wrap" VerticalAlignment="Top" Height="28"/>
      </Grid>
    </Label>
    <Button Name="inc" Content="Increment" HorizontalAlignment="Left" Margin="10,53,0,0" VerticalAlignment="Top" Width="186" Height="45">
        </Button>
  </Grid>
</Window>
'@
add-type -AssemblyName PresentationFramework
# obseverd occasionally leading to a crash
$window=[Windows.Markup.XamlReader]::Load((new-object System.Xml.XmlNodeReader $xaml) )
# illustrates some xpath manip
$xaml.SelectNodes("//*[@*[contains(translate(name(.),'n','N'),'Name')]]") | foreach-object{
  set-variable -Name ($_.Name) -Value $window.FindName($_.Name) -Scope Global
}
$inc.add_click({
  $count.text=([int]$Count.text)+1
})
[void]$window.ShowDialog()
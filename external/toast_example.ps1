# based on: http://www.cyberforum.ru/powershell/thread2478736.html
# where it is not working
$app = '{1AC14E77-02E7-4E5D-B744-2EB1AE5198B7}\WindowsPowerShell\v1.0\powershell.exe'
[Windows.UI.Notifications.ToastNotificationManager, Windows.UI.Notifications, ContentType = WindowsRuntime]
$Template = [Windows.UI.Notifications.ToastTemplateType]::ToastImageAndText04
[xml]$ToastTemplate = ([Windows.UI.Notifications.ToastNotificationManager]::GetTemplateContent($Template).GetXml())
[xml]$ToastTemplate = @'
<toast scenario="reminder" launch="app-defined-string">
  <visual>
    <binding template="ToastGeneric">
      <text>Downloading...</text>
      <image placement="hero" src="E:\303336.png"/>
    </binding>
  </visual>
  <actions>
  </actions>
</toast>
'@
$ToastXml = New-Object -TypeName Windows.Data.Xml.Dom.XmlDocument
$ToastXml.LoadXml($ToastTemplate.OuterXml)
$notify = [Windows.UI.Notifications.ToastNotificationManager]::CreateToastNotifier($app)
$notify.Show($ToastXml)

# https://answers.microsoft.com/en-us/windows/forum/all/how-to-create-a-task-to-shutdown-pc-when-ac-power/c65fbcd8-83c9-49ac-a2d8-533d0dced8d2
# https://www.reddit.com/r/Surface/comments/6coxgp/how_to_run_a_task_when_plugged_into_ac_or_losing/
# https://www.zdnet.com/article/windows-10-telemetry-secrets/
# https://superuser.com/questions/389689/triggering-tasks-and-running-programs-when-windows-7-is-plugged-in-or-unplugged
# example event (NOTE: Windows 10 Russian)
<# 
Имя журнала:   Microsoft-Windows-UniversalTelemetryClient/Operational
Источник:      Microsoft-Windows-UniversalTelemetryClient
Дата:          26.10.2022 21:03:25
Код события:   60
Категория задачи:(60)
Уровень:       Сведения
Ключевые слова:(262144)
Пользователь:  СИСТЕМА
Компьютер:     DESKTOP-82V9KDO
Описание:
Устройство работает от аккумулятора: true
Xml события:
<Event xmlns="http://schemas.microsoft.com/win/2004/08/events/event">
  <System>
    <Provider Name="Microsoft-Windows-UniversalTelemetryClient" Guid="{6489b27f-7c43-5886-1d00-0a61bb2a375b}" />
    <EventID>60</EventID>
    <Version>0</Version>
    <Level>4</Level>
    <Task>60</Task>
    <Opcode>0</Opcode>
    <Keywords>0x8000000000040000</Keywords>
    <TimeCreated SystemTime="2022-10-26T18:03:25.1130676Z" />
    <EventRecordID>260</EventRecordID>
    <Correlation />
    <Execution ProcessID="2324" ThreadID="3028" />
    <Channel>Microsoft-Windows-UniversalTelemetryClient/Operational</Channel>
    <Computer>DESKTOP-82V9KDO</Computer>
    <Security UserID="S-1-5-18" />
  </System>
  <EventData>
    <Data Name="Environment">ServiceHost</Data>
    <Data Name="State">true</Data>
  </EventData>
</Event>
#>
New-EventLog -Source BatteryStatusMonitor -LogName Application

Function OnBatteryStatusChange ($NewStatus) {
  If ($NewStatus -eq 1) {
    $EventID = 5001
    $Message = "The computer was unplugged."
  } ElseIf ($NewStatus -eq 2) {
    $EventID = 5002
    $Message = "The computer was plugged in."
  } Else {
    $EventID = 5000
    $Message = "Battery status changed to $NewStatus"
  }
  Write-EventLog -LogName Application -Source BatteryStatusMonitor -EventID $EventID -Message $Message
}

$Query = "select * from __instancemodificationevent within 3 where targetinstance isa 'win32_battery' and targetinstance.batterystatus <> previousinstance.batterystatus"

Register-WmiEvent -Query $Query -Action {OnBatteryStatusChange $Event.SourceEventArgs.NewEvent.TargetInstance.BatteryStatus} -SourceIdentifier "BatteryStatusChange"

For (;;) {}
# creates the following events:
<#


Log Name:      Application
Source:        BatteryStatusMonitor
Date:          10/26/2022 8:02:57 PM
Event ID:      5001
Task Category: (1)
Level:         Information
Keywords:      Classic
User:          N/A
Computer:      sergueik53
Description:
The computer was unplugged.
Event Xml:
<Event xmlns="http://schemas.microsoft.com/win/2004/08/events/event">
  <System>
    <Provider Name="BatteryStatusMonitor" />
    <EventID Qualifiers="0">5001</EventID>
    <Level>4</Level>
    <Task>1</Task>
    <Keywords>0x80000000000000</Keywords>
    <TimeCreated SystemTime="2022-10-27T00:02:57.000000000Z" />
    <EventRecordID>378645</EventRecordID>
    <Channel>Application</Channel>
    <Computer>sergueik53</Computer>
    <Security />
  </System>
  <EventData>
    <Data>The computer was unplugged.</Data>
  </EventData>
</Event>

#>

<#
Log Name:      Application
Source:        BatteryStatusMonitor
Date:          10/26/2022 8:03:51 PM
Event ID:      5002
Task Category: (1)
Level:         Information
Keywords:      Classic
User:          N/A
Computer:      sergueik53
Description:
The computer was plugged in.
Event Xml:
<Event xmlns="http://schemas.microsoft.com/win/2004/08/events/event">
  <System>
    <Provider Name="BatteryStatusMonitor" />
    <EventID Qualifiers="0">5002</EventID>
    <Level>4</Level>
    <Task>1</Task>
    <Keywords>0x80000000000000</Keywords>
    <TimeCreated SystemTime="2022-10-27T00:03:51.000000000Z" />
    <EventRecordID>378647</EventRecordID>
    <Channel>Application</Channel>
    <Computer>sergueik53</Computer>
    <Security />
  </System>
  <EventData>
    <Data>The computer was plugged in.</Data>
  </EventData>
</Event>

#>

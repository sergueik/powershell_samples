New-task | 
    Add-TaskTrigger -at "5:57 PM" |
    Add-TaskAction -NoExit -Sta -Hidden -Script { 
        Import-Module WPK 
        New-Label "Task Scheduler" -Show
    } |
    Register-ScheduledTask "$(Get-Random)"
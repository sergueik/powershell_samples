function Update-WPFJob
{
    <#
    .Synopsis
        Updates a running WPF Job by running a PowerShell script
    .Description
        Runs a PowerShell script within a WPF Job and returns the results.
        This enables two way communication with WPF Jobs.
        You can use the $Window variable and Get-ChildControl to talk to 
        individual controls within a UI.
    .Example
        $Job = New-Label "Hello World" -AsJob
        $job | Update-WPFJob { $window.Close() }
    #>
    [CmdletBinding(DefaultParameterSetName='NoParameters')]
    param(
    # The Job to update
    [Parameter(Mandatory=$true, ValueFromPipeline=$true)]
    [Management.Automation.Job]
    $Job,
    
    [Parameter(Position=0)]
    [ScriptBlock]
    $Command,
    
    [Parameter(ParameterSetName='IDictionary', Position=1)]
    [Collections.IDictionary]
    $Dictionary,
    
    [Parameter(ParameterSetName='IList', Position=1)]
    [Collections.IList]
    $List,
    
    [switch]$Asynchronously
    )
    
    process {
    
    $realCommand = ([ScriptBlock]::Create(@"
Initialize-EventHandler -resource (Get-Resource `$Window.Content) -parent `$Window.Content
`$ErrorActionPreference = 'stop'

$Command

trap {
    . Write-WPFError `$_
    continue
}
"@))
    
        if ($job.InvokeScriptInJob) {
            switch ($psCmdlet.ParameterSetName) {
                NoParameters {
                    $job.InvokeScriptInJob($realCommand, $null, $Asynchronously)
                }
                List {
                    $job.InvokeScriptInJob($realCommand, $List, $Asynchronously)
                }
                Dictionary {
                    $job.InvokeScriptInJob($realCommand, $Dictionary, $Asynchronously)
                }
            }
        }
    }    
}

. $psScriptRoot\Add-TaskAction.ps1
. $psScriptRoot\Add-TaskTrigger.ps1
. $psScriptRoot\Connect-ToTaskScheduler.ps1
. $psScriptRoot\Get-RunningTask.ps1
. $psScriptRoot\Get-ScheduledTask.ps1
. $psScriptRoot\New-Task.ps1
. $psScriptRoot\Start-Task.ps1
. $psScriptRoot\Stop-Task.ps1
. $psScriptRoot\Register-ScheduledTask.ps1
. $psScriptRoot\Remove-Task.ps1

if ($Host.Name -eq "PowerGUIScriptEditorHost") {
	New-Module {
		$pgSE = [Quest.PowerGUI.SDK.ScriptEditorFactory]::CurrentInstance 

		$cmd = New-Object Quest.PowerGUI.SDK.ItemCommand ("TaskSchedulerCommand", "ScheduleScript") 
		$cmd.ScriptBlock = {
			Import-Module Winformal
			$pgSE = [Quest.PowerGUI.SDK.ScriptEditorFactory]::CurrentInstance 

			$tempToolWindow = $pgSE.ToolWindows.Add((Get-Random))
			$tempToolWindow.Control = New-Panel -Tag $tempToolWindow -Controls {
				New-TabControl -Top 15 -left 15 -TabPages {
					New-TabPage -Text "Once"
					New-TabPage -Text "Daily" 
					New-TabPage -Text "Weekly"
					New-TabPage -Text "Monthly" 					
				}
				New-Button -Top 350 -Left 15 -Text "Click" -On_Click {
					$pgSE = [Quest.PowerGUI.SDK.ScriptEditorFactory]::CurrentInstance 					
					$pgSE.ToolWindows.Remove($this.PArent.Tag )
				}								
			}
			$tempToolWindow.Visible = $true
		} 
		$cmd.Text = 'S&chedule Script'  
		$cmd.AddShortcut('Ctrl+Alt+Shift+S') 
		$pgSE.Commands.Add($cmd) 
		
		$pgSE.Menus["MenuBar.File"].Items.Add($cmd) 
	}		
}

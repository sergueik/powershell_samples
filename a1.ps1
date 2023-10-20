function Get-NetFramework {
    # based on https://raw.githubusercontent.com/lazywinadmin/PowerShell/master/TOOL-Get-NetFramework/Get-NetFramework.ps1
		$netFramework = Get-ChildItem -Path 'HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP' -recurse |
		Get-ItemProperty -name Version -EA 0 |
		Where-Object { $_.PSChildName -match '^(?!S)\p{L}' } |
		Select-Object -Property PSChildName, Version
		$netFramework
		
		# New-Object -TypeName PSObject -Property $Properties
}
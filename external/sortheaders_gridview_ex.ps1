Add-Type -assembly System.Windows.Forms
Add-Type -assembly System.Drawing
$form = New-Object System.Windows.Forms.Form
$form.Text = '?????? ????????????? ??????????'
$form.Size = New-Object System.Drawing.Size(1024,720)
 
$UninstallList1 = & {
    Get-ItemProperty HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*
    Get-ItemProperty HKLM:\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*
} | ? DisplayName | Sort-Object -Property @{ Expression = "DisplayName"; Ascending = $true }|Select-Object @{Name = "???"; Expression = "DisplayName"},
@{Name = "??????"; Expression = "DisplayVersion"},
@{Name = "????????"; Expression = "Publisher"},
@{Name = "???? ?????????"; Expression = {[DateTime]::ParseExact($_.InstallDate, 'yyyyMMdd', $null)}}
 
$list = New-Object System.collections.ArrayList
 
$list.AddRange($UninstallList1)
 
$dataGridView = New-Object System.Windows.Forms.DataGridView -Property @{Size=New-Object System.Drawing.Size(1000,600)} 
 
 
$dataGridView.ColumnHeadersVisible = $true
$dataGridView.AllowUserToResizeColumns = $false
$dataGridView.AllowUserToResizeRows = $false
$dataGridView.DataSource = $list
 
$buttonName1 = New-Object System.Windows.Forms.Button
$buttonName1.Text = '/\'
$buttonName1.Location = New-Object System.Drawing.Point(120,3)
$buttonName1.Size = '20, 20'
$dataGridView.Controls.Add($buttonName1)
 
$buttonName2 = New-Object System.Windows.Forms.Button
$buttonName2.Text = '\/'
$buttonName2.Location = New-Object System.Drawing.Point(120,3)
$buttonName2.Size = '20, 20'
 
$buttonName3 = New-Object System.Windows.Forms.Button
$buttonName3.Text = ' '
$buttonName3.Location = New-Object System.Drawing.Point(120,3)
$buttonName3.Size = '20, 20'
 
$buttonName1.Add_Click({
$list.Clear()
$UninstallList1 = $UninstallList1 | Sort-Object -Property @{ Expression = "???"; Descending = $true }
$dataGridView.Controls.Remove($buttonName1)
 
$dataGridView.Controls.Remove($buttonVersion2)
$dataGridView.Controls.Remove($buttonVersion1)
$dataGridView.Controls.Add($buttonVersion3)
$dataGridView.Controls.Remove($buttonPublisher2)
$dataGridView.Controls.Remove($buttonPublisher1)
$dataGridView.Controls.Add($buttonPublisher3)
$dataGridView.Controls.Remove($buttonDate2)
$dataGridView.Controls.Remove($buttonDate1)
$dataGridView.Controls.Add($buttonDate3)
 
$list.AddRange($UninstallList1)
$dataGridView.DataSource = $list
$dataGridView.Controls.Add($buttonName2)
$form.Refresh()
})
 
$buttonName2.Add_Click({
$list.Clear()
$UninstallList1 = $UninstallList1 | Sort-Object -Property @{ Expression = "???"; Ascending = $true }
$dataGridView.Controls.Remove($buttonName2)
 
$dataGridView.Controls.Remove($buttonVersion2)
$dataGridView.Controls.Remove($buttonVersion1)
$dataGridView.Controls.Add($buttonVersion3)
$dataGridView.Controls.Remove($buttonPublisher2)
$dataGridView.Controls.Remove($buttonPublisher1)
$dataGridView.Controls.Add($buttonPublisher3)
$dataGridView.Controls.Remove($buttonDate2)
$dataGridView.Controls.Remove($buttonDate1)
$dataGridView.Controls.Add($buttonDate3)
 
$list.AddRange($UninstallList1)
$dataGridView.DataSource = $list
$dataGridView.Controls.Add($buttonName1)
$form.Refresh()
})
 
$buttonName3.Add_Click({
$buttonName1.PerformClick()
$dataGridView.Controls.Remove($buttonName3)
$dataGridView.Controls.Add($buttonName2)
$form.Refresh()
})
 
 
$buttonVersion1 = New-Object System.Windows.Forms.Button
$buttonVersion1.Text = '/\'
$buttonVersion1.Location = New-Object System.Drawing.Point(220,3)
$buttonVersion1.Size = '20, 20'
 
$buttonVersion2 = New-Object System.Windows.Forms.Button
$buttonVersion2.Text = '\/'
$buttonVersion2.Location = New-Object System.Drawing.Point(220,3)
$buttonVersion2.Size = '20, 20'
 
$buttonVersion3 = New-Object System.Windows.Forms.Button
$buttonVersion3.Text = ' '
$buttonVersion3.Location = New-Object System.Drawing.Point(220,3)
$buttonVersion3.Size = '20, 20'
$dataGridView.Controls.Add($buttonVersion3)
 
$buttonVersion1.Add_Click({
$list.Clear()
$UninstallList1 = $UninstallList1 | Sort-Object -Property @{ Expression = "??????"; Descending = $true }
$dataGridView.Controls.Remove($buttonVersion1)
 
$dataGridView.Controls.Remove($buttonName2)
$dataGridView.Controls.Remove($buttonName1)
$dataGridView.Controls.Add($buttonName3)
$dataGridView.Controls.Remove($buttonPublisher2)
$dataGridView.Controls.Remove($buttonPublisher1)
$dataGridView.Controls.Add($buttonPublisher3)
$dataGridView.Controls.Remove($buttonDate2)
$dataGridView.Controls.Remove($buttonDate1)
$dataGridView.Controls.Add($buttonDate3)
 
$list.AddRange($UninstallList1)
$dataGridView.DataSource = $list
$dataGridView.Controls.Add($buttonVersion2)
$form.Refresh()
})
 
$buttonVersion2.Add_Click({
$list.Clear()
$UninstallList1 = $UninstallList1 | Sort-Object -Property @{ Expression = "??????"; Ascending = $true }
$dataGridView.Controls.Remove($buttonVersion2)
 
$dataGridView.Controls.Remove($buttonName2)
$dataGridView.Controls.Remove($buttonName1)
$dataGridView.Controls.Add($buttonName3)
$dataGridView.Controls.Remove($buttonPublisher2)
$dataGridView.Controls.Remove($buttonPublisher1)
$dataGridView.Controls.Add($buttonPublisher3)
$dataGridView.Controls.Remove($buttonDate2)
$dataGridView.Controls.Remove($buttonDate1)
$dataGridView.Controls.Add($buttonDate3)
 
$list.AddRange($UninstallList1)
$dataGridView.DataSource = $list
$dataGridView.Controls.Add($buttonVersion1)
$form.Refresh()
})
 
$buttonVersion3.Add_Click({
$buttonVersion1.PerformClick()
$dataGridView.Controls.Remove($buttonVersion3)
$dataGridView.Controls.Add($buttonVersion2)
$form.Refresh()
})
 
$buttonPublisher1 = New-Object System.Windows.Forms.Button
$buttonPublisher1.Text = '/\'
$buttonPublisher1.Location = New-Object System.Drawing.Point(320,3)
$buttonPublisher1.Size = '20, 20'
 
$buttonPublisher2 = New-Object System.Windows.Forms.Button
$buttonPublisher2.Text = '\/'
$buttonPublisher2.Location = New-Object System.Drawing.Point(320,3)
$buttonPublisher2.Size = '20, 20'
 
$buttonPublisher3 = New-Object System.Windows.Forms.Button
$buttonPublisher3.Text = ' '
$buttonPublisher3.Location = New-Object System.Drawing.Point(320,3)
$buttonPublisher3.Size = '20, 20'
$dataGridView.Controls.Add($buttonPublisher3)
 
$buttonPublisher1.Add_Click({
$list.Clear()
$UninstallList1 = $UninstallList1 | Sort-Object -Property @{ Expression = "????????"; Descending = $true }
$dataGridView.Controls.Remove($buttonPublisher1)
 
$dataGridView.Controls.Remove($buttonName2)
$dataGridView.Controls.Remove($buttonName1)
$dataGridView.Controls.Add($buttonName3)
$dataGridView.Controls.Remove($buttonVersion2)
$dataGridView.Controls.Remove($buttonVersion1)
$dataGridView.Controls.Add($buttonVersion3)
$dataGridView.Controls.Remove($buttonDate2)
$dataGridView.Controls.Remove($buttonDate1)
$dataGridView.Controls.Add($buttonDate3)
 
$list.AddRange($UninstallList1)
$dataGridView.DataSource = $list
$dataGridView.Controls.Add($buttonPublisher2)
$form.Refresh()
})
 
$buttonPublisher2.Add_Click({
$list.Clear()
$UninstallList1 = $UninstallList1 | Sort-Object -Property @{ Expression = "????????"; Ascending = $true }
$dataGridView.Controls.Remove($buttonPublisher2)
 
$dataGridView.Controls.Remove($buttonName2)
$dataGridView.Controls.Remove($buttonName1)
$dataGridView.Controls.Add($buttonName3)
$dataGridView.Controls.Remove($buttonVersion2)
$dataGridView.Controls.Remove($buttonVersion1)
$dataGridView.Controls.Add($buttonVersion3)
$dataGridView.Controls.Remove($buttonDate2)
$dataGridView.Controls.Remove($buttonDate1)
$dataGridView.Controls.Add($buttonDate3)
 
$list.AddRange($UninstallList1)
$dataGridView.DataSource = $list
$dataGridView.Controls.Add($buttonPublisher1)
$form.Refresh()
})
 
$buttonPublisher3.Add_Click({
$buttonPublisher1.PerformClick()
$dataGridView.Controls.Remove($buttonPublisher3)
$dataGridView.Controls.Add($buttonPublisher2)
$form.Refresh()
})
 
$buttonDate1 = New-Object System.Windows.Forms.Button
$buttonDate1.Text = '/\'
$buttonDate1.Location = New-Object System.Drawing.Point(420,3)
$buttonDate1.Size = '20, 20'
 
$buttonDate2 = New-Object System.Windows.Forms.Button
$buttonDate2.Text = '\/'
$buttonDate2.Location = New-Object System.Drawing.Point(420,3)
$buttonDate2.Size = '20, 20'
 
$buttonDate3 = New-Object System.Windows.Forms.Button
$buttonDate3.Text = ' '
$buttonDate3.Location = New-Object System.Drawing.Point(420,3)
$buttonDate3.Size = '20, 20'
$dataGridView.Controls.Add($buttonDate3)
 
$buttonDate1.Add_Click({
$list.Clear()
$UninstallList1 = $UninstallList1 | Sort-Object -Property @{ Expression = "???? ?????????"; Descending = $true }
$dataGridView.Controls.Remove($buttonDate1)
 
$dataGridView.Controls.Remove($buttonName2)
$dataGridView.Controls.Remove($buttonName1)
$dataGridView.Controls.Add($buttonName3)
$dataGridView.Controls.Remove($buttonVersion2)
$dataGridView.Controls.Remove($buttonVersion1)
$dataGridView.Controls.Add($buttonVersion3)
$dataGridView.Controls.Remove($buttonPublisher2)
$dataGridView.Controls.Remove($buttonPublisher1)
$dataGridView.Controls.Add($buttonPublisher3)
 
$list.AddRange($UninstallList1)
$dataGridView.DataSource = $list
$dataGridView.Controls.Add($buttonDate2)
$form.Refresh()
})
 
$buttonDate2.Add_Click({
$list.Clear()
$UninstallList1 = $UninstallList1 | Sort-Object -Property @{ Expression = "???? ?????????"; Ascending = $true }
$dataGridView.Controls.Remove($buttonDate2)
 
$dataGridView.Controls.Remove($buttonName2)
$dataGridView.Controls.Remove($buttonName1)
$dataGridView.Controls.Add($buttonName3)
$dataGridView.Controls.Remove($buttonVersion2)
$dataGridView.Controls.Remove($buttonVersion1)
$dataGridView.Controls.Add($buttonVersion3)
$dataGridView.Controls.Remove($buttonPublisher2)
$dataGridView.Controls.Remove($buttonPublisher1)
$dataGridView.Controls.Add($buttonPublisher3)
 
$list.AddRange($UninstallList1)
$dataGridView.DataSource = $list
$dataGridView.Controls.Add($buttonDate1)
$form.Refresh()
})
 
$buttonDate3.Add_Click({
$buttonDate1.PerformClick()
$dataGridView.Controls.Remove($buttonDate3)
$dataGridView.Controls.Add($buttonDate2)
$form.Refresh()
})
 
$form.Controls.Add($dataGridView)
$form.ShowDialog()
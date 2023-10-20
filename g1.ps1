Add-Type -AssemblyName System.Windows.Forms
 
# Create a new form
$form = New-Object System.Windows.Forms.Form
$form.Size = New-Object System.Drawing.Size(600, 400)
$form.Text = "GridView Example"
 
# Create a GridView control
$gridView = New-Object System.Windows.Forms.DataGridView
$gridView.Location = New-Object System.Drawing.Point(10, 10)
$gridView.Size = New-Object System.Drawing.Size(580, 300)
 
# Create columns for the GridView
$column1 = New-Object System.Windows.Forms.DataGridViewTextBoxColumn
$column1.HeaderText = "Name"
$column2 = New-Object System.Windows.Forms.DataGridViewTextBoxColumn
$column2.HeaderText = "Age"
 
# Add the columns to the GridView
$gridView.Columns.Add($column1)
$gridView.Columns.Add($column2)
 
# Create some sample data
$data = @(
    [PSCustomObject]@{
        Name = "John"
        Age = 25
    },
    [PSCustomObject]@{
        Name = "Alice"
        Age = 30
    },
    [PSCustomObject]@{
        Name = "Bob"
        Age = 35
    }
)
 
# Add the data to the GridView
$gridView.DataSource = $data

 
# Add the GridView to the form
$form.Controls.Add($gridView)
[System.Windows.Forms.DataGridViewRow[]]$rows = $gridView.Rows
$rows | get-member
 
# Create a button for exporting to CSV
$button = New-Object System.Windows.Forms.Button
$button.Location = New-Object System.Drawing.Point(10, 320)
$button.Size = New-Object System.Drawing.Size(100, 25)
$button.Text = "Export to CSV"
 
# Define the button click event
$button.Add_Click({
    $saveFileDialog = New-Object System.Windows.Forms.SaveFileDialog
    $saveFileDialog.Filter = "CSV Files (*.csv)|*.csv"
    $saveFileDialog.Title = "Export to CSV"
    
    if ($saveFileDialog.ShowDialog() -eq 'OK') {
        $filePath = $saveFileDialog.FileName
        $exportData = @()
        
        # Loop through the GridView rows and add data to the export array
        foreach ($row in $gridView.Rows) {
            $rowData = @{
                Name = $row.Cells[0].Value
                Age = $row.Cells[1].Value
            }
            $exportData += New-Object PSObject -Property $rowData
        }
        
        $exportData | Export-Csv -Path $filePath -NoTypeInformation
        Write-Host "Data exported to $filePath"
    }
})
 
# Add the button to the form
$form.Controls.Add($button)
 
# Show the form
$form.ShowDialog()
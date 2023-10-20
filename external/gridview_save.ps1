#Copyright (c) 2023 Serguei Kouzmine
#
#Permission is hereby granted, free of charge, to any person obtaining a copy
#of this software and associated documentation files (the "Software"), to deal
#in the Software without restriction, including without limitation the rights
#to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
#copies of the Software, and to permit persons to whom the Software is
#furnished to do so, subject to the following conditions:
#
#The above copyright notice and this permission notice shall be included in
#all copies or substantial portions of the Software.
#
#THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
#IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
#FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
#AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
#LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
#OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
#THE SOFTWARE.


# origin: https://www.cyberforum.ru/powershell/thread3110371.html
# NOTE: the original example lacked the ability to load the data
add-type -AssemblyName System.Windows.Forms

$f = new-object System.Windows.Forms.Form
$f.Size = new-object System.Drawing.Size(400, 240)
$f.Text = 'GridView Example'

$b = new-object System.Windows.Forms.Button
$b.Location = new-object System.Drawing.Point(10, 160)
$b.Size = new-object System.Drawing.Size(100, 25)
$b.Text = 'Export to CSV'

$b.Add_Click({
  # NOTE: with WPF signature is different:
  # param( [object]$sender,  [System.Windows.RoutedEventArgs ]$eventargs )
  param(
    [Object]$sender,
    [System.EventArgs]$e
  )
  # NOTE: no parenthesis shoukd be present in the cast
  [System.Windows.Forms.Button] $b = [System.Windows.Forms.Button]$sender

  # https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.savefiledialog?view=netframework-4.5
  [System.Windows.Forms.SaveFileDialog] $fd = new-object System.Windows.Forms.SaveFileDialog
  $fd.Filter = 'CSV Files (*.csv)|*.csv'
  $fd.Title = $sender.Text
  [System.Windows.Forms.DialogResult] $status = $fd.ShowDialog()
  # NOTE: can use constant or plain String in test
  if ( ($status -eq [System.Windows.Forms.DialogResult]::OK) -or ($status -eq 'OK')) {
    $filePath = $fd.FileName
    $exportData = @()

    foreach ($row in $g.Rows) {
      $rowData = @{
        Name = $row.Cells[0].Value
        Age = $row.Cells[1].Value
      }
      $exportData += new-object PSObject -Property $rowData
    }

    $exportData | export-csv -Path $filePath -NoTypeInformation
    write-host ('Data exported to {0}' -f $filePath)
  }
})

$f.Controls.Add($b)

$g = new-object System.Windows.Forms.DataGridView
$g.Location = new-object System.Drawing.Point(10, 10)
$g.Size = new-object System.Drawing.Size(380, 140)

$c1 = new-object System.Windows.Forms.DataGridViewTextBoxColumn
$c1.HeaderText = 'Name'
$c2 = new-object System.Windows.Forms.DataGridViewTextBoxColumn
$c2.HeaderText = 'Age'

$g.Columns.Add($c1) | out-null
$g.Columns.Add($c2) | out-null

$data = @(
  [PSCustomObject]@{
    Name = 'John'
    Age = 25
  },
  [PSCustomObject]@{
    Name = 'Alice'
    Age = 30
  },
  [PSCustomObject]@{
    Name = 'Bob'
    Age = 35
  }
)

write-host ('Adding {0} rows' -f $data.count)

# NOTE: avoid vars like $rows when adding cells, rows etc.: cannot use 
[System.Windows.Forms.DataGridViewRow[]]$rows = $g.Rows

try {
  (0..([int]$data.count -1) ) | foreach-object {
    $g.rows.add((new-object System.Windows.Forms.DataGridViewRow)) | out-null
  }
} catch [Exception] {
  $e = $_[0].Exception 
  write-host ('Exception:{0}{2}Message: {1}' -f $e.getType(), $e.Message,[char]10 )
  # Message: Exception calling "Add" with "1" argument(s): "Collection was of a fixed size."
}
# NOTE: it is OK to introduce variables like $row to access the vallues
[System.Windows.Forms.DataGridViewRow[]]$rows = $g.Rows
if ( $rows -ne $null ) {
 write-host('The number of rows in DataGridView is {0}' -f $rows.Count)
} else {
  write-host ('Unable to access rows of the DataGridView')
}

(0..([int]$data.count -1) ) | foreach-object {
  $cnt = $_
  $entry = $data[$cnt]

  [System.Windows.Forms.DataGridViewRow]$row = $g.rows[$cnt]

  [System.Windows.Forms.DataGridViewTextBoxCell[]]$cells = $row.Cells
  $cells[0].Value = $entry.Name
  $cells[1].Value = $entry.Age

  write-host ('Row[{0}] Cells [0] value: {1}' -f $cnt, $cells[0].Value)
  write-host ('Row[{0}] Cells [1] value: {1}' -f $cnt, $cells[1].Value)
}
# this does not show the data. It actually wipes the data if uncommened
# $g.DataSource = $data

$f.Controls.Add($g)

$f.ShowDialog()

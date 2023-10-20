# based on: https://www.cyberforum.ru/powershell/thread901801.html

$Script:o = [Hashtable]::Synchronized(@{})
$RunSpace = [Management.Automation.Runspaces.RunspaceFactory]::CreateRunspace()
$RunSpace.ApartmentState = "STA"
$RunSpace.ThreadOptions = "ReuseThread"
$RunSpace.Open()
$RunSpace.SessionStateProxy.SetVariable('o', $Script:o)
$PowerShellCmd = [Management.Automation.PowerShell]::Create().AddScript({

  [void] [System.Reflection.Assembly]::LoadWithPartialName('System.Drawing')
  [void] [System.Reflection.Assembly]::LoadWithPartialName('System.Windows.Forms')
    
    $f = New-Object System.Windows.Forms.Form
    $InitialFormWindowState = New-Object System.Windows.Forms.FormWindowState
    $f.Size = New-Object System.Drawing.Size(340, 200)
    $f.Name = "form"
    $f.Text = "Test form"
    
    $l = New-Object System.Windows.Forms.Label
    $l.Location = New-Object System.Drawing.Size(10,20)
    $l.Size = New-Object System.Drawing.Size(320,20)
    $l.Text = 'enter data, press buttin or Enter:'
    $f.Controls.Add($l)
    
    $t = New-Object System.Windows.Forms.TextBox
    $t.Location = New-Object System.Drawing.Size(10,40)
    $t.Size = New-Object System.Drawing.Size(300,20)
    $f.Controls.Add($t)
    
    $f.KeyPreview = $True
    $f.Add_KeyDown( { 
      if ($_.KeyCode -eq "Enter") { 
         $Script:o.X = $t.Text
         $f.Close() 
      } 
    })
    $f.Add_KeyDown( {
      if ($_.KeyCode -eq "Escape") { 
        $f.Close() 
      }
    })
    
    $b = New-Object System.Windows.Forms.Button
    $b.Location = New-Object System.Drawing.Size(75,120)
    $b.Size = New-Object System.Drawing.Size(75,23)
    $b.Text = "OK"
    $b.Add_Click( {
      $Script:o.X = $Script:o.t.Text;
      $Script:o.f.Close() 
    } )
    $f.Controls.Add($b)
    
    $Script:o.InitialFormWindowState = $f.WindowState
    $f.add_Load($Script:o.OnLoadForm_StateCorrection)
    [void] $f.ShowDialog()
    })
$PowerShellCmd.Runspace = $RunSpace
$obj = $PowerShellCmd.BeginInvoke()

$BindingFlags = [Reflection.BindingFlags]'nonpublic','instance'
$Field = $PowerShellCmd.GetType().GetField('invokeAsyncResult',$BindingFlags)
$AsyncObject = $Field.GetValue($PowerShellCmd)
Write-Host 'Здесь ваш код, который исполняется параллельно с кодом формы'
Write-Host '(любой длины и времени исполнения)'
Write-Host "`$PowerShellCmd.EndInvoke($AsyncObject) будет ждать результаты формы из формы,"
Write-Host "или подхватит их, если они будут уже готовы."
$PowerShellCmd.EndInvoke($AsyncObject)
write-output ('result: {0}' -f $Script:o.X)
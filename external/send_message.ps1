# origin: https://habrahabr.ru/post/138008/
# revisited in https://www.cyberforum.ru/powershell/thread1306627.html
# https://github.com/foxmuldercp/shell_scripts/tree/master/PowerShell

$ErrorActionPreference = 'silentlycontinue'
$DebugPreference = 'Continue'

Add-Type -Assembly System.Windows.Forms

$f = New-Object System.Windows.Forms.Form

$tt = New-Object System.Windows.Forms.ToolTip
$tt.BackColor = [System.Drawing.Color]::LightGoldenrodYellow
$tt.IsBalloon = $true
# $tt.InitialDelay = 500
# $tt.ReshowDelay = 500

$f.StartPosition = 'CenterScreen'
$f.Text = 'Broadcast messages to users'
$f.Width = 418
$f.Height = 233

$l1 = New-Object System.Windows.Forms.Label
$l1.Location = New-Object System.Drawing.Point (10,12)
$l1.Text = 'Servers:'

$l2 = New-Object System.Windows.Forms.Label
$l2.Location = New-Object System.Drawing.Point (10,42)
$l2.Text = 'Users:'
$l2.AutoSize = 1

$l4 = New-Object System.Windows.Forms.Label
$l4.Location = New-Object System.Drawing.Point (10,73)
$l4.Text = 'Message'
$l4.AutoSize = 1
# $tt.SetToolTip($l4, '')

$l3 = New-Object System.Windows.Forms.Label
$l3.Location = New-Object System.Drawing.Point (10,103)
$l3.Text = 'Signature'
$l3.AutoSize = 1
$tt.SetToolTip($l3,'author is required')

$t1 = New-Object System.Windows.Forms.ComboBox
$t1.Location = New-Object System.Drawing.Point (90,10)
$t1.DataSource = @( 'target_hostname' )
$t1.Width = 300
$t1.add_TextChanged({ $target_servers = $t1.Text })
$t1.TabIndex = 1
$tt.SetToolTip($t1,'destination servers')

$t2 = New-Object System.Windows.Forms.ComboBox
$t2.Location = New-Object System.Drawing.Point (90,40)
$t2.DataSource = @( '*' ) # '*' means everybody
$t2.Text = $recipients_array[1]
$t2.add_click({ $t2.SelectAll() })
$t2.add_TextChanged({ $recipients = $t2.Text })
$t2.Width = 300
$t2.TabIndex = 2
$tt.SetToolTip($t2,'recipients')

$t4 = New-Object System.Windows.Forms.TextBox
$t4.Location = New-Object System.Drawing.Point (90,70)
$t4.Text = 'Sample Message.'
$t4.add_click({ $t4.SelectAll() })
$t4.add_TextChanged({ $global:Message = $t4.Text })
$t4.Width = 300
$t4.TabIndex = 3
$tt.SetToolTip($t4,'what is the message?')

$t3 = New-Object System.Windows.Forms.ComboBox
$t3.Location = New-Object System.Drawing.Point (90,103)
$t3.DataSource = @( 'Administrator','Sender Signature')
$t3.Text = $signature[1]
$t3.add_TextChanged({ $signature = $t3.Text })
$t3.Width = 300
$t3.TabIndex = 4
$tt.SetToolTip($t3,"sender's signature")

$global:testrun = New-Object System.Windows.Forms.CheckBox
$global:testrun.Location = New-Object System.Drawing.Point (200,150)
$global:testrun.Text = 'Test'
$global:testrun.Checked = 1
$global:testrun.AutoSize = 1
$global:testrun.TabIndex = 6
$tt.SetToolTip($global:testrun,'Test: localhost only')

$b1 = New-Object System.Windows.Forms.Button
$b1.Location = New-Object System.Drawing.Point (10,150)
$b1.Text = 'Send'
$b1.AutoSize = 1
$b1.TabIndex = 5

function SendMessage {
  param(
    [string[]]$target_servers = @( 'localhost'),
    [string]$recipients = 'Console',
    [string]$Message,
    [string]$signature,
    [bool]$Test = $false
  )

  $message = $message.Trim()
  $signature = $signature.Trim()
  if ( test-path 'c:\windows\system32\msg.exe') {
    $target_servers | ForEach-Object {
      $server = $_
      # https://www.winsentmessenger.com/msg/
      # https://stackoverflow.com/questions/30972651/i-cannot-use-the-msg-command-in-cmd-or-batch-for-that-matter-how-can-i-fix-th
      # suggesting copy the one from windows7
      # absent msg.exe one can ptobably get same result by composing an equivalent net send command
      # https://www.lifewire.com/net-send-2618095
      # https://www.lifewire.com/msg-command-2618093
      <#
        NOTE: binaries found in side-by-side dir, are non functional

        This version of c:\Windows\WinSxS\amd64_microsoft-windows-t..commandlinetoolsmqq31bf3856ad364e35_6.3.9600.17415_none_1a346c487fc2917e\msg.exe
        This version of c:\Windows\WinSxS\amd64_microsoft-windows-t..commandlinetoolsmqq_31bf3856ad364e35_6.3.9600.16384_none_19e7d16c7ffc24f6\msg.exe

        is not compatible with the version of Windows you're running.
        Check your computer's system informtion and then contact the software publisher.
      #>

      $recipients |
        foreach-Object {
          $user = $_
          write-debug "c:\windows\system32\msg.exe ${user} /Server:${server} ${message} ${signature}"
          c:\windows\system32\msg.exe $user /Server:$server $message $signature
        }
      Confirm
    }
  } else {
    Confirm -text 'msg.exe not found'
  }
}

function Confirm {
  param(
    [String]$text = 'Message was sent'
  )
  $f = New-Object System.Windows.Forms.Form
  $f.StartPosition = 'CenterScreen'
  $f.Text = 'OK'
  $f.Width = 190
  $f.Height = 133
  $f.ControlBox = 0
  $b = New-Object System.Windows.Forms.Button
  $b.add_click({ $f.Close(); $f.Close() })
  $b.Text = 'Close'

  $b.AutoSize = 1
  $b.Location = New-Object System.Drawing.Point (50,50)

  $t = New-Object System.Windows.Forms.Label
  $t.Text = $text
  $t.AutoSize = 1
  $t.Location = New-Object System.Drawing.Point (10,10)
  $f.Controls.Add($t)
  $f.Controls.Add($b)
<#
  $f.add_ResizeEnd({
    $size = $f.Size
    write-host ('Size : {0},{1}' -f $size.Width, $size.Height)
  })
#>
  $f.ShowDialog() | Out-Null
}

$b1.add_click({ $recipients = $t2.Text.Split(',');
    $target_servers = $t1.Text.Split(','); $signature = $t3.Text;
    if ($global:testrun.Checked -eq 1) {
      write-debug 'Test run'
      $target_servers = @( 'localhost')
      $recipients = 'Console'
      write-debug "SendMessage -test $true -target_servers ${target_servers} -recipients ${recipients} -Message ${global:Message} $signatureX"
    }
    SendMessage -Test $true -target_servers $target_servers -recipients $recipients -Message $global:Message -signature $signature
  })
# $tt.SetToolTip($b1, '')

$b2 = New-Object System.Windows.Forms.Button
$b2.Location = New-Object System.Drawing.Point (315,150)
$b2.Text = 'Close'
$b2.add_click({ $f.Close() })
$b2.AutoSize = 1
$b2.TabIndex = 7
$tt.SetToolTip($b2,'Quit')

$f.Controls.AddRange(@( $b1,$b2,$t1,$t2,$t3,$t4,$l1,$l2,$l3,$l4,$global:testrun))

$f.ShowDialog() | Out-Null



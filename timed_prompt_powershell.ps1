#Copyright (c) 2022 Serguei Kouzmine
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

param(
  [string]$title = 'Prompt with Timeout',
  [string]$message = 'Continue ?',
  [int]$expiration = 10,
  [int]$interval = 1
)

$RESULT_OK = 0
$RESULT_CANCEL = 1
$RESULT_TIMEOUT = 2
$RESULT_UNKNOWN = 4

$Readable = @{
  $RESULT_OK = 'OK';
  $RESULT_CANCEL = 'CANCEL';
  $RESULT_TIMEOUT = 'TIMEOUT';
  $RESULT_UNKNOWN = 'UNKNOWN';
}

function PromptAuto {

  param(
    [string]$title,
    [string]$message,
    [int]$expiration = 60,
    [int]$interval = 1
 )


  @( 'System.Drawing','System.Windows.Forms') | ForEach-Object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }

  $f = new-object System.Windows.Forms.Form

  $f.Text = $title

  $f.Size = new-object System.Drawing.Size (260,140)

  $f.StartPosition = 'CenterScreen'

  $f.KeyPreview = $True
  $f.Add_KeyDown({
    if ($_.KeyCode -eq 'O') { $script:Data = $RESULT_OK }
    elseif ($_.KeyCode -eq 'Escape') { $script:Data = $RESULT_CANCEL }
    else { return }
    $f.Close()

  })

  $b1 = new-object System.Windows.Forms.Button
  $b1.Font = new-object System.Drawing.Font ('Arial',8,[System.Drawing.FontStyle]::Bold,[System.Drawing.GraphicsUnit]::Point,0)
  $b1.Location = new-object System.Drawing.Size (50,46)
  $b1.Size = new-object System.Drawing.Size (75,23)
  $b1.Text = 'OK'
  $b1.add_click({
    $script:Data = $RESULT_OK; $f.Close();
  })
  $f.Controls.Add($b1)

  $b2 = new-object System.Windows.Forms.Button

  $b2.Font = new-object System.Drawing.Font ('Arial',8,[System.Drawing.FontStyle]::Bold,[System.Drawing.GraphicsUnit]::Point,0)
  $b2.Location = new-object System.Drawing.Size (130,46)
  $b2.Size = new-object System.Drawing.Size (75,23)
  $b2.Text = 'Cancel'
  $b2.add_click({
    $script:Data = $RESULT_CANCEL
    $f.Close()
  })
  $f.Controls.Add($b2)

  $l = new-object System.Windows.Forms.Label
  $l.Font = new-object System.Drawing.Font ('Arial',8,[System.Drawing.FontStyle]::Bold,[System.Drawing.GraphicsUnit]::Point,0)
  $l.Location = new-object System.Drawing.Size (10,20)
  $l.Size = new-object System.Drawing.Size (280,20)
  $l.Text = $message
  $f.Controls.Add($l)



  $end = (Get-Date -UFormat '%s')
  $end = ([int]$end + $expiration)
  # NOTE: cannot add timer to form directly - not a control
  # $f.Controls.Add($t2)
  $t2 = new-object System.Timers.Timer
  $t2.Interval = 1000

  $t2.add_Elapsed({
    $start = (get-date -UFormat '%s')
    $elapsed = new-timespan -seconds ($end - $start)
    if ($end - $start -lt 0) {
      $script:Data = $RESULT_TIMEOUT
      # NOTE: probably GUI thread
      # start-sleep -millisecond 1000
      # [System.Threading.Thread]::sleep(3000)
      $t2.Stop()
      $f.Close()
    } else {
    # } elseif ($end - $start -ge 1) {
      $l.Text = ('Remaining time {0:00}:{1:00}:{2:00}' -f $elapsed.Hours,$elapsed.Minutes, $elapsed.Seconds,($end - $start))
    # } else {
    #  $l.Text = 'Timed out'
    }

  })
  $t2.Enabled = $true
  $t2.SynchronizingObject = $f
  $t2.Start()


  $f.Topmost = $true

  $script:Data = $RESULT_UNKNOWN
  $f.Add_Closing({
    if ($script:Data -eq $RESULT_UNKNOWN) {
      $script:Data = $RESULT_CANCEL
    }
   })

  $f.Add_Shown({ $f.Activate() })

  [void]$f.ShowDialog()
  $f.Dispose()
}

$DebugPreference = 'Continue'


PromptAuto -Title $title -Message $message -expiration $expiration -interval $interval
$result = $script:Data
Write-Debug ("Result is : {0} ({1})" -f $Readable.Item($result), $result)


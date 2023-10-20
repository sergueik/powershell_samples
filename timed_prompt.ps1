#Copyright (c) 2014,2015,2022 Serguei Kouzmine
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
  [int]$expiration = 60,
  [int]$interval = 1
)

Add-Type -TypeDefinition @"

using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

public class TimerPanel : Panel {
    private System.Timers.Timer t;
    private Container components = null;
    public System.Timers.Timer T {
      get {
        return t;
      }
      set { t = value; }
    }


    public TimerPanel() {
        InitializeComponent();
    }

    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (components != null) {
          components.Dispose();
        }
      }
      t.Stop();
      base.Dispose(disposing);
    }

    private void InitializeComponent() {
      t = new System.Timers.Timer();
      ((System.ComponentModel.ISupportInitialize)(t)).BeginInit();
      SuspendLayout();
      t.Interval = 1000;
      t.Start();
      t.Enabled = true;
      t.SynchronizingObject = this;
      // timer event handler is defined in Powershell code
      // t.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimerElapsed);
      ((System.ComponentModel.ISupportInitialize)(t)).EndInit();
      ResumeLayout(false);
    }

   /* private void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e) {
      // Console.WriteLine(".");
    }
   */

}
"@ -ReferencedAssemblies 'System.Windows.Forms.dll'

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
    [int]$interval = 1,
    [object]$caller
  )


  @( 'System.Drawing','System.Windows.Forms') | ForEach-Object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }

  $f = new-object System.Windows.Forms.Form

  $f.Text = $title

  $f.Size = new-object System.Drawing.Size (240,110)

  $f.StartPosition = 'CenterScreen'

  $f.KeyPreview = $True
  $f.Add_KeyDown({

      if ($_.KeyCode -eq 'O') { $caller.Data = $RESULT_OK }
      elseif ($_.KeyCode -eq 'Escape') { $caller.Data = $RESULT_CANCEL }
      else { return }
      $f.Close()

    })

  $b1 = new-object System.Windows.Forms.Button
  $b1.Font = new-object System.Drawing.Font ('Arial',10,[System.Drawing.FontStyle]::Bold,[System.Drawing.GraphicsUnit]::Point,0)
  $b1.Location = new-object System.Drawing.Size (50,46)
  $b1.Size = new-object System.Drawing.Size (75,23)
  $b1.Text = 'OK'
  $b1.add_click({ $caller.Data = $RESULT_OK; $f.Close(); })
  $p = new-object TimerPanel
  $p.Size = $f.Size

  $p.Controls.Add($b1)
  $end = (Get-Date -UFormat '%s')
  $end = ([int]$end + $expiration)
  $b2 = new-object System.Windows.Forms.Button
  # screen
  $b2.Font = new-object System.Drawing.Font ('Arial',10,[System.Drawing.FontStyle]::Bold,[System.Drawing.GraphicsUnit]::Point,0)
  $b2.Location = new-object System.Drawing.Size (130,46)
  $b2.Size = new-object System.Drawing.Size (75,23)
  $b2.Text = 'Cancel'
  $b2.add_click({
      $caller.Data = $RESULT_CANCEL
      $f.Close()
    })
  $p.Controls.Add($b2)

  $l = new-object System.Windows.Forms.Label
  $l.Font = new-object System.Drawing.Font ('Arial',8,[System.Drawing.FontStyle]::Bold,[System.Drawing.GraphicsUnit]::Point,0)
  $l.Location = new-object System.Drawing.Size (10,20)
  $l.Size = new-object System.Drawing.Size (280,20)
  $l.Text = $message
  $p.Controls.Add($l)

  $p.T.Stop()
  $p.T.Interval = $interval * 1000
  $p.T.Start()
  $caller.Data = $RESULT_UNKNOWN

  $p.T.add_Elapsed({
      $start = (Get-Date -UFormat '%s')

      $elapsed = New-TimeSpan -Seconds ($end - $start)
      $l.Text = ('Remaining time {0:00}:{1:00}:{2:00}' -f $elapsed.Hours,$elapsed.Minutes,$elapsed.Seconds,($end - $start))

      if ($end - $start -lt 0) {
        $caller.Data = $RESULT_TIMEOUT
        $f.Close()
      }

    })

  $f.Controls.Add($p)

  $f.Topmost = $True

  $f.Add_Shown({ $f.Activate() })
  $f.Add_Closing({
    if ($caller.Data -eq $RESULT_UNKNOWN) {
      $caller.Data = $RESULT_CANCEL
    }
   })

  [void]$f.ShowDialog([win32window]($caller))
  $f.Dispose()
}

Add-Type -TypeDefinition @"

using System;
using System.Windows.Forms;
public class Win32Window : IWin32Window
{
    private IntPtr _hWnd;
    private int _data;

    public int Data
    {
        get { return _data; }
        set { _data = value; }
    }

    public Win32Window(IntPtr handle)
    {
        _hWnd = handle;
    }

    public IntPtr Handle
    {
        get { return _hWnd; }
    }
}

"@ -ReferencedAssemblies 'System.Windows.Forms.dll'

$DebugPreference = 'Continue'


$caller = new-object Win32Window -ArgumentList ([System.Diagnostics.Process]::GetCurrentProcess().MainWindowHandle)

PromptAuto -Title $title -Message $message -caller $caller -expiration $expiration -interval $interval
$result = $caller.Data
Write-Debug ("Result is : {0} ({1})" -f $Readable.Item($result),$result)


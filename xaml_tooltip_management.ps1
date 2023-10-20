#Copyright (c) 2014 Serguei Kouzmine
#Permission is hereby granted, free of charge, to any person obtaining a copy
#of this software and associated documentation files (the "Software"), to deal
#in the Software without restriction, including without limitation the rights
#to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
#copies of the Software, and to permit persons to whom the Software is
#furnished to do so, subject to the following conditions:
#The above copyright notice and this permission notice shall be included in
#all copies or substantial portions of the Software.
#THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
#IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
#FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
#AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
#LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
#OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
#THE SOFTWARE.
#requires -version 2
Add-Type -AssemblyName PresentationFramework
# http://stackoverflow.com/questions/8343767/how-to-get-the-current-directory-of-the-cmdlet-being-executed
function Get-ScriptDirectory
{
  $Invocation = (Get-Variable MyInvocation -Scope 1).Value
  if ($Invocation.PSScriptRoot)
  {
    $Invocation.PSScriptRoot
  }
  elseif ($Invocation.MyCommand.Path)
  {
    Split-Path $Invocation.MyCommand.Path
  }
  else
  {
    $Invocation.InvocationName.Substring(0,$Invocation.InvocationName.LastIndexOf("\"))
  }
}

$so = [hashtable]::Synchronized(@{
    'Result' = [string]'';
    'ScriptDirectory' = [string]'';
    'Window' = [System.Windows.Window]$null;
    'Control' = [System.Windows.Controls.ToolTip]$null;
    'Contents' = [System.Windows.Controls.TextBox]$null;
    'NeedData' = [bool]$false;
    'HaveData' = [bool]$false;

  })
$so.ScriptDirectory = Get-ScriptDirectory

$so.Result = ''
$rs = [runspacefactory]::CreateRunspace()
$rs.ApartmentState = 'STA'
$rs.ThreadOptions = 'ReuseThread'
$rs.Open()
$rs.SessionStateProxy.SetVariable('so',$so)
$run_script = [powershell]::Create().AddScript({

    Add-Type -AssemblyName PresentationFramework
    [xml]$xaml = @"
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
  Title="About WPF" 
  Background="LightGray" Height="190" Width="168">
        <Canvas>

        <Image Opacity=".7" Source="$('{0}\{1}' -f $so.ScriptDirectory, 'clock.jpg' )" 
               Width="150">
          <Image.ToolTip>
            <ToolTip Name="tooltip">
            <StackPanel>
              <Label FontWeight="Bold" Background="Blue" Foreground="White">
                The CheckBox
              </Label>
              <StackPanel Orientation="Horizontal">
                <Image Name="hourglass" Visibility="Collapsed" Width="20" Margin="2" Source="$('{0}\{1}' -f $so.ScriptDirectory, 'hourglass.jpg' )"/>
              <TextBlock Padding="10" TextWrapping="WrapWithOverflow" Width="200" Name="tooltip_textbox">
                please wait...
              </TextBlock>
              </StackPanel>
            </StackPanel>
           </ToolTip>
          </Image.ToolTip>
        </Image>
        </Canvas>
</Window>
"@

    $reader = (New-Object System.Xml.XmlNodeReader $xaml)
    $target = [Windows.Markup.XamlReader]::Load($reader)
    $so.Window = $target
    $control = $target.FindName("tooltip")
    $so.Indicator = $target.FindName("hourglass")
    $contents = $target.FindName("tooltip_textbox")
    $so.Control = $control
    $so.Contents = $contents
    $handler_opened = {
      param(
        [object]$sender,
        [System.Windows.RoutedEventArgs]$eventargs
      )
      $so.Contents.Text = 'please wait...'
      $so.Indicator.Visibility = 'Visible'
      $so.NeedData = $true
      $so.Result = ''
    }
    $handler_closed = {
      param(
        [object]$sender,
        [System.Windows.RoutedEventArgs]$eventargs
      )
      $so.HaveData = $false
      $so.NeedData = $false
    }

    [System.Management.Automation.PSMethod]$event_opened = $control.Add_Opened
    [System.Management.Automation.PSMethod]$event_closed = $control.Add_Closed

    $event_opened.Invoke($handler_opened)
    $event_closed.Invoke($handler_closed)
    $target.ShowDialog() | Out-Null
  })

function send_text {
  param(
    $content,
    [switch]$append
  )

  # NOTE - host-specific method signature:
  $so.Indicator.Dispatcher.Invoke([System.Action]{
      $so.Indicator.Visibility = 'Collapsed'
    },'Normal')
  $so.Contents.Dispatcher.Invoke([System.Action]{

      if ($PSBoundParameters['append_content']) {
        $so.Contents.AppendText($content)
      } else {
        $so.Contents.Text = $content
      }
      $so.Result = $so.Contents.Text
    },'Normal')

}


$run_script.Runspace = $rs
Clear-Host

$handle = $run_script.BeginInvoke()
while (-not $handle.IsCompleted) {
  Start-Sleep -Milliseconds 100
  if ($so.NeedData -and -not $so.HaveData) {
    Write-Output ('Need to provide data')
    Start-Sleep -Milliseconds 10
    send_text -Content (Date)
    Write-Output ('Sent {0}' -f $so.Result)
    $so.HaveData = $true
  }
}
$run_script.EndInvoke($handle)
$rs.Close()

#Copyright (c) 2022,2023 Serguei Kouzmine
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


param (
  [String]$name = 'LoadAverageService',
  [String]$datafile = 'C:\temp\loadaverage.txt',
  [int]$command = 200,
  [switch]$eventlog,
  [switch]$clean,
  [switch]$debug
)

# safe read and write with retry for the "File in use by another process"
# based on: https://stackoverflow.com/questions/876473/is-there-a-way-to-check-if-a-file-is-in-use
$data_class = 'FileHelper'

if (-not ($data_class -as [type])) {
  # interpolate class name
  add-type -TypeDefinition @"
  using System;
  using System.IO;
  using System.Text;
  using System.Threading;

  public class $data_class {
    private int interval = 500;
    private string filePath = null;
    public int Retries { get; set; }
    public int Interval { set { interval = value; } get {return interval;}}
    public string FilePath { set { filePath = value; }  get {return filePath;}}
    public bool Debug { get; set; }
    public string Text { get; set; }
    private byte[] bytes;
    public byte[] Bytes { get { return bytes; } }

    private FileStream stream = null;

    public void WriteContents() {
      Boolean done = false;
      if (!string.IsNullOrEmpty(filePath)) {
        // Console.Error.WriteLine(String.Format("Writing data to {0}.", filePath));
        for (int cnt = 0; cnt != Retries; cnt++) {
          if (done)
            break;
          try {
            stream = new FileInfo(filePath).Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            bytes = Encoding.ASCII.GetBytes(Text);
            // stream.Lock(0, bytes.Length);
            // have to truncate
            stream.SetLength(0);
            if (Debug)
              Console.Error.WriteLine(String.Format("Writing text {0}.", Text));
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
            if (Debug)
              Console.Error.WriteLine(String.Format("Written text {0}.", Text));
            // stream.Unlock(0, bytes.Length);
            done = true;

          } catch (IOException e) {
            Console.Error.WriteLine(String.Format("Got Exception during Write: {0}. " + "Wait {1, 4:f2} sec and retry", e.Message, (interval / 1000F)));
          } finally {
            if (stream != null)
              stream.Close();
          }
          // wait and retry
          if (!done)
            Thread.Sleep(interval);
        }
      }
      return;
    }

    // retries if "File in use by another process"
    // because file is being processed by another thread
    public void ReadContents() {
      if (Debug)
        Console.Error.WriteLine(String.Format("Reading the file {0}", filePath));
      Text = null;
      Boolean done = false;
      if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath)) {
        for (int cnt = 0; cnt != Retries && Text == null; cnt++) {

          if (done)
            break;
          try {
            stream = new FileInfo(filePath).Open(FileMode.Open, FileAccess.Read, FileShare.None);

            int numBytesToRead = (int)stream.Length;
            if (numBytesToRead > 0) {
              bytes = new byte[numBytesToRead];
              int numBytesRead = 0;
              while (numBytesToRead > 0) {
                if (Debug)
                  Console.Error.WriteLine(String.Format("{0} bytes to read", numBytesToRead));
                int n = stream.Read(bytes, numBytesRead, numBytesToRead);
                if (n == 0)
                  break;

                numBytesRead += n;
                numBytesToRead -= n;
              }
              numBytesToRead = bytes.Length;
              if (bytes.Length > 0)
                Text = Encoding.ASCII.GetString(bytes);
              // the below call is race condition prone
              // text =  System.IO.File.ReadAllText(filePath);
              done = true;
            }
          } catch (IOException e) {
            if (Debug)
              Console.Error.WriteLine(String.Format("Got Exception during Read: {0}. " + "Wait {1, 4:f2} sec and retry", e.Message, (interval / 1000F)));
          } finally {
            if (Debug)
              Console.Error.WriteLine(String.Format("Read text \"{0}\". Retry: {1}", Text, cnt));
            if (stream != null)
              stream.Close();
          }
          // wait and retry
          if (Text == null) {
            if (Debug)
              Console.Error.WriteLine(String.Format("Wait {0, 4:f2} sec and retry: {1}", (interval / 1000F), cnt));
            Thread.Sleep(interval);
          }
        }
      }
      return;
    }
  }
"@
  # NOTE: the previous line with "@" marker should not be indented
}

### Main
$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent -bor $debug.ToBool()
$eventlog_flag = [bool]$PSBoundParameters['eventlog'].IsPresent -bor $eventlog.ToBool()
$clean_flag = [bool]$PSBoundParameters['clean'].IsPresent -bor $clean.ToBool()

[System.Reflection.Assembly]::LoadWithPartialName('System.ServiceProcess') | Out-Null
try {
  [System.ServiceProcess.ServiceController] $controller = new-object System.ServiceProcess.ServiceController($name)
}  catch [Exception] {
  $message = $_.Exception.Message
  write-host -foregroundcolor 'red' ('Exception: ' +  $message)
  # WindowsService.NET was not found on computer '.'
  # Cannot control WindowsService.NET service on computer '.'
}

$script_dir = $null
if ($PSScriptRoot -ne $null) {
  if ($debug_flag) {
    write-host ('$PSScriptRoot = {0}' -f $PSScriptRoot )
  }
  $script_dir = $PSScriptRoot
}
if ($PSCommandPath -ne $null) {
  $script_dir = ($PSCommandPath -replace '\\[^\\]*$' )
  if ($debug_flag) {
    write-host ('$PSCommandPath = {0}' -f $PSCommandPath )
  }
}
if ($debug_flag) {
  write-host ('$script_dir = {0}' -f $script_dir)
}
if ($script_dir -ne $null){
  pushd $script_dir
}
$execute_ok = $false
if ($controller -ne $null){
  if ( $controller.Status -eq 'Running') {
    $execute_ok = $true
  } else {
    write-host -foregroundcolor 'red' ('{0} not running' -f $name)
  }
} else {
  write-host -foregroundcolor 'red' ('{0} not found' -f $name)
}
if ($execute_ok) {
  if ($clean_flag){
    if (test-path -path $datafile) {
      write-host ('Removing file: {0}' -f $datafile )
      remove-item -path $datafile
      start-sleep -millisecond 1000
    }
  }
  try {
    write-host -foregroundcolor 'Green' ('Sending {0} to {1}' -f $command, $name)
    $controller.ExecuteCommand($command)
      start-sleep -millisecond 250

  } catch [Exception] {
    # WindowsService.NET was not found on computer '.'
    # Cannot control WindowsService.NET service on computer '.'
    $message = $_.Exception.Message
    write-host -foregroundcolor 'red' ('Exception: ' +  $message)
  }
}


write-host ('Check if file is present: {0}' -f $datafile )
if (test-path -path $datafile) {
  # NOTE: frequent error:
  # 32   The process cannot access the file because it is being used by another process.
  # when using "get-content" cmdlet
  # get-content -path $datafile
  $interval = 250
  $retries = 3
  $o = new-object -typeName $data_class
  $o.Retries = $retries
  $o.Interval = $interval
  $o.FilePath = $datafile
  $o.ReadContents()
  $data = $o.Text
  $entries = @{}
  # convert from regular to Powershell notation
  $separaror = '\r?\n' -replace '\\', '`'
  $pattern = 'LOAD(?:1|5|15)MIN'
  # write-host ('Read: "{0}"' -f $data )
  write-host 'executing select-string:'
  $data -split "`r?`n"| select-string -pattern $pattern
  # alternatively reuse the code from
  $pattern =  '^ *([^ ]*): *([^ ]*.*)$'

  [Microsoft.PowerShell.Commands.MatchInfo]$matchInfo = $null

  $data -split "`r?`n" |
  where-object {
    $line = $_
    # write-host ('matching line {0}' -f $line )
    $line -match $pattern
    } |
  foreach-object {
    $line = $_
    $_matches = select-string -pattern $pattern -InputObject $line
    $_group = $_matches.Matches.Groups
    $_key = $_group.Item(1).Value
    $_value = $_group.Item(2).Value
    $entries[$_key] = $_value
  }
  write-host ('Loaded entries:' + [char]10 + ( $entries | convertto-json ) -join '')
  if  ($eventlog_flag) {
    start-sleep -second 1
    .\tail_eventlog.ps1
  }
} else{
  write-host -foreground 'Red' ('file not found: {0}' -f $datafile)
}



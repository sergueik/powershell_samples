#Copyright (c) 2022,2023,2024 Serguei Kouzmine
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
# [CmdletBinding()]
param(
 # [parameter(Mandatory=$true,Position=1)] [ValidateScript({ Test-Path -PathType Leaf $_ })] [String] $datafile,
 [string] $line = 'somekey: somevalue',
 [switch] $passthru,
 [string] $datafile,
 [string] $key = 'test',
 [string] $value = 'data',
 [switch]$delete,
# NOTE: 
# A parameter with the name 'Debug' was defined multiple times for the command (??)
# see also: https://stackoverflow.com/questions/65700615/powershell-a-parameter-with-the-name-was-defined-multiple-times-for-the-command# ,
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

function readData {
  param(
    [String]$filepath,
    [int]$retries = 3,
    [bool]$debug
  )
  [String]$local:data = $null
  if ($debug) {
    write-host ('Check if file is present: {0}' -f $filepath )
  }
  if (test-path -path $filepath) {
    # NOTE: frequent error:
    # 32   The process cannot access the file because it is being used by another process.
    # when using "get-content" cmdlet
    if ($debug) {
      write-host ('File is present: {0}' -f $filepath )
    }
    try {
      if ($debug) {
        write-host ('Reading File: {0}' -f $filepath )
      }
      if ($debug) {
        get-content -path $filepath
      }
    } catch [exception]{
      Write-Output ("Exception: {0}" -f $_.Exception.Message)
    }

    $interval = 250
    $o = new-object -typeName $data_class
    $o.Debug = $debug
    $o.Retries = $retries
    $o.Interval = $interval
    $o.FilePath = $filepath
    if ($debug) {
      write-host ('Read file {0} safely' -f $o.FilePath )
    }
    $o.ReadContents()
    $local:data = $o.Text

    if ($debug) {
      write-host ('readData Data (raw):' + [char]10 + '"' + $local:data + '"' + [char]10)
    }
    $o = $null
  }
  return $local:data
}

function updateData {
  param(
    [String]$filepath,
    [System.Collections.Hashtable]$y = @{},
    [int]$retries = 3,
    [bool]$delete = $false,
    [bool]$debug
  )
  if ($y -eq $null -or $y.Keys.Count -eq 0 ){
    if ($debug) {
      write-host ('No data was provided')
    }
    return
  }
  if (test-path -path $filepath) {

    [System.Collections.Hashtable]$x = @{}
    [String]$local:data = readData -filepath  $filepath -debug $debug -retries $retries

    if ($debug) {
      write-host ('updateData Data (raw):' + [char]10 + '"' + $local:data + '"' + [char]10)
    }
    $pattern =  '^ *([^ ]*): *([^ ]*.*)$'

    [Microsoft.PowerShell.Commands.MatchInfo]$m = $null
    $local:data -split "`r?`n" |
    where-object {
      $line = $_
      if ($debug){ 
        write-host ('examine line {0}' -f $line )
      } 
      $line -match $pattern
    } |
    foreach-object {
      $line = $_
      $m = select-string -pattern $pattern -InputObject $line
      $g = $m.Matches.Groups
      $k = $g.Item(1).Value
      $v = $g.Item(2).Value
      $x[$k] = $v
    }
  
    if ($debug){
      write-host ('Loaded entries:' + [char]10 + ( $x | convertto-json ) -join '')
    }
  } else{
    write-host -foreground 'Red' ('file not found: {0}' -f $datafile)
    return
  }
  # NOTE: cannot use addition
  if ($delete){ 
    $y.Keys | foreach-object {
      $k = $_
      if ($debug ){
        write-host ('deleting {0}' -f $k)
      }
      if ($x.ContainsKey($k)){
        $x.Remove($k)
      }
    }
  } else {
    $y.Keys | foreach-object {
      $k = $_
      $v = $y[$k]
      if ($debug ){
        write-host ('adding {0} = {1}' -f $k, $v)
      }
      $x[$k] = $v
    }
  }

  [String[]]$result = @()
  $x.Keys | foreach-object {
    $k = $_
    $v = $x[$k]
    $result += ('{0}: {1}' -f $k,$v)
  }
  [String]$text = (($result -join "`r`n" ) + "`r`n")
  if ($debug){
    write-host ('Write data:' + [char]10 + $text)
  }
  $interval = 250
  $o = new-object -typeName $data_class
  $o.Debug = $debug
  $o.Retries = $retries
  $o.Interval = $interval
  $o.FilePath = $filepath
  $o.Text = $text
  $o.WriteContents()
  $o = $null
  return
}


function updateData.OLD {
  param(
    [String]$datafile,
    [System.Collections.Hashtable]$y = @{},
    [int]$retries = 3,
    [bool]$delete = $false,
    [bool]$debug
  )
  if ($debug) {
    write-host ('Check if data is provided')
  }
  if ($y -eq $null -or $y.Keys.Count -eq 0 ){
    if ($debug) {
      write-host ('No data was provided')
    }
    return
  }
  [System.Collections.Hashtable]$x = @{}
  [String]$data = $null
  if ($debug) {
    write-host ('Check if file is present: {0}' -f $datafile )
  }
  if (test-path -path $datafile) {
    # NOTE: frequent error:
    # 32   The process cannot access the file because it is being used by another process.
    # when using "get-content" cmdlet
    if ($debug) {
      write-host ('File is present: {0}' -f $datafile )
    }
    try {
      if ($debug) {
        write-host ('Reading File: {0}' -f $datafile )
      }
      if ($debug) {
        get-content -path $datafile
      }
    } catch [exception]{
      Write-Output ("Exception: {0}" -f $_.Exception.Message)
    }

    $interval = 250
    $o = new-object -typeName $data_class
    $o.Debug = $debug
    $o.Retries = $retries
    $o.Interval = $interval
    $o.FilePath = $datafile
    if ($debug) {
      write-host ('Read {0} safely' -f $o.FilePath )
    }
    $o.ReadContents()
    $data = $o.Text

    if ($debug) {
      write-host ('Data (raw):' + [char]10 + '"' + $data + '"' + [char]10)
    }
    $pattern =  '^ *([^ ]*): *([^ ]*.*)$'

    [Microsoft.PowerShell.Commands.MatchInfo]$m = $null
    $data -split "`r?`n" |
    where-object {
      $line = $_
      if ($debug){ 
        write-host ('examine line {0}' -f $line )
      } 
      $line -match $pattern
    } |
    foreach-object {
      $line = $_
      $m = select-string -pattern $pattern -InputObject $line
      $g = $m.Matches.Groups
      $k = $g.Item(1).Value
      $v = $g.Item(2).Value
      $x[$k] = $v
    }
    if ($debug){
      write-host ('Loaded entries:' + [char]10 + ( $x | convertto-json ) -join '')
    }
  } else{
    write-host -foreground 'Red' ('file not found: {0}' -f $datafile)
    return
  }
  # NOTE: cannot use addition
  if ($delete){ 
    $y.Keys | foreach-object {
      $k = $_
      if ($debug ){
        write-host ('deleting {0}' -f $k)
      }
      if ($x.ContainsKey($k)){
        $x.Remove($k)
      }
    }
  } else {
    $y.Keys | foreach-object {
      $k = $_
      $v = $y[$k]
      if ($debug ){
        write-host ('adding {0} = {1}' -f $k, $v)
      }
      $x[$k] = $v
    }
  }

  [String[]]$result = @()
  $x.Keys | foreach-object {
    $k = $_
    $v = $x[$k]
    $result += ('{0}: {1}' -f $k,$v)
  }
  [String]$text = (($result -join "`r`n" ) + "`r`n")
  if ($debug){
    write-host ('Write data:' + [char]10 + $text)
  }
  $o.Text = $text
  $o.WriteContents()
  return
}

function csvFields {
  param (
    [String]$filepath,
    [string]$fields = 'key1,key2,key3',
    [int]$retries = 3,
    [bool]$debug
  )

    [String]$local:data = readData -filepath  $filepath -debug $debug -retries $retries
    [System.Collections.Hashtable]$x = @{}
    if ($debug) {
      write-host ('Data (raw):' + [char]10 + '"' + $local:data + '"' + [char]10)
    }
    $pattern =  '^ *([^ ]*): *([^ ]*.*)$'

    [Microsoft.PowerShell.Commands.MatchInfo]$m = $null
    $data -split "`r?`n" |
    where-object {
      $line = $_
      if ($debug){ 
        write-host ('examine line {0}' -f $line )
      } 
      $line -match $pattern
    } |
    foreach-object {
      $line = $_
      $m = select-string -pattern $pattern -InputObject $line
      $g = $m.Matches.Groups
      $k = $g.Item(1).Value
      $v = $g.Item(2).Value
      $x[$k] = $v
    }
    if ($debug){
      write-host ('Loaded entries:' + [char]10 + ( $x | convertto-json ) -join '')
    }

  $values = @()
  ($fields -split ',') | 
  foreach-object {
    $k = $_
    if ($x.ContainsKey($k)) {
      $v = $x[$k]
    } else {
      $v = ''
    }
    $values += $v
  }
  return ( $values -join ',')
}

function passthru {
  param (
    [string] $line = 'somekey: somevalue',
    [string] $filepath,
    [bool]$debug
  )

  [System.Collections.Hashtable]$y = @{}
  # alternatively reuse the code from update_data itself
  $pattern =  '^ *([^ ]*): *([^ ]*.*)$'

  if ($debug){
    write-host ('examine passhhru line {0}' -f $line )
  } 
  if ( $line -match $pattern ) {

    $m = select-string -pattern $pattern -InputObject $line
    $g = $m.Matches.Groups
    $k = $g.Item(1).Value
    $v = $g.Item(2).Value
    if ($debug){
      write-host('key: {0}; value: {1}' -f $k, $v)
    }
    $y[$k] = $v
    updateData -filepath $filepath -y $y -debug $debug_flag -delete $delete_flag
  }
}
### Main
# You cannot call a method on a null-valued expression.
# this happen if the annotation [CmdletBinding()] is used and one cannot explicitly
# $debug_flag = [bool]$PSBoundParameters['debug'].IsPresent -bor $DebugPreference -eq 'Continue'

$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent -bor $debug.ToBool()
$delete_flag = [bool]$PSBoundParameters['delete'].IsPresent -bor $delete.ToBool()
$passthru_flag = [bool]$PSBoundParameters['passthru'].IsPresent -bor $passthru.ToBool()

write-host ('updateData -filepath {0} -y @{{{1} = {2}}} -debug {3} -delete {4}' -f $datafile,$key,$value,$debug_flag,$delete_flag )

updateData -filepath $datafile -y @{$key = $value} -debug $debug_flag -delete $delete_flag
<#
. .\updatedata.ps1 -datafile ((resolve-path '.' ).path + '\' + 'data.txt') -key 'foo' -value 'bar42' -debug -delete
#>

if ($passthru_flag){
  passthru -line $line -debug $debug_flag -filepath $datafile
 <#
  . .\updatedata.ps1 -datafile ((resolve-path '.' ).path + '\' + 'data.txt') -line 'foo: bar2' -debug -passthru
  #>
}

# csvFields -debug $true -filepath $datafile
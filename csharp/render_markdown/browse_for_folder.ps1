#Copyright (c) 2025 Serguei Kouzmine
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
# converted from http://www.java2s.com/Tutorial/CSharp/0460__GUI-Windows-Forms/AsimpleBrowser.htm
# http://www.java2s.com/Code/CSharpAPI/System.Windows.Forms/TabControlControlsAdd.htm
# with sizes adjusted to run the focus demo
# see also:
# https://stackoverflow.com/questions/17926197/open-local-file-in-system-windows-forms-webbrowser-control
# http://www.java2s.com/Tutorial/CSharp/0460__GUI-Windows-Forms/AsimpleBrowser.htm
# https://www.c-sharpcorner.com/UploadFile/mahesh/webbrowser-control-in-C-Sharp-and-windows-forms/
param (
  [string]$filename = 'README.md'
)

# origin: https://4sysops.com/archives/how-to-create-an-open-file-folder-dialog-box-with-powershell/

if (($filename -eq $null ) -or ($filename -eq '' ) -or ( -not (test-path -path $filename ))){
  $initial_directory = ( resolve-path '.' ).Path
  $filter_expression = 'Markdown Documents (*.md)|*.md|All files (*.*)| *.*'
  add-type -AssemblyName 'System.Windows.Forms'
  $o = new-object System.Windows.Forms.OpenFileDialog -Property @{
    InitialDirectory = $initial_directory
    Filter = $filter_expression
  }
  $null = $o.ShowDialog()
  $filename = $o | select-object -expandproperty FileName
}
if ($filename -ne $null) {
  write-output ('selected filename: {0}' -f $filename)
}

#Copyright (c) 2014,2025 Serguei Kouzmine
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

# http://www.java2s.com/Code/CSharpAPI/System.Windows.Forms/TabControlControlsAdd.htm
# with sizes adjusted to run the focus demo
# see also:
# https://stackoverflow.com/questions/17926197/open-local-file-in-system-windows-forms-webbrowser-control
# http://www.java2s.com/Tutorial/CSharp/0460__GUI-Windows-Forms/AsimpleBrowser.htm
param (
  [string]$filename = 'README.md'
)
function assembly_is_loaded{
  param(
    [string[]]$defined_type_names = @(),
    [string]$assembly_path
  )

  $loaded_project_specific_assemblies = @()
  $loaded_defined_type_names = @()

  if ($defined_type_names.count -ne 0) {
    $loaded_defined_type_names = [appdomain]::currentdomain.getassemblies() |
        where-object {$_.location -eq ''} |
        select-object -expandproperty DefinedTypes |
        select-object -property Name
    # TODO: return if any of the types from Add-type is already there
    return ($loaded_defined_type_names -contains $defined_type_names[0])
  }

  if ($assembly_path -ne $null) {
    [string]$check_assembly_path = ($assembly_path -replace '\\\\' , '/' ) -replace '/', '\'
    # NOTE: the location property may both be $null or an empty string
    $loaded_project_specific_assemblies =
    [appdomain]::currentdomain.getassemblies() |
      where-object {$_.GlobalAssemblyCache -eq $false -and $_.Location -match '\S' } |
      select-object -expandproperty Location
      # write-debug ('Check if loaded: {0} {1}' -f $check_assembly_path,$assembly_path)
    write-debug ("Loaded asseblies:  {0}" -f $loaded_project_specific_assemblies.count)
    if ($DebugPreference -eq 'Continue') {
     if (($loaded_project_specific_assemblies -contains $check_assembly_path)) {
        write-debug ('Already loaded: {0}' -f $assembly_path)
      } else {
        write-debug ('Not loaded: {0}' -f $assembly_path)
      }
    }
    return ($loaded_project_specific_assemblies -contains $assembly_path)
  }
}

function load_shared_assemblies {

  param(
    [string]$shared_assemblies_path = 'C:\java\selenium\csharp\sharedassemblies',
    [string[]]$shared_assemblies = @(
      'Markdig.dll'
      )
  )

  write-debug ('Loading "{0}" from ' -f ($shared_assemblies -join ',' ), $shared_assemblies_path)
  pushd $shared_assemblies_path

  $shared_assemblies | ForEach-Object {
    $shared_assembly_filename = $_
    if ( assembly_is_loaded -assembly_path ("${shared_assemblies_path}\\{0}" -f $shared_assembly_filename)) {
      write-debug ('Skipping from  assembly "{0}"' -f $shared_assembly_filename)
     } else {
      write-debug ('Loading assembly "{0}" ' -f $shared_assembly_filename)
      Unblock-File -Path $shared_assembly_filename;
      Add-Type -Path $shared_assembly_filename
    }
  }
  popd
}

load_shared_assemblies


Add-Type -TypeDefinition @"

// "
using System;
using System.Text;
using System.Windows.Forms;
using Markdig;

using System.Runtime.InteropServices;

public class Win32Window : IWin32Window {
	private IntPtr _hWnd;
	private string _payload;
	public string Payload {
		get { return _payload; }
		set { _payload = value; }
	}

	public Win32Window(IntPtr handle)
	{
		_hWnd = handle;
	}

	public IntPtr Handle {
		get { return _hWnd; }
	}

	public string convert(string payload) {
		var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
		var result = Markdown.ToHtml(payload, pipeline);
		return result;
	}

}

"@ -ReferencedAssemblies 'System.Windows.Forms.dll','System.Runtime.InteropServices.dll','C:\java\selenium\csharp\sharedassemblies\Markdig.dll'

function render {

param(
  [object]$caller = $null
)
  @( 'System.Drawing','System.Collections','System.ComponentModel','System.Windows.Forms','System.Data') | ForEach-Object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }

  $f = new-object System.Windows.Forms.Form
  $f.Text = $title

  $f.SuspendLayout()

  $b = new-object System.Windows.Forms.WebBrowser
  $f.SuspendLayout();

  $b.Dock = [System.Windows.Forms.DockStyle]::Fill
  $b.Location = new-object System.Drawing.Point (0,0)
  $b.Name = 'webBrowser1'
  $b.Size = new-object System.Drawing.Size (600,600)
  $b.TabIndex = 0

  $f.AutoScaleDimensions = new-object System.Drawing.SizeF (6,13)
  $f.AutoScaleMode = [System.Windows.Forms.AutoScaleMode]::Font
  $f.ClientSize = new-object System.Drawing.Size (600,600)
  $f.Controls.Add($b)
  $f.Text = 'Render Markdoown'
  $f.ResumeLayout($false)
  $f.add_Load({
      param([object]$sender,[System.EventArgs]$eventArgs)
        [String]$html = $caller.convert($caller.payload)
        $b.DocumentText = $html
    })

  $f.ResumeLayout($false)
  $f.Topmost = $True

  $f.Add_Shown({ $f.Activate() })

  # TODO: debug Exception 
  # calling "ShowDialog" with "1" argument(s): "The handle is invalid"

  [void]$f.ShowDialog([win32window]($caller))
  $b.Dispose()
}

if (($filename -eq $null ) -or ($filename -eq '' ) -or (-not (test-path -path $filename ))){
  # NOTE: format
  $initial_directory = ( resolve-path '.' ).Path
  $filter_expression = 'Markdown Documents (*.md)|*.md|All files (*.*)| *.*'
  add-type -AssemblyName 'System.Windows.Forms'
  $o = new-object System.Windows.Forms.OpenFileDialog -Property @{
    InitialDirectory = $initial_directory
    Filter = $filter_expression
  }
  $null = $o.ShowDialog()
  $filename = $o | select-object -expandproperty FileName
  if ($filename -ne $null) {
    write-output ('selected filename: {0}' -f $filename)
  }
}

$caller = new-object Win32Window -ArgumentList ([System.Diagnostics.Process]::GetCurrentProcess().MainWindowHandle)

$payload = ( get-content -path $filename -encoding utf8 ) -join "`r`n"

$caller.payload = $payload
[String]$html = $caller.convert($caller.payload)

render -caller $caller

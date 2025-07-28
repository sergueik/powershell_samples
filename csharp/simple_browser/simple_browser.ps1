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
using System.Net;
using System.Windows.Forms;
using Markdig;

using System.Runtime.InteropServices;

public class Win32Window : IWin32Window
{
    private IntPtr _hWnd;
    private string _cookies;
    private string _url;

    public string Cookies
    {
        get { return _cookies; }
        set { _cookies = value; }
    }

    public string Url
    {
        get { return _url; }
        set { _url = value; }
    }

    public Win32Window(IntPtr handle)
    {
        _hWnd = handle;
    }

    public IntPtr Handle
    {
        get { return _hWnd; }
    }

    [DllImport("wininet.dll", SetLastError = true)]
    public static extern bool InternetGetCookieEx(
        string url,
        string cookieName,
        StringBuilder cookieData,
        ref int size,
        Int32 dwFlags,
        IntPtr lpReserved);

    private const int INTERNET_COOKIE_HTTPONLY = 0x00002000;
    private const int INTERNET_OPTION_END_BROWSER_SESSION = 42;

    public string GetGlobalCookies(string uri) {
        int datasize = 1024;
        StringBuilder cookieData = new StringBuilder((int)datasize);
        if (InternetGetCookieEx(uri, null, cookieData, ref datasize, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero)
            && cookieData.Length > 0) {
            return cookieData.ToString().Replace(';', ',');
        } else {
            return null;
        }
    }

  // https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.webbrowser?view=netframework-4.0
// Navigates to the given URL if it is valid.
/*
private void Navigate(String address) {
    var prefix = "http://";
    if (String.IsNullOrEmpty(address)) return;
    if (address.Equals("about:blank")) return;
    if (!address.StartsWith(prefix)) {
        address = prefix + address;
    }
    try {
        webBrowser1.Navigate(new Uri(address));
    } catch (System.UriFormatException) {
        return;
    }
}
*/

		public string convert(string payload) {
			var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
			var result = Markdown.ToHtml(payload, pipeline);
			return result;
		}

// Updates the URL in TextBoxAddress upon navigation.
/*
private void Navigated(object sender, WebBrowserNavigatedEventArgs e) {
    toolStripTextBox1.Text = webBrowser1.Url.ToString();
}
*/
}

"@ -ReferencedAssemblies 'System.Windows.Forms.dll','System.Runtime.InteropServices.dll','System.Net.dll', 'C:\java\selenium\csharp\sharedassemblies\Markdig.dll'

function promptForContinueWithCookies (
  [string]$login_url = $null,
  [object]$caller = $null
)
{
  @( 'System.Drawing','System.Collections','System.ComponentModel','System.Windows.Forms','System.Data') | ForEach-Object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }

  $f = new-object System.Windows.Forms.Form
  $f.Text = $title

  $timer1 = new-object System.Timers.Timer
  $label1 = new-object System.Windows.Forms.Label

  $f.SuspendLayout()
  $components = new-object System.ComponentModel.Container

  $browser = new-object System.Windows.Forms.WebBrowser
  $f.SuspendLayout();

  # webBrowser1
  $browser.Dock = [System.Windows.Forms.DockStyle]::Fill
  $browser.Location = new-object System.Drawing.Point (0,0)
  $browser.Name = "webBrowser1"
  $browser.Size = new-object System.Drawing.Size (600,600)
  $browser.TabIndex = 0
  # Form1 
  $f.AutoScaleDimensions = new-object System.Drawing.SizeF (6,13)
  $f.AutoScaleMode = [System.Windows.Forms.AutoScaleMode]::Font
  $f.ClientSize = new-object System.Drawing.Size (600,600)
  $f.Controls.Add($browser)
  $f.Text = "Login to octopus"
  $f.ResumeLayout($false)



  $f.add_Load({
      param([object]$sender,[System.EventArgs]$eventArgs)
      $browser.Navigate($login_url)
$data = @'
### Info


[Markdown](https://www.markdownguide.org/basic-syntax/) renderer relying on Markdown-to-HTML converter [Markdig](https://github.com/xoofx/markdig) added as nuget dependency
[Markdig](https://www.nuget.org/packages/Markdig) version __0.15.0__ to perform Markdown to HTML conversion then render in
 [WebBrowser1](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/webbrowser-control-overview) ActiveX control embedded in an Windows Forms or in WPF through `System.Windows.Forms.WebBrowser`

| dependency  | version  |         |
|------------------------|---------|
| Markdig     | 0.15.0   |         |
|             |          |         |

### Usage

* make sure  to place `Markdig.dll` into default assembly cache directory `c:\java\selenium\csharp\sharedassemblies`
```powershell
simple_browser_localfile.ps1 -file <MARKDOWN>
```
or

```powershell
simple_browser_localfile.ps1 -browse
```
to have file dialog rendered (WIP)


![form](screenshots/form.png)

### See Also
   * `nuget.exe` [download](https://dist.nuget.org/win-x86-commandline/v2.8.6/nuget.exe). SharpDevelop installs one under `c:\Program Files\SharpDevelop\4.4\AddIns\Misc\PackageManagement` (`c:\Program Files (x86)\SharpDevelop\5.1\AddIns\Misc\PackageManagement" on 64 bit host) 
  * tls 1.2 issue  (*The request was aborted: Could not create SSL/TLS secure channel.*) [fix](https://stackoverflow.com/questions/58993743/could-not-create-ssl-tls-secure-channel-while-trying-to-search-for-nuget-package)
  * [CommonMark.NET](https://github.com/Knagis/CommonMark.NET) nuget dependency [CommonMark.NET](https://www.nuget.org/packages/CommonMark.NET) - lacks table support
  * https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.webbrowser?view=netframework-4.8

### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)





'@
[String]$html = $caller.convert($data) 
$browser.DocumentText = $html
    })

  $browser.Add_Navigated(
    {

      param([object]$sender,[System.Windows.Forms.WebBrowserNavigatedEventArgs]$eventArgs)
      # wait for the user to successfully log in 
      # then capture the global cookies and sent to $caller
      $url = $browser.Url.ToString()
      if ($caller -ne $null -and $url -ne $null -and $url -match $caller.Url) {
        $caller.Cookies = $caller.GetGlobalCookies($url)
      }
    }
  )

  $f.ResumeLayout($false)
  $f.Topmost = $True

  $f.Add_Shown({ $f.Activate() })

  [void]$f.ShowDialog([win32window]($caller))
  $browser.Dispose() 
}

$caller = new-object Win32Window -ArgumentList ([System.Diagnostics.Process]::GetCurrentProcess().MainWindowHandle)
$service_host = 'http://localhost:8088'
$login_route = 'app#/users/sign-in'
$login_url = ('{0}/{1}' -f $service_host,$login_route)

$caller.Url = 'app#/environments'
promptForContinueWithCookies $login_url $caller

Write-Host ("{0}->{1}" -f $caller.Url,$caller.Cookies)

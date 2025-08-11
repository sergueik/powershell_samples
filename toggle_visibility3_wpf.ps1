#Copyright (c) 2023,2025 Serguei Kouzmine
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

# converted from textbook example in C#
#requires -version 2
Add-Type -AssemblyName PresentationFramework
    # XAML for the second dialog
<#
NOTE: Exception calling "Load" with "1" argument(s): "Specified class name
'SecondDialog' doesn't match actual root instance type
'System.Windows.Window'. Remove the Class directive or provide an instance via
XamlObjectWriterSettings.RootObjectInstance."

#>

[xml]$xaml2 =
@'

<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Second Dialog" Height="200" Width="300">
  <Grid>
    <TextBlock Text="This is the second dialog!" HorizontalAlignment="Center" VerticalAlignment="Center"/>
  </Grid>
 </Window>
'@
[xml]$xaml =
@'
<Window 
	        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="IsVisible Example" Height="130" Width="300">
<StackPanel Margin="2" Orientation="Vertical">
        <Button x:Name="toggleButton" Content="Create new dialog" />
    </StackPanel>
</Window>
'@

clear-host
$reader = (New-Object System.Xml.XmlNodeReader $xaml)
$target = [Windows.Markup.XamlReader]::Load($reader)

  @( 'System.Drawing','System.Collections.Generic','System.Collections','System.ComponentModel','System.Text','System.Data','System.Windows.Forms') | ForEach-Object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }
$toggleButton = $target.FindName("toggleButton")
# TODO: log
# $statusTextBlock = $target.FindName("statusTextBlock")
# You cannot call a method on a null-valued expression.
$global:secondWindow = $null
# add extra method to be called
$eventMethod = $toggleButton.add_click
$eventMethod.Invoke({
  param(
    [object]$sender,
    [System.Windows.RoutedEventArgs]$eventargs
  )
  if ($global:secondWindow ) {
    write-host 'second window: ' + $global:secondWindow.Visibility
    write-host 'second window: ' + $global:secondWindow.WindowState
  }
  # https://learn.microsoft.com/en-us/dotnet/api/system.windows.visibility?view=netframework-4.8
  # https://learn.microsoft.com/en-us/dotnet/api/system.windows.windowstate?view=netframework-4.8
  if (( -not $global:secondWindow ) -or ($global:secondWindow.WindowState -ne [System.Windows.WindowState]::Normal) ) {
    $reader2 = (New-Object System.Xml.XmlNodeReader $xaml2)
    # TODO: 
    # $reader2 = (New-Object System.Xml.XmlNodeReader (New-Object System.Xml.XmlDocument)).LoadXml($xamlSecondDialog)
    $global:secondWindow = [Windows.Markup.XamlReader]::Load($reader2)

    # Show the second dialog modally
    # $secondWindow.ShowDialog()
    # Show the second dialog 
    $global:secondWindow.Show()
    $global:secondWindow.Add_Closing({
      $global:secondWindow = $null
    })
  }
})
 
$target.ShowDialog() | out-null


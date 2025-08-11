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
[xml]$xaml =
@'
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="IsVisible Example" Height="130" Width="300">
  <StackPanel Margin="2" Orientation="Vertical">
    <Button x:Name="toggleButton" Content="Toggle Visibility" />
    <TextBlock x:Name="myTextBlock" Text="This text's visibility will be toggled." Margin="0,10,0,0"/>
    <TextBlock x:Name="statusTextBlock" Margin="0,10,0,0"/>
  </StackPanel>
  <!--   
  NOTE: creating the Click="ToggleButton_Click" attribute on Button element leads to the following:
  Exception calling "Load" with "1" argument(s): "Failed to create a 'Click' from the text 'ToggleButton_Click'."
https://stackoverflow.com/questions/7665401/dynamically-adding-event-handlers-to-wpf
  http://social.msdn.microsoft.com/Forums/vstudio/en-US/753be353-a2f9-4b29-ba75-cd962a7260a4/dynamic-xaml-and-attaching-event-handlers-problem-wpf-4?forum=wpf
  -->
   <x:Code>
<!--
    public void ToggleButton_Click(object sender, RoutedEventArgs e) {
      // Toggle the Visibility property
      myTextBlock.Visibility = (myTextBlock.Visibility == Visibility.Visible) ?
                                Visibility.Collapsed : Visibility.Visible;
      UpdateStatus();
    }

    private void UpdateStatus() {
      // update the status TextBlock text reporting the IsVisible property
      if (myTextBlock.IsVisible)
      {
          statusTextBlock.Text = "TextBlock is currently visible.";
      } else {
          statusTextBlock.Text = "TextBlock is currently NOT visible.";
      }
    }
-->  </x:Code>
   
</Window>
'@

clear-host
$reader = (New-Object System.Xml.XmlNodeReader $xaml)
$target = [Windows.Markup.XamlReader]::Load($reader)
<#
NOTE: 
  when specifying the class attribute as in copied source:
   <Window x:Class="WpfApplication1.Window1" ... 
  the $target = ... leads to
  Exception calling "Load" with "1" argument(s): "Specified class name 'WpfApplication1.Window1' doesn't match actual root instance type 'System.Windows.Window'. Remove the Class directive or provide an instance via XamlObjectWriterSettings.RootObjectInstance."
  can drop the attribute
#>

  @( 'System.Drawing','System.Collections.Generic','System.Collections','System.ComponentModel','System.Text','System.Data','System.Windows.Forms') | ForEach-Object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }
$toggleButton = $target.FindName("toggleButton")
$myTextBlock = $target.FindName("myTextBlock")
$statusTextBlock = $target.FindName("statusTextBlock")
# You cannot call a method on a null-valued expression.
$passwordBox = $target.FindName("passwordBox")
# add extra method to be called
$eventMethod = $toggleButton.add_click
$eventMethod.Invoke({
  param(
    [object]$sender,
    [System.Windows.RoutedEventArgs]$eventargs
  )
    write-host $myTextBlock.Visibility
    # NOTE: ternary in Powershell 7.x
    # $myTextBlock.Visibility = ($myTextBlock.Visibility -eq [System.Windows.Visibility]::Visible) ? [System.Windows.Visibility]::Collapsed : [System.Windows.Visibility]::Visible;
    if ( $myTextBlock.IsVisible -or  ($myTextBlock.Visibility -eq [System.Windows.Visibility]::Visible) ) {
        $statusTextBlock.Text = "TextBlock is currently NOT visible.";
        $myTextBlock.Visibility = [System.Windows.Visibility]::Collapsed;
    } else  {
        $statusTextBlock.Text = "TextBlock is currently visible.";
        $myTextBlock.Visibility = [System.Windows.Visibility]::Visible;
    }
      
})
 
$target.ShowDialog() | out-null


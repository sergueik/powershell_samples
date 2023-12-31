#Copyright (c) 2023 Serguei Kouzmine
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
# origin: 
# http://www.java2s.com/Tutorial/CSharp/0470__Windows-Presentation-Foundation/DisplayaPasswordEntryBoxandgettheinput.htm
# see also: https://stackoverflow.com/questions/10091466/showing-password-characters-on-some-event-for-passwordbox
#requires -version 2
Add-Type -AssemblyName PresentationFramework
[xml]$xaml =
@'
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" x:Class="System.Windows.Window" Title="WPF" Height="100" Width="300">
  <StackPanel Orientation="Horizontal">
    <TextBlock Margin="5" VerticalAlignment="Center">
            Enter Password:
        </TextBlock>
    <PasswordBox Name="passwordBox" PasswordChar="ӿ" VerticalAlignment="Center" Width="150"/>
    <Button Content="OK" IsDefault="True" Margin="5" Name="button1" VerticalAlignment="Center"/>
  </StackPanel>
  <!--   
  NOTE: creating the Click="button1_Click" attribute on Button element leads to the following:
  Exception calling "Load" with "1" argument(s): "Failed to create a 'Click' from the text 'Button1_Click'."
  http://social.msdn.microsoft.com/Forums/vstudio/en-US/753be353-a2f9-4b29-ba75-cd962a7260a4/dynamic-xaml-and-attaching-event-handlers-problem-wpf-4?forum=wpf

  <x:Code>
    private void button1_Click(object sender, RoutedEventArgs e){
      MessageBox.Show("Password entered: " + passwordBox.Password,Title);
    }
  </x:Code>
   -->
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
$control = $target.FindName("button1")
# You cannot call a method on a null-valued expression.
$passwordBox = $target.FindName("passwordBox")
# add extra method to be called
$eventMethod = $control.add_click
$eventMethod.Invoke({
  param(
    [object]$sender,
    [System.Windows.RoutedEventArgs]$eventargs
  )
  [System.Windows.Forms.MessageBox]::Show("Password entered: " + $passwordBox.Password)
  $target.Close()
})
 
$target.ShowDialog() | out-null


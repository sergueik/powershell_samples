#Copyright (c) 2014,2024 Serguei Kouzmine
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
  [string]$sentence = 'Lorem ipsum dolor sit amet, consectetur adipisicing elit'  
)
function rotate_message {
  param(
    [string] $sentence = 'Lorem ipsum dolor sit amet, consectetur adipisicing elit',
    [string] $word = 'ipsum'
  )
  $parts = @($sentence -split $word)
  # TODO: check if found
  return $word + $parts[1] + ' ' + $parts[0]
}
Add-Type -TypeDefinition @'

using System;
using System.Windows.Forms;
public class Win32Window : IWin32Window {
    private IntPtr handle;
    private int data;
    private string message;

    public int Data {
        get { return data; }
        set { data = value; }
    }
    public string Message {
        get { return message; }
        set { message = value; }
    }

    public Win32Window(IntPtr handle) {
        this.handle = handle;
    }

    public IntPtr Handle {
        get { return handle; }
    }
}

'@ -ReferencedAssemblies 'System.Windows.Forms.dll'

# http://www.java2s.com/Code/CSharp/GUI-Windows-Form/CheckedListBoxItemCheckevent.htm

function PromptCheckedList {
  Param(
    [String] $title, 
    [String] $message
  )
  [string]$rotated_message = ''
  [void] [System.Reflection.Assembly]::LoadWithPartialName('System.Drawing') 
  [void] [System.Reflection.Assembly]::LoadWithPartialName('System.Windows.Forms')
  $f = new-object System.Windows.Forms.Form 
  $f.Text = $title

  $i = new-object System.Windows.Forms.CheckedListBox
  $d = new-object System.Windows.Forms.ListBox
  $d.SuspendLayout()
  $i.SuspendLayout()
  $f.SuspendLayout()
  $i.Font = new-object System.Drawing.Font('Microsoft Sans Serif', 11, [System.Drawing.FontStyle]::Regular, [System.Drawing.GraphicsUnit]::Point, 0)
  $i.FormattingEnabled = $true
  $i.Items.AddRange(( $message -split '[ ,]+' ))

  $i.Location = new-object System.Drawing.Point(17, 12)
  $i.Name = 'inputCheckedListBox'
  $i.Size = new-object System.Drawing.Size(202, 188)
  $i.TabIndex = 0
  $i.TabStop = $false
  $event_handler = {  
    param(
      [Object] $sender, 
      [System.Windows.Forms.ItemCheckEventArgs ] $eventargs 
    )

    $word = $i.SelectedItem

    $rotated_message = rotate_message -word $word -sentence $message
    # write-host ('rotated_message: "{0}"' -f $rotated_message)
    if ( $eventargs.NewValue -eq  [System.Windows.Forms.CheckState]::Checked ) {
      $d.Items.Add( $rotated_message)
    } else {
      $d.Items.Remove( $rotated_message )
    }
  }
  $i.Add_ItemCheck($event_handler) 

  $d.Font = new-object System.Drawing.Font('Verdana', 11)
  $d.FormattingEnabled = $true
  $d.ItemHeight = 20
  $d.Location =  new-object System.Drawing.Point(236, 12)
  $d.Name = 'displayListBox'
  $d.Size = new-object System.Drawing.Size(230, 184)
  $d.TabIndex = 1

  $b  = new-object System.Windows.Forms.Button
  $b.Location = new-object System.Drawing.Point(8, 280)
  $b.Name = 'button1'
  $b.Size = new-object System.Drawing.Size(112, 32)
  $b.TabIndex = 4
  $b.Text = 'Done'


  $b.Add_Click({
    $shapes = @()    
    foreach ($o in $d.Items){
      $shapes += $o
    } 
    $caller.Message =  [String]::Join(';', $shapes )
    $f.Close()
 })
     
  $f.AutoScaleBaseSize = new-object System.Drawing.Size(5, 13)
  $f.ClientSize = new-object System.Drawing.Size(478, 317)
  $components =  new-object System.ComponentModel.Container

  $f.Controls.AddRange( @( $i, $d, $b))

  $f.Name = 'Form1'
  $f.Text = 'CheckListBox Sample'
  $i.ResumeLayout($false)
  $d.ResumeLayout($false)

  $f.ResumeLayout($false)

  $f.StartPosition = 'CenterScreen'

  $f.KeyPreview = $true

  $f.Topmost = $true
  $caller = new-object Win32Window -ArgumentList ([System.Diagnostics.Process]::GetCurrentProcess().MainWindowHandle)

  $f.Add_Shown( { $f.Activate() } )

  [Void] $f.ShowDialog([Win32Window] ($caller) )
  $f.Dispose()
  $result = $caller.Message
  $caller = $null
  return $result
}

$DebugPreference = 'Continue'
$result = PromptCheckedList '' $sentence

write-debug ('Selection is : {0}' -f  , $result )

# . .\check_item_list.ps1 -sentence 'quick fox jumped over lazy dog'
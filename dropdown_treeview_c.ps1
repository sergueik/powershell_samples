#Copyright (c) 2014,2026 Serguei Kouzmine
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

# https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.iwin32window?view=netframework-4.5
Add-Type -TypeDefinition @'
using System;
using System.Windows.Forms;
public class Win32Window : IWin32Window
{
    private IntPtr _hWnd;
    private int _data;
    private string _script_directory;
    private string _message;

    public int Data
    {
        get { return _data; }
        set { _data = value; }
    }
    public string ScriptDirectory
    {
        get { return _script_directory; }
        set { _script_directory = value; }
    }
    public string Message
    {
        get { return _message; }
        set { _message = value; }
    }

    public Win32Window(IntPtr handle)
    {
        _hWnd = handle;
    }

    public IntPtr Handle
    {
        get { return _hWnd; }
    }
}

'@ -ReferencedAssemblies 'System.Windows.Forms.dll'
# see also: 
# https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_type_operators?view=powershell-5.1
if ( -not ('DropDownTreeView.DropDownTreeNode' -as [type]) ) {
Add-Type -typeDefinition @'

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.Serialization;

// origin: http://www.codeproject.com/Articles/14544/A-TreeView-Control-with-ComboBox-Dropdown-Nodes
namespace DropDownTreeView
{
    // https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.treenode?view=netframework-4.5
    public class DropDownTreeNode : TreeNode
    {
        public DropDownTreeNode()
            : base()
        {
        }

        public DropDownTreeNode(string text)
            : base(text)
        {
        }

        public DropDownTreeNode(string text, TreeNode[] children)
            : base(text, children)
        {
        }

        public DropDownTreeNode(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }

        public DropDownTreeNode(string text, int imageIndex, int selectedImageIndex)
            : base(text, imageIndex, selectedImageIndex)
        {
        }

        public DropDownTreeNode(string text, int imageIndex, int selectedImageIndex, TreeNode[] children)
            : base(text, imageIndex, selectedImageIndex, children)
        {
        }

        private ComboBox m_ComboBox = new ComboBox();
        public ComboBox ComboBox
        {
            get
            {
                this.m_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                return this.m_ComboBox;
            }
            set
            {
                this.m_ComboBox = value;
                this.m_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            }
        }
    }

    public class DropDownTreeView : TreeView
    {
        public DropDownTreeView()
            : base()
        {
        }
        private DropDownTreeNode m_CurrentNode = null;


        protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            // Are we dealing with a dropdown node?
            if (e.Node is DropDownTreeNode)
            {
                this.m_CurrentNode = (DropDownTreeNode)e.Node;

                // Need to add the node's ComboBox to the TreeView's list of controls for it to work
                this.Controls.Add(this.m_CurrentNode.ComboBox);

                // Set the bounds of the ComboBox, with a little adjustment to make it look right
                this.m_CurrentNode.ComboBox.SetBounds(
                    this.m_CurrentNode.Bounds.X - 1,
                    this.m_CurrentNode.Bounds.Y - 2,
                    this.m_CurrentNode.Bounds.Width + 25,
                    this.m_CurrentNode.Bounds.Height);

                // Listen to the SelectedValueChanged event of the node's ComboBox
                this.m_CurrentNode.ComboBox.SelectedValueChanged += new EventHandler(ComboBox_SelectedValueChanged);
                this.m_CurrentNode.ComboBox.DropDownClosed += new EventHandler(ComboBox_DropDownClosed);

                // Now show the ComboBox
                this.m_CurrentNode.ComboBox.Show();
                this.m_CurrentNode.ComboBox.DroppedDown = true;
            }
            base.OnNodeMouseClick(e);
        }

        void ComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            HideComboBox();
        }

        void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            HideComboBox();
            // MessageBox.Show(this.SelectedNode.Text.ToString());
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            HideComboBox();
            base.OnMouseWheel(e);
        }

        private void HideComboBox()
        {
            if (this.m_CurrentNode != null)
            {
                // Unregister the event listener
                // https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.combobox.selectedindexchanged?view=netframework-4.5
                this.m_CurrentNode.ComboBox.SelectedValueChanged -= ComboBox_SelectedValueChanged;
                // https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.combobox.dropdownclosed?view=netframework-4.5
                this.m_CurrentNode.ComboBox.DropDownClosed -= ComboBox_DropDownClosed;

                // Copy the selected text from the ComboBox to the TreeNode
                this.m_CurrentNode.Text = this.m_CurrentNode.ComboBox.Text;

                // Hide the ComboBox
                this.m_CurrentNode.ComboBox.Hide();
                this.m_CurrentNode.ComboBox.DroppedDown = false;

                // Remove the control from the TreeView's list of currently-displayed controls
                this.Controls.Remove(this.m_CurrentNode.ComboBox);

                // And return to the default state (no ComboBox displayed)
                this.m_CurrentNode = null;
            }
        }
    }
}


'@ -ReferencedAssemblies 'System.Windows.Forms.dll','System.Drawing.dll','System.Data.dll'
}
<#
$shared_assemblies = @(
  'DropDownTreeView.dll'
)

$shared_assemblies_folder = 'c:\developer\sergueik\csharp\SharedAssemblies'
pushd $shared_assemblies_folder
$shared_assemblies | ForEach-Object { Unblock-File -Path $_; Add-Type -Path $_ }
popd
#>

# http://www.codeproject.com/Articles/14544/A-TreeView-Control-with-ComboBox-Dropdown-Nodes

function PromptTreeView
{
  param(
    [string]$title,
    [object]$caller = $null
  )

  @( 'System.Drawing','System.Collections.Generic','System.Collections','System.ComponentModel','System.Windows.Forms','System.Text','System.Data') | ForEach-Object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }

  $f = New-Object System.Windows.Forms.Form
  $f.Text = $title
  $t = New-Object DropDownTreeView.DropDownTreeView
  $components = New-Object System.ComponentModel.Container
  $f.SuspendLayout()
  $t.Font = New-Object System.Drawing.Font ('Tahoma',10.25,[System.Drawing.FontStyle]::Regular,[System.Drawing.GraphicsUnit]::Point,[System.Byte]0)

  $i = New-Object System.Windows.Forms.ImageList ($components)
  $i.Images.Add([System.Drawing.SystemIcons]::Application)
  $script_path = $caller.ScriptDirectory
  foreach ($n in @( 1,2,3)) {
    $image_path = ('{0}\color{1}.gif' -f $script_path,$n)
    $image = [System.Drawing.Image]::FromFile($image_path)
    $i.Images.Add($image)
  }
  $t.ImageList = $i

  $t.Anchor = ((([System.Windows.Forms.AnchorStyles]::Top -bor [System.Windows.Forms.AnchorStyles]::Bottom) `
         -bor [System.Windows.Forms.AnchorStyles]::Left) `
       -bor [System.Windows.Forms.AnchorStyles]::Right)
  $t.ImageIndex = -1
  $t.Location = New-Object System.Drawing.Point (4,5)
  $t.Name = "treeFood"
  $t.SelectedImageIndex = -1
  $t.Size = New-Object System.Drawing.Size (284,256)
  $t.TabIndex = 1;
  $treeview_AfterSelect = $t.add_AfterSelect
  $treeview_AfterSelect.Invoke({
      param(
        [object]$sender,
        [System.Windows.Forms.TreeViewEventArgs]$eventargs
      )
      if ($eventargs.Action -eq [System.Windows.Forms.TreeViewAction]::ByMouse) {
        # [System.Windows.Forms.MessageBox]::Show($eventargs.Node.Text);
        $caller.Message += ('{0},' -f $eventargs.Node.Text)
      }
    })
  $f.AutoScaleBaseSize = New-Object System.Drawing.Size (5,13)
  $f.ClientSize = New-Object System.Drawing.Size (292,266)
  $f.Controls.AddRange(@( $t))
  $f.Name = 'DropDownComboboxTreeViewExample'
  $f.Text = 'DropDown Combobox TreeView Example'
  $f_Load = $f.add_Load
  $f_Load.Invoke({
      param(
        [object]$sender,
        [System.EventArgs]$eventargs
      )
      [System.Windows.Forms.TreeNode]$tn1 = New-Object System.Windows.Forms.TreeNode ("Node")

      [DropDownTreeView.DropDownTreeNode]$script:dtn1 = New-Object DropDownTreeView.DropDownTreeNode ("Credentials")
      $script:dtn1.ComboBox.Items.AddRange(@( "LocalService","LocalSystem","NetworkService"))
      $script:dtn1.ComboBox.SelectedIndex = 0

      [DropDownTreeView.DropDownTreeNode]$script:dtn2 = New-Object DropDownTreeView.DropDownTreeNode ("Install")
      $installs = @( 'Typical','Compact','Custom')
      $script:dtn2.ComboBox.Items.AddRange($installs)
      $script:dtn2.ComboBox.SelectedIndex = 0
      # NOTE: the Powershell event handlers are not closures. 
      # The defining scope variables are not captured
      # Also the Variables declared within an event-handling script block are local to that execution instance and cannot easily persist or communicate with the parent script's state without careful scoping
      # Either keep it in Win32Window attached to the current Handle of the Form
      # or use the script: or global: scope prefixes
      # https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_scopes?view=powershell-5.1
      # https://www.varonis.com/blog/powershell-variable-scope
      $handler1_combobox_closed = {
        param(
          [object]$sender,
          [System.EventArgs]$eventargs
        )
        try {

          [System.Windows.Forms.ComboBox]$cb = $sender
          $text = $cb.SelectedItem.ToString() 
          # [System.Windows.Forms.MessageBox]::Show(('Credentials -> "{0}"' -f $selectedItemText ))
          write-host ('Selected Item Text: {0}' -f $text)
          [System.Windows.Forms.MessageBox]::Show(('Credentials -> "{0}"' -f $text ))
          # https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.treenode.nodes?view=netframework-4.5
          # write-host ('dtn1: {0}' -f $script:dtn1.GetTypeName())
          if ($script:dtn1 -ne $null) {
            write-host ('Clear {0}' -f $script:dtn1.Nodes)
            $script:dtn1.Nodes.Clear()
            write-host ('Cleared {0}' -f $script:dtn1.Nodes)
           }

          if ($text -eq 'LocalService' ) {
             [DropDownTreeView.DropDownTreeNode]$x1 = New-Object DropDownTreeView.DropDownTreeNode ('Lime')
             [void]$script:dtn1.Nodes.Add($x1)
             [DropDownTreeView.DropDownTreeNode]$x2 = New-Object DropDownTreeView.DropDownTreeNode ('Leaf')
             [void]$script:dtn1.Nodes.Add($x2)
             [DropDownTreeView.DropDownTreeNode]$x3 = New-Object DropDownTreeView.DropDownTreeNode ('Land')
             [void]$script:dtn1.Nodes.Add($x3)
          } 
          if ($text -eq 'LocalSystem') {
             [System.Windows.Forms.TreeNode]$x1 = New-Object System.Windows.Forms.TreeNode('Salt')
             [void]$script:dtn1.Nodes.Add($x1)
             write-host ('Added: {0}' -f $x1)
             [System.Windows.Forms.TreeNode]$x2 = New-Object System.Windows.Forms.TreeNode('Smell')
             [void]$script:dtn1.Nodes.Add($x2)
          } 

          if ($text -eq 'NetworkService') { 
             [DropDownTreeView.DropDownTreeNode]$x1 = New-Object DropDownTreeView.DropDownTreeNode ('New')
             [void]$script:dtn1.Nodes.Add($x1)
             [DropDownTreeView.DropDownTreeNode]$x2 = New-Object DropDownTreeView.DropDownTreeNode ('North')
             [void]$script:dtn1.Nodes.Add($x2)
             [DropDownTreeView.DropDownTreeNode]$x3 = New-Object DropDownTreeView.DropDownTreeNode ('Near')
             [void]$script:dtn1.Nodes.Add($x3)
             [DropDownTreeView.DropDownTreeNode]$x4 = New-Object DropDownTreeView.DropDownTreeNode ('Nick')
             [void]$script:dtn1.Nodes.Add($x4)
          } 
          $script:dtn1.Expand()
          # $caller.Message += ('{0},' -f $cb.SelectedItem.ToString())
        } catch [exception]{
          write-host $_.exception.message
          # You cannot call a method on a null-valued expression.
        }
      }

      $handler2_combobox_closed = {
        param(
          [object]$sender,
          [System.EventArgs]$eventargs
        )
        try {
          [System.Windows.Forms.ComboBox]$cb = $sender
          $text = $cb.SelectedItem.ToString() 
          # [System.Windows.Forms.MessageBox]::Show(('Credentials -> "{0}"' -f $selectedItemText ))
          write-host ('Selected Item Text: {0}' -f $text)
        } catch [exception]{
        }
      }

      $t.Nodes.Add($tn1)
      $t.Nodes.Add($script:dtn2)
      $t.Nodes.Add($script:dtn1)

      $combobox1_DropDownClosed = $script:dtn1.ComboBox.add_DropDownClosed
      $combobox1_DropDownClosed.Invoke($handler1_combobox_closed)

      $combobox2_DropDownClosed = $script:dtn2.ComboBox.add_DropDownClosed
      $combobox2_DropDownClosed.Invoke($handler2_combobox_closed)

    })


  $f.ResumeLayout($false)


  $f.Name = 'Form1'
  $f.Text = 'TreeView Sample'
  $t.ResumeLayout($false)

  $f.ResumeLayout($false)

  $f.StartPosition = 'CenterScreen'

  $f.KeyPreview = $false

  $f.Topmost = $True
  if ($caller -eq $null) {
    $caller = New-Object Win32Window -ArgumentList ([System.Diagnostics.Process]::GetCurrentProcess().MainWindowHandle)
  }

  $f.Add_Shown({ $f.Activate() })
  # Passing an IWin32Window argument, such as this from a calling form, properly establishes the owner-child relationship
  # https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.form.showdialog?view=netframework-4.5
  [void]$f.ShowDialog([win32window]($caller))

  $t.Dispose()
  $f.Dispose()
  $result = $caller.Message
  $caller = $null
  return $result
}

Function Get-FileName {  
  param(
    [string]$initialDirectory = '.',
    [string]$filter = 'JSON files (*.json)|*.json|All files (*.*)| *.*'
  )
  [System.Reflection.Assembly]::LoadWithPartialName("System.windows.forms") | Out-Null

  $OpenFileDialog = New-Object System.Windows.Forms.OpenFileDialog
  $OpenFileDialog.initialDirectory = $initialDirectory
  $OpenFileDialog.filter = $filter
  $OpenFileDialog.ShowDialog() | Out-Null
  $OpenFileDialog.filename
}



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
# origin: https://4sysops.com/archives/convert-json-to-a-powershell-hash-table/
function ConvertTo-Hashtable {
    [CmdletBinding()]
    [OutputType('hashtable')]
    param (
        [Parameter(ValueFromPipeline)]
        $InputObject
    )

    process {
        if ($null -eq $InputObject) {
            return $null
        }

        if ($InputObject -is [System.Collections.IEnumerable] -and $InputObject -isnot [string]) {
            $collection = @(
                foreach ($object in $InputObject) {
                    $object = $_ 
                    ConvertTo-Hashtable -InputObject $object
                }
            )

            Write-Output -NoEnumerate $collection
        } elseif ($InputObject -is [psobject]) { 
            # convertFrom-Json produces System.Management.Automation.PSCustomObject 
            # https://stackoverflow.com/questions/14012773/difference-between-psobject-hashtable-and-pscustomobject#:~:text=%5BPSCustomObject%5D%20is%20a%20type%20accelerator,called%20with%20no%20constructor%20parameters.
            # which properties enumeration is fastest through its PSObject
            $dictionary = @{}
            foreach ($property in $InputObject.PSObject.Properties) {
                $dictionary[$property.Name] = ConvertTo-Hashtable -InputObject $property.Value
            }
            $dictionary
        } else {
            # the object is likely a hash table
            $InputObject
        }
    }
}


$filename = 'example.json'
<#
$filename = Get-FileName -initialDirectory (Get-ScriptDirectory)
$data = (Get-Content -encoding UTF8 -path (resolve-path $filename ).Path) | ConvertFrom-Json | ConvertTo-HashTable
$data
# TODO: debug
write-output $data['alternatives'][0]
exit 
#>
$DebugPreference = 'Continue'
$caller = New-Object Win32Window -ArgumentList ([System.Diagnostics.Process]::GetCurrentProcess().MainWindowHandle)
$caller.ScriptDirectory = Get-ScriptDirectory
$result = PromptTreeView 'Items' $caller

Write-Debug ('Selection is : {0}' -f,$result)

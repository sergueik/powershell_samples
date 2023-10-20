# http://poshcode.org/5854
function Show-Object{
param(
    ## The object to examine
    [Parameter(ValueFromPipeline = $true)]
    $InputObject
)

[void][System.Reflection.Assembly]::LoadWithPartialName('presentationframework')
add-type @"
using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Globalization;


namespace _treeListView
{
    public class TreeListView : TreeView
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListViewItem;
        }
    }

    public class TreeListViewItem : TreeViewItem
    {
        private int _level = -1;
        public int Level
        {
            get
            {
                if (_level != -1) return _level;
                var parent = ItemsControl.ItemsControlFromItemContainer(this) as TreeListViewItem;
                _level = (parent != null) ? parent.Level + 1 : 0;
                return _level;
            }
        }

        public TreeListViewItem(object header)
        {
            Header = header;
        }

        public TreeListViewItem(){}

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListViewItem;
        }
    }

    public class LevelToIndentConverter : IValueConverter
    {
        private const double c_IndentSize = 19.0;
        public object Convert(object o, Type type, object parameter,CultureInfo culture)
        {
            return new Thickness((int)o * c_IndentSize, 0, 0, 0);
        }

        public object ConvertBack(object o, Type type, object parameter,CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

}
"@ -ReferencedAssemblies presentationFramework,PresentationCore,WindowsBase,System.Xaml -ErrorAction SilentlyContinue
## form layout
[xml]$xaml = @"
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
	    xmlns:s='clr-namespace:System;assembly=mscorlib' 
	    xmlns:l='clr-namespace:_treeListView;assembly=$([_treeListView.LevelToIndentConverter].Assembly)' 
		Title="TreeListView" Width="640" Height="480">
    <Window.Resources>
    <Style x:Key="ExpandCollapseToggleStyle"
           TargetType="{x:Type ToggleButton}">
            <Setter Property="Focusable" Value="False"/>
            <Setter Property="Width" Value="19"/>
            <Setter Property="Height" Value="13"/>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="{x:Type ToggleButton}">
                        <Border Width="19" Height="13" Background="Transparent">
                            <Border Width="9" Height="9" BorderThickness="1" BorderBrush="#FF7898B5" CornerRadius="1" SnapsToDevicePixels="true">
                                <Border.Background>
                                    <LinearGradientBrush StartPoint="0,0" EndPoint="1,1">
                                        <LinearGradientBrush.GradientStops>
                                            <GradientStop Color="White" Offset=".2"/>
                                            <GradientStop Color="#FFC0B7A6" Offset="1"/>
                                        </LinearGradientBrush.GradientStops>
                                    </LinearGradientBrush>
                                </Border.Background>
                                <Path x:Name="ExpandPath" Margin="1,1,1,1" Fill="Black" 
                                      Data="M 0 2 L 0 3 L 2 3 L 2 5 L 3 5 L 3 3 L 5 3 L 5 2 L 3 2 L 3 0 L 2 0 L 2 2 Z"/>
                            </Border>
                        </Border>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsChecked" Value="True">
                                <Setter Property="Data" TargetName="ExpandPath" Value="M 0 2 L 0 3 L 5 3 L 5 2 Z"/>
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>

        <l:LevelToIndentConverter x:Key="LevelToIndentConverter"/>

        <DataTemplate x:Key="CellTemplate_Name">
            <DockPanel>
                <ToggleButton x:Name="Expander" 
                      Style="{StaticResource ExpandCollapseToggleStyle}" 
                      Margin="{Binding Level, Converter={StaticResource LevelToIndentConverter},
                             RelativeSource={RelativeSource AncestorType={x:Type l:TreeListViewItem}}}"
                      IsChecked="{Binding Path=IsExpanded, RelativeSource=
                    {RelativeSource AncestorType={x:Type l:TreeListViewItem}}}"
                      ClickMode="Press"/>
                <TextBlock Text="{Binding Name}"/>
            </DockPanel>
            <DataTemplate.Triggers>
                <DataTrigger Binding="{Binding Path=HasItems,RelativeSource={RelativeSource AncestorType={x:Type l:TreeListViewItem}}}" Value="False">
                    <Setter TargetName="Expander" Property="Visibility" Value="Hidden"/>
                </DataTrigger>
            </DataTemplate.Triggers>
        </DataTemplate>

        <GridViewColumnCollection x:Key="gvcc">
            <GridViewColumn Header="Name" CellTemplate="{StaticResource CellTemplate_Name}" />
            <GridViewColumn Header="MemberType" DisplayMemberBinding="{Binding MemberType}" />
            <GridViewColumn Header="Definition" DisplayMemberBinding="{Binding Definition}" />
            <GridViewColumn Header="Value" DisplayMemberBinding="{Binding Value}" />
        </GridViewColumnCollection>

        <Style TargetType="{x:Type l:TreeListViewItem}">
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="{x:Type l:TreeListViewItem}">
                        <StackPanel>
                            <Border Name="Bd"
                      Background="{TemplateBinding Background}"
                      BorderBrush="{TemplateBinding BorderBrush}"
                      BorderThickness="{TemplateBinding BorderThickness}"
                      Padding="{TemplateBinding Padding}">
                                <GridViewRowPresenter x:Name="PART_Header" 
                                      Content="{TemplateBinding Header}" 
                                      Columns="{StaticResource gvcc}" />
                            </Border>
                            <ItemsPresenter x:Name="ItemsHost" />
                        </StackPanel>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsExpanded" Value="false">
                                <Setter TargetName="ItemsHost" Property="Visibility" Value="Collapsed"/>
                            </Trigger>
                            <MultiTrigger>
                                <MultiTrigger.Conditions>
                                    <Condition Property="HasHeader" Value="false"/>
                                    <Condition Property="Width" Value="Auto"/>
                                </MultiTrigger.Conditions>
                                <Setter TargetName="PART_Header" Property="MinWidth" Value="75"/>
                            </MultiTrigger>
                            <MultiTrigger>
                                <MultiTrigger.Conditions>
                                    <Condition Property="HasHeader" Value="false"/>
                                    <Condition Property="Height" Value="Auto"/>
                                </MultiTrigger.Conditions>
                                <Setter TargetName="PART_Header" Property="MinHeight" Value="19"/>
                            </MultiTrigger>
                            <Trigger Property="IsSelected" Value="true">
                                <Setter TargetName="Bd" Property="Background" Value="{DynamicResource {x:Static SystemColors.HighlightBrushKey}}"/>
                                <Setter Property="Foreground" Value="{DynamicResource {x:Static SystemColors.HighlightTextBrushKey}}"/>
                            </Trigger>
                            <MultiTrigger>
                                <MultiTrigger.Conditions>
                                    <Condition Property="IsSelected" Value="true"/>
                                    <Condition Property="IsSelectionActive" Value="false"/>
                                </MultiTrigger.Conditions>
                                <Setter TargetName="Bd" Property="Background" Value="{DynamicResource  {x:Static SystemColors.ControlBrushKey}}"/>
                                <Setter Property="Foreground" Value="{DynamicResource {x:Static SystemColors.ControlTextBrushKey}}"/>
                            </MultiTrigger>
                            <Trigger Property="IsEnabled" Value="false">
                                <Setter Property="Foreground" Value="{DynamicResource {x:Static SystemColors.GrayTextBrushKey}}"/>
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>

        <Style TargetType="{x:Type l:TreeListView}">
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="{x:Type l:TreeListView}">
                        <Border BorderBrush="{TemplateBinding BorderBrush}" BorderThickness="{TemplateBinding BorderThickness}">
                            <DockPanel>
                                <GridViewHeaderRowPresenter Columns="{StaticResource gvcc}" DockPanel.Dock="Top"/>
                                <ScrollViewer CanContentScroll="True">
                                    <Grid>
                                    <ItemsPresenter/>
                                    </Grid>
                                </ScrollViewer>
                            </DockPanel>
                        </Border>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>
    </Window.Resources>
    <l:TreeListView x:Name="Tlv"/>
</Window>

"@

$rootVariableName = dir variable:\* -Exclude InputObject,Args |
    Where-Object {
        $_.Value -and
        ($_.Value.GetType() -eq $InputObject.GetType()) -and
        ($_.Value.GetHashCode() -eq $InputObject.GetHashCode())
}

## If we got multiple, pick the first
$rootVariableName = $rootVariableName| % Name | Select -First 1

## If we didn't find one, use a default name
if(-not $rootVariableName)
{
    $rootVariableName = "InputObject"
}

## A function to add an object to the display tree
function PopulateNode($node, $object)
{
    ## If we've been asked to add a NULL object, just return
    if(-not $object) { return }

    ## If the object is a collection, then we need to add multiple
    ## children to the node
    if([System.Management.Automation.LanguagePrimitives]::GetEnumerator($object))
    {
        ## Some very rare collections don't support indexing (i.e.: $foo[0]).
        ## In this situation, PowerShell returns the parent object back when you
        ## try to access the [0] property.
        $isOnlyEnumerable = $object.GetHashCode() -eq $object[0].GetHashCode()

        ## Go through all the items
        $count = 0
        foreach($childObjectValue in $object)
        {
            ## Create the new node to add, with the node text of the item and
            ## value, along with its type
            $newChildNode = New-Object _treeListView.TreeListViewItem
            #make sure the string version of it isnt more than a single line.
            $showValue = if(($arr="$childObjectValue" -split "\n" | ?{$_}).count -gt 1)
            {
                "$($arr[0].trim()) ..."
            }
            else
            {
                "$childObjectValue"
            }
            $newChildNode.ToolTip = "$childObjectValue"

            $newChildNode.Header = [pscustomobject] @{
                Name=$childObjectValue.GetType().name
                Value="$showValue"
                Definition=$childObjectValue.gettype()
                MemberType="Collection"
            }
            
            ## Use the node name to keep track of the actual property name
            ## and syntax to access that property.
            ## If we can't use the index operator to access children, add
            ## a special tag that we'll handle specially when displaying
            ## the node names.
            if($isOnlyEnumerable)
            {
                $newChildNode.Tag = "@"
            }

            $newChildNode.Tag += "[$count]"
            $null = $node.Items.Add($newChildNode)               

            ## If this node has children or properties, add a placeholder
            ## node underneath so that the node shows a '+' sign to be
            ## expanded.
            AddPlaceholderIfRequired $newChildNode $childObjectValue

            $count++
        }
    }
    else
    {
        ## If the item was not a collection, then go through its
        ## properties
        $members = Get-Member -InputObject $object
        foreach($member in $members)
        {
            $childNode = New-Object _treeListView.TreeListViewItem
            
            $memberValue = if($member.MemberType -like "*Propert*")
            {
                
                $prop = $object.($member.Name)                
                if($prop)
                {
                    $prop
                    $childnode.ToolTip = $prop
                    if($prop.gettype().fullname | ?{($_ -split '\.').count -gt 2})
                    {
                        AddPlaceholderIfRequired $childNode $prop
                    }
                }
                else { '$null' }
            }
            elseif($member.MemberType -eq "Method")
            {
               $childNode.ToolTip = ($object.($member.name) | Out-String).trim()
            }

            $showValue = if(($arr="$memberValue" -split "\n"|?{$_}).count -gt 1)
            {
                "$($arr[0].trim()) ..."
            }
            else
            {
                "$memberValue"
            }
            

            $childNode.Header = [pscustomobject] @{
                Name=$member.name
                Value=$showValue
                Definition=$member.Definition
                MemberType=$member.MemberType
            }

            $childNode.Tag = $member.Name
            $null = $node.Items.Add($childNode)
        }
    }
}

## A function to add a placeholder if required to a node.
## If there are any properties or children for this object, make a temporary
## node with the text "..." so that the node shows a '+' sign to be
## expanded.
function AddPlaceholderIfRequired($node, $object)
{
    if(-not $object) { return }

    if([System.Management.Automation.LanguagePrimitives]::GetEnumerator($object) -or
        @($object.PSObject.Properties))
    {
        $null = $node.Items.Add( (New-Object _treeListView.TreeListViewItem ([pscustomobject]@{Name="..."}) ) )
    }
}

## A function invoked when a node is selected.
function OnSelect
{
    param($Sender, $TreeViewEventArgs)

    ## Determine the selected node
    $nodeSelected = $TreeViewEventArgs.source

    ## Walk through its parents, creating the virtual
    ## PowerShell syntax to access this property.
    $nodePath = GetPathForNode $nodeSelected

    ## Now, invoke that PowerShell syntax to retrieve
    ## the value of the property.
    $resultObject = Invoke-Expression $nodePath
    #$outputPane.Text = $nodePath

    ## If we got some output, put the object's member
    ## information in the text box.
    
    if($resultObject)
    {
        $members = Get-Member -InputObject $resultObject | Out-String       
        #$outputPane.Text += "`n" + $members
    }
}

## A function invoked when the user is about to expand a node
function OnExpand
{
    param($Sender, $TreeViewCancelEventArgs)
    ## Determine the selected node
    $selectedNode = $TreeViewCancelEventArgs.Source
    ## If it has a child node that is the placeholder, clear
    ## the placeholder node.
    if($selectedNode.items.Count -eq 1 -and
        ($selectedNode.Items[0].Header.Name-eq "..."))
    {
        $selectedNode.items.Clear()
    }
    else
    {
        return
    }

    ## Walk through its parents, creating the virtual
    ## PowerShell syntax to access this property.
    $nodePath = GetPathForNode $selectedNode 
    $global:nodepath= $nodePath
    ## Now, invoke that PowerShell syntax to retrieve
    ## the value of the property.
    Invoke-Expression "`$resultObject = $nodePath"

    ## And populate the node with the result object.
    PopulateNode $selectedNode $resultObject
}

## A function to handle keypresses on the form.
## In this case, we capture ^C to copy the path of
## the object property that we're currently viewing.
function OnKeyPress
{
    param($Sender, $KeyPressEventArgs)

    ## [Char] 3 = Control-C
    if($KeyPressEventArgs.KeyChar -eq 3)
    {
        $KeyPressEventArgs.Handled = $true

        ## Get the object path, and set it on the clipboard
        $node = $Sender.SelectedNode
        $nodePath = GetPathForNode $node
        [System.Windows.Forms.Clipboard]::SetText($nodePath)

        $form.Close()
    }
}

## A function to walk through the parents of a node,
## creating virtual PowerShell syntax to access this property.
function GetPathForNode
{
    param($Node)

    $nodeElements = @()

    ## Go through all the parents, adding them so that
    ## $nodeElements is in order.
    while($Node.Tag)
    {
        $nodeElements = ,$Node + $nodeElements
        $Node = $Node.Parent
    }

    ## Now go through the node elements
    $nodePath = ""
    foreach($Node in $nodeElements)
    {
        $nodeName = $Node.Tag 

        ## If it was a node that PowerShell is able to enumerate
        ## (but not index), wrap it in the array cast operator.
        if($nodeName.StartsWith('@'))
        {
            $nodeName = $nodeName.Substring(1)
            $nodePath = "@(" + $nodePath + ")"
        }
        elseif($nodeName.StartsWith('['))
        {
            ## If it's a child index, we don't need to
            ## add the dot for property access
        }
        elseif($nodePath)
        {
            ## Otherwise, we're accessing a property. Add a dot.
            $nodePath += "."
        }

        ## Append the node name to the path
        $nodePath += $nodeName
    }

    ## And return the result
    $nodePath
}


## Create the TreeView, which will hold our object navigation
## area.

$reader=(New-Object System.Xml.XmlNodeReader $xaml)
$Form=[Windows.Markup.XamlReader]::Load( $reader )

$tlv = $Form.FindName('Tlv')


[System.Windows.RoutedEventHandler]$Script:Select = { OnSelect @args }
[System.Windows.RoutedEventHandler]$Script:Expand = { OnExpand @args }
[System.Windows.RoutedEventHandler]$Script:KeyPress = { OnKeyPress @args }
$tlv.AddHandler([_treeListView.TreeListViewItem]::SelectedEvent, $Script:Select)
$tlv.AddHandler([_treeListView.TreeListViewItem]::ExpandedEvent, $Script:Expand)
$tlv.AddHandler([_treeListView.TreeListViewItem]::KeyUpEvent, $Script:KeyPress)


## Create the root node, which represents the object
## we are trying to show.
$root = New-Object _treeListView.TreeListViewItem ([pscustomobject]@{
    Name=$InputObject.gettype().name
    Value="$InputObject"
    Definition=$InputObject.gettype().fullname})

#root.Header = "$InputObject : " + $InputObject.GetType()
$root.Tag = '$' + $rootVariableName
$root.ToolTip = "$InputObject"
$root.IsExpanded = $true

## And populate the initial information into the tree
## view.
PopulateNode $root $InputObject

$null = $tlv.Items.Add($root)
$null = $Form.ShowDialog()


}

# example: 
<# 
 $shared_assemblies = @(
   'WebDriver.dll',
   'WebDriver.Support.dll',
   'nunit.framework.dll'
 )

 $env:SHARED_ASSEMBLIES_PATH = 'c:\developer\sergueik\csharp\SharedAssemblies'
 $shared_assemblies_path = $env:SHARED_ASSEMBLIES_PATH
 pushd $shared_assemblies_path
 $shared_assemblies | ForEach-Object { Unblock-File -Path $_; Add-Type -Path $_; Write-Debug ('Loaded {0} ' -f $_) }
 popd

 $phantomjs_executable_folder = ;C:\tools\phantomjs'
 $selenium = New-Object OpenQA.Selenium.PhantomJS.PhantomJSDriver ($phantomjs_executable_folder)
 Show-Object -InputObject $selenium
#>
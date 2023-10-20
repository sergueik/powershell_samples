# http://poshcode.org/5520
# Asynchronous GUI by Peter Kriegel
<#
  How to create a non blocking GUI Thread and how to write Data with PowerShell to GUI controls correctly from a different Runspace.
  
  First of all excuse me for my bad English.
  
  PowerShell is a single threaded application.
  If PowerShell creates a Graphical User Interface (GUI) and should do other things (in the background) it
  can not react on events that are produced by the GUI.
  The user has the impression that the GUI is frozen (blocked).  
  To force PowerShell to do 2 things at the same time you can use PowerShell Jobs or use PowerShell runspaces.
  PowerShell Jobs do not support easy access to Objects inside so we use runspaces to run the GUI.     
  PowerShell runspaces are a kind of a thread inside the PowerShell Process so the word Thread and runspace are used interchangeably here.
  
  Access to Windows Forms controls is not inherently thread safe.
  If you have two or more threads manipulating the state of a control,
  it is possible to force the control into an inconsistent state.
  Other thread-related bugs are possible as well, including race conditions and deadlocks.
  It is important to ensure that access to your controls is done in a thread-safe way.
  
  There was one thing that bothers me all the Time:
  How can I create a non blocking GUI with PowerShell which does not need a synchronized hash table.
  To operate your GUI through a synchronized hash table is like to operate your whole living room through a keyhole
    
  Microsoft suggests the following pattern to make thread-safe calls to Windows Forms controls:
    1. Query the control's InvokeRequired property.
    2. If InvokeRequired returns true, call Invoke with a delegate that makes the actual call to the control.
    3. If InvokeRequired returns false, call the control directly.
  
  See: How to: Make Thread-Safe Calls to Windows Forms Controls
  http://msdn.microsoft.com/en-us/library/ms171728%28v=vs.85%29.aspx
  and dokumentation of the System.Windows.Forms.Control.Invoke or System.Windows.Forms.Control.BeginInvoke methods.

  In the fact that we know that we are running the GUI his own thread the use of InvokeRequired is not necessary.
  So we allways use the  System.Windows.Forms.Control.Invoke ore  System.Windows.Forms.Control.BeginInvoke methods here.

  Even Windows Presentation Foundation (WPF) uses the Invoke and BeginInvoke Methods of the System.Windows.Threading.Dispatcher class to
  make Thread-Safe calls to WPF controls

  Here I present a Pattern how to make thread-safe calls to Windows Forms controls
  I think you can adopt it easily to WPF
  
  Credits are going to:
  Simon Mourier See: http://stackoverflow.com/questions/14401704/update-winforms-not-wpf-ui-from-another-thread
#>

# ----------------------------------------------
# Step 1
# Import the Assemblies
# ----------------------------------------------

Add-Type -AssemblyName 'System.Windows.Forms'
Add-Type -AssemblyName 'System.Drawing'
# VisualStyles are only needed for a very few Windows Forms controls like ProgessBar
[Void][Reflection.Assembly]::LoadWithPartialName('System.Windows.Forms.VisualStyles')

# ----------------------------------------------
# Step 2
# Design the Form
# ----------------------------------------------

# Generated Form Objects
$Form = New-Object System.Windows.Forms.Form
$OkButton = New-Object System.Windows.Forms.Button
$TextBox = New-Object System.Windows.Forms.TextBox
$Label = New-Object System.Windows.Forms.Label
$InitialFormWindowState = New-Object System.Windows.Forms.FormWindowState

# initializes Form
$Form.Text = 'Demo Form'
$Form.Name = 'Form'
$Form.DataBindings.DefaultDataSourceUpdateMode = 0
$Form.ClientSize =  '156,91'

# initializes Textbox
$TextBox.Size = '100,20'
$TextBox.DataBindings.DefaultDataSourceUpdateMode = 0
$TextBox.Name = 'TextBox'
$TextBox.Location = '27,13'

# add Textbox to Form
$Form.Controls.Add($TextBox)

# initializes Label
$Label.Location = '27,50'
$Label.Size = '100,23'
$Label.Text = 'Round:'

# add Label to Form
$Form.Controls.Add($Label)

#Save the initial state of the form
$InitialFormWindowState = $Form.WindowState

# initializes the OnLoad event to correct the initial state of the form
$Form.add_Load({
  #Correct the initial state of the form to prevent the .Net maximized form issue
	$Form.WindowState = $InitialFormWindowState
})

# ----------------------------------------------
# Step 3
# Create the Runspace (thread) to run the GUI inside
# ----------------------------------------------

<#
  Create a new runspace
  runspace is an object which is used to configure the PowerShell object
#>
$Runspace = [Management.Automation.Runspaces.RunspaceFactory]::CreateRunspace($Host)

# initializes the runspace object

# Some Windows Forms and WPF controls are running only in the Single Thread Apartment (STA) state correctly
$Runspace.ApartmentState = 'STA'
$Runspace.ThreadOptions = 'ReuseThread'
$Runspace.Open()

# add the Form object to the Runspace environment, so you can use it inside the runspace
$Runspace.SessionStateProxy.SetVariable('Form', $Form)


# Create a new PowerShell object (a Thread)
$PowerShellRunspace = [System.Management.Automation.PowerShell]::Create()

# initializes the PowerShell object with the runspace
$PowerShellRunspace.Runspace = $Runspace

# add the scriptblock which should run inside the runspace
$PowerShellRunspace.AddScript(
     {
     # enables visual styles with this application object 
     [System.Windows.Forms.Application]::EnableVisualStyles()
     # bind the Windows message loop (messagepump) to the Form and start it (show the Form)  
     [System.Windows.Forms.Application]::Run($Form)
     })

# starting and show the GUI
# open and run the runspace asynchronously
$AsyncResult = $PowerShellRunspace.BeginInvoke()

# ----------------------------------------------
# Step 4
# Invoke the UI with thread-save invoke calls from
# the PowerShell Main-Runspace (Main-Thread)
# ----------------------------------------------

<#
  Beneath this line you can do your normal PowerShell stuff and you can access the GUI controls
  in a thread-safe manner like it is shown in the examples below.
#>


<#
  block PowerShell Main-Thread to leave it alive until user enter something
  this simulates even that the main-thread is doing hard work
  which normally blocks the GUI Thread
#>
$MyMessage = Read-Host 'Enter a message Text'

# Executing a Scriptblock (which represents a specified delegate), on the GUI thread that
# owns the control's underlying window handle, with the specified list of arguments.
$TextBox.Invoke(
    <#
    Casting the Scriptblock to an suitable delegate
    The documentation for Control.Invoke (or Control.BeginInvoke) states that:
    The delegate can be an instance of EventHandler, in which case the sender parameter will contain this control,
    and the event parameter will contain EventArgs.Empty.
    The delegate can also be an instance of MethodInvoker, or any other delegate that takes a void parameter list.
    
    System.Action<T> is a delegate and encapsulates a method that
    has a single parameter of type T and does not return a value.
    #>
    [System.Action[string]] { param($Message) $TextBox.Text = $Message },
    # Argument for the System.Action delegate scriptblock
    $MyMessage
)

<#
  Control.Invoke is a synchronous call and can be very slow because it waits until the called thread returns
  Better is to use Control.BeginInvoke wich returns immediately a call to
  Control.EndInvoke is optional and makes no sense because all calls should not return anything
#>

<#
  The documentation for Control.Invoke (or Control.BeginInvoke) even states that:
  A call to an EventHandler or MethodInvoker delegate will be faster than a call to another type of delegate.
  Unfortunately I do not found out how to create an MethodInvoker delegate which takes Parameters so I use the EventHandler delegate.
#>

# create an empty EventArg
$Eventargs = [System.EventArgs]::Empty
# add needed Property to the Eventarg object to create a custom eventarg 
Add-Member -InputObject $eventArgs -MemberType 'NoteProperty' -Name 'MyText' -Value 'Alle Achtung!' -Force

# create the specialized EventHandler delegate to fit our need
$Handler = [System.EventHandler]{Param($Sender,$Eventargs); $Sender.Text = $Eventargs.MyText }


# simulating heavy PowerShell work with a loop
1..50 | ForEach-Object {
  # pass argument to the eventargs
  $Eventargs.MyText = "Round: $_"
  # Invoke the Control
  $Label.BeginInvoke($Handler,($Label,$Eventargs))
  # slow down the loop to see the numbers rolling by
  Start-Sleep -Milliseconds 20
}

# block PowerShell Main-Thread to leave it alive until user enter something
Read-Host 'Enter something to quit'

# clean up 

# close the Windows Form and unbind the message loop
[System.Windows.Forms.Application]::Exit()

# end the pipeline of the PowerShell object
$PowerShellRunspace.EndInvoke($AsyncResult)

# close the runspace
$Runspace.Close()

# remove the PowerShell object and its resources
$PowerShellRunspace.Dispose()

<#
  Note:
    The call to control.Begininvoke sometime blocks the PowerShell ISE
    I donot se this behavior inside the PowerShell console (PowerShell.exe)  
#>
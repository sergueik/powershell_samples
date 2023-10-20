  @( 'System.Drawing','System.Collections','System.ComponentModel','System.Windows.Forms','System.Data') | ForEach-Object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }

# origin: https://www.codeproject.com/Articles/20379/Disabling-Close-Button-on-Forms

Add-Type -TypeDefinition @'


using System;
using System.Windows.Forms;

public class FormCloseButtonDisabled : Form {

	private  const int CP_NOCLOSE_BUTTON = 0x200;
	protected override CreateParams CreateParams {
		get {
			CreateParams myCp = base.CreateParams;
			myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
			return myCp;
		}
	}
}


'@ -ReferencedAssemblies 'System.Windows.Forms'

#// see also:
# NOTE: will fail to
class CustomForm : FormCloseButtonDisabled {
  [string]$Value
}


#  see also: https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_classes?view=powershell-5.1

# NOTE: adding properties to custom class will nor compile:
# class CustomForm2 : System.Windows.Forms.Form {
# Missing closing '}' in statement block or type definition.
class CustomForm2 : System.Windows.Forms.Form {
# [int] CP_NOCLOSE_BUTTON = 0x200;
}
# $f = [FormCloseButtonDisabled]::new()
$f = [CustomForm]::new()
$f.Value = 'test'
$f.ResumeLayout($false)
$f.Topmost = $True
$b = new-object System.Windows.Forms.Button
$b.Text = 'Close'
$b.Dock = [System.Windows.Forms.DockStyle]::Bottom
$b.add_click({ $f.Close() })
$f.Controls.Add($b)
$f.Add_Shown({ 
  $f.Text = $f.Value
  $f.Activate() 
})

[void]$f.ShowDialog()

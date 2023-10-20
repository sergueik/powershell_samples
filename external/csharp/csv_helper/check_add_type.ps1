$added_class = 'Win32Window_2'
if (-not ($added_class -as [System.Type])) {

Add-Type -TypeDefinition @"

using System;
using System.Windows.Forms;
public class ${added_class}: IWin32Window {
    private IntPtr _hWnd;
    private string _value;

    public string Value {
        get { return _value; }
        set { _value = value; }
    }

    public ${added_class}(IntPtr handle) {
        _hWnd = handle;
    }

    public IntPtr Handle {
        get { return _hWnd; }
    }
}

"@ -ReferencedAssemblies 'System.Windows.Forms.dll'
write-output ('Created class {0}' -f $added_class)

} else { 
write-output ('Already have defined class {0}' -f $added_class)

}

$window_handle = [System.Diagnostics.Process]::GetCurrentProcess().MainWindowHandle

$caller = new-object -typename $added_class -ArgumentList ($window_handle)
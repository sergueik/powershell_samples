# origin:
# https://jike.in/?qa=826847/c#-hook-detect-windows-language-change-even-when-app-not-focused&show=826848#a826848
# detect the active keyboard layout by spying for the active window keyboard change messages
public partial class MainWindow : Window {
    [DllImport("user32.dll")]
    static extern IntPtr GetKeyboardLayout(uint idThread);
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")]
    static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr processId);

    private CultureInfo _currentLanaguge;

    public MainWindow()
    {
        InitializeComponent();

        Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    HandleCurrentLanguage();
                    Thread.Sleep(500);
                }
            });
    }

    private static CultureInfo GetCurrentCulture()
    {
        var l = GetKeyboardLayout(GetWindowThreadProcessId(GetForegroundWindow(), IntPtr.Zero));
        return new CultureInfo((short)l.ToInt64());
    }

    private void HandleCurrentLanguage()
    {
        var currentCulture = GetCurrentCulture();
        if (_currentLanaguge == null || _currentLanaguge.LCID != currentCulture.LCID)
        {
            _currentLanaguge = currentCulture;
            MessageBox.Show(_currentLanaguge.Name);
        }
    }
}


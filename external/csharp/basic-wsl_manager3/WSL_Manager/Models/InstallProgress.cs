using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace WslManagerFramework.Models
{
    public class InstallProgress
    {
        public Process InstallProcess { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
        public bool IsPaused { get; set; }
        public ProgressBar ProgressBar { get; set; }
        public Button StopButton { get; set; }
        public Button PauseButton { get; set; }
        public Label StatusLabel { get; set; }
        public Panel ControlPanel { get; set; }
        
        public InstallProgress()
        {
            CancellationTokenSource = new CancellationTokenSource();
            IsPaused = false;
        }
    }
}
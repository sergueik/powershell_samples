using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;

namespace DontSleep {
    public class Program {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto,SetLastError = true)]
        private static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        [FlagsAttribute]
        public enum EXECUTION_STATE : uint {
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002
        }

        private static void Main(string[] args) {
            var timer = new Timer((s) => {
                SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED);
            }, null, TimeSpan.Zero, TimeSpan.FromMinutes(9));
            ToggleGoodbye(false);

            Console.WriteLine($"Tap any key to stop program!");
            Console.ReadKey();
            timer.Dispose();
            ToggleGoodbye(true);
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
        }

        private static void ToggleGoodbye(bool state) {
            using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
            var newValue = Convert.ToInt32(state);
            var keyValue = key?.GetValue("EnableGoodbye") ?? newValue;
            if ((int)keyValue != newValue) {
                key.SetValue("EnableGoodbye", newValue);
            }
        }
    }
}

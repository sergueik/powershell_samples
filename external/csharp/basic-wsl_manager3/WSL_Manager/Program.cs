using System;
using System.Windows.Forms;
using WSL_Manager;

namespace WslManagerFramework
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // .NET Framework の標準初期化
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new Form1());
        }
    }
}

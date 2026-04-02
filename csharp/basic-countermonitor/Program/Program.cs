using System;
using System.Linq;
using System.Windows.Forms;
namespace Program {
    static class Program {
        [STAThread]
        static void Main( ){
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // NOTE: cannot Run(new Program.Form1:
            // The type name 'Form1' does not exist in the type 'Program.Program' (CS0426)
            Application.Run(new Form1());
        }
    }
}

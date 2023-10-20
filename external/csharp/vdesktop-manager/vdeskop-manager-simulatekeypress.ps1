$Source = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;

namespace CSharpPS
{
    public static class PS
    {
        public static void NewVD()
        {
            InputSimulator.SimulateKeyDown(VirtualKeyCode.LWIN);
            InputSimulator.SimulateKeyDown(VirtualKeyCode.CONTROL);
            InputSimulator.SimulateKeyPress(VirtualKeyCode.VK_D);
            InputSimulator.SimulateKeyUp(VirtualKeyCode.LWIN);
            InputSimulator.SimulateKeyUp(VirtualKeyCode.CONTROL);
        }        
    }
}
"@;
 
Add-Type -TypeDefinition $Source -Language CSharp -ReferencedAssemblies InputSimulator.dll
# https://web.archive.org/web/20210501220444/https://archive.codeplex.com/?p=inputsimulator


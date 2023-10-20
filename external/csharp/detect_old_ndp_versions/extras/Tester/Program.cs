using System;
using System.Collections.Generic;
using System.Text;

using Campari.Software;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            bool fx10Installed = FrameworkVersionDetection.IsInstalled(FrameworkVersion.Fx10);
            bool fx11Installed = FrameworkVersionDetection.IsInstalled(FrameworkVersion.Fx11);
            bool fx20Installed = FrameworkVersionDetection.IsInstalled(FrameworkVersion.Fx20);
            bool fx30Installed = FrameworkVersionDetection.IsInstalled(FrameworkVersion.Fx30);
            bool fx35Installed = FrameworkVersionDetection.IsInstalled(FrameworkVersion.Fx35);

            Console.WriteLine(".NET Framework 1.0 installed? {0}", fx10Installed);
            if (fx10Installed)
            {
                Console.WriteLine(".NET Framework 1.0 Exact Version: {0}",
                   FrameworkVersionDetection.GetExactVersion(FrameworkVersion.Fx10));
                Console.WriteLine(".NET Framework 1.0 Service Pack: {0}",
                   FrameworkVersionDetection.GetServicePackLevel(FrameworkVersion.Fx10));
            }
            Console.WriteLine();

            Console.WriteLine(".NET Framework 1.1 installed? {0}", fx11Installed);
            if (fx11Installed)
            {
                Console.WriteLine(".NET Framework 1.1 Exact Version: {0}",
                   FrameworkVersionDetection.GetExactVersion(FrameworkVersion.Fx11));
                Console.WriteLine(".NET Framework 1.1 Service Pack: {0}",
                   FrameworkVersionDetection.GetServicePackLevel(FrameworkVersion.Fx11));
            }
            Console.WriteLine();

            Console.WriteLine(".NET Framework 2.0 installed? {0}", fx20Installed);
            if (fx20Installed)
            {
                Console.WriteLine(".NET Framework 2.0 Exact Version: {0}",
                   FrameworkVersionDetection.GetExactVersion(FrameworkVersion.Fx20));
                Console.WriteLine(".NET Framework 2.0 Service Pack: {0}",
                   FrameworkVersionDetection.GetServicePackLevel(FrameworkVersion.Fx20));
            }
            Console.WriteLine();

            Console.WriteLine(".NET Framework 3.0 installed? {0}", fx30Installed);
            if (fx30Installed)
            {
                Console.WriteLine(".NET Framework 3.0 Exact Version: {0}",
                  FrameworkVersionDetection.GetExactVersion(FrameworkVersion.Fx30));
                Console.WriteLine(".NET Framework 3.0 Service Pack: {0}",
                   FrameworkVersionDetection.GetServicePackLevel(FrameworkVersion.Fx30));


                bool fx30PlusWCFInstalled = FrameworkVersionDetection.IsInstalled(WindowsFoundationLibrary.WCF);
                bool fx30PlusWPFInstalled = FrameworkVersionDetection.IsInstalled(WindowsFoundationLibrary.WPF);
                bool fx30PlusWFInstalled = FrameworkVersionDetection.IsInstalled(WindowsFoundationLibrary.WF);
                bool fx30PlusCardSpacesInstalled = FrameworkVersionDetection.IsInstalled(WindowsFoundationLibrary.CardSpace);

                Console.WriteLine();

                Console.WriteLine("Windows Communication Foundation installed? {0}", fx30PlusWCFInstalled);
                if (fx30PlusWCFInstalled)
                {
                    Console.WriteLine("Windows Communication Foundation Exact Version: {0}",
                      FrameworkVersionDetection.GetExactVersion(WindowsFoundationLibrary.WCF));
                }
                Console.WriteLine();

                Console.WriteLine("Windows Presentation Foundation installed? {0}", fx30PlusWPFInstalled);
                if (fx30PlusWPFInstalled)
                {
                    Console.WriteLine("Windows Presentation Foundation Exact Version: {0}",
                      FrameworkVersionDetection.GetExactVersion(WindowsFoundationLibrary.WPF));
                }
                Console.WriteLine();

                Console.WriteLine("Windows Workflow Foundation installed? {0}", fx30PlusWFInstalled);
                if (fx30PlusWFInstalled)
                {
                    Console.WriteLine("Windows Workflow Foundation Exact Version: {0}",
                      FrameworkVersionDetection.GetExactVersion(WindowsFoundationLibrary.WF));
                }
                Console.WriteLine();

                Console.WriteLine("Windows CardSpaces installed? {0}", fx30PlusCardSpacesInstalled);
                if (fx30PlusCardSpacesInstalled)
                {
                    Console.WriteLine("Windows CardSpaces Exact Version: {0}",
                      FrameworkVersionDetection.GetExactVersion(WindowsFoundationLibrary.CardSpace));
                }
                Console.WriteLine();

            }
            Console.WriteLine();

            Console.WriteLine(".NET Framework 3.5 installed? {0}", fx35Installed);
            if (fx35Installed)
            {
                Console.WriteLine(".NET Framework 3.5 Exact Version: {0}",
                  FrameworkVersionDetection.GetExactVersion(FrameworkVersion.Fx35));
                Console.WriteLine(".NET Framework 3.5 Service Pack: {0}",
                   FrameworkVersionDetection.GetServicePackLevel(FrameworkVersion.Fx35));
            }
            Console.WriteLine();

			foreach (Version v in FrameworkVersionDetection.InstalledFrameworkVersions)
			{
				Console.WriteLine(v.ToString());
			}
            Console.ReadLine();
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace ScriptServices
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<hosting.WindowsService>(s =>
                {
                    s.ConstructUsing(name => new hosting.WindowsService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("Exposes powershell scripts as REST-based micro services");
                x.SetDisplayName("ScriptServices");
                x.SetServiceName("ScriptServices");
            });
        }
    }
}

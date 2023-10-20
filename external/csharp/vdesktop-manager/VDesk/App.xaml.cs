﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WindowsDesktop;

namespace VDesk {
    partial class App {
        private async void ApplicationStart(object sender, StartupEventArgs e) {
            try {

                if (!VirtualDesktop.IsSupported)
                    throw new NotSupportedException("Virtual Desktops are not supported on this system.");
                
                await VirtualDesktopProvider.Default.Initialize();
                
                string[] clArgs = Environment.GetCommandLineArgs();

                int exeNameLength = Regex.Match(Environment.CommandLine, "^(?:\".+?\"|\\S+)").Value.Length;
                string commandline = string.Concat(Environment.CommandLine.Skip(exeNameLength + 1));

                Dictionary<string, string> args = new Dictionary<string, string> {
                    
                };
                args["noswitch"] = "false";
                foreach (string arg in clArgs) {
                    string[] kv = arg.Split(':');
                    args[kv[0]] = kv.Length > 1 ? string.Concat(string.Concat(kv.Skip(1)).Split()[0]) : "";
                }
				// https://stackoverflow.com/questions/2870544/c-sharp-4-0-optional-out-ref-arguments
				int switchIndex;
                if (int.TryParse(commandline, out switchIndex)) {
                    //commandline is just an integer, switch to that desktop.
                    //launch on desktop i
                    VirtualDesktop[] desktops = VirtualDesktop.GetDesktops();

                    while (switchIndex > VirtualDesktop.GetDesktops().Length)
                        VirtualDesktop.Create();

                    // TODO: if...else
                    VirtualDesktop.GetDesktops()[Math.Max(0, --switchIndex)].Switch();

                } else if (commandline.Contains("create")) {
                    //just create desktops.
                    int n;
                    if (args.ContainsKey("create") && int.TryParse(args["create"], out n)) {
                        while (n > VirtualDesktop.GetDesktops().Length)
                            VirtualDesktop.Create();
                    } else {
                        VirtualDesktop.Create();
                    }

                } else if (commandline.Contains("run:")) {
                    //launch program:
                    int startIndex = commandline.IndexOf("run:") + 4;
                    string appString = commandline.Substring(startIndex);

                    string appPath = Regex.Match(appString, "^(?:\".+?\"|\\S+)").Value;
                    string appArgs = appString.Length >= appPath.Length + 1 ? appString.Substring(appPath.Length + 1) : "";
                    appPath = appPath.Trim('"');

                    VirtualDesktop targetDesktop;
                    int i;
                    if (args.ContainsKey("on") && int.TryParse(args["on"], out i)) {
                        //launch on desktop i
                        VirtualDesktop[] desktops = VirtualDesktop.GetDesktops();

                        while (i > VirtualDesktop.GetDesktops().Length)
                            VirtualDesktop.Create();

                        targetDesktop = VirtualDesktop.GetDesktops()[Math.Max(0, --i)];
                    } else {
                        //launch on new desktop
                        targetDesktop = VirtualDesktop.Create();
                    }
                    bool noswitch;
                    if (bool.TryParse(args["noswitch"], out noswitch) && !noswitch)
                        targetDesktop.Switch();


                    ProcessStartInfo startInfo = new ProcessStartInfo(appPath, appArgs);

                    try {
                        if (Directory.Exists(Path.GetDirectoryName(appPath)))
                            startInfo.WorkingDirectory = Path.GetDirectoryName(appPath);

                    } catch {
                        //Don't really want to do anything here.
                    }

                    Process proc = Process.Start(startInfo);

                    if (noswitch) {
                        for (int backoff = 1; proc.MainWindowHandle.ToInt64() == 0 && backoff <= 0x1000; backoff <<= 1)
                            Thread.Sleep(backoff);

                        if (proc.MainWindowHandle.ToInt64() != 0)
                            VirtualDesktopHelper.MoveToDesktop(proc.MainWindowHandle, targetDesktop);
                    }


                } else {
                    //wip: gui mode
                    MessageBox.Show(
                        @"Usage:
vdesk create[:n]
vdesk [on:<n>] [noswitch:{true|false}] <run:command> [args]
",
                        "VDesk Usage");
                }
            } catch (FileNotFoundException ex) {
				MessageBox.Show(String.Format("{0}:{1}'{2}'",ex.Message,Environment.NewLine, ex.FileName), "VDesk", MessageBoxButton.OK, MessageBoxImage.Hand);

            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "VDesk");

            } finally {
                Current.Shutdown();
            }
        }

    }
}

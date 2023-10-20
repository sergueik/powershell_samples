/// @file SimpleService.cs
/// @author Ron Wilson

using System;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;

namespace RPW.Simple
{
	static class SimpleService
	{
		private static ISimpleServiceWorker m_worker;
		private static bool m_consoleMode;

		public static ISimpleServiceWorker Worker
		{
			set
			{
				m_worker = value;
			}
		}

		public static void WriteLog(string message, EventLogEntryType entryType)
		{
			if (!string.IsNullOrEmpty(message))
			{
				if (m_consoleMode)
				{
					Console.WriteLine(
						string.Format("= {0} : {2} : {1}",
							SimpleServiceInstaller.ServiceName,
							message,
							entryType.ToString()
						)
					);
				}
				else
				{
					EventLog.WriteEntry(SimpleServiceInstaller.ServiceName, message, entryType);
				}
			}
		}

		public static void WriteLog(string message)
		{
			WriteLog(message, EventLogEntryType.Information);
		}

		public static void WriteLog(Exception e)
		{
			if (e != null)
			{
				WriteLog(e.InnerException);
				WriteLog(string.Format("Error: {0}\r\nSource: {1}\r\nStack:\r\n{2}"
					, e.Message
					, e.Source
					, e.StackTrace)
					, EventLogEntryType.Error);
			}
		}

		static void ShowHelp()
		{
			Console.WriteLine("= usage:");
			Console.WriteLine("=   {0} -install   == install service", SimpleServiceInstaller.ServiceName);
			Console.WriteLine("=   {0} -uninstall == uninstall service", SimpleServiceInstaller.ServiceName);
			Console.WriteLine("=   {0} -start     == start service", SimpleServiceInstaller.ServiceName);
			Console.WriteLine("=   {0} -stop      == stop service", SimpleServiceInstaller.ServiceName);
			Console.WriteLine("=   {0} -status    == get the current status of the service", SimpleServiceInstaller.ServiceName);
			Console.WriteLine("=   {0} -console   == run in console mode", SimpleServiceInstaller.ServiceName);
			Console.WriteLine("=   {0} -help      == show this help message", SimpleServiceInstaller.ServiceName);
		}

		public static void Start(string[] args)
		{
			m_consoleMode = args != null && args.Length > 0;

			if (m_worker == null)
			{
				WriteLog("Null worker", EventLogEntryType.Error);
			}

			if (m_consoleMode)
			{
				if (args.Length > 1)
				{
					WriteLog("Only one argument is allowed");
					ShowHelp();
					return;
				}

				switch (args[0].Trim().ToLower())
				{
					case "-console":
					case "-consol":
					case "-conso":
					case "-cons":
					case "-con":
					case "-co":
					case "-c":
						break;

					case "-install":
					case "-instal":
					case "-insta":
					case "-inst":
					case "-ins":
					case "-in":
					case "-i":
						ManagedInstallerClass.InstallHelper(
							new string[] { Assembly.GetExecutingAssembly().Location }
						);
						return;

					case "-uninstall":
					case "-uninstal":
					case "-uninsta":
					case "-uninst":
					case "-unins":
					case "-unin":
					case "-uni":
					case "-un":
					case "-u":
						ManagedInstallerClass.InstallHelper(
							new string[] { "/u", Assembly.GetExecutingAssembly().Location }
						);
						return;

					case "-start":
					case "-star":
						using (ServiceController oController = new ServiceController(SimpleServiceInstaller.ServiceName))
						{
							oController.Start();
						}
						return;

					case "-stop":
					case "-sto":
						using (ServiceController oController = new ServiceController(SimpleServiceInstaller.ServiceName))
						{
							oController.Stop();
						}
						return;

					case "-status":
					case "-statu":
					case "-stat":
						using (ServiceController oController = new ServiceController(SimpleServiceInstaller.ServiceName))
						{
							try
							{
								WriteLog(oController.Status.ToString());
							}
							catch (Win32Exception e)
							{
								WriteLog(e);
							}
							catch (InvalidOperationException e)
							{
								WriteLog(e);
							}
						}
						return;

					case "-help":
					case "-hel":
					case "-he":
					case "-h":
					case "-?":
						ShowHelp();
						return;

					default:
						WriteLog(string.Format("Invalid argument: {0}", args[0]));
						ShowHelp();
						return;
				}
			}

			using (EventWaitHandle threadFinish = new EventWaitHandle(false, EventResetMode.ManualReset))
			{
				Thread thread = null;
				try
				{
					m_worker.Init();
					thread = new Thread(() =>
						{
							try
							{
								m_worker.Run();
							}
							catch (ThreadAbortException)
							{
								SimpleService.WriteLog(string.Format("{0} stopping", SimpleServiceInstaller.ServiceName));
							}
							finally
							{
								threadFinish.Set();
							}
						});
					thread.Start();

					if (m_consoleMode)
					{
						// Console.TreatControlCAsInput = true;
						WriteLog("Press the Escape (Esc) key to quit:");
						while (Console.ReadKey(true).Key != ConsoleKey.Escape)
							;
					}
					else
					{
						ServiceBase[] ServicesToRun = new ServiceBase[] { new ServiceBase { ServiceName = SimpleServiceInstaller.ServiceName, AutoLog = true } };
						ServiceBase.Run(ServicesToRun);
					}
				}
				catch (Exception e)
				{
					WriteLog(e);
				}
				finally
				{
					try
					{
						if (thread != null)
						{
							thread.Abort();
							thread = null;
						}
					}
					catch (Exception e)
					{
						WriteLog(e);
					}
					finally
					{
						Mutex.WaitAll(new EventWaitHandle[] {threadFinish}, Timeout.Infinite);
						m_worker.Cleanup();
					}
				}
			}
		}
	}
}

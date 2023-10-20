using System;
using System.Configuration.Install;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Collections;
using System.Diagnostics;

namespace Utils {
	// NOTE: cannot user the same class name 
	// Circular base class dependency involving 'Utils.ServiceInstaller' and 'Utils.ServiceInstaller'
	// (CS0146) 
	public class ServiceInstallerEx : ServiceInstaller {

		[StructLayout(LayoutKind.Sequential)]
		public struct SERVICE_DESCRIPTION {
			public string lpDescription;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SERVICE_FAILURE_ACTIONS {
			public int dwResetPeriod;
			public string	lpRebootMsg;
			public string	lpCommand;
			public int cActions;
			public int lpsaActions;
		}

		[DllImport("advapi32.dll")]
		public static extern  IntPtr OpenSCManager(string lpMachineName, string lpDatabaseName, int dwDesiredAccess);

		[DllImport("advapi32.dll")]
		public static extern IntPtr  OpenService(IntPtr hSCManager, string lpServiceName, int dwDesiredAccess);

		[DllImport("advapi32.dll")]
		public static extern IntPtr LockServiceDatabase(IntPtr hSCManager);

		[DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig2")]
		public static extern bool ChangeServiceFailureActions(IntPtr hService, int dwInfoLevel, 
			[ MarshalAs(UnmanagedType.Struct)] ref SERVICE_FAILURE_ACTIONS lpInfo);
		[DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig2")]
		public static extern bool 
			ChangeServiceDescription(IntPtr hService, int dwInfoLevel, 
			[ MarshalAs(UnmanagedType.Struct)] ref SERVICE_DESCRIPTION lpInfo);

		[DllImport("advapi32.dll")]
		public static extern bool  CloseServiceHandle(IntPtr hSCObject);
	
		[DllImport("advapi32.dll")]
		public static extern bool  UnlockServiceDatabase(IntPtr hSCManager);

		[DllImport("kernel32.dll")]
		public static extern int  GetLastError();
		private const int SC_MANAGER_ALL_ACCESS = 0xF003F;
		private const int SERVICE_ALL_ACCESS = 0xF01FF;
		private const int SERVICE_CONFIG_DESCRIPTION = 0x1;
		private const int SERVICE_CONFIG_FAILURE_ACTIONS	= 0x2;
		private const int SERVICE_NO_CHANGE = -1;
		private const int ERROR_ACCESS_DENIED = 5;


		[StructLayout(LayoutKind.Sequential)]
		public struct LUID_AND_ATTRIBUTES {
			public long Luid;
			public int Attributes;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct TOKEN_PRIVILEGES {
			public int PrivilegeCount;
			public LUID_AND_ATTRIBUTES Privileges;
		}

		[DllImport("advapi32.dll")]
		public static extern bool
			AdjustTokenPrivileges(IntPtr TokenHandle, bool DisableAllPrivileges, 
			[ MarshalAs(UnmanagedType.Struct)] ref TOKEN_PRIVILEGES NewState, int BufferLength,
			IntPtr PreviousState, ref int ReturnLength);

		[DllImport("advapi32.dll")]
		public static extern bool
			LookupPrivilegeValue(string lpSystemName, string lpName, ref long lpLuid);

		[DllImport("advapi32.dll")]
		public static extern bool
			OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, ref IntPtr TokenHandle);

		[DllImport("kernel32.dll")]
		public static extern IntPtr
			GetCurrentProcess();

		[DllImport("kernel32.dll")]
		public static extern bool 
			CloseHandle(IntPtr hndl);

		private const int TOKEN_ADJUST_PRIVILEGES = 32;
		private const int TOKEN_QUERY = 8;
		private const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
		private const int SE_PRIVILEGE_ENABLED = 2;

		private string description = "";
		private int failResetTime = SERVICE_NO_CHANGE;
		private string failRebootMsg = "";
		private string failRunCommand = "";
		private bool setDescription = false;
		private bool setFailActions = false;
		private bool startOnInstall = false;
		private int startTimeout = 15000;

		private string logMsgBase;
		public ArrayList FailureActions;

		public ServiceInstallerEx()
			: base() {

			FailureActions = new ArrayList();

			base.Committed += new InstallEventHandler(this.UpdateServiceConfig);
			base.Committed += new InstallEventHandler(this.StartIfNeeded);
			logMsgBase = "ServiceInstallerEx : " + base.ServiceName + " : ";
		}

		public new string  Description { 
			set {
				description = value;
				setDescription = true;
			}
		}

		public int FailCountResetTime {
			set { 
				failResetTime = value; 
				setFailActions = true;
			}
		}

		public string FailRebootMsg {
			set { 
				failRebootMsg = value; 
				setFailActions = true;
			}
		}

		public string FailRunCommand {
			set { 
				failRunCommand = value; 
				setFailActions = true;
			}
		}

		public bool StartOnInstall {
			set {
				this.startOnInstall = value;
			}
		}

		public int StartTimeout {
			set {
				this.startTimeout = value;
			}
		}

		private void LogInstallMessage(EventLogEntryType logLevel, string msg) {
			Console.WriteLine(msg);
			try {
				EventLog.WriteEntry(base.ServiceName, msg, logLevel);
			} catch (Exception ex) {
				Console.WriteLine(ex.ToString());
			}

		}

		private bool GrandShutdownPrivilege() {

			// This code mimics the MSDN defined way to adjust privilege for shutdown
			// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/sysinfo/base/shutting_down.asp

			bool retRslt = false;

			IntPtr hToken = IntPtr.Zero;
			IntPtr myProc = IntPtr.Zero;

			TOKEN_PRIVILEGES tkp = new TOKEN_PRIVILEGES();

			long Luid = 0;
			int retLen = 0;

			try {

				myProc = GetCurrentProcess();
				bool rslt = OpenProcessToken(myProc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref hToken);
				if (!rslt)
					return retRslt;

				LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, ref Luid);

				tkp.PrivilegeCount = 1;
				tkp.Privileges.Luid = Luid;
				tkp.Privileges.Attributes = SE_PRIVILEGE_ENABLED;

				rslt = AdjustTokenPrivileges(hToken, false, ref tkp, 0, IntPtr.Zero, ref retLen);

				if (GetLastError() != 0) {

					throw new Exception("Failed to grant shutdown privilege");

				}

				retRslt = true;

			} catch (Exception ex) {

				LogInstallMessage(EventLogEntryType.Error, logMsgBase + ex.Message);

			} finally {

				if (hToken != IntPtr.Zero) {

					CloseHandle(hToken);

				}
			}

			return retRslt;

		}

		private void UpdateServiceConfig(object sender, InstallEventArgs e) {

			this.setFailActions = false;

			int numActions = FailureActions.Count;

			if (numActions > 0) {
				setFailActions = true;
			}
			if (!(this.setDescription || this.setFailActions))
				return;

			IntPtr scmHndl = IntPtr.Zero;
			IntPtr svcHndl = IntPtr.Zero;
			IntPtr tmpBuf = IntPtr.Zero;
			IntPtr svcLock = IntPtr.Zero;

			bool rslt = false;


			try {
				scmHndl = OpenSCManager(null, null, SC_MANAGER_ALL_ACCESS);
				if (scmHndl.ToInt32() <= 0) {
					LogInstallMessage(EventLogEntryType.Error, logMsgBase + "Failed to Open Service Control Manager");
					return;
				}

				svcLock = LockServiceDatabase(scmHndl);

				if (svcLock.ToInt32() <= 0) {
					LogInstallMessage(EventLogEntryType.Error, logMsgBase + "Failed to Lock Service Database for Write");
					return;
				}

				svcHndl = OpenService(scmHndl, base.ServiceName, SERVICE_ALL_ACCESS);

				if (svcHndl.ToInt32() <= 0) {
					LogInstallMessage(EventLogEntryType.Information, logMsgBase + "Failed to Open Service ");
					return;
				}

				// NOTE:  API lets set as many failure actions as one want, yet the Service Control Manager GUI only lets us see the first 3.
				if (this.setFailActions) {
				
					// We're gonna serialize the SA_ACTION structs into an array of ints
					// for simplicity in marshalling this variable length ptr to win32

					int[] actions = new int[numActions * 2];
					
					int currInd = 0;

					bool needShutdownPrivilege = false;

					foreach (FailureAction fa in FailureActions) {

						actions[currInd]	= (int)fa.Type;
						actions[++currInd]	= fa.Delay;
						currInd++;

						if (fa.Type == RecoverAction.Reboot) {
							needShutdownPrivilege = true;
						}
												 
					}

					// If we need shutdown privilege, then grant it to this process
					if (needShutdownPrivilege) {

						rslt = this.GrandShutdownPrivilege();

						if (!rslt)
							return;

					}

					// Need to pack 8 bytes per struct
					tmpBuf = Marshal.AllocHGlobal(numActions * 8);

					// Move array into marshallable pointer
					Marshal.Copy(actions, 0, tmpBuf, numActions * 2);

					// Set the SERVICE_FAILURE_ACTIONS struct
					SERVICE_FAILURE_ACTIONS sfa = new SERVICE_FAILURE_ACTIONS();

					sfa.cActions = numActions;
					sfa.dwResetPeriod = this.failResetTime;
					sfa.lpCommand = this.failRunCommand;
					sfa.lpRebootMsg = this.failRebootMsg;
					sfa.lpsaActions = tmpBuf.ToInt32();


					// Call the ChangeServiceFailureActions() abstraction of ChangeServiceConfig2()
					rslt = ChangeServiceFailureActions(svcHndl, SERVICE_CONFIG_FAILURE_ACTIONS, ref sfa);

					//Check the return
					if (!rslt) {

						int err = GetLastError();

						if (err == ERROR_ACCESS_DENIED) {
							throw new Exception(logMsgBase + "Access Denied while setting Failure Actions");
						}

					}
					Marshal.FreeHGlobal(tmpBuf);
					tmpBuf = IntPtr.Zero;
					LogInstallMessage(EventLogEntryType.Information, logMsgBase + "Successfully configured Failure Actions");

				}

				if (this.setDescription) {
					SERVICE_DESCRIPTION sd = new SERVICE_DESCRIPTION();
					sd.lpDescription = this.description;
					rslt = ChangeServiceDescription(svcHndl, SERVICE_CONFIG_DESCRIPTION, ref sd);
					if (!rslt) {
						throw new Exception(logMsgBase + "Failed to set description");
					}
					LogInstallMessage(EventLogEntryType.Information, logMsgBase + "Successfully set description");
				}

			}
			catch (Exception ex) {
				LogInstallMessage(EventLogEntryType.Error, ex.Message);
			} finally {
				if (scmHndl != IntPtr.Zero) {
					if (svcLock != IntPtr.Zero) {
						UnlockServiceDatabase(svcLock);
						svcLock = IntPtr.Zero;
					}
					CloseServiceHandle(scmHndl);
					scmHndl = IntPtr.Zero;
				}
				if (svcHndl != IntPtr.Zero) {
					CloseServiceHandle(svcHndl);
					svcHndl = IntPtr.Zero;
				}
				if (tmpBuf != IntPtr.Zero) {
					Marshal.FreeHGlobal(tmpBuf);
					tmpBuf = IntPtr.Zero;
				}
			}
		}


		
		// Method to start the service automatically after installation
		private void StartIfNeeded(object sender, InstallEventArgs e) {
			if (!this.startOnInstall)
				return;
			try {
				TimeSpan waitTo = new TimeSpan(0, 0, this.startTimeout);

				ServiceController sc = new ServiceController(base.ServiceName);
				sc.Start();
				sc.WaitForStatus(ServiceControllerStatus.Running, waitTo);
				sc.Close();
				LogInstallMessage(EventLogEntryType.Information, logMsgBase + " Service Started");
			}
			catch (Exception ex) {
				LogInstallMessage(EventLogEntryType.Error, logMsgBase + ex.Message);
			}
		}
	}

	public enum RecoverAction {
		None = 0,
		Restart = 1,
		Reboot = 2,
		RunCommand = 3
	}
	
	public class FailureAction {
		private RecoverAction type = RecoverAction.None;
		private int delay = 0;

		public FailureAction()	{ }

		public FailureAction(RecoverAction actionType, int actionDelay) {
			this.type = actionType;
			this.delay = actionDelay;
		}
		public RecoverAction Type {
			get{ return type; }
			set {
				type = value;
			}
		}
        
		public int Delay {
			get{ return delay; }
			set {
				delay = value;
			}				   
		}			
	}
}

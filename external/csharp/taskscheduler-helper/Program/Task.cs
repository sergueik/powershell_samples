using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using TaskSchedulerInterop;

namespace TaskScheduler {
	[Flags]
	public enum TaskFlags {
		Interactive = 0x1,
		DeleteWhenDone = 0x2,
		Disabled = 0x4,
		StartOnlyIfIdle = 0x10,
		KillOnIdleEnd = 0x20,
		DontStartIfOnBatteries = 0x40,
		KillIfGoingOnBatteries = 0x80,
		RunOnlyIfDocked = 0x100,
		Hidden = 0x200,
		RunIfConnectedToInternet = 0x400,
		RestartOnIdleResume = 0x800,
		SystemRequired = 0x1000,
		RunOnlyIfLoggedOn = 0x2000
	}

	public enum TaskStatus {
		Ready = HResult.SCHED_S_TASK_READY,
		Running = HResult.SCHED_S_TASK_RUNNING,
		NotScheduled = HResult.SCHED_S_TASK_NOT_SCHEDULED,
		NeverRun = HResult.SCHED_S_TASK_HAS_NOT_RUN,
		Disabled = HResult.SCHED_S_TASK_DISABLED,
		NoMoreRuns = HResult.SCHED_S_TASK_NO_MORE_RUNS,
		Terminated = HResult.SCHED_S_TASK_TERMINATED,
		NoTriggers = HResult.SCHED_S_TASK_NO_VALID_TRIGGERS,
		NoTriggerTime = HResult.SCHED_S_EVENT_TRIGGER
	}

	public class Task : IDisposable {
		private ITask iTask;
		private string name;
		private TriggerList triggers;
		internal Task(ITask iTask, string taskName) {
			this.iTask = iTask;
			if (taskName.EndsWith(".job"))
				name = taskName.Substring(0, taskName.Length-4);
			else
				name = taskName;
			triggers = null;
			this.Hidden = GetHiddenFileAttr();
		}
		public string Name {
			get {
				return name;
			}
		}

		public TriggerList Triggers {
			get {
				if (triggers == null) {
					// Trigger list has not been requested before; create it
					triggers = new TriggerList(iTask);
				}
				return triggers;
			}
		}

		public string ApplicationName {
			get {
				IntPtr lpwstr;
				iTask.GetApplicationName(out lpwstr);
				return CoTaskMem.LPWStrToString(lpwstr);
			}
			set {
				iTask.SetApplicationName(value);
			}
		}
		public string AccountName {
			get {
				IntPtr lpwstr = IntPtr.Zero;
				iTask.GetAccountInformation(out lpwstr);
				return CoTaskMem.LPWStrToString(lpwstr);
			}
		}

		public string Comment {
			get {
				IntPtr lpwstr;
				iTask.GetComment(out lpwstr);
				return CoTaskMem.LPWStrToString(lpwstr);
			}
			set {
				iTask.SetComment(value);
			}
		}

		public string Creator {
			get {
				IntPtr lpwstr;
				iTask.GetCreator(out lpwstr);
				return CoTaskMem.LPWStrToString(lpwstr);
			}
			set {
				iTask.SetCreator(value);
			}
		}

		private short ErrorRetryCount {
			get {
				ushort ret;
				iTask.GetErrorRetryCount(out ret);
				return (short)ret;
			}
			set {
				iTask.SetErrorRetryCount((ushort)value);
			}
		}

		private short ErrorRetryInterval {
			get {
				ushort ret;
				iTask.GetErrorRetryInterval(out ret);
				return (short)ret;
			}
			set {
				iTask.SetErrorRetryInterval((ushort)value);
			}
		}

		public int ExitCode {
			get {
				uint ret = 0;
				iTask.GetExitCode(out ret);
				return (int)ret;
			}
		}

		public TaskFlags Flags {
			get {
				uint ret;
				iTask.GetFlags(out ret);
				return (TaskFlags)ret;
			}
			set {
				iTask.SetFlags((uint)value);
			}
		}

		public short IdleWaitMinutes {
			get {
				ushort ret, nothing;
				iTask.GetIdleWait(out ret, out nothing);
				return (short)ret;
			}
			set {
				ushort m = (ushort)IdleWaitDeadlineMinutes;
				iTask.SetIdleWait((ushort)value, m);
			}
		}

		public short IdleWaitDeadlineMinutes {
			get {
				ushort ret, nothing;
				iTask.GetIdleWait(out nothing, out ret);
				return (short)ret;
			}
			set {
				ushort m = (ushort)IdleWaitMinutes;
				iTask.SetIdleWait(m, (ushort)value);
			}
		}

		public TimeSpan MaxRunTime {
			get {
				uint ret;
				iTask.GetMaxRunTime(out ret);
				return new TimeSpan((long)ret * TimeSpan.TicksPerMillisecond);
			}
			set {
				double proposed = ((TimeSpan)value).TotalMilliseconds;
				if (proposed >= uint.MaxValue) { 
					iTask.SetMaxRunTime(uint.MaxValue);
				} else {
					iTask.SetMaxRunTime((uint)proposed);
				}

				//iTask.SetMaxRunTime((uint)((TimeSpan)value).TotalMilliseconds);
			}
		}

		public bool MaxRunTimeLimited {
			get {
				uint ret;
				iTask.GetMaxRunTime(out ret);
				return (ret == uint.MaxValue);
			}
			set {
				if (value) {
					uint ret;
					iTask.GetMaxRunTime(out ret);
					if (ret == uint.MaxValue) {
						iTask.SetMaxRunTime(72*360*1000); //72 hours.  Thats what Explorer sets.
					}
				} else {
					iTask.SetMaxRunTime(uint.MaxValue);
				}
			}
		}

		public DateTime MostRecentRunTime {
			get {
				SystemTime st = new SystemTime();
				iTask.GetMostRecentRunTime(ref st);
				if (st.Year == 0)
					return DateTime.MinValue;
				return new DateTime((int)st.Year, (int)st.Month, (int)st.Day, (int)st.Hour, (int)st.Minute, (int)st.Second, (int)st.Milliseconds);
			}
		}

		public DateTime NextRunTime {
			get {
				SystemTime st = new SystemTime();
				iTask.GetNextRunTime(ref st);
				if (st.Year == 0)
					return DateTime.MinValue;
				return new DateTime((int)st.Year, (int)st.Month, (int)st.Day, (int)st.Hour, (int)st.Minute, (int)st.Second, (int)st.Milliseconds);
			}
		}

		public string Parameters {
			get {
				IntPtr lpwstr;
				iTask.GetParameters(out lpwstr);
				return CoTaskMem.LPWStrToString(lpwstr);
			}
			set {
				iTask.SetParameters(value);
			}
		}
		public System.Diagnostics.ProcessPriorityClass Priority {
			get {
				uint ret;
				iTask.GetPriority(out ret);
				return (System.Diagnostics.ProcessPriorityClass)ret;
			}
			set {
				if (value==System.Diagnostics.ProcessPriorityClass.AboveNormal ||
					value==System.Diagnostics.ProcessPriorityClass.BelowNormal ) {
					throw new ArgumentException("Unsupported Priority Level");
				}
				iTask.SetPriority((uint)value);
			}
		}

		public TaskStatus Status {
			get {
				int ret;
				iTask.GetStatus(out ret);
				return (TaskStatus)ret;
			}
		}

		private int FlagsEx {
			get {
				uint ret;
				iTask.GetTaskFlags(out ret);
				return (int)ret;
			}
			set {
				iTask.SetTaskFlags((uint)value);
			}
		}

		public string WorkingDirectory {
			get {
				IntPtr lpwstr;
				iTask.GetWorkingDirectory(out lpwstr);
				return CoTaskMem.LPWStrToString(lpwstr);
			}
			set {
				iTask.SetWorkingDirectory(value);
			}
		}

		public bool Hidden {
			get {
				return (this.Flags & TaskFlags.Hidden) != 0;
			}
			set {
				if (value) {
					this.Flags |= TaskFlags.Hidden;
				} else {
					this.Flags &= ~TaskFlags.Hidden;
				}
			}
		}

		public object Tag {
			get {
				ushort DataLen;
				IntPtr Data;
				iTask.GetWorkItemData(out DataLen, out Data);
				byte[] bytes = new byte[DataLen];
				Marshal.Copy(Data, bytes, 0, DataLen);
				MemoryStream stream = new MemoryStream(bytes, false);
				BinaryFormatter b = new BinaryFormatter();
				return b.Deserialize(stream);
			}
			set {
				if (!value.GetType().IsSerializable)
					throw new ArgumentException("Objects set as Data for Tasks must be serializable", "value");
				BinaryFormatter b = new BinaryFormatter();
				MemoryStream stream = new MemoryStream();
				b.Serialize(stream, value);
				iTask.SetWorkItemData((ushort)stream.Length, stream.GetBuffer());
			}
		}
		private void SetHiddenFileAttr(bool set) {
			IPersistFile iFile = (IPersistFile)iTask;
			string fileName;
			iFile.GetCurFile(out fileName);
			System.IO.FileAttributes attr;
			attr = System.IO.File.GetAttributes(fileName);
			if (set)
				attr |= System.IO.FileAttributes.Hidden;
			else
				attr &= ~System.IO.FileAttributes.Hidden;
			System.IO.File.SetAttributes(fileName, attr);
		}
		private bool GetHiddenFileAttr() {
			IPersistFile iFile = (IPersistFile)iTask;
			string fileName;
			iFile.GetCurFile(out fileName);
			System.IO.FileAttributes attr;
			try {
				attr = System.IO.File.GetAttributes(fileName);
				return (attr & System.IO.FileAttributes.Hidden) != 0;
			} catch {
				return false;
			}
		}

		public DateTime NextRunTimeAfter(DateTime after) {
			//Add one second to get a run time strictly greater than the specified time.
			after = after.AddSeconds(1); 
			//Convert to a valid SystemTime
			SystemTime stAfter = new SystemTime();
			stAfter.Year = (ushort)after.Year;
			stAfter.Month = (ushort)after.Month;
			stAfter.Day = (ushort)after.Day;
			stAfter.DayOfWeek = (ushort)after.DayOfWeek;
			stAfter.Hour = (ushort)after.Hour;
			stAfter.Minute = (ushort)after.Minute;
			stAfter.Second = (ushort)after.Second;
			SystemTime stLimit = new SystemTime();
			// Would like to pass null as the second parameter to GetRunTimes, indicating that
			// the interval is unlimited.  Can't figure out how to do that, so use a big time value.
			stLimit = stAfter;
			stLimit.Year = (ushort)DateTime.MaxValue.Year;
			stLimit.Month = 1;  //Just in case stAfter date was Feb 29, but MaxValue.Year is not a leap year!
			IntPtr pTimes;
			ushort nFetch = 1;
			iTask.GetRunTimes(ref stAfter, ref stLimit, ref nFetch, out pTimes);
			if (nFetch == 1) {
				SystemTime stNext = new SystemTime();
				stNext = (SystemTime)Marshal.PtrToStructure(pTimes, typeof(SystemTime));
				Marshal.FreeCoTaskMem(pTimes);
				return new DateTime(stNext.Year, stNext.Month, stNext.Day, stNext.Hour, stNext.Minute, stNext.Second);
			} else {
				return DateTime.MinValue;
			}
		}

		public void Run() {
			iTask.Run();
		}

		public void Save() {
			IPersistFile iFile = (IPersistFile)iTask;
			iFile.Save(null, false);
			SetHiddenFileAttr(Hidden);  //Do the Task Scheduler's work for it because it doesn't reset properly
		}

		public void Save(string name) {
			IPersistFile iFile = (IPersistFile)iTask;
			string path;
			iFile.GetCurFile(out path);
			string newPath;
			newPath = Path.GetDirectoryName(path) + Path.DirectorySeparatorChar + name + Path.GetExtension(path);
			iFile.Save(newPath, true);
			iFile.SaveCompleted(newPath); /* probably unnecessary */
			this.name = name;
			SetHiddenFileAttr(Hidden);  //Do the Task Scheduler's work for it because it doesn't reset properly
		}

		public void Close() {
			if (triggers != null) {
				triggers.Dispose();
			}
			Marshal.ReleaseComObject(iTask);
			iTask = null;
		}

		public void DisplayForEdit() {
			iTask.EditWorkItem(0, 0);  
		}

		[Flags]
		public enum PropPages {
			Task = 0x01,
			Schedule = 0x02,
			Settings = 0x04
		}
		public bool DisplayPropertySheet() {
			//iTask.EditWorkItem(0, 0);  //This implementation saves automatically, so we don't use it.
			return DisplayPropertySheet(PropPages.Task | PropPages.Schedule | PropPages.Settings);
		}

		public bool DisplayPropertySheet(PropPages pages) {
			PropSheetHeader hdr = new PropSheetHeader();
			IProvideTaskPage iProvideTaskPage = (IProvideTaskPage)iTask;
			IntPtr[] hPages = new IntPtr[3];
			IntPtr hPage;
			int nPages = 0;
			if ((pages & PropPages.Task) != 0) {
				//get task page
				iProvideTaskPage.GetPage(0, false, out hPage);
				hPages[nPages++] = hPage;
			}
			if ((pages & PropPages.Schedule) != 0) {
				//get task page
				iProvideTaskPage.GetPage(1, false, out hPage);
				hPages[nPages++] = hPage;
			}
			if ((pages & PropPages.Settings) != 0) {
				//get task page
				iProvideTaskPage.GetPage(2, false, out hPage);
				hPages[nPages++] = hPage;
			}
			if (nPages == 0) throw (new ArgumentException("No Property Pages to display"));
			hdr.dwSize = (uint)Marshal.SizeOf(hdr);
			hdr.dwFlags = (uint) (PropSheetFlags.PSH_DEFAULT | PropSheetFlags.PSH_NOAPPLYNOW);
			hdr.pszCaption = this.Name;
			hdr.nPages = (uint)nPages;
			GCHandle gch = GCHandle.Alloc(hPages, GCHandleType.Pinned);
			hdr.phpage = gch.AddrOfPinnedObject();
			int res = PropertySheetDisplay.PropertySheet(ref hdr);
			gch.Free();
			if (res < 0) throw (new Exception("Property Sheet failed to display"));
			return res>0;
		}


		public void SetAccountInformation(string accountName, string password) {
			IntPtr pwd = Marshal.StringToCoTaskMemUni(password);
			iTask.SetAccountInformation(accountName, pwd);
			Marshal.FreeCoTaskMem(pwd);
		}
		public void SetAccountInformation(string accountName, SecureString password) {
			IntPtr pwd = Marshal.SecureStringToCoTaskMemUnicode(password);
			iTask.SetAccountInformation(accountName, pwd);
			Marshal.ZeroFreeCoTaskMemUnicode(pwd);
		}
		public void Terminate() {
				iTask.Terminate();
		}

		public override string ToString() {
			return string.Format("{0} (\"{1}\" {2})", name, ApplicationName, Parameters);
		}
		public void Dispose() {
			this.Close();
		}
	}
}

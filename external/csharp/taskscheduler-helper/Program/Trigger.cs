using System;
using System.Runtime.InteropServices;
using TaskSchedulerInterop;

namespace TaskScheduler {
	internal enum TriggerType {
		RunOnce = 0,
		RunDaily = 1,
		RunWeekly = 2,
		RunMonthly = 3,
		RunMonthlyDOW = 4,
		OnIdle = 5,
		OnSystemStart = 6,
		OnLogon = 7
	}
	[Flags]
	public enum DaysOfTheWeek : short {
		Sunday = 0x1,
		Monday = 0x2,
		Tuesday = 0x4,
		Wednesday = 0x8,
		Thursday = 0x10,
		Friday = 0x20,
		Saturday = 0x40
	}

	public enum WhichWeek : short {
		FirstWeek = 1,
		SecondWeek = 2,
		ThirdWeek = 3,
		FourthWeek = 4,
		LastWeek = 5
	}

	[Flags]
	public enum MonthsOfTheYear : short {
		January = 0x1,
		February = 0x2,
		March = 0x4,
		April = 0x8,
		May = 0x10,
		June = 0x20,
		July = 0x40,
		August = 0x80,
		September = 0x100,
		October = 0x200,
		November = 0x400,
		December = 0x800
	}
	public abstract class Trigger : ICloneable {
		[Flags]
			private enum TaskTriggerFlags {
			HasEndDate = 0x1,
			KillAtDurationEnd = 0x2,
			Disabled = 0x4
		}
		private ITaskTrigger iTaskTrigger; //null for an unbound Trigger
		internal TaskTrigger taskTrigger;
		internal Trigger() {
			iTaskTrigger = null;
			taskTrigger = new TaskTrigger();
			taskTrigger.TriggerSize = (ushort)Marshal.SizeOf(taskTrigger);
			taskTrigger.BeginYear = (ushort)DateTime.Today.Year;
			taskTrigger.BeginMonth = (ushort)DateTime.Today.Month;
			taskTrigger.BeginDay = (ushort)DateTime.Today.Day;
		}

		internal Trigger(ITaskTrigger iTrigger) {
			if (iTrigger == null)
				throw new ArgumentNullException("iTrigger", "ITaskTrigger instance cannot be null");
			taskTrigger = new TaskTrigger();
			taskTrigger.TriggerSize = (ushort)Marshal.SizeOf(taskTrigger);
			iTrigger.GetTrigger(ref taskTrigger);
			iTaskTrigger = iTrigger;
		}

		public object Clone() {
			Trigger newTrigger = (Trigger)this.MemberwiseClone();
			newTrigger.iTaskTrigger = null; // The clone is not bound
			return newTrigger;
		}

		internal bool Bound {
			get {
				return iTaskTrigger != null;
			}
		}
		public DateTime BeginDate {
			get {
				return new DateTime(taskTrigger.BeginYear, taskTrigger.BeginMonth, taskTrigger.BeginDay);
			}
			set {
				taskTrigger.BeginYear = (ushort)value.Year;
				taskTrigger.BeginMonth = (ushort)value.Month;
				taskTrigger.BeginDay = (ushort)value.Day;
				SyncTrigger();
			}
		}

		public bool HasEndDate {
			get {
				return ((taskTrigger.Flags & (uint)TaskTriggerFlags.HasEndDate) == (uint)TaskTriggerFlags.HasEndDate);
			}
			set {
				if (value)
					throw new ArgumentException("HasEndDate can only be set false");
				taskTrigger.Flags &= ~(uint)TaskTriggerFlags.HasEndDate;
				SyncTrigger();
			}
		}
	
		public DateTime EndDate {
			get {
				if (taskTrigger.EndYear == 0)
					return DateTime.MinValue;
				return new DateTime(taskTrigger.EndYear, taskTrigger.EndMonth, taskTrigger.EndDay);
			}
			set {
				taskTrigger.Flags |= (uint)TaskTriggerFlags.HasEndDate;
				taskTrigger.EndYear = (ushort)value.Year;
				taskTrigger.EndMonth = (ushort)value.Month;
				taskTrigger.EndDay = (ushort)value.Day;
				SyncTrigger();
			}
		}

		public int DurationMinutes {
			get {
				return (int)taskTrigger.MinutesDuration;
			}
			set {
				if (value < taskTrigger.MinutesInterval)
					throw new ArgumentOutOfRangeException("DurationMinutes", value, "DurationMinutes must be greater than or equal the IntervalMinutes value");
				taskTrigger.MinutesDuration = (uint)value;
				SyncTrigger();
			}
		}

		public int IntervalMinutes {
			get {
				return (int)taskTrigger.MinutesInterval;
			}
			set {
				if (value > taskTrigger.MinutesDuration)
					throw new ArgumentOutOfRangeException("IntervalMinutes", value, "IntervalMinutes must be less than or equal the DurationMinutes value");
				taskTrigger.MinutesInterval = (uint)value;
				SyncTrigger();
			}
		}

		public bool KillAtDurationEnd {
			get {
				return ((taskTrigger.Flags & (uint)TaskTriggerFlags.KillAtDurationEnd) == (uint)TaskTriggerFlags.KillAtDurationEnd);
			}
			set {
				if (value)
					taskTrigger.Flags |= (uint)TaskTriggerFlags.KillAtDurationEnd;
				else
					taskTrigger.Flags &= ~(uint)TaskTriggerFlags.KillAtDurationEnd;
				SyncTrigger();
			}
		}

		public bool Disabled {
			get {
				return ((taskTrigger.Flags & (uint)TaskTriggerFlags.Disabled) == (uint)TaskTriggerFlags.Disabled);
			}
			set {
				if (value)
					taskTrigger.Flags |= (uint)TaskTriggerFlags.Disabled;
				else
					taskTrigger.Flags &= ~(uint)TaskTriggerFlags.Disabled;
				SyncTrigger();
			}
		}

		internal static Trigger CreateTrigger(ITaskTrigger iTaskTrigger) {
			if (iTaskTrigger == null)
				throw new ArgumentNullException("iTaskTrigger", "Instance of ITaskTrigger cannot be null");
			TaskTrigger sTaskTrigger = new TaskTrigger();
			sTaskTrigger.TriggerSize = (ushort)Marshal.SizeOf(sTaskTrigger);
			iTaskTrigger.GetTrigger(ref sTaskTrigger);
			switch ((TriggerType)sTaskTrigger.Type) {
			case TriggerType.RunOnce:
				return new RunOnceTrigger(iTaskTrigger);
			case TriggerType.RunDaily:
				return new DailyTrigger(iTaskTrigger);
			case TriggerType.RunWeekly:
				return new WeeklyTrigger(iTaskTrigger);
			case TriggerType.RunMonthlyDOW:
				return new MonthlyDOWTrigger(iTaskTrigger);
			case TriggerType.RunMonthly:
				return new MonthlyTrigger(iTaskTrigger);
			case TriggerType.OnIdle:
				return new OnIdleTrigger(iTaskTrigger);
			case TriggerType.OnSystemStart:
				return new OnSystemStartTrigger(iTaskTrigger);
			case TriggerType.OnLogon:
				return new OnLogonTrigger(iTaskTrigger);
			default:
				throw new ArgumentException("Unable to recognize type of trigger referenced in iTaskTrigger", 
											"iTaskTrigger");
			}
		}

		protected void SyncTrigger() {
			if (iTaskTrigger!=null) iTaskTrigger.SetTrigger(ref taskTrigger);
		}

		internal void Bind(ITaskTrigger iTaskTrigger) {
			if (this.iTaskTrigger != null)
				throw new ArgumentException("Attempt to bind an already bound trigger");
			this.iTaskTrigger = iTaskTrigger;
			iTaskTrigger.SetTrigger(ref taskTrigger);
		}

		internal void Bind(Trigger trigger) {
			Bind(trigger.iTaskTrigger);
		}

		internal void Unbind() {
			if (iTaskTrigger != null) {
				Marshal.ReleaseComObject(iTaskTrigger);
				iTaskTrigger = null;
			}
		}

		public override string ToString() {
			if (iTaskTrigger != null) {
				IntPtr lpwstr;
				iTaskTrigger.GetTriggerString(out lpwstr);
				return CoTaskMem.LPWStrToString(lpwstr);
			} else {
				return "Unbound " + this.GetType().ToString();
			}
		}

		public override bool Equals(object obj) {
			return taskTrigger.Equals(((Trigger)obj).taskTrigger);
		}

		public override int GetHashCode() {
			return taskTrigger.GetHashCode();
		}
	}

	public abstract class StartableTrigger : Trigger {
		internal StartableTrigger() : base() {
		}

		internal StartableTrigger(ITaskTrigger iTrigger) : base(iTrigger) {
		}

		protected void SetStartTime(ushort hour, ushort minute) {
//			if (hour < 0 || hour > 23)
//				throw new ArgumentOutOfRangeException("hour", hour, "hour must be between 0 and 23");
//			if (minute < 0 || minute > 59)
//				throw new ArgumentOutOfRangeException("minute", minute, "minute must be between 0 and 59");
//			taskTrigger.StartHour = hour;
//			taskTrigger.StartMinute = minute;
//			base.SyncTrigger();
			StartHour = (short)hour;
			StartMinute = (short)minute;
		}

		public short StartHour {
			get {
				return (short)taskTrigger.StartHour;
			}
			set {
				if (value < 0 || value > 23)
					throw new ArgumentOutOfRangeException("hour", value, "hour must be between 0 and 23");
				taskTrigger.StartHour = (ushort)value;
				base.SyncTrigger();
			}
		}

		public short StartMinute {
			get {
				return (short)taskTrigger.StartMinute;
			}
			set {
				if (value < 0 || value > 59)
					throw new ArgumentOutOfRangeException("minute", value, "minute must be between 0 and 59");
				taskTrigger.StartMinute = (ushort)value;
				base.SyncTrigger();
			}
		}
	}

	public class RunOnceTrigger : StartableTrigger {

		public RunOnceTrigger(DateTime runDateTime) : base() {
			taskTrigger.BeginYear = (ushort)runDateTime.Year;
			taskTrigger.BeginMonth = (ushort)runDateTime.Month;
			taskTrigger.BeginDay = (ushort)runDateTime.Day;
			SetStartTime((ushort)runDateTime.Hour, (ushort)runDateTime.Minute);
			taskTrigger.Type = TaskTriggerType.TIME_TRIGGER_ONCE;
		}

		internal RunOnceTrigger(ITaskTrigger iTrigger) : base(iTrigger) {
		}
	}

	public class DailyTrigger : StartableTrigger {
		public DailyTrigger(short hour, short minutes, short daysInterval) : base() {
			SetStartTime((ushort)hour, (ushort)minutes);
			taskTrigger.Type = TaskTriggerType.TIME_TRIGGER_DAILY;
			taskTrigger.Data.daily.DaysInterval = (ushort)daysInterval;
		}

		public DailyTrigger(short hour, short minutes) : this(hour, minutes, 1) {
		}

		internal DailyTrigger(ITaskTrigger iTrigger) : base(iTrigger) {
		}

		public short DaysInterval {
			get {
				return (short)taskTrigger.Data.daily.DaysInterval;
			}
			set {
				taskTrigger.Data.daily.DaysInterval = (ushort)value;
				base.SyncTrigger();
			}
		}
	}

	public class WeeklyTrigger : StartableTrigger {

		public WeeklyTrigger(short hour, short minutes, DaysOfTheWeek daysOfTheWeek, short weeksInterval) : base() {
			SetStartTime((ushort)hour, (ushort)minutes);
			taskTrigger.Type = TaskTriggerType.TIME_TRIGGER_WEEKLY;
			taskTrigger.Data.weekly.WeeksInterval = (ushort)weeksInterval;
			taskTrigger.Data.weekly.DaysOfTheWeek = (ushort)daysOfTheWeek;
		}

		public WeeklyTrigger(short hour, short minutes, DaysOfTheWeek daysOfTheWeek) : this(hour, minutes, daysOfTheWeek, 1) {
		}

		internal WeeklyTrigger(ITaskTrigger iTrigger) : base(iTrigger) {
		}

		public short WeeksInterval {
			get {
				return (short)taskTrigger.Data.weekly.WeeksInterval;
			}
			set {
				taskTrigger.Data.weekly.WeeksInterval = (ushort)value;
				base.SyncTrigger();
			}
		}

		public DaysOfTheWeek WeekDays {
			get {
				return (DaysOfTheWeek)taskTrigger.Data.weekly.DaysOfTheWeek;
			}
			set {
				taskTrigger.Data.weekly.DaysOfTheWeek = (ushort)value;
				base.SyncTrigger();
			}
		}
	}

	public class MonthlyDOWTrigger : StartableTrigger {
		public MonthlyDOWTrigger(short hour, short minutes, DaysOfTheWeek daysOfTheWeek, WhichWeek whichWeeks, MonthsOfTheYear months) : base() {
			SetStartTime((ushort)hour, (ushort)minutes);
			taskTrigger.Type = TaskTriggerType.TIME_TRIGGER_MONTHLYDOW;
			taskTrigger.Data.monthlyDOW.WhichWeek = (ushort)whichWeeks;
			taskTrigger.Data.monthlyDOW.DaysOfTheWeek = (ushort)daysOfTheWeek;
			taskTrigger.Data.monthlyDOW.Months = (ushort)months;
		}

		public MonthlyDOWTrigger(short hour, short minutes, DaysOfTheWeek daysOfTheWeek, WhichWeek whichWeeks) :
			this(hour, minutes, daysOfTheWeek, whichWeeks,
			MonthsOfTheYear.January|MonthsOfTheYear.February|MonthsOfTheYear.March|MonthsOfTheYear.April|MonthsOfTheYear.May|MonthsOfTheYear.June|MonthsOfTheYear.July|MonthsOfTheYear.August|MonthsOfTheYear.September|MonthsOfTheYear.October|MonthsOfTheYear.November|MonthsOfTheYear.December) {
		}

		internal MonthlyDOWTrigger(ITaskTrigger iTrigger) : base(iTrigger) {
		}

		public short WhichWeeks {
			get {
				return (short)taskTrigger.Data.monthlyDOW.WhichWeek;
			}
			set {
				taskTrigger.Data.monthlyDOW.WhichWeek = (ushort)value;
				base.SyncTrigger();
			}
		}

		public DaysOfTheWeek WeekDays {
			get {
				return (DaysOfTheWeek)taskTrigger.Data.monthlyDOW.DaysOfTheWeek;
			}
			set {
					taskTrigger.Data.monthlyDOW.DaysOfTheWeek = (ushort)value;
					base.SyncTrigger();
			}
		}

		public MonthsOfTheYear Months {
			get {
				return (MonthsOfTheYear)taskTrigger.Data.monthlyDOW.Months;
			}
			set {
				taskTrigger.Data.monthlyDOW.Months = (ushort)value;
				base.SyncTrigger();
			}
		}
	}

	public class MonthlyTrigger : StartableTrigger {
		public MonthlyTrigger(short hour, short minutes, int[] daysOfMonth, MonthsOfTheYear months): base() {
			SetStartTime((ushort)hour, (ushort)minutes);
			taskTrigger.Type = TaskTriggerType.TIME_TRIGGER_MONTHLYDATE;
			taskTrigger.Data.monthlyDate.Months = (ushort)months;
			taskTrigger.Data.monthlyDate.Days = (uint)IndicesToMask(daysOfMonth);
		}

		public MonthlyTrigger(short hour, short minutes, int[] daysOfMonth) :
			this(hour, minutes, daysOfMonth,
			MonthsOfTheYear.January|MonthsOfTheYear.February|MonthsOfTheYear.March|MonthsOfTheYear.April|MonthsOfTheYear.May|MonthsOfTheYear.June|MonthsOfTheYear.July|MonthsOfTheYear.August|MonthsOfTheYear.September|MonthsOfTheYear.October|MonthsOfTheYear.November|MonthsOfTheYear.December) {
		}

		internal MonthlyTrigger(ITaskTrigger iTrigger) : base(iTrigger) {
		}


		public MonthsOfTheYear Months {
			get {
				return (MonthsOfTheYear)taskTrigger.Data.monthlyDate.Months;
			}
			set {
				taskTrigger.Data.monthlyDOW.Months = (ushort)value;
				base.SyncTrigger();
			}
		}

		private static int[] MaskToIndices(int mask) {
			//count bits in mask
			int cnt = 0;
			for (int i=0; (mask>>i)>0; i++)
				cnt = cnt + (1 & (mask>>i));
			//allocate return array with one entry for each bit
			int[] indices = new int[cnt];
			//fill array with bit indices
			cnt = 0;
			for (int i=0; (mask>>i)>0; i++)
				if ((1 & (mask>>i)) == 1)
					indices[cnt++] = i+1;
			return indices;
		}

		private static int IndicesToMask(int[] indices) {
			int mask = 0;
			foreach (int index in indices) {
				if (index<1 || index>31) throw new ArgumentException("Days must be in the range 1..31");
				mask = mask | 1<<(index-1);
			}
			return mask;
		}

		public int[] Days {
			get {
				return MaskToIndices((int)taskTrigger.Data.monthlyDate.Days);
			}
			set {
				taskTrigger.Data.monthlyDate.Days = (uint)IndicesToMask(value);
				base.SyncTrigger();
			}
		}
	}

	public class OnIdleTrigger : Trigger {
		public OnIdleTrigger() : base() {
			taskTrigger.Type = TaskTriggerType.EVENT_TRIGGER_ON_IDLE;
		}

		internal OnIdleTrigger(ITaskTrigger iTrigger) : base(iTrigger) {
		}
	}

	public class OnSystemStartTrigger : Trigger {
		public OnSystemStartTrigger() : base() {
			taskTrigger.Type = TaskTriggerType.EVENT_TRIGGER_AT_SYSTEMSTART;
		}

		internal OnSystemStartTrigger(ITaskTrigger iTrigger) : base(iTrigger) {
		}
	}

	public class OnLogonTrigger : Trigger {
		public OnLogonTrigger() : base() {
			taskTrigger.Type = TaskTriggerType.EVENT_TRIGGER_AT_LOGON;
		}
		internal OnLogonTrigger(ITaskTrigger iTrigger) : base(iTrigger) {
		}
	}


}
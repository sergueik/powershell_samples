using System;
using System.Collections;

namespace TaskScheduler{
	public class TaskList : IEnumerable, IDisposable {
		private ScheduledTasks st = null;
		private string nameComputer;
		internal TaskList() {
			st = new ScheduledTasks();
		}

		internal TaskList(string computer) {
			st = new ScheduledTasks(computer);
		}

		private class Enumerator : IEnumerator {
			private ScheduledTasks outer;
			private string[] nameTask;
			private int curIndex;
			private Task curTask;

			internal Enumerator(ScheduledTasks st) {
				outer = st;
				nameTask = st.GetTaskNames();
				Reset();
			}

			public bool MoveNext() {
				bool ok = ++curIndex < nameTask.Length;
				if (ok) curTask = outer.OpenTask(nameTask[curIndex]);
				return ok;
			}

			public void Reset() {
				curIndex = -1;
				curTask = null;
			}

			public object Current {
				get {
					return curTask;
				}
			}
		}
		
		internal string TargetComputer {
			get {
				return nameComputer;
			}
			set {
				st.Dispose();
				st = new ScheduledTasks(value);
				nameComputer = value;
			}
		}

		public Task NewTask(string name) {
			return st.CreateTask(name);
		}

		public void Delete(string name) {
			st.DeleteTask(name);
		}

		public Task this[string name] {
			get {
				return st.OpenTask(name);
			}
		}

		public System.Collections.IEnumerator GetEnumerator() {
			return new Enumerator(st);
		}
		public void Dispose() {
			st.Dispose();
		}
	}

}
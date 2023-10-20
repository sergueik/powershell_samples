using System;
using System.Collections;
using TaskSchedulerInterop;

namespace TaskScheduler {

	public class TriggerList : IList, IDisposable {
		private ITask iTask;
		private ArrayList oTriggers;

		internal TriggerList(ITask iTask) {
			this.iTask = iTask;
			ushort cnt = 0;
			iTask.GetTriggerCount(out cnt);
			oTriggers = new ArrayList(cnt+5); //Allow for five additional entries without growing base array
			for (int i=0; i<cnt; i++) {
				ITaskTrigger iTaskTrigger;
				iTask.GetTrigger((ushort)i, out iTaskTrigger);
				oTriggers.Add(Trigger.CreateTrigger(iTaskTrigger));
			}
		}

		private class Enumerator : IEnumerator {
			private TriggerList outer;
			private int currentIndex;

			internal Enumerator(TriggerList outer) {
				this.outer = outer;
				Reset();
			}

			public bool MoveNext() {
				return ++currentIndex < outer.oTriggers.Count;
			}

			public void Reset() {
				currentIndex = -1;
			}

			public object Current {
				get { return outer.oTriggers[currentIndex]; }
			}
		}

		public void RemoveAt(int index) {
			if (index >= Count)
				throw new ArgumentOutOfRangeException("index", index, "Failed to remove Trigger. Index out of range.");
			((Trigger)oTriggers[index]).Unbind(); //releases resources in the trigger
			oTriggers.RemoveAt(index); //Remove the Trigger object from the array representing the list
			iTask.DeleteTrigger((ushort)index); //Remove the trigger from the Task Scheduler
		}

		void IList.Insert(int index, object value) {
			throw new NotImplementedException("TriggerList does not support Insert().");
		}

		public void Remove(Trigger trigger) {
			int i = IndexOf(trigger);
			if (i != -1)
				RemoveAt(i);
		}

		void IList.Remove(object value) {
			Remove(value as Trigger);
		}

		public bool Contains(Trigger trigger) {
			return (IndexOf(trigger) != -1);
		}

		bool IList.Contains(object value) {
			return Contains(value as Trigger);
		}

		public void Clear() {
			for (int i = Count-1; i >= 0; i--)  {
				RemoveAt(i);
			}
		}

		public int IndexOf(Trigger trigger) {
			for (int i = 0; i < Count; i++) {
				if (this[i].Equals(trigger))
					return i;
			}
			return -1;
		}

		int IList.IndexOf(object value) {
			return IndexOf(value as Trigger);
		}

		public int Add(Trigger trigger) {
			// if trigger is already bound a list throw an exception
			if (trigger.Bound) 
				throw new ArgumentException("A Trigger cannot be added if it is already in a list.");
			// Add a trigger to the task for this TaskList
			ITaskTrigger iTrigger;
			ushort index;
			iTask.CreateTrigger(out index, out iTrigger);
			// Add the Trigger to the TaskList
			trigger.Bind(iTrigger);
			int index2 = oTriggers.Add(trigger);
			// Verify index is the same in task and in list
			if (index2 != (int)index) 
				throw new ApplicationException("Assertion Failure");
			return (int)index;
		}

		int IList.Add(object value) {
			return Add(value as Trigger);
		}
		public bool IsReadOnly {
			get { return false; }
		}

		public Trigger this[int index] {
			get {
				if (index >= Count)
					throw new ArgumentOutOfRangeException("index", index, "TriggerList collection");
				return (Trigger)oTriggers[index];
			}
			set {
				if (index >= Count)
					throw new ArgumentOutOfRangeException("index", index, "TriggerList collection");
				Trigger previous = (Trigger)oTriggers[index];
				value.Bind(previous);
				oTriggers[index] = value;
			}
		}

		object IList.this[int index] {
			get { return this[index]; }
			set { this[index] = (value as Trigger); }
		}

		public bool IsFixedSize {
			get { return false; }
		}
		public int Count {
			get {
				return oTriggers.Count;
			}
		}

		public void CopyTo(System.Array array, int index) {
			if (oTriggers.Count > array.Length - index) {
				throw new ArgumentException("Array has insufficient space to copy the collection.");
			}
			for (int i = 0; i<oTriggers.Count; i++) {
				array.SetValue( ((Trigger)oTriggers[i]).Clone(), index + i );
			}
		}

		public bool IsSynchronized {
			get { return false; }
		}
		public object SyncRoot {
			get { return null; }
		}

		public System.Collections.IEnumerator GetEnumerator() {
			return new Enumerator(this);
		}
		public void Dispose() {
			foreach (object o in oTriggers) {
				((Trigger)o).Unbind();
			}
			oTriggers = null;
			iTask = null;
		}
	}

}
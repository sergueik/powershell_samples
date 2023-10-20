using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NanoTube.Collections {

	public sealed class SimpleObjectPool<T>
		where T : class {
		private readonly Stack<T> pool;
		
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Necessary to nest generics to support passing a factory method")]
		public SimpleObjectPool(int capacity, Func<SimpleObjectPool<T>, T> constructor) {
			if (null == constructor) {
				throw new ArgumentNullException("constructor");
			}

			this.pool = new Stack<T>(capacity);
			for (int i = 0; i < capacity; ++i) {
				var instance = constructor(this);
				if (null == instance) {
					throw new ArgumentException("constructor produced null object", "constructor");
				}

				this.pool.Push(instance);
			}
		}
		
		public T Pop() {
			//TODO: consider using a millisecond timeout here
			lock (this.pool) {
				if (this.pool.Count > 0) {
					return this.pool.Pop();
				} else {
					return null;
				}
			}
		}

		public void Push(T item) {
			if (item == null) {
				throw new ArgumentNullException("item", "Items added to a SimpleObjectPool cannot be null"); 
			}
			lock (this.pool) {
				this.pool.Push(item);
			}
		}
	}
}
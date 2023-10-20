using System;
using System.Net.Sockets;
using NanoTube.Collections;

namespace NanoTube.Net {
	public sealed class PoolAwareSocketAsyncEventArgs : SocketAsyncEventArgs {
		private SimpleObjectPool<SocketAsyncEventArgs> _parentPool;

		public PoolAwareSocketAsyncEventArgs(SimpleObjectPool<SocketAsyncEventArgs> parentPool) {
			if (null == parentPool) {
				throw new ArgumentNullException("parentPool");
			}

			_parentPool = parentPool;
		}

		protected override void OnCompleted(SocketAsyncEventArgs e) {
			base.OnCompleted(e);
			_parentPool.Push(this);
		}
	}
}
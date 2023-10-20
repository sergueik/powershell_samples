﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace ServiceChassis {

	public class CancellableTask {
		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
		private readonly CancellationTokenSource _cancellationTokenSource;

		public CancellableTask(Task task, CancellationTokenSource cancellationTokenSource) {
			Task = task;
			_cancellationTokenSource = cancellationTokenSource;
		}

		public Task Task { get; private set; }

		public void Cancel() {
			_cancellationTokenSource.Cancel();
		}

		public void CancelAndWait() {
			_cancellationTokenSource.Cancel();
			Task.Wait();
		}
		
		public void Start() {
			DateTime taskStart = DateTime.Now;

			try {
				this.Task.Start();
			} catch (Exception ex) {
				_log.ErrorException("Unspecified Error While executing task", ex);

				//ReportProgress(100, new TaskProgress(TaskStatus.Error,
				//	String.Format("Unexpected exception synchronizing data - {0}{1}", Environment.NewLine, ex)));
			} finally {
				_log.Info(() => string.Format("Task Elapsed Time {0}", (DateTime.Now - taskStart)));
				//unblock waiting threads -- regardless if we were successful or failed
			}
		}
	}
}
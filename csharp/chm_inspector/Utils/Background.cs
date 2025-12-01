using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using Serilog;
using Serilog.Core;
// using Serilog.Sinks.Console;
using Serilog.Debugging;
using Serilog.Sinks.Elasticsearch;
using Serilog.Formatting.Json;
using Serilog.Sinks.File;

using Elasticsearch;


/**
 * Copyright 2025 Serguei Kouzmine
 */


namespace Utils {
	
	public class BackgroundRunner {
		public static bool openBackgroundThreadWithTimeout( string argument, int timeout, Func<string, List<TocEntry>> method,	out List<TocEntry> result) {
			result = null;
			List<TocEntry> data = null;
			Exception threadException = null;

			var thread = new Thread(() => {
				try {
					data = method.Invoke(argument);
				} catch (Exception e) {
					threadException = e;
					Log.Error(String.Format("Exception: {0}", e.Message));
 					// TODO: log later on GUI thread
				}
			});

			// NOTE: COM needs to run on STA Thread
			thread.SetApartmentState(ApartmentState.STA);
			thread.IsBackground = true;
			thread.Start();

			if (!thread.Join(timeout)) {
				// timed out — thread keeps running, 
				// but caller is impatient and counting delayed run as failure
				Log.Warning(String.Format("Timeout: {0}", timeout));
				return false;
			}

			// If an exception happened inside the thread
			if (threadException != null) {
				return false;
			}

			result = data;
			// Caller to decide whether blank result is failure or OK.
			return result != null;
		}
	}
}
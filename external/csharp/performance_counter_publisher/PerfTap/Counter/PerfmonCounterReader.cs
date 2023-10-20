﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PerfTap.Interop;


namespace PerfTap.Counter {
	public class PerfmonCounterReader {
		private readonly IEnumerable<string> _computerNames = new string[0];
		private readonly bool _ignoreBadStatusCodes = true;
		private const int INFINITIY = -1;

		public PerfmonCounterReader(IEnumerable<string> computerNames) {
			if (null == computerNames) {
				throw new ArgumentNullException("computerNames");
			}

			this._computerNames = computerNames;
		}
		
		public PerfmonCounterReader() {
			this._computerNames = new string[0];
		}

		public PerfmonCounterReader(bool ignoreBadStatusCodes) {
			this._computerNames = new string[0];
			this._ignoreBadStatusCodes = ignoreBadStatusCodes;
		}

		public IEnumerable<PerformanceCounterSampleSet> GetCounterSamples(TimeSpan sampleInterval, int count, CancellationToken token) {
			if (count <= 0) {
				throw new ArgumentOutOfRangeException("count", "must be greater than zero");
			}
			if (null == token) {
				throw new ArgumentNullException("token");
			}
 
			return ProcessGetCounter(GetDefaultCounters(), sampleInterval, count, token);	
		}
		public IEnumerable<PerformanceCounterSampleSet> StreamCounterSamples(TimeSpan sampleInterval, CancellationToken token) {
			if (null == token) {
				throw new ArgumentNullException("token");
			}

			return ProcessGetCounter(GetDefaultCounters(), sampleInterval, INFINITIY, token);
		}

		public IEnumerable<PerformanceCounterSampleSet> GetCounterSamples(IEnumerable<string> counters, TimeSpan sampleInterval, int count, CancellationToken token) {
			if (null == counters) {
				throw new ArgumentNullException("counters");
			}
			if (count <= 0) {
				throw new ArgumentOutOfRangeException("count", "must be greater than zero");
			}
			if (null == token) {
				throw new ArgumentNullException("token");
			}

			return ProcessGetCounter(counters, sampleInterval, count, token);
		}

		public IEnumerable<PerformanceCounterSampleSet> StreamCounterSamples(IEnumerable<string> counters, TimeSpan sampleInterval, CancellationToken token) {
			if (null == counters) {
				throw new ArgumentNullException("counters");
			}
			if (null == token) {
				throw new ArgumentNullException("token");
			}

			return ProcessGetCounter(counters, sampleInterval, INFINITIY, token);
		}

		private IEnumerable<PerformanceCounterSampleSet> ProcessGetCounter(IEnumerable<string> counters, TimeSpan sampleInterval, int maxSamples, CancellationToken token) {
			using (PdhHelper helper = new PdhHelper(this._computerNames, counters, this._ignoreBadStatusCodes)) {
				int samplesRead = 0;

				do {
					PerformanceCounterSampleSet set = helper.ReadNextSet();
					if (null != set) {
						yield return set;
					}
					samplesRead++;
				} while (((maxSamples == INFINITIY) || (samplesRead < maxSamples)) && !token.WaitHandle.WaitOne(sampleInterval, true));
			}
		}

		public static List<string> DefaultCounters {
			get { 
				return new List<string>() { @"\network interface(*)\bytes total/sec", 
					@"\processor(_total)\% processor time", 
					@"\memory\% committed bytes in use", 
					@"\memory\cache faults/sec", 
					@"\physicaldisk(_total)\% disk time", 
					@"\physicaldisk(_total)\current disk queue length"
				};
			}
		}

		private IEnumerable<string> GetDefaultCounters() {
			return DefaultCounters.Select(path => {
				return PdhHelper.TranslateLocalCounterPath(path);
			});
		}
	}
}
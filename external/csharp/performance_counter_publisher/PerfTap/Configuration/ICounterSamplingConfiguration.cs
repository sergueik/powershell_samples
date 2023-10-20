using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace PerfTap.Configuration{

	public interface ICounterSamplingConfiguration {
		ReadOnlyCollection<ICounterDefinitionsFilePath> DefinitionFilePaths { get; }
		ReadOnlyCollection<ICounterName> CounterNames { get; }
		TimeSpan SampleInterval { get; }
	}
}
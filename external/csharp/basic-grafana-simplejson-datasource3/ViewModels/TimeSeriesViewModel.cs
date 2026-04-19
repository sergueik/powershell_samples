using GrafanaGenericSimpleJsonDataSource.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrafanaGenericSimpleJsonDataSource.ViewModels
{
	public struct TimeSeriesViewModel<T>:IResponseViewModel
	{
		public TimeSeriesViewModel(string target, DataPoint<T>[] dataPoints)
		{
			Target = target;
			DataPoints = dataPoints;
		}

		public string Target { get; }

		[JsonProperty("datapoints")]
		public DataPoint<T>[] DataPoints { get; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrafanaGenericSimpleJsonDataSource.ViewModels
{
    public class QueryRangeViewModel
    {
		public QueryRangeViewModel(DateTime from, DateTime to)
		{
			From = from;
			To = to;
		}

		public DateTime From { get; }
		public DateTime To { get; }
	}
}

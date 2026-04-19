using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrafanaGenericSimpleJsonDataSource.ViewModels
{
    public class QueryViewModel
    {
        public QueryViewModel(string panelId, QueryRangeViewModel range, int intervalMs, TargetViewModel[] targets, FilterViewModel filters,
            int maxDataPoints, OutputFormat format)
        {
            PanelId = panelId;
            RangeViewModel = range;
            IntervalMs = intervalMs;
            Targets = targets;
            MaxDataPoints = maxDataPoints;
            Format = format;
            Filters = filters;

        }

        public string PanelId { get; private set; }

        public QueryRangeViewModel RangeViewModel { get; private set; }

        public int IntervalMs { get; private set; }
        public TargetViewModel[] Targets { get; }
        public FilterViewModel Filters { get; }
        public int MaxDataPoints { get; }
        public OutputFormat Format { get; }
    }
}

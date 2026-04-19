using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrafanaGenericSimpleJsonDataSource.ViewModels
{
    public class TargetViewModel
    {
        public TargetViewModel(string target, string refId, PlotFormat type)
        {
            Target = target;
            RefId = refId;
            Format = type;
        }

        public string Target { get; }

        public string RefId { get; private set; }

        public PlotFormat Format { get; private set; }
    }

}

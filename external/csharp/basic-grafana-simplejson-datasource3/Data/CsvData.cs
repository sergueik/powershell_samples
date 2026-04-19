using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrafanaGenericSimpleJsonDataSource.Data
{
    public struct CsvData
    {
        public CsvData(string name, DateTime date, int filled, int notional)
        {
            Name = name;
            Date = date;
            TotalFilled = filled;
            TotalNotional = notional;
        }
        public string Name { get; private set; }

        public int TotalNotional { get; private set; } 

        public int TotalFilled { get; private set; }

        public DateTime Date { get; private set; }

    }
}

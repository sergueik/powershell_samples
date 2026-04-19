using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrafanaGenericSimpleJsonDataSource.Data
{
    public class DataPoint<T>
    {
        public DataPoint(DateTime dateTime, T value)
        {
            EpochTime = new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
            Value = value;
        }

        public long EpochTime { get; }
        public T Value { get; }
    }
}

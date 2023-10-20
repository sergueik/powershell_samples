using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogDynamicsLab.InfluxDataAccess
{
    /// <summary>
    /// Class structure for deserialization of JSON results from InfluxDB
    /// </summary>
    public class InfluxReturnObject
    {
        /// <summary>
        /// Name field for timeseries name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Column field as a list of column headings for following points
        /// </summary>
        public List<string> Columns { get; set; }

        /// <summary>
        /// Points field which includes a list of datarows whereas each data row has n datapoints (according to the number of columns)
        /// </summary>
        public List<List<object>> Points { get; set; }
    }
}

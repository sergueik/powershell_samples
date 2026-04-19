using System.Threading.Tasks;

namespace GrafanaSimpleDataSource.Data
{
    public interface IDataSource
    {
        string Metric { get; }

        Task<IDataResponse> GetDataAsync(TimeSeriesRequest request);
    }
}

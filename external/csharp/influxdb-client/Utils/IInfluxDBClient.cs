using System.Collections.Generic;

namespace Utils
{
    public interface IInfluxDBClient
    {
        object Request(string url, string method, object data = null);

        void CreateDatabase(string name);
        
        void DeleteDatabase(string name);
        
        void DeleteSerie(string name);
        
        List<string> GetDatabaseList();
        
        List<InfluxServerInfo> GetServers();
        
        void Insert(List<Serie> series);
        
        List<Serie> Query(string query, TimePrecision? precision = null);
        
        bool Ping();
    }
}
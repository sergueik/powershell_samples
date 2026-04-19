using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrafanaGenericSimpleJsonDataSource.Repositories
{
    public interface IRepository
    {
        string RepositoryDisplayName { get; }

        List<string> GetTags();

        List<string> GetTagValues(string tag);

        Dictionary<string,string> GetColumns();
    }
    public interface IRepository<T> : IRepository
    {

        void Add(T obj);

        void Remove(T obj);

        List<T> Get(string objKey);

        List<T> Get(Predicate<bool> predicate);

        List<T> GetAll();
    }
}

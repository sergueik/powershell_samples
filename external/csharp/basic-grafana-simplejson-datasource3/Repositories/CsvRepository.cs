using GrafanaGenericSimpleJsonDataSource.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GrafanaGenericSimpleJsonDataSource.Repositories
{
    public class CsvRepository : IRepository<CsvData>
    {
        List<CsvData> _repoData = new List<CsvData>();
        Dictionary<string, List<string>> _tags = new Dictionary<string, List<string>>();

        public CsvRepository(string repoName)
        {
            RepositoryDisplayName = repoName;

            Add(new CsvData("TestA", DateTime.Now.AddDays(-4), 3500, 4500));
            Add(new CsvData("TestB", DateTime.Now.AddDays(-2), 3200, 4800));
            Add(new CsvData("TestC", DateTime.Now.AddDays(-1), 3700, 4800));
            Add(new CsvData("TestA", DateTime.Now.AddDays(-3), 3560, 4400));
            Add(new CsvData("TestA", DateTime.Now.AddDays(-2), 3760, 4200));
            Add(new CsvData("TestA", DateTime.Now.AddDays(-1), 3600, 4900));
        }

        public string RepositoryDisplayName { get; }

        public void Add(CsvData obj)
        {
            _repoData.Add(obj);
            if (!_tags.ContainsKey(nameof(CsvData.Name)))
            {
                _tags.Add(nameof(CsvData.Name), new List<string>());
            }
            if (!_tags[nameof(CsvData.Name)].Any(x_ => x_ == obj.Name))
            {
                _tags[nameof(CsvData.Name)].Add(obj.Name);
            }
        }

        public List<CsvData> Get(string objKey)
        {
            throw new NotImplementedException();
        }

        public List<CsvData> Get(Predicate<bool> predicate)
        {
            throw new NotImplementedException();
        }

        public List<CsvData> GetAll()
        {
            return _repoData;
        }

        public Dictionary<string, string> GetColumns()
        {
            return new Dictionary<string, string>() { { "Name", "string" }, { "Date", "time" }, 
                { "Filled", "number" }, { "Notional", "number" } };
        }

        public List<string> GetTags()
        {
            return _tags.Keys.ToList();
        }

        public List<string> GetTagValues(string tag)
        {
            return _tags[tag];
        }

        public void Remove(CsvData obj)
        {
            _repoData.Remove(obj);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrafanaGenericSimpleJsonDataSource.Repositories
{
    public class RepositoryFactory
    {
        private Dictionary<string, IRepository> allRepos = new Dictionary<string, IRepository>();
        public IRepository<T> CreateRepository<T>(string repoName, string repoType)
        {
            if (repoType == nameof(CsvRepository))
            {
                IRepository<T> repo = (IRepository<T>)new CsvRepository(repoName);
                allRepos.Add(repoName, repo);
                return repo;
            }
            return null;
        }

        public IRepository GetRepository(string repoName)
        {
            return allRepos[repoName];
        }

        public List<string> GetAllRepositories()
        {
            return allRepos.Keys.ToList();
        }
    }
}

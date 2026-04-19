using GrafanaGenericSimpleJsonDataSource.Data;
using GrafanaGenericSimpleJsonDataSource.Repositories;
using GrafanaGenericSimpleJsonDataSource.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrafanaGenericSimpleJsonDataSource.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JsonSourceController : Controller
    {
        private readonly RepositoryFactory _factory;

        public JsonSourceController(RepositoryFactory factory)
        {
            _factory = factory;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }

        [HttpPost("search")]
        public IActionResult Search()
        {
            return Ok(_factory.GetAllRepositories());
        }

        [HttpPost("query")]
        public IActionResult Query([FromBody] QueryViewModel query)
        {

            return Ok(query.Targets.Select(
                x_ =>
                {
                    var repository = _factory.GetRepository(_factory.GetAllRepositories().First(repo => repo == x_.Target));
                    return (Target: x_, Repository: repository);
                }).
                Select(
                    kvp =>
                    {
                        if (kvp.Target.Format == PlotFormat.table)
                        {
                            return new TableViewModel(kvp.Repository) as IResponseViewModel;
                        }
                        List<DataPoint<CsvData>> dataPoints = new List<DataPoint<CsvData>>();
                        dataPoints.AddRange((kvp.Repository as CsvRepository)?.GetAll().Select(x_ => new DataPoint<CsvData>(x_.Date, x_)));

                        return new TimeSeriesViewModel<CsvData>(kvp.Target.Target,dataPoints.ToArray());
                    }));

        }

        [HttpPost("annotations")]
        public IActionResult GetAnnotations()
        {
            return Ok();
        }

        [HttpPost("tag-keys")]
        public IActionResult GetTagKeys(object inp)
        {
            return Ok();
        }
        [HttpPost("tag-values")]
        public IActionResult GetTagValues(object inp)
        {
            return Ok();
        }

    }
}

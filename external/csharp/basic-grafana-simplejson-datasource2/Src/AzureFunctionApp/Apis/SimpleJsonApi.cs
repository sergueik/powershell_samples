using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using GrafanaSimpleDataSource.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureFunctions.Extensions.Swashbuckle.Attribute;

namespace GrafanaSimpleJsonDataSourceExample.Apis
{
    public class SimpleJsonApi
    {
        private readonly Dictionary<string, IDataSource> _dataSources;

        public SimpleJsonApi(Dictionary<string, IDataSource> dataSources)
        {
            _dataSources = dataSources;
        }


        [FunctionName("test")]
        public IActionResult Test(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest request
            )
        {
            return new OkResult();
        }




        [FunctionName("search")]
        public IActionResult Search(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "api/search")] HttpRequest request)
        {
            var metrics = _dataSources.Keys;
            return new JsonResult(metrics);
        }


        /// <summary>
        /// Here is a summary
        /// </summary>
        /// <param name="request">This is the request</param>
        /// <returns>I return something</returns>
        /// <remarks>
        /// These are some really cool remarks
        /// </remarks>
        /// <example>
        /// 
        /// Example time series request:
        ///     {"app":"dashboard","requestId":"Q129","timezone":"browser","panelId":4,          "dashboardId":3,"range":{"from":"2021-02-12T04:28:26.679Z","to":"2021-02-12T10:28:26.679Z","raw":{"from":"now-6h","to":"now"}},"timeInfo":"","interval":"20s","intervalMs":20000,"targets":[{"target":"ExampleTimeSeriesDataSource","refId":"A","type":"timeserie"}],"maxDataPoints":1212,"scopedVars":{"__interval":{"text":"20s","value":"20s"},"__interval_ms":{"text":"20000","value":20000}},"startTime":1613125706681,"rangeRaw":{"from":"now-6h","to":"now"},"adhocFilters":[]}
        ///     
        /// Example table request:
        ///     {"app":"dashboard","requestId":"Q104","timezone":"browser","panelId":23763571993,"dashboardId":3,"range":{"from":"2021-02-12T03:00:19.914Z","to":"2021-02-12T09:00:19.914Z","raw":{"from":"now-6h","to":"now"}},"timeInfo":"","interval":"10s","intervalMs":10000,"targets":[{"target":"ExampleTableDataSource",     "refId":"A","type":"table"}],    "maxDataPoints":2110,"scopedVars":{"__interval":{"text":"10s","value":"10s"},"__interval_ms":{"text":"10000","value":10000}},"startTime":1613120419915,"rangeRaw":{"from":"now-6h","to":"now"},"adhocFilters":[]}
        ///     
        /// </example>
        [Consumes("application/json")]        
        [FunctionName("query")]
        public async Task<IActionResult> Query(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "api/query")]
            [RequestBodyType(typeof(TimeSeriesRequest), "Grafana request")]TimeSeriesRequest request
            //HttpRequest request
            )
        {
            //var jsonContent = await request.ReadAsStringAsync();
            
            var responses = new List<IDataResponse>();

            foreach(var target in request.Targets)
            {
                if(_dataSources.ContainsKey(target.Metric))
                {
                    var response = await _dataSources[target.Metric].GetDataAsync(request);
                    responses.Add(response);
                }
                else
                {

                }
            }

            return new OkObjectResult(responses);
        }

    }
}

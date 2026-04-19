using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace simplesource.Controllers
{
    [Produces("application/json")]
    [Route("query")]
    public class QueryController : Controller
    {
        // POST /query
        [HttpPost]
        public string Post([FromBody]string value)
        {
            using (StreamWriter sw = new StreamWriter("logQ.txt", true))
            {
                sw.WriteLine($"{DateTime.UtcNow:HH:mm:ss}: query");
            }

            return "{ \"aa\": \"11\", \"bb\": \"qq\" }";
        }
    }
}
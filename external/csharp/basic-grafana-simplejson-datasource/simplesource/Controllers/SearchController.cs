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
    [Route("search")]
    public class SearchController : Controller
    {
        // POST /search
        [HttpPost]
        public string Post([FromBody] string value)
        {
            using var sw = new StreamWriter("logS.txt", true);
            sw.WriteLine($"{DateTime.UtcNow:HH:mm:ss}: search");

            return "{ \"aa\": 11, \"bb\": \"ss\" }";
        }
    }
}

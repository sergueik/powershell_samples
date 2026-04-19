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
    [Route("")]
    public class RootController : Controller
    {
        // POST /
        [HttpPost]
        public string Post([FromBody] string value)
        {
            using var sw = new StreamWriter("logR.txt", true);
            sw.WriteLine($"{DateTime.UtcNow:HH:mm:ss}: root");

            return "{ \"aa\": 11, \"bb\": \"rr\" }";
        }
    }
}

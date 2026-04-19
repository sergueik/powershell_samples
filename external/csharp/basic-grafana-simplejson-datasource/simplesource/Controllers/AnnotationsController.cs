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
    [Route("annotations")]
    public class AnnotationsController : Controller
    {
        // POST /annotations
        [HttpPost]
        public string Post([FromBody]string value)
        {
            using (StreamWriter sw = new StreamWriter("logA.txt", true))
            {
                sw.WriteLine($"{DateTime.UtcNow:HH:mm:ss}: annotations");
            }

            return "{ \"aa\": \"11\", \"bb\": \"aa\" }";
        }
    }
}
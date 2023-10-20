using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace Dockedit.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // Default Docker Engine on Windows
        DockerClient client = new DockerClientConfiguration(
            new Uri("npipe://./pipe/docker_engine"))
             .CreateClient();
        

        [HttpGet("GetAllImages")]
        public async Task<List<string>> GetAllImages()
        {           
            IList<ImagesListResponse> containers = await client.Images.ListImagesAsync(
                new ImagesListParameters()
                {
                    All=true,
                });
            return containers.Select(i => i.ID.ToString()).ToList();
        }
    }
}

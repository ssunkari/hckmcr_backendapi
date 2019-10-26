using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Zuto.Uk.Sample.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        // GET api/health
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            WebClient client = new WebClient();
            string data = await client.DownloadStringTaskAsync(new Uri("https://jsonplaceholder.typicode.com/todos/1"));
            Console.WriteLine(data);
            try
            {
                throw new Exception();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
            return Ok("Healthy");
        }
    }
}

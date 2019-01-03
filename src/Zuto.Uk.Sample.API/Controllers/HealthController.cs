using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Zuto.Uk.Sample.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        // GET api/health
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return Ok("Healthy");
        }
    }
}

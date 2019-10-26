using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Zuto.Uk.Sample.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IJobsRepo _jobsRepo;

        public UserController(IJobsRepo jobsRepo)
        {
            _jobsRepo = jobsRepo;
        }

        // GET api/health
        [HttpGet]
        [Route("seekhelp")]
        public async Task<ActionResult<IEnumerable<string>>> SeekHelp()
        {
            var jobs = await _jobsRepo.GetAll();
            return Ok(jobs);
        }
    }
}

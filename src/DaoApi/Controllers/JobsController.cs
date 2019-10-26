using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Zuto.Uk.Sample.API.Models.Api;
using Zuto.Uk.Sample.API.Repositories;

namespace Zuto.Uk.Sample.API.Controllers
{
    [Route("api/jobs")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IJobsRepo _jobsRepo;

        public JobsController(IJobsRepo jobsRepo)
        {
            _jobsRepo = jobsRepo;
        }

        // GET api/health
        [HttpGet]
        [Route("seekhelp")]
        public async Task<ActionResult<IEnumerable<string>>> GetJobs()
        {
            var jobs = await _jobsRepo.GetAll();
            return Ok(jobs);
        }


        [HttpPost]
        [Route("seekhelp")]
        public async Task<ActionResult<IEnumerable<string>>> PostJob([FromBody] JobApiRequestModel model)
        {
            var jobs = await _jobsRepo.GetAll();
            return Ok(jobs);
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Zuto.Uk.Sample.API.Models;
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
        public async Task<ActionResult<IEnumerable<string>>> GetAllJobs()
        {
            var jobs = await _jobsRepo.GetAll();
            return Ok(jobs);
        }

        [HttpGet]
        [Route("{phoneNumber}")]
        public async Task<ActionResult<JobsModel>> GetJobsByPhoneNumber(string phoneNumber)
        {
            var jobs = await _jobsRepo.GetJobsByPhoneNumber(phoneNumber);
            if (jobs == null)
                return NotFound();
            return Ok(jobs);
        }


        [HttpPost]
        [Route("seekhelp")]
        public async Task<ActionResult<IEnumerable<string>>> CreateJob([FromBody] JobApiRequestModel model)
        {
            await _jobsRepo.CreateJob(model.Model());
            return Ok();
        }
    }
}

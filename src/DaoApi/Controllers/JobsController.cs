using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IJobScheduler _jobScheduler;

        public JobsController(IJobsRepo jobsRepo, IJobScheduler jobScheduler)
        {
            _jobsRepo = jobsRepo;
            _jobScheduler = jobScheduler;
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
            var jobModel = model.Model();
            await _jobsRepo.CreateJob(jobModel);
            await _jobScheduler.ScheduleJob(jobModel);
            return Ok();
        }

        [HttpPost]
        [Route("clear")]
        public async Task<ActionResult<IEnumerable<string>>> DeleteJobs()
        {
            await _jobsRepo.DeleteAllJobs();
            return Ok();
        }
    }
}

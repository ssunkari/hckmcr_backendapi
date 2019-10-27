﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using DaoApi.Controllers;
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
        private readonly ISendMessageToQueue _sendMessageToQueue;
        private readonly IBuddiesRepo _buddiesRepo;

        public JobsController(IJobsRepo jobsRepo, IJobScheduler jobScheduler, ISendMessageToQueue sendMessageToQueue, IBuddiesRepo buddiesRepo)
        {
            _jobsRepo = jobsRepo;
            _jobScheduler = jobScheduler;
            _sendMessageToQueue = sendMessageToQueue;
            _buddiesRepo = buddiesRepo;
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

        [HttpPost]
        [Route("reply")]
        public async Task<ActionResult<IEnumerable<string>>> Reply([FromBody] ReplyRequestApimodel model)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.Number) || string.IsNullOrWhiteSpace(model.Message) || model.Message.Length < 11)
                return BadRequest();

            if (model.Message.ToUpper().Contains("CONFIRM"))
            {
                var job_identifier = model.Message.Substring(0, 4);
                var requestorJobByIdMatch = (await _jobsRepo.GetJobsByIdMatch(job_identifier));
                var buddy_identifier = model.Message.Substring(5, 4);
                var buddy = (await _buddiesRepo.GetBuddyByIdMatch(buddy_identifier));
                if (requestorJobByIdMatch == null)
                    return BadRequest();
                if (requestorJobByIdMatch.JobConfirmedTo == null)
                {
                    await _jobsRepo.Update(requestorJobByIdMatch, new Dictionary<string, AttributeValueUpdate>
                    {
                        {
                            "JobConfirmed",
                            new AttributeValueUpdate
                                {Value = new AttributeValue {BOOL = true}}
                        },
                        {
                            "JobConfirmedTo",
                            new AttributeValueUpdate
                                {Value = new AttributeValue {S = buddy.Id}}
                        }
                    });
                    //Send Confirmation Text to Confirmed Helper
                    var msgTemplate = $"Hi {buddy.FirstName}, {requestorJobByIdMatch.Name} confirmed your acceptance and is looking forward to meet you at location {requestorJobByIdMatch.Location}";
                    await _sendMessageToQueue.Dispatch(buddy.MobileNumber, msgTemplate);
                    //Send Cancellation Text to Accepted Buddies, remove confirmed one from the list
                    var acceptedBuddiesExceptConfirmedBuddy = requestorJobByIdMatch.BuddiesAccepted?.Where(x => x != buddy.Id);
                    if (acceptedBuddiesExceptConfirmedBuddy != null)
                    {
                        foreach (var item in acceptedBuddiesExceptConfirmedBuddy)
                        {
                            var getBuddy = await _buddiesRepo.GetBuddyByIdMatch(item);
                            var msg =
                                $"Hi {getBuddy.FirstName}, {requestorJobByIdMatch.Name} is being currently assisted by someone and would like to thank you for accepting the request.";
                            await _sendMessageToQueue.Dispatch(getBuddy.MobileNumber, msg);
                        }
                        await _jobsRepo.Update(requestorJobByIdMatch, new Dictionary<string, AttributeValueUpdate>
                        {
                            {"BuddiesAccepted", new AttributeValueUpdate{Value = new AttributeValue { SS = requestorJobByIdMatch.BuddiesAccepted },Action = AttributeAction.DELETE}}
                        });
                    }
                    else
                    {
                        var getBuddy = await _buddiesRepo.GetBuddyByIdMatch(requestorJobByIdMatch.JobConfirmedTo);
                        var msg =
                            $"Hi {requestorJobByIdMatch.Name}, look like you have already confirmed to seek the assistance from {getBuddy.FirstName}, and is on their way.";
                        await _sendMessageToQueue.Dispatch(getBuddy.MobileNumber, msg);
                    }
                }

            }
            else
            {
                var requestor_identifier = model.Message.Substring(0, 4);
                var requestor_response =
                    model.Message.Substring(5).Trim()?.ToUpper() == "ACCEPT" ? "Accepted" : "Declined";
                var requestorJobByIdMatch = (await _jobsRepo.GetJobsByIdMatch(requestor_identifier));
                var allBuddies = (await _buddiesRepo.GetAllBuddies());
                var buddy = allBuddies.FirstOrDefault(x => x.MobileNumber == model.Number);
                if (requestorJobByIdMatch == null || buddy == null)
                    return BadRequest();
                if (requestor_response == "Accepted" && HasNotAcceptedAlready(requestorJobByIdMatch, buddy))
                {
                    var msgTemplate =
                        $"Your request for help is {requestor_response} by {buddy.FirstName} in your neighbourhood. {buddy.FirstName} has rating {buddy.Rating}, checkout {buddy.FirstName} profile {buddy.Profile}. Reply {requestorJobByIdMatch.Id.Substring(0, 4)}-{buddy.Id.Substring(0, 4)}-Confirm to confirm";

                    await _sendMessageToQueue.Dispatch(requestorJobByIdMatch.MobileNumber, msgTemplate);

                    var getCurrentBuddyAcceptedList = requestorJobByIdMatch.BuddiesAccepted ?? new List<string>();
                    if (!getCurrentBuddyAcceptedList.Contains(buddy.Id))
                    {
                        getCurrentBuddyAcceptedList.Add(buddy.Id);
                    }
                    await _jobsRepo.Update(requestorJobByIdMatch, new Dictionary<string, AttributeValueUpdate>
                    {
                        {"BuddiesAccepted", new AttributeValueUpdate{Value = new AttributeValue { SS = getCurrentBuddyAcceptedList },Action = AttributeAction.ADD}}
                    });
                }


            }

            return Ok();
        }

        private static bool HasNotAcceptedAlready(JobsModel requestorJobByIdMatch, BuddyModel buddy)
        {
            return requestorJobByIdMatch.BuddiesAccepted == null ||
                   !requestorJobByIdMatch.BuddiesAccepted.Contains(buddy.Id);
        }
    }

    public class ReplyRequestApimodel
    {
        [Required]
        public string Number { get; set; }
        [Required]
        public string Message { get; set; }
    }
}

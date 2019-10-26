using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using Zuto.Uk.Sample.API.Models;

namespace Zuto.Uk.Sample.API.Controllers
{
    public class JobScheduler : IJobScheduler
    {
        private readonly IBuddyFinder _buddyFinder;
        private readonly IAmazonSQS _sqsClient;

        public JobScheduler(IBuddyFinder buddyFinder, IAmazonSQS sqsClient)
        {
            _buddyFinder = buddyFinder;
            _sqsClient = sqsClient;
        }

        public async Task ScheduleJob(JobsModel model)
        {
            List<(string phoneNumber,string message)> eventsToDispatch = new List<(string, string)>();
            var buddies = await _buddyFinder.GetBuddiesByGeoLocationAsync(model.Lat, model.Long);
            foreach (var buddy in buddies)
            {
                eventsToDispatch.Add((buddy.MobileNumber,TemplateMessage(model,buddy)));
            }

            var queueUrlResponse = await _sqsClient.GetQueueUrlAsync("hack-manchester-2019-request-help-queue");
            foreach (var @event in eventsToDispatch)
            {
                await _sqsClient.SendMessageAsync(new SendMessageRequest(queueUrlResponse.QueueUrl, JsonConvert.SerializeObject(new
                {
                    number=@event.phoneNumber,
                    message = @event.message
                })));
            }
        }

        private static string TemplateMessage(JobsModel model, BuddyModel buddy)
        {
            string messageTmp =
                $"Dear {buddy.FirstName}, {model.Name} in your neighbourhood having disabilities {model.Disabilities.Join(",")} is seeking assistance. {model.TranslatedMessage}. Please reply {model.MobileNumber.Substring(model.MobileNumber.Length-4)} Yes/No to provide help.";
            return messageTmp;
        }
    }
}
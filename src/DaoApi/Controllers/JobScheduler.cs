using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Zuto.Uk.Sample.API.Controllers;
using Zuto.Uk.Sample.API.Models;

namespace DaoApi.Controllers
{
    public class JobScheduler : IJobScheduler
    {
        private readonly IBuddyFinder _buddyFinder;
        private readonly ISendMessageToQueue _sendMessageToQueue;

        public JobScheduler(IBuddyFinder buddyFinder, ISendMessageToQueue sendMessageToQueue)
        {
            _buddyFinder = buddyFinder;
            _sendMessageToQueue = sendMessageToQueue;
        }

        public async Task ScheduleJob(JobsModel model)
        {
            List<(string phoneNumber, string message)> eventsToDispatch = new List<(string, string)>();
            var buddies = await _buddyFinder.GetBuddiesByGeoLocationAsync(model.Lat, model.Long);
            foreach (var buddy in buddies)
            {
                eventsToDispatch.Add((buddy.MobileNumber, TemplateMessage(model, buddy)));
            }

            foreach (var @event in eventsToDispatch)
            {
                await _sendMessageToQueue.Dispatch(@event.phoneNumber, @event.message);
               
            }
        }

        private static string TemplateMessage(JobsModel model, BuddyModel buddy)
        {
            string messageTmp =
                $"Dear {buddy.FirstName}, {model.Name} in your neighbourhood having disabilities {model.Disabilities.Join(",")} is seeking assistance. {model.TranslatedMessage}. Please reply {model.Id.Substring(0,4)}-Accept/Decline to provide help.";
            return messageTmp;
        }
    }
}
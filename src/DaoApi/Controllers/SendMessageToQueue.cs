using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;

namespace DaoApi.Controllers
{
    public class SendMessageToQueue : ISendMessageToQueue
    {
        private readonly IAmazonSQS _sqsClient;

        public SendMessageToQueue(IAmazonSQS sqsClient)
        {
            _sqsClient = sqsClient;
        }

        public async Task Dispatch(string phoneNumber, string message)
        {
            var queueUrlResponse = await _sqsClient.GetQueueUrlAsync("hack-manchester-2019-request-help-queue");
            await _sqsClient.SendMessageAsync(new SendMessageRequest(queueUrlResponse.QueueUrl, JsonConvert.SerializeObject(new
            {
                number = phoneNumber,
                message = message
            })));
        }
    }
}
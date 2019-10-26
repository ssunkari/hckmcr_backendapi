using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Zuto.Uk.Sample.API.Models;

namespace Zuto.Uk.Sample.API.Repositories
{
    public class JobsRepo :IJobsRepo
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private  const string TableName = "hackmcr_jobs";

        public JobsRepo(IAmazonDynamoDB dbClient)
        {
            _dynamoDbClient = dbClient;
        }

        public async Task<List<JobsModel>> GetAll()
        {
            var request = new ScanRequest
            {
                TableName = "hackmcr_jobs"
            };

            var response = await _dynamoDbClient.ScanAsync(request);
            return response.Items.Select(Map).ToList();
        }

        public async Task<JobsModel> GetJobsByPhoneNumber(string phoneNumber)
        {
            var request = new GetItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue> { { "MobileNumber", new AttributeValue { S = phoneNumber } } },
            };

            var response = await _dynamoDbClient.GetItemAsync(request);

            return response.Item.Any() ? Map(response.Item) : null;

        }

        public async Task CreateJob(JobsModel model)
        {
            var putItemRequest = new PutItemRequest
            {
                TableName = TableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    ["MobileNumber"] = new AttributeValue { S = model.MobileNumber },
                    ["Name"] = new AttributeValue { S = model.Name },
                    ["Location"] = new AttributeValue { S = model.Location },
                    ["Lat"] = new AttributeValue { S = model.Lat },
                    ["Long"] = new AttributeValue { S = model.Long },
                    ["TimestampRequested"] = new AttributeValue { S = model.TimestampRequested },
                    ["TimestampRequiredFor"] = new AttributeValue { S = model.TimestampRequiredFor },
                    ["Message"] = new AttributeValue { S = model.Message },
                    ["TranslatedMessage"] = new AttributeValue { S = model.TranslatedMessage },
                    ["LanguageRequested"] = new AttributeValue { S = model.LanguageRequested },
                    ["Disabilities"] = new AttributeValue { SS = model.Disabilities }
                }
            };

            await _dynamoDbClient.PutItemAsync(putItemRequest);

        }

        private JobsModel Map(Dictionary<string, AttributeValue> result)
        {
            var job = new JobsModel
            {
                Name = result["Name"].S,
                MobileNumber = result["MobileNumber"].S,
                Location = result["Location"].S,
                Lat = result["Lat"].S,
                Long = result["Long"].S,
                TimestampRequested = result["TimestampRequested"].S,
                TimestampRequiredFor = result["TimestampRequiredFor"].S,
                Message = result["Message"].S,
                TranslatedMessage = result["TranslatedMessage"].S,
                LanguageRequested = result["LanguageRequested"].S,
                Disabilities = result["Disabilities"].SS
     
            };
            return job;
        }
    }
}
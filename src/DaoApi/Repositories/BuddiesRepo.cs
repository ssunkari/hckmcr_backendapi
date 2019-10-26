using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Zuto.Uk.Sample.API.Models;

namespace Zuto.Uk.Sample.API.Repositories
{
    public class BuddiesRepo : IBuddiesRepo
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;

        public BuddiesRepo(IAmazonDynamoDB dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
        }

        public async Task<List<BuddyModel>> GetAllBuddies()
        {
            var request = new ScanRequest
            {
                TableName = "hackmcr_buddies"
            };

            var response = await _dynamoDbClient.ScanAsync(request);
            return response.Items.Select(Map).ToList();
        }

        public async Task CreateUser(BuddyModel model)
        {
            var putItemRequest = new PutItemRequest
            {
                TableName = "hackmcr_buddies",
                Item = new Dictionary<string, AttributeValue>
                {
                    ["Id"] = new AttributeValue { S = Guid.NewGuid().ToString() },
                    ["FirstName"] = new AttributeValue { S = model.FirstName },
                    ["LastName"] = new AttributeValue { S = model.LastName },
                    ["Location"] = new AttributeValue { S = model.Location },
                    ["Lat"] = new AttributeValue { S = model.Lat },
                    ["Long"] = new AttributeValue { S = model.Long },
                    ["MobileNumber"] = new AttributeValue { S = model.MobileNumber }
                }
            };

            await _dynamoDbClient.PutItemAsync(putItemRequest);
        }

        private BuddyModel Map(Dictionary<string, AttributeValue> result)
        {
            var job = new BuddyModel
            {
                Id = result["Id"].S,
                FirstName = result["FirstName"].S,
                LastName = result["LastName"].S,
                Location = result["Location"].S,
                Lat = result["Lat"].S,
                Long = result["Long"].S,
                MobileNumber = result["MobileNumber"].S
            };
            return job;
        }
    }
}
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
        private string _tableName = "hackmcr_buddies";

        public BuddiesRepo(IAmazonDynamoDB dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
        }

        public async Task<List<BuddyModel>> GetAllBuddies()
        {
            var request = new ScanRequest
            {
                TableName = _tableName
            };

            var response = await _dynamoDbClient.ScanAsync(request);
            return response.Items.Select(Map).ToList();
        }

        public async Task CreateUser(BuddyModel model)
        {
            var putItemRequest = new PutItemRequest
            {
                TableName = _tableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    ["Id"] = new AttributeValue { S = Guid.NewGuid().ToString() },
                    ["FirstName"] = new AttributeValue { S = model.FirstName },
                    ["LastName"] = new AttributeValue { S = model.LastName },
                    ["Location"] = new AttributeValue { S = model.Location },
                    ["Lat"] = new AttributeValue { S = model.Lat },
                    ["Long"] = new AttributeValue { S = model.Long },
                    ["MobileNumber"] = new AttributeValue { S = model.MobileNumber },
                    ["Rating"] = new AttributeValue { S = model.Rating },
                    ["Profile"] = new AttributeValue { S = model.Profile }
                }
            };

            await _dynamoDbClient.PutItemAsync(putItemRequest);
        }

        public async Task DeleteAllBuddies()
        {
            var request = new ScanRequest
            {
                TableName = _tableName
            };

            var response = await _dynamoDbClient.ScanAsync(request);

            var items = response.Items.Select(Map);
            foreach (var item in items)
            {
                var deleteRequest = new DeleteItemRequest
                {
                    TableName = _tableName,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        ["Id"] = new AttributeValue { S = item.Id }
                    }
                };

                await _dynamoDbClient.DeleteItemAsync(deleteRequest);
            }
        }

        public async Task<BuddyModel> GetBuddyByIdMatch(string id)
        {
            Amazon.DynamoDBv2.Model.Condition cond = new Condition();
            cond.ComparisonOperator = "CONTAINS";
            cond.AttributeValueList = new List<AttributeValue>() { new AttributeValue { S = id } };

            try
            {
                ScanResponse scRes = await _dynamoDbClient.ScanAsync(new ScanRequest { TableName = _tableName, ScanFilter = new Dictionary<string, Condition>() { { "Id", cond } } });

                return scRes.Items.Any() ? scRes.Items.Select(Map).FirstOrDefault() : null;
            }
            catch (Exception e)
            {
                return null;
            }
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
                MobileNumber = result["MobileNumber"].S,
                Rating = result["Rating"].S,
                Profile = result["Profile"].S
            };
            return job;
        }
    }
}
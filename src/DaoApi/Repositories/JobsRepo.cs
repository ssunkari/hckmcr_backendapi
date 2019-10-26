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

        private JobsModel Map(Dictionary<string, AttributeValue> result)
        {
            var job = new JobsModel
            {
                Name = result["id"].S,
     
            };
            return job;
        }
    }
}
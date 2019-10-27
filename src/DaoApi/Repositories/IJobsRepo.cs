using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using Zuto.Uk.Sample.API.Models;

namespace Zuto.Uk.Sample.API.Repositories
{
    public interface IJobsRepo
    {
        Task<List<JobsModel>> GetAll();
        Task CreateJob(JobsModel model);
        Task<List<JobsModel>> GetJobsByPhoneNumber(string phoneNumber);
        Task DeleteAllJobs();
        Task<JobsModel> GetJobsByIdMatch(string idPrefix);
        Task Update(JobsModel jobsModel, Dictionary<string, AttributeValueUpdate> attributeValueUpdates);
    }
}
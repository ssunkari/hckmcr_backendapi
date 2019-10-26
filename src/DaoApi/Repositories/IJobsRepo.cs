using System.Collections.Generic;
using System.Threading.Tasks;
using Zuto.Uk.Sample.API.Models;

namespace Zuto.Uk.Sample.API.Repositories
{
    public interface IJobsRepo
    {
        Task<List<JobsModel>> GetAll();
    }
}
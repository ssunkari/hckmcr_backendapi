using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zuto.Uk.Sample.API.Controllers
{
    public interface IJobsRepo
    {
        Task<List<JobsModel>> GetAll();
    }
}
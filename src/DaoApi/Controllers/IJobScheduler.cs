using System.Threading.Tasks;
using Zuto.Uk.Sample.API.Models;

namespace Zuto.Uk.Sample.API.Controllers
{
    public interface IJobScheduler
    {
        Task ScheduleJob(JobsModel model);
    }
}
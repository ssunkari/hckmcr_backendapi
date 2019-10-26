using System.Collections.Generic;
using System.Threading.Tasks;
using Zuto.Uk.Sample.API.Models;

namespace Zuto.Uk.Sample.API.Controllers
{
    public interface IBuddyFinder
    {
        Task<List<BuddyModel>> GetBuddiesByGeoLocationAsync(string lat, string lon);
    }
}
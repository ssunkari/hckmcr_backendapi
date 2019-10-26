using System.Collections.Generic;
using System.Threading.Tasks;
using Zuto.Uk.Sample.API.Models;
using Zuto.Uk.Sample.API.Repositories;

namespace Zuto.Uk.Sample.API.Controllers
{
    public class BuddyFinder : IBuddyFinder
    {
        private readonly IBuddiesRepo _buddiesRepo;

        public BuddyFinder(IBuddiesRepo buddiesRepo)
        {
            _buddiesRepo = buddiesRepo;
        }

        public async Task<List<BuddyModel>> GetBuddiesByGeoLocationAsync(string lat, string lon)
        {
            var buddies = await _buddiesRepo.GetAllBuddies();
            return buddies;
        }
    }
}
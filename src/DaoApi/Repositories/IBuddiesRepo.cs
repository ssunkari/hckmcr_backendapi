using System.Collections.Generic;
using System.Threading.Tasks;
using Zuto.Uk.Sample.API.Models;

namespace Zuto.Uk.Sample.API.Repositories
{
    public interface IBuddiesRepo
    {
        Task<List<BuddyModel>> GetAllBuddies();
        Task CreateUser(BuddyModel model);
        Task DeleteAllBuddies();
        Task<BuddyModel> GetBuddyByIdMatch(string id);
    }
}
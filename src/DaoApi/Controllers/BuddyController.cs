using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Zuto.Uk.Sample.API.Models;
using Zuto.Uk.Sample.API.Models.Api;
using Zuto.Uk.Sample.API.Repositories;

namespace Zuto.Uk.Sample.API.Controllers
{
    [Route("api/buddies")]
    [ApiController]
    public class BuddyController : ControllerBase
    {
        private readonly IBuddiesRepo _buddiesRepo;

        public BuddyController(IBuddiesRepo buddiesRepo)
        {
            _buddiesRepo = buddiesRepo;
        }
  
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetAllBuddies()
        {
            var buddies = await _buddiesRepo.GetAllBuddies();
            return Ok(buddies);
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<IEnumerable<string>>> CreateBuddy([FromBody] BuddyApiModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _buddiesRepo.CreateUser(new BuddyModel(model));
            return Ok();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAllBuddies()
        {
            await _buddiesRepo.DeleteAllBuddies();
            return Ok();
        }
    }
}

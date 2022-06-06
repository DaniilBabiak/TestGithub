using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Practice.Entities.Entities;
using Practice.Service;
using Practice.Service.Interfaces;
using Practice.Service.Paging;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Practice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChallengeController : BaseController<ChallengeDto>
    {
        private readonly IChallengeService _challengeService;
        private readonly IChallengeTypeService _challengeTypeService;

        public ChallengeController(IChallengeService challengeService, IChallengeTypeService challengeTypeService,
                                   UserManager<User> userManager)
            : base(userManager)
        {
            _challengeService = challengeService;
            _challengeTypeService = challengeTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetChallengesAsync([FromQuery] ChallengePagingParameters pagingParameters)
        {
            var challenges = await _challengeService.GetAllAsync(pagingParameters);

            return PaginatedResult(challenges);
        }

        [HttpGet, Route("types")]
        public async Task<IActionResult> GetChallengeTypes()
        {
            var types = await _challengeTypeService.GetAllAsync();

            return Ok(types);
        }

        [HttpGet, Route("{id}")]
        public async Task<ChallengeDto> GetChallengeAsync(Guid id)
        {
            var challenge = await _challengeService.GetAsync(id);

            Log.Information($"Returned challenge with id: {id} from database.");

            return challenge;
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Practice.Entities.Entities;
using Practice.Service;
using Practice.Service.Interfaces;
using Practice.Service.Paging;
using System;
using System.Threading.Tasks;

namespace Practice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AchievementController : BaseController<AchievementDto>
    {
        private readonly IAchievementService _achievementService;

        public AchievementController(IAchievementService achievementService, UserManager<User> userManager)
            : base(userManager)
        {
            _achievementService = achievementService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAchievementsAsync([FromQuery] AchievementPagingParameters pagingParameters)
        {
            var achievements = await _achievementService.GetAllAsync(pagingParameters);

            return PaginatedResult(achievements);
        }

        [HttpGet, Route("{id}")]
        public async Task<IActionResult> GetAchievementAsync(Guid id)
        {
            var achievement = await _achievementService.GetAsync(id);

            return Ok(achievement);
        }

        [HttpGet, Route("user/{id}")]
        public async Task<IActionResult> GetUserAchievements([FromQuery] AchievementPagingParameters pagingParameters, [FromRoute] Guid id)
        {
            var achievements = await _achievementService.GetAllForUserAsync(id, pagingParameters);

            return PaginatedResult(achievements);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
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
    [Route("api/[controller]"), Authorize]
    [ApiController]
    public class UserChallengeController : BaseController<UserChallengeDto>
    {
        private readonly IUserChallengeService _userChallengeService;

        public UserChallengeController(IUserChallengeService userChallengeService, UserManager<User> userManager)
         : base(userManager)
        {
            _userChallengeService = userChallengeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserChallenges([FromQuery] UserChallengePagingParameters pagingParameters)
        {
            var userId = await GetCurrentUserIdAsync();

            var userChallenges = await _userChallengeService.GetAllAsync(userId, pagingParameters);

            return PaginatedResult(userChallenges);
        }

        [HttpGet, Route("{id}")]
        public async Task<UserChallengeDto> GetUserChallengeAsync(Guid id)
        {
            var challenge = await _userChallengeService.GetAsync(id);

            return challenge;
        }

        [HttpPost]
        public async Task<IActionResult> AddUserChallenge([FromBody] UserChallengeDto userChallengeDto)
        {
            var userId = await GetCurrentUserIdAsync();
            userChallengeDto.UserId = userId;

            userChallengeDto = await _userChallengeService.CreateAsync(userChallengeDto);

            return Ok(userChallengeDto);
        }

        [HttpDelete, Route("{id}")]
        public async Task<IActionResult> DeleteChallenge(Guid id)
        {
            await _userChallengeService.DeleteAsync(id);

            return Ok();
        }
    }
}

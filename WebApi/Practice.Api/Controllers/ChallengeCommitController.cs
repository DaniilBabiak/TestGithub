using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Practice.Entities.Entities;
using Practice.Service;
using Practice.Service.Interfaces;
using Practice.Service.Paging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Practice.Api.Controllers
{
    [Route("api/[controller]"), Authorize]
    [ApiController]
    public class ChallengeCommitController : BaseController<ChallengeCommitDto>
    {
        private readonly IChallengeCommitService _challengeCommitService;

        public ChallengeCommitController(IChallengeCommitService challengeCommitService, UserManager<User> userManager)
            : base(userManager)
        {
            _challengeCommitService = challengeCommitService;
        }

        [HttpPost]
        public async Task<IActionResult> AddCommit([FromBody] ChallengeCommitDto challengeCommitDto)
        {
            var userId = await GetCurrentUserIdAsync();
            challengeCommitDto.UserId = userId;

            challengeCommitDto = await _challengeCommitService.CreateAsync(challengeCommitDto);

            return Ok(challengeCommitDto);
        }

        [HttpGet, Route("{id}")]
        public async Task<ChallengeCommitDto> GetCommitAsync(Guid id)
        {
            var commit = await _challengeCommitService.GetAsync(id);

            return commit;
        }

        [HttpGet, Route("/userChallengeCommits/{userChallengeId}")]
        public async Task<IActionResult> GetCommitsForUserChallenge(Guid userChallengeId, [FromQuery] ChallengeCommitPagingParameters pagingParameters)
        {
            var commits = await _challengeCommitService.GetAllForUserChallengeAsync(userChallengeId, pagingParameters);

            var userId = await GetCurrentUserIdAsync();
            if (commits.FirstOrDefault() != null)
            {
                if (commits.FirstOrDefault().UserId != userId)
                {
                    return Forbid();
                }
            }

            return PaginatedResult(commits);
        }

        [HttpDelete, Route("{id}")]
        public async Task<IActionResult> DeleteChallenge(Guid id)
        {
            await _challengeCommitService.DeleteAsync(id);

            return Ok();
        }
    }
}
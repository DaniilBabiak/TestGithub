using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Practice.Api.Controllers;
using Practice.Entities.Entities;
using Practice.QueueProducers.Interfaces;
using Practice.Service;
using Practice.Service.Interfaces;
using Practice.Service.Paging;
using Practice.Shared.Notifications;
using System.Threading.Tasks;

namespace Practice.Api.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[area]/[controller]")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class ChallengeCommitController : BaseController<ChallengeCommitDto>
    {
        private readonly IChallengeCommitService _challengeCommitService;
        private readonly INotificationProducer _notificationProducer;

        public ChallengeCommitController(IChallengeCommitService challengeCommitService,
            INotificationProducer notificationProducer, UserManager<User> userManager)
            : base(userManager)
        {
            _challengeCommitService = challengeCommitService;
            _notificationProducer = notificationProducer;
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCommit([FromBody] ChallengeCommitDto challengeCommitDto)
        {
            var approverId = await GetCurrentUserIdAsync();
            challengeCommitDto.ApproverId = approverId;
            challengeCommitDto = await _challengeCommitService.UpdateAsync(challengeCommitDto);

            var notification = new CommitNotification(challengeCommitDto.UserChallenge.Challenge.Name, challengeCommitDto.Status);
            _notificationProducer.Send(notification, challengeCommitDto.UserId.ToString());

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetCommits([FromQuery] ChallengeCommitPagingParameters pagingParameters)
        {
            var commits = await _challengeCommitService.GetAllAsync(pagingParameters);

            return PaginatedResult(commits);
        }
    }
}

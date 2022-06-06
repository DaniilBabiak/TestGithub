using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Practice.Api.Controllers;
using Practice.Api.Services;
using Practice.Entities.Entities;
using Practice.QueueProducers.Interfaces;
using Practice.Service;
using Practice.Service.Interfaces;
using Practice.Shared.Notifications;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Practice.Api.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[area]/[controller]")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class ChallengeController : BaseController<ChallengeDto>
    {
        private readonly IChallengeService _challengeService;
        private readonly IFileService _fileService;
        private readonly INotificationProducer _notificationProducer;

        public ChallengeController(IChallengeService challengeService, IFileService fileService, UserManager<User> userManager, INotificationProducer notificationProducer)
            : base(userManager)
        {
            _challengeService = challengeService;
            _fileService = fileService;
            _notificationProducer = notificationProducer;
        }

        [HttpPost]
        public async Task<IActionResult> AddChallenge([FromBody] ChallengeDto challengeDto)
        {
            var userId = await GetCurrentUserIdAsync();
            challengeDto.CreatorId = userId.ToString();

            challengeDto = await _challengeService.CreateAsync(challengeDto);

            Log.Information($"Added challenge with id: {challengeDto.Id} to database.");

            if (challengeDto.Status == "Enabled")
            {
                var notification = new ChallengeNotification(challengeDto.Name, "A new challenge is available.");
                _notificationProducer.Send(notification, "all");
            }

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateChallenge([FromBody] ChallengeDto challengeDto)
        {
            var userId = await GetCurrentUserIdAsync();
            challengeDto.CreatorId = userId.ToString();

            var newChallenge = await _challengeService.UpdateAsync(challengeDto);

            Log.Information($"Updated challenge with id: {newChallenge.Id} to database.");

            return Ok(newChallenge);

        }

        [HttpDelete]
        public async Task<IActionResult> DeleteChallenge(Guid id)
        {
            var challenge = await _challengeService.GetAsync(id);

            _fileService.DeleteFile(challenge.ImagePath);
            _fileService.DeleteFile(challenge.ThumbnailPath);

            await _challengeService.DeleteAsync(id);

            return Ok();
        }
    }
}

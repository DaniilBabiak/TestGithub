using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Practice.Api.Services;
using Practice.Service;
using Practice.Service.Interfaces;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Practice.Api.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[area]/[controller]")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class AchievementController : ControllerBase
    {
        private readonly IAchievementService _achievementService;
        private readonly IFileService _fileService;

        public AchievementController(IAchievementService achievementService, IFileService fileService)
        {
            _achievementService = achievementService;
            _fileService = fileService;
        }

        [HttpPost]
        public async Task<IActionResult> AddAchievement([FromBody] AchievementDto achievementDto)
        {
            achievementDto = await _achievementService.CreateAsync(achievementDto);

            Log.Information($"Added achievement with id: {achievementDto.Id} to database.");

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAchievement([FromBody] AchievementDto achievementDto)
        {
            var newAchievement = await _achievementService.UpdateAsync(achievementDto);

            Log.Information($"Updated achievement with id: {newAchievement.Id} to database.");

            return Ok(newAchievement);

        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAchievement(Guid id)
        {
            var achievement = await _achievementService.GetAsync(id);

            _fileService.DeleteFile(achievement.ImagePath);
            _fileService.DeleteFile(achievement.ThumbnailPath);

            await _achievementService.DeleteAsync(id);

            return Ok();
        }
    }
}
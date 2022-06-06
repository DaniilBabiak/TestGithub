using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Practice.Entities.Entities;
using Practice.Service;
using Practice.Service.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserNotificationsSettingsService _userNotificationsSettingsService;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private const string DefaultPhotoPath = "UserPhotos/defaultPhoto.jpg";

        public AuthController(UserManager<User> userManager, ITokenService tokenService, IUserService userService, IUserNotificationsSettingsService userNotificationsSettingsService)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _userService = userService;
            _userNotificationsSettingsService = userNotificationsSettingsService;
        }

        [HttpGet, Route("notificationSettings")]
        [Authorize]
        public async Task<IActionResult> GetNotificationSettings()
        {
            var userId = Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var settings = await _userNotificationsSettingsService.GetSettingsAsync(userId);
            return Ok(settings);
        }

        [HttpPut, Route("notificationSettings")]
        [Authorize]
        public async Task<IActionResult> UpdateNotificationSettings(UserNotificationsSettings notificationsSettings)
        {
            var settings = await _userNotificationsSettingsService.UpdateSettingsAsync(notificationsSettings);

            return Ok(settings);
        }

        [HttpGet, Route("userInfo")]
        [Authorize]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _userService.GetUserAsync(Guid.Parse(userId));

            return Ok(user);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateUser(UserDto userDto)
        {
            userDto.Id = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var user = await _userService.UpdateUserAsync(userDto);

            return Ok(user);
        }

        [HttpGet, Route("contestants/{challengeId}")]
        public async Task<IActionResult> GetContestants(Guid challengeId)
        {
            var result = await _userService.GetContestantsOfChallengeAsync(challengeId);
            return Ok(result);
        }

        [HttpPost, Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid || loginModel == null)
            {
                Log.Error("Error in trying to login.");
                return BadRequest("Invalid client request");
            }

            User user = await ValidateUser(loginModel);

            if (user == null)
            {
                Log.Error($"Error in trying to login. Incorrect userName or password. UserName: {loginModel.UserName}");
                return Unauthorized(new { Message = "Incorrect Username or password!" });
            }

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, loginModel.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("Coins", user.Coins.ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(user);

            return Ok(new TokensDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost, Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            if (!ModelState.IsValid || registerModel == null)
            {
                return new BadRequestObjectResult(new { Message = "User Registration Failed" });
            }

            var user = new User()
            {
                UserName = registerModel.UserName,
                Email = registerModel.Email,
                PhotoPath = DefaultPhotoPath,
                NotificationsSettings = new UserNotificationsSettings()
                {
                    Id = Guid.NewGuid(),
                    SendAchievementNotification = true,
                    SendChallengeNotification = true,
                    SendCommitNotification = true,
                    SendUserChallengeNotification = true,
                    SendVenueNotification = true
                }
            };
            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (!result.Succeeded)
            {
                return ReturnBadRequest(result.Errors);
            }

            result = await _userManager.AddToRoleAsync(user, "User");

            if (!result.Succeeded)
            {
                return ReturnBadRequest(result.Errors);
            }

            return Ok(new { Message = "User Reigstration Successful" });
        }

        private IActionResult ReturnBadRequest(IEnumerable<IdentityError> errors)
        {
            var dictionary = new ModelStateDictionary();
            var errorStrings = string.Empty;

            foreach (var error in errors)
            {
                dictionary.AddModelError(error.Code, error.Description);
                errorStrings += $"Error code: {error.Code}, error description: {error.Description}\n";
            }
            Log.Error($"Error in trying to register. Errors: \n{errorStrings}");

            return new BadRequestObjectResult(new { Message = "User Registration Failed", Errors = dictionary });
        }

        private async Task<User> ValidateUser(LoginModel loginModel)
        {
            var identityUser = await _userManager.FindByNameAsync(loginModel.UserName);
            if (identityUser != null)
            {
                var result = _userManager
                            .PasswordHasher
                            .VerifyHashedPassword(identityUser, identityUser.PasswordHash, loginModel.Password);

                return result == PasswordVerificationResult.Failed ? null : identityUser;
            }

            return null;
        }
    }
}

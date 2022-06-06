using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Practice.Api.Exceptions;
using Practice.Data.Interfaces;
using Practice.Entities.Entities;
using System;
using System.Threading.Tasks;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;

        public TokenController(IRepository<User> userRepository, ITokenService tokenService, UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> Refresh(TokensDto tokensDto)
        {
            if (tokensDto is null)
            {
                return BadRequest("Invalid client request");
            }

            string accessToken = tokensDto.AccessToken;
            string refreshToken = tokensDto.RefreshToken;

            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var username = principal.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new AuthException("Invalid tokens");
            }

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userRepository.SaveChangesAsync();

            return new ObjectResult(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken
            });
        }

        [HttpPost, Authorize]
        [Route("revoke")]
        public async Task<IActionResult> Revoke()
        {
            var username = User.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return BadRequest();

            user.RefreshToken = null;

            await _userRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}

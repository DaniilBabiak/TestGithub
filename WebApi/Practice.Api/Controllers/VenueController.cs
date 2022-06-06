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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VenueController : BaseController<VenueDto>
    {
        private readonly IVenueService _venueService;

        public VenueController(IVenueService venueService, UserManager<User> userManager)
            : base(userManager)
        {
            _venueService = venueService;
        }

        [HttpGet, Route("nearest")]
        public async Task<IActionResult> GetNearestVenues()
        {
            var userId = await GetCurrentUserIdAsync();

            var venues = await _venueService.GetNearestAsync(userId);

            return Ok(venues);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVenues([FromQuery] VenuePagingParameters pagingParameters)
        {
            var userId = await GetCurrentUserIdAsync();
            var venues = await _venueService.GetAllAsync(userId, pagingParameters);

            return PaginatedResult(venues);
        }

        [HttpGet, Route("{id}")]
        public async Task<IActionResult> GetVenue(Guid id)
        {
            var venue = await _venueService.GetVenueAsync(id);
            return Ok(venue);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVenue([FromBody] VenueDto venueDto)
        {
            var userId = await GetCurrentUserIdAsync();

            venueDto.CreatorId = userId;

            var venue = await _venueService.CreateAsync(venueDto);

            return Ok(venue);
        }

        [HttpPost, Route("assign/{venueId}")]
        public async Task<IActionResult> AssignToVenue([FromRoute] Guid venueId)
        {
            var userId = await GetCurrentUserIdAsync();
            await _venueService.AssignUserToVenue(venueId, userId);

            return Ok();
        }

        [HttpPost, Route("unassign/{venueId}")]
        public async Task<IActionResult> UnassignFromVenue([FromRoute] Guid venueId)
        {
            var userId = await GetCurrentUserIdAsync();

            await _venueService.UnassignUserFromVenue(venueId, userId);

            return Ok();
        }

        [HttpDelete, Route("{id}")]
        public async Task<IActionResult> DeleteVenue([FromRoute] Guid id)
        {
            await _venueService.DeleteAsync(id);
            return Ok();
        }
    }
}

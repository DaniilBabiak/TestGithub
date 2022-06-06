using Practice.Service.Paging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Practice.Service.Interfaces
{
    public interface IVenueService
    {
        public Task<VenueDto> GetVenueAsync(Guid id);
        public Task<IEnumerable<VenueDto>> GetNearestAsync(Guid userId);
        public Task<PagedList<VenueDto>> GetAllAsync(Guid userId, VenuePagingParameters pagingParameters);
        public Task<VenueDto> CreateAsync(VenueDto venue);
        public Task AssignUserToVenue(Guid venueId, Guid userId);
        public Task UnassignUserFromVenue(Guid venueId, Guid userId);
        public Task DeleteAsync(Guid id);
    }
}

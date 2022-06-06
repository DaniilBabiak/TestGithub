using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Practice.Data.Interfaces;
using Practice.Entities.Entities;
using Practice.QueueProducers.Interfaces;
using Practice.Service.Exceptions;
using Practice.Service.Interfaces;
using Practice.Service.Paging;
using Practice.Shared.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Practice.Service.Services
{
    public class VenueService : IVenueService
    {
        private readonly IRepository<Venue> _venueRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly INotificationProducer _notificationProducer;
        private const double _distanceRange = 15000;

        public VenueService(IRepository<Venue> venueRepository, IMapper mapper, UserManager<User> userManager, INotificationProducer notificationProducer)
        {
            _venueRepository = venueRepository;
            _mapper = mapper;
            _userManager = userManager;
            _notificationProducer = notificationProducer;
        }

        public async Task AssignUserToVenue(Guid venueId, Guid userId)
        {
            var user = await _userManager.Users.FirstAsync(u => u.Id == userId);
            var venue = await _venueRepository.GetAsync(venueId, include => include.Attendees);

            if (venue.Attendees.FirstOrDefault(a => a.Id == userId) != null)
            {
                throw new UserAlreadyAssignedToVenueException();
            }

            venue.Attendees.Add(user);
            await _venueRepository.SaveChangesAsync();
        }

        public async Task<VenueDto> CreateAsync(VenueDto venue)
        {
            var newVenue = _mapper.Map<Venue>(venue);

            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            var point = geometryFactory.CreatePoint(new Coordinate(venue.Location.Latitude, venue.Location.Longitude));

            var location = new Entities.Entities.Location { Id = Guid.NewGuid(), Coordinates = point };

            newVenue.Location = location;
            newVenue.LocationId = location.Id;
               
            await _venueRepository.CreateAsync(newVenue);
            SendNewVenueNotification(newVenue);

            venue = _mapper.Map<VenueDto>(newVenue);
            return venue;
        }

        public async Task DeleteAsync(Guid id)
        {
            if (!await VenueExistsAsync(id))
            {
                throw new KeyNotFoundException($"Venue not found with id {id}.");
            }

            var venue = await _venueRepository.GetAsync(id);

            await _venueRepository.DeleteAsync(venue);
        }

        public async Task<PagedList<VenueDto>> GetAllAsync(Guid userId, VenuePagingParameters pagingParameters)
        {
            var venues = _venueRepository.GetAllAsNoTracking();
            var user = await _userManager.Users.Include(u => u.Location).FirstAsync(u => u.Id == userId);

            venues = venues.OrderBy(v => v.Location.Coordinates.Distance(user.Location.Coordinates)).Include(v => v.Creator).Include(v => v.Location);

            var venueDtos = _mapper.Map<List<VenueDto>>(venues);

            foreach (var venueDto in venueDtos)
            {
                venueDto.Distance = await (from v in venues
                                           where venueDto.Id == v.Id.ToString()
                                           select (int)v.GetDistance(user.Location)).FirstAsync();
            }

            var result = new PagedList<VenueDto>(venueDtos, pagingParameters.PageNumber, pagingParameters.PageSize);

            return result;
        }

        public async Task<IEnumerable<VenueDto>> GetNearestAsync(Guid userId)
        {
            var user = await _userManager.Users.Include(u => u.Location).FirstAsync(u => u.Id == userId);

            var venues = await _venueRepository.GetAllAsNoTracking().Include(v => v.Location).ToListAsync();

            venues = venues.Where(v => IsInRange(v, user)).ToList();

            var result = _mapper.Map<IEnumerable<VenueDto>>(venues);

            return result;
        }

        public async Task<VenueDto> GetVenueAsync(Guid id)
        {
            var venue = await _venueRepository.GetAsync(id, include => include.Location,
                                                        include => include.Attendees, include => include.Creator);
            var result = _mapper.Map<VenueDto>(venue);

            return result;
        }

        public async Task UnassignUserFromVenue(Guid venueId, Guid userId)
        {
            var venue = await _venueRepository.GetAsync(venueId, include => include.Attendees);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null)
            {
                venue.Attendees.Remove(user);
                await _venueRepository.SaveChangesAsync();
            }

        }

        private void SendNewVenueNotification(Venue venue)
        {
            var users = _userManager.Users.Include(u => u.Location);

            foreach (var user in users)
            {
                if (IsInRange(venue, user))
                {
                    var notification = new VenueNotification($"New venue near you! {venue.Title}");
                    _notificationProducer.Send(notification, user.Id.ToString());
                }
            }
        }

        private async Task<bool> VenueExistsAsync(Guid id)
        {
            var venue = await _venueRepository.GetAsync(id);

            return venue != null;
        }

        private bool IsInRange(Venue venue, User user)
        {
            return venue.GetDistance(user.Location) <= _distanceRange;
        }
    }
}

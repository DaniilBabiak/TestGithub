using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Practice.Data.Interfaces;
using Practice.Entities.Entities;
using Practice.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Practice.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IRepository<Entities.Entities.Location> _locationRepository;
        private readonly IRepository<UserChallenge> _userChallengeRepository;

        public UserService(UserManager<User> userManager, IMapper mapper, IRepository<UserChallenge> userChallengeRepository, IRepository<Entities.Entities.Location> locationRepository)
        {
            _userManager = userManager;
            _mapper = mapper;
            _userChallengeRepository = userChallengeRepository;
            _locationRepository = locationRepository;
        }

        public async Task<List<UserDto>> GetContestantsOfChallengeAsync(Guid challengeId)
        {
            var userChallenges = _userChallengeRepository.GetAllAsNoTracking()
                                                      .Where(u => u.ChallengeId == challengeId);

            var contestants = await userChallenges.Include(u => u.User).Select(u => u.User).ToListAsync();

            var result = _mapper.Map<List<UserDto>>(contestants);
            return result;
        }

        public async Task<UserDto> GetUserAsync(Guid userId)
        {
            var user = await _userManager.Users.Include(u => u.Location).SingleAsync(u => u.Id == userId);

            var result = _mapper.Map<UserDto>(user);
            return result;
        }

        public async Task<UserDto> UpdateUserAsync(UserDto userDto)
        {
            var user = await _userManager.Users.Include(u => u.Location).SingleAsync(u => u.Id.ToString() == userDto.Id);

            if (userDto.Location != null)
            {
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

                var point = geometryFactory.CreatePoint(new Coordinate(userDto.Location.Latitude, userDto.Location.Longitude));
                point.SRID = 4326;

                if (user.Location != null)
                {
                    user.Location.Coordinates = point;
                }
                else
                {
                    var location = new Entities.Entities.Location { Id = Guid.NewGuid(), Coordinates = point };
                    await _locationRepository.CreateAsync(location);
                    user.Location = location;
                }
            }

            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.About = userDto.About;
            user.PhotoPath = userDto.PhotoPath;
            await _userManager.UpdateAsync(user);

            var result = _mapper.Map<UserDto>(user);
            return result;
        }
    }
}

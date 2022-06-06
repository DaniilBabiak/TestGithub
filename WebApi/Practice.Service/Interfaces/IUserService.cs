using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Practice.Service.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetUserAsync(Guid userId);
        Task<UserDto> UpdateUserAsync(UserDto userDto);
        Task<List<UserDto>> GetContestantsOfChallengeAsync(Guid challengeId);
    }
}

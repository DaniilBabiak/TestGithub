using Practice.Service.Paging;
using System;
using System.Threading.Tasks;

namespace Practice.Service.Interfaces
{
    public interface IUserChallengeService
    {
        public Task<PagedList<UserChallengeDto>> GetAllAsync(Guid userId, UserChallengePagingParameters pagingParameters);
        public Task<UserChallengeDto> GetAsync(Guid id);
        public Task<UserChallengeDto> CreateAsync(UserChallengeDto userChallenge);
        public Task<UserChallengeDto> UpdateAsync(UserChallengeDto userChallenge);
        public Task DeleteAsync(Guid id);
    }
}

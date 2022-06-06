using Practice.Service.Paging;
using System;
using System.Threading.Tasks;

namespace Practice.Service.Interfaces
{
    public interface IChallengeService
    {
        public Task<PagedList<ChallengeDto>> GetAllAsync(ChallengePagingParameters pagingParameters);
        public Task<ChallengeDto> GetAsync(Guid id);
        public Task<ChallengeDto> CreateAsync(ChallengeDto challengeDto);
        public Task<ChallengeDto> UpdateAsync(ChallengeDto challengeDto);
        public Task DeleteAsync(Guid id);
    }
}

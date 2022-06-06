using Practice.Service.Paging;
using System;
using System.Threading.Tasks;

namespace Practice.Service.Interfaces
{
    public interface IChallengeCommitService
    {
        public Task<PagedList<ChallengeCommitDto>> GetAllAsync(ChallengeCommitPagingParameters pagingParameters);
        public Task<PagedList<ChallengeCommitDto>> GetAllForUserChallengeAsync(Guid userChallengeId, ChallengeCommitPagingParameters pagingParameters);
        public Task<ChallengeCommitDto> GetAsync(Guid id);
        public Task<ChallengeCommitDto> CreateAsync(ChallengeCommitDto challengeCommit);
        public Task<ChallengeCommitDto> UpdateAsync(ChallengeCommitDto challengeCommit);
        public Task DeleteAsync(Guid id);
    }
}

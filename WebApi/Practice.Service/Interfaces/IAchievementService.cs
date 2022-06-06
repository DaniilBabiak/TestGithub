using Practice.Service.Paging;
using System;
using System.Threading.Tasks;

namespace Practice.Service.Interfaces
{
    public interface IAchievementService
    {
        public Task<PagedList<AchievementDto>> GetAllAsync(AchievementPagingParameters pagingParameters);
        public Task<PagedList<AchievementDto>> GetAllForUserAsync(Guid userId, AchievementPagingParameters pagingParameters);
        public Task<AchievementDto> GetAsync(Guid id);
        public Task<AchievementDto> CreateAsync(AchievementDto achievementDto);
        public Task<AchievementDto> UpdateAsync(AchievementDto achievementDto);
        public Task DeleteAsync(Guid id);
    }
}

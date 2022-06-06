using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Practice.Data.Interfaces;
using Practice.Entities.Entities;
using Practice.Service.Helpers;
using Practice.Service.Interfaces;
using Practice.Service.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Practice.Service.Services
{
    public class AchievementService : IAchievementService
    {
        private readonly IRepository<Achievement> _achievementRepository;
        private readonly UserManager<User> _userManager;
        private readonly IRepository<Image> _imageRepository;
        private readonly IMapper _mapper;
        private readonly ISortHelper<Achievement> _sortHelper;

        public AchievementService(IRepository<Image> imageRepository, IRepository<Achievement> achievementRepository, IMapper mapper, ISortHelper<Achievement> sortHelper, UserManager<User> userManager)
        {
            _imageRepository = imageRepository;
            _achievementRepository = achievementRepository;
            _mapper = mapper;
            _sortHelper = sortHelper;
            _userManager = userManager;
        }

        public async Task<AchievementDto> CreateAsync(AchievementDto achievementDto)
        {
            var achievement = new Achievement(achievementDto.TypeId,
                                             achievementDto.Name, achievementDto.Streak);

            var image = new Image { EntityId = achievement.Id, Path = achievementDto.ImagePath, ThumbnailPath = achievementDto.ThumbnailPath };

            await _achievementRepository.CreateAsync(achievement);
            await _imageRepository.CreateAsync(image);

            achievement = await _achievementRepository.GetAsync(achievement.Id, include => include.ChallengeType);
            var result = _mapper.Map<AchievementDto>(achievement);
            result.ImagePath = image.Path;
            result.ThumbnailPath = image.ThumbnailPath;

            return result;
        }

        public async Task DeleteAsync(Guid id)
        {
            var achievement = await _achievementRepository.GetAsync(id);
            if (achievement == null)
            {
                throw new KeyNotFoundException($"Achievement not found with id {id}.");
            }

            await _achievementRepository.DeleteAsync(achievement);
        }

        public async Task<PagedList<AchievementDto>> GetAllAsync(AchievementPagingParameters pagingParameters)
        {
            var achievements = _achievementRepository.GetAllAsNoTracking()
                                                                .Include(a => a.ChallengeType);


            var sortedAchievements = await _sortHelper.ApplySort(achievements, pagingParameters.OrderBy).ToListAsync();
            var mappedAchievements = _mapper.Map<List<AchievementDto>>(sortedAchievements);

            foreach (var achievement in mappedAchievements)
            {
                var image = await _imageRepository.GetAllAsNoTracking().FirstOrDefaultAsync(i => i.EntityId == achievement.Id);
                achievement.ImagePath = image.Path;
                achievement.ThumbnailPath = image.ThumbnailPath;
            }

            var result = new PagedList<AchievementDto>(mappedAchievements, pagingParameters.PageNumber, pagingParameters.PageSize);

            return result;
        }

        public async Task<PagedList<AchievementDto>> GetAllForUserAsync(Guid userId, AchievementPagingParameters pagingParameters)
        {
            var user = await _userManager.Users.Include(u => u.Achievements).FirstOrDefaultAsync(u => u.Id == userId);
            var sortedAchievements = _sortHelper.ApplySort(user.Achievements.AsQueryable(), pagingParameters.OrderBy).ToList();
            var mappedAchievements = _mapper.Map<List<AchievementDto>>(sortedAchievements);

            foreach (var achievement in mappedAchievements)
            {
                var image = await _imageRepository.GetAllAsNoTracking().FirstOrDefaultAsync(i => i.EntityId == achievement.Id);
                achievement.ImagePath = image.Path;
                achievement.ThumbnailPath = image.ThumbnailPath;
            }

            var result = new PagedList<AchievementDto>(mappedAchievements, pagingParameters.PageNumber, pagingParameters.PageSize);

            return result;
        }

        public async Task<AchievementDto> GetAsync(Guid id)
        {
            var achievement = await _achievementRepository.GetAsync(id, include => include.ChallengeType);
            var mappedAchievement = _mapper.Map<AchievementDto>(achievement);

            var image = await _imageRepository.GetAllAsNoTracking().FirstOrDefaultAsync(i => i.EntityId == achievement.Id);
            mappedAchievement.ImagePath = image.Path;
            mappedAchievement.ThumbnailPath = image.ThumbnailPath;

            return mappedAchievement;
        }

        public async Task<AchievementDto> UpdateAsync(AchievementDto achievementDto)
        {
            var achivement = new Achievement((Guid)achievementDto.Id, achievementDto.TypeId,
                                                achievementDto.Name, achievementDto.Streak);

            await _achievementRepository.UpdateAsync(achivement);

            var image = _imageRepository.GetAllAsNoTracking().FirstOrDefault(i => i.EntityId == achivement.Id);
            image.Path = achievementDto.ImagePath;
            image.ThumbnailPath = achievementDto.ThumbnailPath;

            await _imageRepository.UpdateAsync(image);

            var result = _mapper.Map<AchievementDto>(achivement);
            result.ImagePath = image.Path;
            result.ThumbnailPath = image.ThumbnailPath;
            return result;

        }
    }
}

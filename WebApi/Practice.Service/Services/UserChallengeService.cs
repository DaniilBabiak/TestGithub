using AutoMapper;
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
    public class UserChallengeService : IUserChallengeService
    {
        private readonly IRepository<UserChallenge> _userChallengeRepository;
        private readonly IRepository<Challenge> _challengeRepository;
        private readonly IRepository<ChallengeType> _challengeTypeRepository;
        private readonly IRepository<ChallengeCommit> _challengeCommitRepository;
        private readonly IRepository<Image> _imageRepository;
        private readonly ISortHelper<UserChallenge> _sortHelper;
        private readonly IMapper _mapper;

        public UserChallengeService(IRepository<UserChallenge> userChallengeRepository, IMapper mapper,
            IRepository<Challenge> challengeRepository, IRepository<Image> imageRepository,
            IRepository<ChallengeCommit> challengeCommitRepository, IRepository<ChallengeType> challengeTypeRepository, ISortHelper<UserChallenge> sortHelper)
        {

            _userChallengeRepository = userChallengeRepository;
            _mapper = mapper;
            _challengeRepository = challengeRepository;
            _imageRepository = imageRepository;
            _challengeCommitRepository = challengeCommitRepository;
            _challengeTypeRepository = challengeTypeRepository;
            _sortHelper = sortHelper;
        }

        public async Task<UserChallengeDto> CreateAsync(UserChallengeDto userChallengeDto)
        {
            var userChallenge = _mapper.Map<UserChallenge>(userChallengeDto);

            if (await _challengeRepository.GetAsync(userChallenge.ChallengeId) == null)
            {
                throw new KeyNotFoundException($"Challenge with id {userChallenge.ChallengeId} not exists.");
            }

            userChallenge.StartedAt = DateTime.Now;
            userChallenge.StartedAt = userChallenge.StartedAt.AddTicks(-(userChallenge.StartedAt.Ticks % TimeSpan.TicksPerSecond));
            userChallenge.Status = UserChallengeStatuses.Started;

            await _userChallengeRepository.CreateAsync(userChallenge);

            userChallengeDto = _mapper.Map<UserChallengeDto>(userChallenge);
            return userChallengeDto;
        }

        public async Task DeleteAsync(Guid id)
        {
            var userChallenge = await _userChallengeRepository.GetAsync(id);
            if (userChallenge == null)
            {
                throw new KeyNotFoundException($"UserChallenge not found with id {id}.");
            }

            var commits = await _challengeCommitRepository.GetAllAsNoTracking().Where(c => c.UserChallengeId == userChallenge.Id)
                                                          .ToListAsync();

            foreach (var commit in commits)
            {
                await _challengeCommitRepository.DeleteAsync(commit);
            }

            await _userChallengeRepository.DeleteAsync(userChallenge);
        }

        public async Task<UserChallengeDto> GetAsync(Guid id)
        {
            var userChallenge = await _userChallengeRepository.GetAsync(id, include => include.Approver,
                                                                            include => include.Challenge,
                                                                            include => include.User);

            userChallenge.Challenge.Type = await _challengeTypeRepository.GetAsync(userChallenge.Challenge.TypeId);


            if (userChallenge == null)
            {
                throw new KeyNotFoundException($"UserChallenge not found with id {id}.");
            }

            var result = _mapper.Map<UserChallengeDto>(userChallenge);
            var image = await _imageRepository.GetAllAsNoTracking().FirstOrDefaultAsync(i => i.EntityId == result.Challenge.Id);

            result.Challenge.ImagePath = image.Path;
            result.Challenge.ThumbnailPath = image.ThumbnailPath;
            return result;
        }

        public async Task<PagedList<UserChallengeDto>> GetAllAsync(Guid userId, UserChallengePagingParameters pagingParameters)
        {
            var userChallengesQuery = _userChallengeRepository.GetAllAsNoTracking()
                                                              .Where(u => u.UserId == userId);

            var userChallenges = userChallengesQuery.Include(u => u.Challenge)
                                                          .ThenInclude(c => c.Type);

            var sortedUserChallenges = await _sortHelper.ApplySort(userChallenges, pagingParameters.OrderBy).ToListAsync();

            var userChallengeDtos = _mapper.Map<List<UserChallengeDto>>(sortedUserChallenges);

            foreach (var userChallengeDto in userChallengeDtos)
            {
                var image = await _imageRepository.GetAllAsNoTracking()
                    .FirstOrDefaultAsync(i => i.EntityId == userChallengeDto.ChallengeId);

                userChallengeDto.Challenge.ImagePath = image.Path;
                userChallengeDto.Challenge.ThumbnailPath = image.Path;
            }

            var result = new PagedList<UserChallengeDto>(userChallengeDtos, pagingParameters.PageNumber, pagingParameters.PageSize);
            return result;
        }

        public async Task<UserChallengeDto> UpdateAsync(UserChallengeDto userChallengeDto)
        {
            var userChallenge = _mapper.Map<UserChallenge>(userChallengeDto);

            if (await UserChallengeExist(userChallenge.ChallengeId) == false)
            {
                throw new KeyNotFoundException($"UserChallenge not found with challengeId {userChallenge.Id}, userId {userChallenge.UserId}");
            }

            await _userChallengeRepository.UpdateAsync(userChallenge);

            userChallengeDto = _mapper.Map<UserChallengeDto>(userChallenge);
            return userChallengeDto;
        }

        private async Task<bool> UserChallengeExist(Guid userChallengeId)
        {
            var challenge = await _userChallengeRepository.GetAsync(userChallengeId);

            return challenge != null;
        }
    }
}

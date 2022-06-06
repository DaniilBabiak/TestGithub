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
    public class ChallengeService : IChallengeService
    {
        private readonly IRepository<Challenge> _challengeRepository;
        private readonly IRepository<UserChallenge> _userChallengeRepository;
        private readonly IRepository<Image> _imageRepository;
        private readonly ISortHelper<Challenge> _sortHelper;
        private readonly IMapper _mapper;

        public ChallengeService(IRepository<Challenge> challengeRepository, IRepository<Image> imageRepository, IMapper mapper, IRepository<UserChallenge> userChallengeRepository, ISortHelper<Challenge> sortHelper)
        {
            _challengeRepository = challengeRepository;
            _imageRepository = imageRepository;
            _mapper = mapper;
            _userChallengeRepository = userChallengeRepository;
            _sortHelper = sortHelper;
        }
        public async Task<ChallengeDto> CreateAsync(ChallengeDto challengeDto)
        {
            var challenge = _mapper.Map<Challenge>(challengeDto);
            challenge.TypeId = Guid.Parse(challengeDto.TypeId);

            await _challengeRepository.CreateAsync(challenge);

            var image = new Image { EntityId = challenge.Id, Path = challengeDto.ImagePath, ThumbnailPath = challengeDto.ThumbnailPath };

            await _imageRepository.CreateAsync(image);

            var result = _mapper.Map<ChallengeDto>(challenge);
            result.ImagePath = image.Path;
            result.ThumbnailPath = image.ThumbnailPath;
            return result;
        }

        public async Task DeleteAsync(Guid id)
        {
            var challenge = await _challengeRepository.GetAsync(id);
            if (challenge == null)
            {
                throw new KeyNotFoundException($"Challenge not found with id {id}.");
            }

            await _challengeRepository.DeleteAsync(challenge);
        }

        public async Task<ChallengeDto> GetAsync(Guid id)
        {
            var challenge = await _challengeRepository.GetAsync(id, include => include.Creator, includePaths => includePaths.Type);

            if (challenge == null)
            {
                throw new KeyNotFoundException($"Challenge not found with id {id}.");
            }

            var challengeDto = _mapper.Map<ChallengeDto>(challenge);
            var image = await _imageRepository.GetAllAsNoTracking().FirstOrDefaultAsync(i => i.EntityId == challengeDto.Id);
            challengeDto.ImagePath = image.Path;
            challengeDto.ThumbnailPath = image.ThumbnailPath;
            return challengeDto;

        }

        public async Task<PagedList<ChallengeDto>> GetAllAsync(ChallengePagingParameters pagingParameters)
        {
            IQueryable<Challenge> challenges;
            if (!pagingParameters.IsLoggedIn || pagingParameters.IsAdmin)
            {

                challenges = _challengeRepository.GetAllAsNoTracking()
                                                           .Include(c => c.Creator)
                                                           .Include(c => c.Type);
            }
            else
            {
                var userChallenges = _userChallengeRepository.GetAllAsNoTracking()
                                                             .Where(u => u.UserId == pagingParameters.UserId)
                                                             .Include(u => u.Challenge)
                                                             .ThenInclude(c => c.Type)
                                                             .Include(u => u.Challenge)
                                                             .ThenInclude(c => c.Creator)
                                                             .Select(u => u.Challenge);

                challenges = _challengeRepository.GetAllAsNoTracking()
                                                       .Include(c => c.Creator)
                                                       .Include(c => c.Type)
                                                       .Except(userChallenges);
            }
            var sortedChallenges = await _sortHelper.ApplySort(challenges, pagingParameters.OrderBy).ToListAsync();
            var challengeDtoes = _mapper.Map<List<ChallengeDto>>(sortedChallenges);

            foreach (var challengeDto in challengeDtoes)
            {
                var image = await _imageRepository.GetAllAsNoTracking().FirstOrDefaultAsync(i => i.EntityId == challengeDto.Id);
                challengeDto.ImagePath = image.Path;
                challengeDto.ThumbnailPath = image.ThumbnailPath;
            }

            var result = new PagedList<ChallengeDto>(challengeDtoes, pagingParameters.PageNumber, pagingParameters.PageSize);

            return result;
        }

        public async Task<ChallengeDto> UpdateAsync(ChallengeDto challengeDto)
        {
            try
            {
                var challenge = _mapper.Map<Challenge>(challengeDto);
                challenge.TypeId = Guid.Parse(challengeDto.TypeId);
                await _challengeRepository.UpdateAsync(challenge);

                var image = _imageRepository.GetAllAsNoTracking().FirstOrDefault(i => i.EntityId == challenge.Id);
                image.Path = challengeDto.ImagePath;
                image.ThumbnailPath = challengeDto.ThumbnailPath;

                await _imageRepository.UpdateAsync(image);

                var result = _mapper.Map<ChallengeDto>(challenge);
                result.ImagePath = image.Path;
                result.ThumbnailPath = image.ThumbnailPath;
                return result;
            }
            catch (DbUpdateException e)
            {
                throw new KeyNotFoundException($"Challenge not found with id { challengeDto.Id }.", e);
            }
        }
    }
}

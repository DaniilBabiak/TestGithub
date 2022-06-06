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
    public class ChallengeCommitService : IChallengeCommitService
    {
        private readonly IRepository<ChallengeCommit> _challengeCommitRepository;
        private readonly IRepository<UserChallenge> _userChallengeRepository;
        private readonly ISortHelper<ChallengeCommit> _sortHelper;
        private readonly IMapper _mapper;

        public ChallengeCommitService(IRepository<ChallengeCommit> challengeCommitRepository,
            IMapper mapper, IRepository<UserChallenge> userChallengeRepository,
            ISortHelper<ChallengeCommit> sortHelper)
        {
            _challengeCommitRepository = challengeCommitRepository;
            _mapper = mapper;
            _userChallengeRepository = userChallengeRepository;
            _sortHelper = sortHelper;
        }

        public async Task<ChallengeCommitDto> CreateAsync(ChallengeCommitDto challengeCommitDto)
        {
            var challengeCommit = _mapper.Map<ChallengeCommit>(challengeCommitDto);

            if (await UserChallengeExist(challengeCommit.UserChallengeId) == false)
            {
                throw new KeyNotFoundException($"UserChallenge not found with id {challengeCommitDto.UserChallengeId}.");
            }

            challengeCommit.CommitDateTime = DateTime.Now;

            await _challengeCommitRepository.CreateAsync(challengeCommit);

            challengeCommitDto = _mapper.Map<ChallengeCommitDto>(challengeCommit);

            var userChallenge = await _userChallengeRepository.GetAsync(challengeCommit.UserChallengeId);

            userChallenge.Status = UserChallengeStatuses.Submitted;
            await _userChallengeRepository.UpdateAsync(userChallenge);
            return challengeCommitDto;
        }

        public async Task DeleteAsync(Guid id)
        {
            var commit = await _challengeCommitRepository.GetAsync(id);
            if (commit == null)
            {
                throw new KeyNotFoundException($"Challenge not found with id {id}.");
            }

            await _challengeCommitRepository.DeleteAsync(commit);
        }

        public async Task<ChallengeCommitDto> GetAsync(Guid id)
        {
            var commit = await _challengeCommitRepository.GetAsync(id,
                                                                      include => include.User,
                                                                      include => include.UserChallenge,
                                                                      include => include.Approver);

            if (commit == null)
            {
                throw new KeyNotFoundException($"Commit not found with id {id}.");
            }

            var result = _mapper.Map<ChallengeCommitDto>(commit);

            return result;
        }

        public async Task<PagedList<ChallengeCommitDto>> GetAllAsync(ChallengeCommitPagingParameters pagingParameters)
        {
            var commits = _challengeCommitRepository.GetAllAsNoTracking()
                                                             .Include(c => c.Approver)
                                                             .Include(c => c.User)
                                                             .Include(c => c.UserChallenge)
                                                             .ThenInclude(u => u.Challenge)
                                                             .ThenInclude(c => c.Type);

            var sortedCommits = await _sortHelper.ApplySort(commits, pagingParameters.OrderBy).ToListAsync();
            var mappedCommits = _mapper.Map<List<ChallengeCommitDto>>(sortedCommits);
            var result = new PagedList<ChallengeCommitDto>(mappedCommits, pagingParameters.PageNumber, pagingParameters.PageSize);

            return result;
        }

        public async Task<ChallengeCommitDto> UpdateAsync(ChallengeCommitDto challengeCommitDto)
        {
            var challengeCommit = _mapper.Map<ChallengeCommit>(challengeCommitDto);

            if (await UserChallengeExist(challengeCommit.UserChallengeId) == false)
            {
                throw new KeyNotFoundException($"Challenge not found with id {challengeCommitDto.Id}.");
            }

            await _challengeCommitRepository.UpdateAsync(challengeCommit);

            if (challengeCommit.Status == ChallengeCommitStatuses.Approved)
            {
                var userChallenge = await _userChallengeRepository.GetAsync(challengeCommit.UserChallengeId,
                                                                            include => include.Challenge,
                                                                            include => include.User);
                userChallenge.Status = UserChallengeStatuses.Completed;
                userChallenge.ApproverId = challengeCommit.ApproverId;
                userChallenge.EndedAt = DateTime.UtcNow;
                userChallenge.ApprovedAt = DateTime.UtcNow;

                userChallenge.User.Coins += userChallenge.Challenge.Reward;
                await _userChallengeRepository.UpdateAsync(userChallenge);
            }

            var updatedUserChallenge = await _userChallengeRepository.GetAsync(challengeCommit.UserChallengeId, include => include.Challenge);
            var userChallengeDto = _mapper.Map<UserChallengeDto>(updatedUserChallenge);

            challengeCommitDto = _mapper.Map<ChallengeCommitDto>(challengeCommit);
            challengeCommitDto.UserChallenge = userChallengeDto;

            return challengeCommitDto;
        }

        public async Task<PagedList<ChallengeCommitDto>> GetAllForUserChallengeAsync(Guid userChallengeId, ChallengeCommitPagingParameters pagingParameters)
        {
            var commits = _challengeCommitRepository.GetAllAsNoTracking()
                                                             .Where(c => c.UserChallengeId == userChallengeId)
                                                             .Include(c => c.Approver)
                                                             .Include(c => c.User)
                                                             .Include(c => c.UserChallenge)
                                                             .ThenInclude(u => u.Challenge)
                                                             .ThenInclude(c => c.Type);

            var sortedCommits = await _sortHelper.ApplySort(commits, pagingParameters.OrderBy).ToListAsync();
            var mappedCommits = _mapper.Map<List<ChallengeCommitDto>>(sortedCommits);
            var result = new PagedList<ChallengeCommitDto>(mappedCommits, pagingParameters.PageNumber, pagingParameters.PageSize);

            return result;
        }

        private async Task<bool> UserChallengeExist(Guid userChallengeId)
        {
            var userChallenge = await _userChallengeRepository.GetAsync(userChallengeId);

            return userChallenge != null;
        }
    }
}

using AutoFixture;
using AutoMapper;
using Moq;
using Practice.Data.Interfaces;
using Practice.Entities.Entities;
using Practice.Service.Helpers;
using Practice.Service.Interfaces;
using Practice.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebApi.Configurations;
using Xunit;

namespace Practice.Service.Tests
{
    public class ChallengeCommitServiceTests
    {
        private Mock<IRepository<UserChallenge>> _mockUserChallengeRepo;
        private Mock<IRepository<ChallengeCommit>> _mockChallengeCommitRepo;
        private Mock<ISortHelper<ChallengeCommit>> _mockSortHelper;
        private IMapper _mapper;
        private IChallengeCommitService _challengeCommitService;

        public ChallengeCommitServiceTests()
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            _mapper = mapperConfig.CreateMapper();

            _mockUserChallengeRepo = new Mock<IRepository<UserChallenge>>();
            _mockChallengeCommitRepo = new Mock<IRepository<ChallengeCommit>>();
            _mockSortHelper = new Mock<ISortHelper<ChallengeCommit>>();

            _challengeCommitService = new ChallengeCommitService(_mockChallengeCommitRepo.Object, _mapper,
                                                               _mockUserChallengeRepo.Object, _mockSortHelper.Object);
        }

        [Fact]
        public async Task ChallengeCommitService_GetCommit_InvalidId()
        {
            //Arrange
            _mockChallengeCommitRepo.Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<ChallengeCommit, object>>[]>()))
                                  .Returns(Task.FromResult<ChallengeCommit>(null));

            //Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _challengeCommitService.GetAsync(new Guid()));
        }

        [Fact]
        public async Task ChallengeCommitService_GetCommit_ValidId()
        {
            //Arrange
            Fixture fixture = new Fixture();
            var userChallengeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var commitId = Guid.NewGuid();

            var userChallenge = fixture.Build<UserChallenge>().With(c => c.Id, userChallengeId).Create();

            var user = fixture.Build<User>().With(u => u.Id, userId).Create();
            var challengeCommit = new ChallengeCommit
            {
                Id = commitId,
                ApproverId = null,
                UserChallengeId = userChallengeId,
                UserChallenge = userChallenge,
                UserId = userId,
                User = user
            };

            _mockChallengeCommitRepo.Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<ChallengeCommit, object>>[]>()))
                                  .Returns(Task.FromResult(challengeCommit));

            //Act
            var result = await _challengeCommitService.GetAsync(commitId);

            var mappedChallengeCommit = _mapper.Map<ChallengeCommitDto>(challengeCommit);

            //Assert
            Assert.Equal(mappedChallengeCommit.Id, result.Id);
            Assert.Equal(mappedChallengeCommit.UserChallengeId, result.UserChallengeId);
            Assert.Equal(mappedChallengeCommit.UserId, result.UserId);
            _mockChallengeCommitRepo.Verify(x => x.GetAsync(It.Is<Guid>(i => i == challengeCommit.Id),
                                                            It.IsAny<Expression<Func<ChallengeCommit, object>>[]>()),
                                                            Times.Once);
        }

        [Fact]
        public async Task ChallengeCommitService_DeleteCommit_InvalidId()
        {
            //Arrange
            _mockChallengeCommitRepo.Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<ChallengeCommit, object>>[]>()))
                                  .Returns(Task.FromResult<ChallengeCommit>(null));

            //Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _challengeCommitService.DeleteAsync(new Guid()));
        }

        [Fact]
        public async Task ChallengeCommitService_DeleteCommit_ValidId()
        {
            //Arrange
            Fixture fixture = new Fixture();
            var commitId = Guid.NewGuid();

            var commit = fixture.Build<ChallengeCommit>().With(c => c.Id, commitId).Create();
            _mockChallengeCommitRepo.Setup(x => x.GetAsync(It.Is<Guid>(i => i == commitId)))
                                  .Returns(Task.FromResult(commit));

            //Act
            await _challengeCommitService.DeleteAsync(commitId);

            //Assert
            _mockChallengeCommitRepo.Verify(x => x.DeleteAsync(It.Is<ChallengeCommit>(c => c.Id == commit.Id)), Times.Once);
        }

        [Fact]
        public async Task ChallengeCommitService_CreateCommit_InvalidUserChallengeId()
        {
            //Arrange
            _mockUserChallengeRepo.Setup(x => x.GetAsync(It.IsAny<Guid>())).Returns(Task.FromResult<UserChallenge>(null));

            //Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _challengeCommitService.CreateAsync(new ChallengeCommitDto()));
        }

        [Fact]
        public async Task ChallengeCommitService_CreateCommit_ValidUserChallengeId()
        {
            //Arrange
            Fixture fixture = new Fixture();
            var userChallengeId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var userChallenge = fixture.Build<UserChallenge>().With(u => u.Id, userChallengeId).Create();

            var commitDto = fixture.Build<ChallengeCommitDto>()
                                   .With(c => c.UserChallengeId, userChallengeId)
                                   .With(c => c.UserId, userId)
                                   .With(c => c.Status, "Unchecked")
                                   .With(c => c.UserChallenge, _mapper.Map<UserChallengeDto>(userChallenge))
                                   .Without(c => c.Id)
                                   .Create();

            _mockUserChallengeRepo.Setup(x => x.GetAsync(It.IsAny<Guid>())).Returns(Task.FromResult(userChallenge));

            //Act
            await _challengeCommitService.CreateAsync(commitDto);

            //Assert
            _mockChallengeCommitRepo.Verify(x => x.CreateAsync(It.Is<ChallengeCommit>
                                                              (c => c.UserChallengeId == commitDto.UserChallengeId
                                                                 && c.UserId == commitDto.UserId
                                                                 && c.Status == ChallengeCommitStatuses.Unchecked)), Times.Once);

            _mockUserChallengeRepo.Verify(x => x.UpdateAsync(It.Is<UserChallenge>
                                                            (c => c.Status == UserChallengeStatuses.Submitted)), Times.Once);
        }

        [Fact]
        public async Task ChallengeCommitService_UpdateCommit_InvalidUserChallengeId()
        {
            //Arrange
            _mockUserChallengeRepo.Setup(x => x.GetAsync(It.IsAny<Guid>())).Returns(Task.FromResult<UserChallenge>(null));

            //Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _challengeCommitService.UpdateAsync(new ChallengeCommitDto()));
        }

        [Fact]
        public async Task ChallengeCommitService_UpdateCommit_ApprovedCommit_ValidUserChallengeId()
        {
            //Arrange
            Fixture fixture = new Fixture();
            var userChallengeId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var userChallenge = fixture.Build<UserChallenge>()
                                       .With(u => u.Id, userChallengeId)
                                       .With(u => u.User, fixture.Build<User>().With(u => u.Coins, 0).With(u => u.Id, userId).Create())
                                       .Create();

            var commitDto = fixture.Build<ChallengeCommitDto>()
                                   .With(c => c.UserChallengeId, userChallengeId)
                                   .With(c => c.UserId, userId)
                                   .With(c => c.Status, "Approved")
                                   .With(c => c.UserChallenge, _mapper.Map<UserChallengeDto>(userChallenge))
                                   .Without(c => c.Id)
                                   .Create();

            _mockUserChallengeRepo.Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<UserChallenge, object>>[]>()))
                                  .Returns(Task.FromResult(userChallenge));

            _mockUserChallengeRepo.Setup(x => x.GetAsync(It.IsAny<Guid>())).Returns(Task.FromResult(userChallenge));

            //Act
            await _challengeCommitService.UpdateAsync(commitDto);

            //Assert
            _mockChallengeCommitRepo.Verify(x => x.UpdateAsync(It.Is<ChallengeCommit>
                                                              (c => c.UserChallengeId == commitDto.UserChallengeId
                                                                 && c.UserId == commitDto.UserId
                                                                 && c.Status == ChallengeCommitStatuses.Approved)), Times.Once);

            _mockUserChallengeRepo.Verify(x => x.UpdateAsync(It.Is<UserChallenge>
                                                            (c => c.Status == UserChallengeStatuses.Completed
                                                               && c.User.Coins == c.Challenge.Reward)), Times.Once);
        }

        [Fact]
        public async Task ChallengeCommitService_UpdateCommit_DisapprovedCommit_ValidUserChallengeId()
        {
            //Arrange
            Fixture fixture = new Fixture();
            var userChallengeId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var userChallenge = fixture.Build<UserChallenge>()
                                       .With(u => u.Id, userChallengeId)
                                       .With(u => u.User, fixture.Build<User>().With(u => u.Coins, 0).With(u => u.Id, userId).Create())
                                       .Create();

            var commitDto = fixture.Build<ChallengeCommitDto>()
                                   .With(c => c.UserChallengeId, userChallengeId)
                                   .With(c => c.UserId, userId)
                                   .With(c => c.Status, "Disapproved")
                                   .With(c => c.UserChallenge, _mapper.Map<UserChallengeDto>(userChallenge))
                                   .Without(c => c.Id)
                                   .Create();

            _mockUserChallengeRepo.Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<UserChallenge, object>>[]>()))
                                  .Returns(Task.FromResult(userChallenge));

            _mockUserChallengeRepo.Setup(x => x.GetAsync(It.IsAny<Guid>())).Returns(Task.FromResult(userChallenge));

            //Act
            await _challengeCommitService.UpdateAsync(commitDto);

            //Assert
            _mockChallengeCommitRepo.Verify(x => x.UpdateAsync(It.Is<ChallengeCommit>
                                                              (c => c.UserChallengeId == commitDto.UserChallengeId
                                                                 && c.UserId == commitDto.UserId
                                                                 && c.Status == ChallengeCommitStatuses.Disapproved)), Times.Once);

            _mockUserChallengeRepo.Verify(x => x.UpdateAsync(It.Is<UserChallenge>
                                                            (c => c.Status == UserChallengeStatuses.Completed
                                                               && c.User.Coins == c.Challenge.Reward)), Times.Never);
        }
    }
}

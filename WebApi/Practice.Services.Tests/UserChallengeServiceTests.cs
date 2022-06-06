using AutoFixture;
using AutoMapper;
using MockQueryable.Moq;
using Moq;
using Practice.Data.Interfaces;
using Practice.Entities.Entities;
using Practice.Service.Helpers;
using Practice.Service.Interfaces;
using Practice.Service.Paging;
using Practice.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebApi.Configurations;
using Xunit;

namespace Practice.Service.Tests
{
    public class UserChallengeServiceTests
    {
        private Mock<IRepository<Challenge>> _mockChallengeRepo;
        private Mock<IRepository<Image>> _mockImageRepo;
        private Mock<IRepository<UserChallenge>> _mockUserChallengeRepo;
        private Mock<IRepository<ChallengeCommit>> _mockChallengeCommitRepo;
        private Mock<IRepository<ChallengeType>> _mockChallengeTypeRepo;
        private Mock<ISortHelper<UserChallenge>> _mockSortHelper;
        private IMapper _mapper;
        private IUserChallengeService _userChallengeService;

        public UserChallengeServiceTests()
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            _mapper = mapperConfig.CreateMapper();

            _mockChallengeRepo = new Mock<IRepository<Challenge>>();
            _mockImageRepo = new Mock<IRepository<Image>>();
            _mockUserChallengeRepo = new Mock<IRepository<UserChallenge>>();
            _mockChallengeCommitRepo = new Mock<IRepository<ChallengeCommit>>();
            _mockChallengeTypeRepo = new Mock<IRepository<ChallengeType>>();
            _mockSortHelper = new Mock<ISortHelper<UserChallenge>>();

            _userChallengeService = new UserChallengeService(_mockUserChallengeRepo.Object, _mapper,
                                                             _mockChallengeRepo.Object,
                                                             _mockImageRepo.Object,
                                                             _mockChallengeCommitRepo.Object,
                                                             _mockChallengeTypeRepo.Object,
                                                             _mockSortHelper.Object);
        }

        [Fact]
        public async Task UserChallengeService_GetUserChallenge_InvalidId()
        {
            //Arrange
            _mockUserChallengeRepo.Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<UserChallenge, object>>[]>()))
                                  .Returns(Task.FromResult<UserChallenge>(null));

            //Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _userChallengeService.GetAsync(new Guid()));
        }

        [Fact]
        public async Task UserChallengeService_GetUserChallenge_ValidId()
        {
            //Arrange
            Fixture fixture = new Fixture();
            var challengeId = Guid.NewGuid();
            var imageId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var userChallengeId = Guid.NewGuid();

            var challenge = fixture.Build<Challenge>().With(c => c.Id, challengeId).Create();
            var images = new List<Image>()
            {
                fixture.Build<Image>().With(i => i.Id, imageId).With(i => i.EntityId, challengeId).Create(),
                fixture.Build<Image>().Create(),
                fixture.Build<Image>().Create(),
                fixture.Build<Image>().Create(),
                fixture.Build<Image>().Create()
            };

            var mockImages = images.AsQueryable().BuildMock();

            var user = fixture.Build<User>().With(u => u.Id, userId).Create();
            var userChallenge = new UserChallenge
            {
                Id = userChallengeId,
                ApproverId = null,
                ChallengeId = challengeId,
                Challenge = challenge,
                UserId = userId,
                User = user,
                StartedAt = DateTime.Now,
                Status = UserChallengeStatuses.Started
            };

            _mockUserChallengeRepo.Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<UserChallenge, object>>[]>()))
                                  .Returns(Task.FromResult(userChallenge));

            _mockImageRepo.Setup(x => x.GetAllAsNoTracking()).Returns(mockImages.Object);

            //Act
            var result = await _userChallengeService.GetAsync(userChallengeId);

            var mappedUserChallenge = _mapper.Map<UserChallengeDto>(userChallenge);
            mappedUserChallenge.Challenge.ImagePath = images.Find(i => i.EntityId == mappedUserChallenge.ChallengeId).Path;

            //Assert
            Assert.Equal(mappedUserChallenge.Id, result.Id);
            Assert.Equal(mappedUserChallenge.ChallengeId, result.ChallengeId);
            Assert.Equal(mappedUserChallenge.Challenge.Id, result.Challenge.Id);
            Assert.Equal(mappedUserChallenge.ApproverId, result.ApproverId);
            Assert.Equal(mappedUserChallenge.UserId, result.UserId);
            Assert.Equal(mappedUserChallenge.Challenge.ImagePath, result.Challenge.ImagePath);
        }

        [Fact]
        public async Task UserChallengeService_DeleteUserChallenge_InValidId()
        {
            //Arrange
            _mockUserChallengeRepo.Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<UserChallenge, object>>[]>()))
                                  .Returns(Task.FromResult<UserChallenge>(null));

            //Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _userChallengeService.GetAsync(new Guid()));
        }

        [Fact]
        public async Task UserChallengeService_DeleteUserChallenge_ValidId()
        {
            //Arrange
            Fixture fixture = new Fixture();
            var userChallengeId = Guid.NewGuid();


            var userChallenge = fixture.Build<UserChallenge>().With(u => u.Id, userChallengeId).Create();

            var commits = new List<ChallengeCommit>
            {
                fixture.Build<ChallengeCommit>().With(c => c.UserChallengeId, userChallengeId).Create(),
                fixture.Build<ChallengeCommit>().Create(),
                fixture.Build<ChallengeCommit>().With(c => c.UserChallengeId, userChallengeId).Create(),
                fixture.Build<ChallengeCommit>().Create()
            };

            var mockCommits = commits.AsQueryable().BuildMock();

            _mockChallengeCommitRepo.Setup(x => x.GetAllAsNoTracking()).Returns(mockCommits.Object);

            _mockUserChallengeRepo.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                                  .Returns(Task.FromResult(userChallenge));

            //Act
            await _userChallengeService.DeleteAsync(userChallenge.Id);

            //Assert
            _mockUserChallengeRepo.Verify(x => x.DeleteAsync(userChallenge), Times.Once);

            _mockChallengeCommitRepo.Verify(x => x.DeleteAsync(commits[0]), Times.Once);
            _mockChallengeCommitRepo.Verify(x => x.DeleteAsync(commits[2]), Times.Once);
        }

        [Fact]
        public async Task UserChallengeService_CreateUserChallenge_ValidChallengeId()
        {
            //Arrange
            var challengeId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var userChallengeDto = new UserChallengeDto { ChallengeId = challengeId, UserId = userId };

            var userChallenge = _mapper.Map<UserChallenge>(userChallengeDto);
            userChallenge.Status = UserChallengeStatuses.Started;
            userChallenge.StartedAt = DateTime.Now;
            userChallenge.StartedAt = userChallenge.StartedAt.AddTicks(-(userChallenge.StartedAt.Ticks % TimeSpan.TicksPerSecond));

            _mockUserChallengeRepo.Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<UserChallenge, object>>[]>()))
                                  .Returns(Task.FromResult(userChallenge));

            _mockChallengeRepo.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                              .Returns(Task.FromResult(new Challenge()));

            //Act
            var result = await _userChallengeService.CreateAsync(userChallengeDto);

            //Assert
            _mockUserChallengeRepo.Verify(x => x.CreateAsync(It.Is<UserChallenge>(
                u => u.ChallengeId == userChallengeDto.ChallengeId
                  && u.UserId == userChallengeDto.UserId
                  && u.Status == UserChallengeStatuses.Started)), Times.Once);
        }

        [Fact]
        public async Task UserChallengeService_CreateUserChallenge_InvalidChallengeId()
        {
            //Arrange
            _mockChallengeRepo.Setup(x => x.GetAsync(It.IsAny<Guid>())).Returns(Task.FromResult<Challenge>(null));

            //Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _userChallengeService.CreateAsync(new UserChallengeDto()));
        }

        [Fact]
        public async Task UserChallengeService_UpdateUserChallenge_InvalidId()
        {
            //Arrange
            _mockUserChallengeRepo.Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<UserChallenge, object>>[]>()))
                                  .Returns(Task.FromResult<UserChallenge>(null));

            //Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _userChallengeService.UpdateAsync(new UserChallengeDto()));
        }

        [Fact]
        public async Task UserChallengeService_UpdateUserChallenge_ValidId()
        {
            //Arrange
            var userChallengeId = Guid.NewGuid();
            var challengeId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var userChallenge = new UserChallenge
            {
                Id = userChallengeId,
                Status = UserChallengeStatuses.Completed,
                ChallengeId = challengeId,
                UserId = userId,
                StartedAt = DateTime.Now
            };

            _mockUserChallengeRepo.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                                  .Returns(Task.FromResult(userChallenge));

            var mappedUserChallenge = _mapper.Map<UserChallengeDto>(userChallenge);

            //Act
            await _userChallengeService.UpdateAsync(mappedUserChallenge);

            //Assert
            _mockUserChallengeRepo.Verify(x => x.UpdateAsync(It.Is<UserChallenge>(
                u => u.ChallengeId == mappedUserChallenge.ChallengeId
                  && u.UserId == mappedUserChallenge.UserId
                  && u.StartedAt == mappedUserChallenge.StartedAt
                  && u.Status == userChallenge.Status)), Times.Once);
        }

        [Fact]
        public async Task UserChallengeService_GetAll()
        {
            //Arrange
            Fixture fixture = new Fixture();
            var userId = Guid.NewGuid();
            var challenge1Id = Guid.NewGuid();
            var challenge2Id = Guid.NewGuid();
            var userChallenges = new List<UserChallenge>
            {
                fixture.Build<UserChallenge>().With(u => u.UserId, userId).With(u => u.ChallengeId, challenge1Id).Create(),
                fixture.Build<UserChallenge>().With(u => u.UserId, userId).With(u => u.ChallengeId, challenge2Id).Create(),
                fixture.Build<UserChallenge>().Create(),
                fixture.Build<UserChallenge>().Create(),
                fixture.Build<UserChallenge>().Create(),
                fixture.Build<UserChallenge>().Create(),
                fixture.Build<UserChallenge>().Create(),
                fixture.Build<UserChallenge>().Create()
            };

            var challenges = new List<Challenge>();

            foreach (var userChallenge in userChallenges)
            {
                challenges.Add(fixture.Build<Challenge>().With(c => c.Id, userChallenge.ChallengeId).Create());
            }

            var images = new List<Image>();

            foreach (var challenge in challenges)
            {
                images.Add(fixture.Build<Image>().With(i => i.EntityId, challenge.Id).Create());
            }

            var mockChallenges = challenges.AsQueryable().BuildMock();
            _mockChallengeRepo.Setup(x => x.GetAllAsNoTracking()).Returns(mockChallenges.Object);

            var mockImages = images.AsQueryable().BuildMock();
            _mockImageRepo.Setup(x => x.GetAllAsNoTracking()).Returns(mockImages.Object);

            var mockUserChallenges = userChallenges.AsQueryable().BuildMock();
            _mockUserChallengeRepo.Setup(x => x.GetAllAsNoTracking()).Returns(mockUserChallenges.Object);

            var pagingParameters = new UserChallengePagingParameters()
            {
                PageNumber = 1,
                PageSize = 5
            };


            //Act
            var result = await _userChallengeService.GetAllAsync(userId, pagingParameters);

            //Assert
            Assert.Equal(2, result.Count);

            foreach (var userChallenge in result)
            {
                Assert.Equal(userId, userChallenge.UserId);
            }
        }
    }
}

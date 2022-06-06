using AutoFixture;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
    public class ChallengeServiceTests
    {
        private Mock<IRepository<Challenge>> _mockChallengeRepo;
        private Mock<IRepository<Image>> _mockImageRepo;
        private IMapper _mapper;
        private Mock<IRepository<UserChallenge>> _mockUserChallengeRepo;
        private IChallengeService _challengeService;
        private Mock<ISortHelper<Challenge>> _mockSortHelper;
        public ChallengeServiceTests()
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            _mapper = mapperConfig.CreateMapper();

            _mockChallengeRepo = new Mock<IRepository<Challenge>>();
            _mockImageRepo = new Mock<IRepository<Image>>();
            _mockUserChallengeRepo = new Mock<IRepository<UserChallenge>>();
            _mockSortHelper = new Mock<ISortHelper<Challenge>>();

            _challengeService = new ChallengeService(_mockChallengeRepo.Object, _mockImageRepo.Object,
                                                        _mapper, _mockUserChallengeRepo.Object, _mockSortHelper.Object);
        }
        [Fact]
        public async Task ChallengeService_GetChallenge_InvalidId()
        {
            //Arrange
            _mockChallengeRepo.Setup(x => x.GetAsync(It.IsAny<Guid>())).Returns(Task.FromResult<Challenge>(null));

            //Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _challengeService.GetAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task ChallengeService_GetAllChallenges_NotAdminAndLoggedIn()
        {
            //Arrange
            Fixture fixture = new Fixture();
            var challenge1Id = Guid.NewGuid();
            var challenge2Id = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var challenges = new List<Challenge>
            {
                fixture.Build<Challenge>().With(c => c.Id, challenge1Id).Without(c => c.Description).Create(),
                fixture.Build<Challenge>().With(c => c.Id, challenge2Id).Create(),
                fixture.Build<Challenge>().Without(c => c.Description).Create(),
                fixture.Build<Challenge>().Without(c => c.Description).Create(),
                fixture.Build<Challenge>().Without(c => c.Description).Create(),
                fixture.Build<Challenge>().Without(c => c.Description).Create(),
                fixture.Build<Challenge>().Without(c => c.Description).Create(),
                fixture.Build<Challenge>().Without(c => c.Description).Create()
            };

            var images = new List<Image>();

            foreach (var challenge in challenges)
            {
                images.Add(fixture.Build<Image>().With(i => i.EntityId, challenge.Id).Create());
            }

            var mockChallenges = challenges.AsQueryable().BuildMock();
            _mockChallengeRepo.Setup(x => x.GetAllAsNoTracking()).Returns(mockChallenges.Object);

            var mockImages = images.AsQueryable().BuildMock();
            _mockImageRepo.Setup(x => x.GetAllAsNoTracking()).Returns(mockImages.Object);


            var userChallenges = new List<UserChallenge>
            {
                fixture.Build<UserChallenge>().With(u => u.ChallengeId, challenge1Id).With(u => u.UserId, userId)
                                              .With(u => u.Challenge, challenges.Find(c => c.Id == challenge1Id)).Create(),
                fixture.Build<UserChallenge>().With(u => u.ChallengeId, challenge2Id).With(u => u.UserId, userId)
                                              .With(u => u.Challenge, challenges.Find(c => c.Id == challenge2Id)).Create(),
                fixture.Build<UserChallenge>().Create()
            };

            var mockUserChallenges = userChallenges.AsQueryable().BuildMock();
            _mockUserChallengeRepo.Setup(x => x.GetAllAsNoTracking()).Returns(mockUserChallenges.Object);

            var pagingParameters = new ChallengePagingParameters()
            {
                IsAdmin = false,
                IsLoggedIn = true,
                PageNumber = 1,
                PageSize = 5,
                UserId = userId
            };


            //Act
            var result = await _challengeService.GetAllAsync(pagingParameters);

            challenges = challenges.Except(from challenge in challenges
                                           from userChallenge in userChallenges
                                           where userChallenge.ChallengeId == challenge.Id
                                           && userChallenge.UserId == pagingParameters.UserId
                                           select challenge).ToList();

            var mappedChallenges = _mapper.Map<List<ChallengeDto>>(challenges);
            foreach (var challenge in mappedChallenges)
            {
                challenge.ImagePath = images.Find(i => i.EntityId == challenge.Id).Path;
            }
            var pagedList = new PagedList<ChallengeDto>(mappedChallenges, pagingParameters.PageNumber, pagingParameters.PageSize);


            //Assert
            Assert.Equal(pagedList.Count, result.Count);
        }

        [Fact]
        public async Task ChallengeService_GetAllChallenges_AdminOrNotLoggedIn()
        {
            //Arrange
            Fixture fixture = new Fixture();
            var challenge1Id = Guid.NewGuid();
            var challenge2Id = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var challenges = new List<Challenge>
            {
                fixture.Build<Challenge>().With(c => c.Id, challenge1Id).Without(c => c.Description).Create(),
                fixture.Build<Challenge>().With(c => c.Id, challenge2Id).Create(),
                fixture.Build<Challenge>().Without(c => c.Description).Create(),
                fixture.Build<Challenge>().Without(c => c.Description).Create(),
                fixture.Build<Challenge>().Without(c => c.Description).Create(),
                fixture.Build<Challenge>().Without(c => c.Description).Create(),
                fixture.Build<Challenge>().Without(c => c.Description).Create(),
                fixture.Build<Challenge>().Without(c => c.Description).Create()
            };

            var images = new List<Image>();

            foreach (var challenge in challenges)
            {
                images.Add(fixture.Build<Image>().With(i => i.EntityId, challenge.Id).Create());
            }

            var mockChallenges = challenges.AsQueryable().BuildMock();
            _mockChallengeRepo.Setup(x => x.GetAllAsNoTracking()).Returns(mockChallenges.Object);

            var mockImages = images.AsQueryable().BuildMock();
            _mockImageRepo.Setup(x => x.GetAllAsNoTracking()).Returns(mockImages.Object);

            var userChallenges = new List<UserChallenge>
            {
                fixture.Build<UserChallenge>().With(u => u.ChallengeId, challenge1Id)
                                              .With(u => u.UserId, userId).Create(),
                fixture.Build<UserChallenge>().With(u => u.ChallengeId, challenge2Id)
                                              .With(u => u.UserId, userId).Create(),
                fixture.Build<UserChallenge>().Create()
            };

            var mockUserChallenges = userChallenges.AsQueryable().BuildMock();
            _mockUserChallengeRepo.Setup(x => x.GetAllAsNoTracking()).Returns(mockUserChallenges.Object);

            var pagingParameters = new ChallengePagingParameters()
            {
                IsAdmin = true,
                IsLoggedIn = true,
                PageNumber = 1,
                PageSize = 5,
                UserId = userId
            };

            var mappedChallenges = _mapper.Map<List<ChallengeDto>>(challenges);
            foreach (var challenge in mappedChallenges)
            {
                challenge.ImagePath = images.Find(i => i.EntityId == challenge.Id).Path;
            }
            var pagedList = new PagedList<ChallengeDto>(mappedChallenges, pagingParameters.PageNumber, pagingParameters.PageSize);

            //Act
            var result = await _challengeService.GetAllAsync(pagingParameters);

            //Assert
            Assert.Equal(pagedList.Count, result.Count);
        }

        [Fact]
        public async Task ChallengeService_GetChallenge_ValidId()
        {
            //Arrange
            Fixture fixture = new Fixture();
            var images = new List<Image>
            {
                fixture.Build<Image>().Create(),
                fixture.Build<Image>().Create(),
                fixture.Build<Image>().Create()
            };

            var mockImages = images.AsQueryable().BuildMock();

            var challengeId = images.First().EntityId;
            var creator = fixture.Build<User>().Create();
            _mockChallengeRepo.Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Challenge, object>>[]>()))
                .Returns(Task.FromResult(new Challenge
                {
                    Id = challengeId,
                    CreatorId = creator.Id,
                    Creator = creator,
                    AvailableFrom = DateTime.Now,
                    AvailableTo = DateTime.Now.AddDays(7),
                    Description = "test",
                    Name = "TestChallenge",
                    Reward = 100,
                    Status = ChallengeStatuses.Enabled
                }));

            _mockImageRepo.Setup(x => x.GetAllAsNoTracking()).Returns(mockImages.Object);

            //Act
            var challenge = await _mockChallengeRepo.Object.GetAsync(challengeId, include => include.Creator);
            var image = await _mockImageRepo.Object.GetAllAsNoTracking().FirstOrDefaultAsync(i => i.EntityId == challenge.Id);
            var challengeDto = await _challengeService.GetAsync(challengeId);

            //Assert
            Assert.Equal(challenge.Id, challengeDto.Id);
            Assert.Equal(image.Path, challengeDto.ImagePath);
            Assert.Equal(creator.UserName, challengeDto.CreatorName);
        }
    }
}

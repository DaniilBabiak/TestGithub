using AutoMapper;
using Practice.Entities.Entities;
using Practice.Service;

namespace WebApi.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Challenge, ChallengeDto>().ForMember("CreatorName", opt => opt.MapFrom(c => c.Creator.UserName))
                                                .ForMember("TypeName", opt => opt.MapFrom(c => c.Type.Name));
            CreateMap<ChallengeDto, Challenge>();

            CreateMap<ChallengeCommit, ChallengeCommitDto>().ForMember("UserName", opt => opt.MapFrom(c => c.User.UserName))
                                                            .ForMember("ApproverName", opt => opt.MapFrom(c => c.Approver.UserName));
            CreateMap<ChallengeCommitDto, ChallengeCommit>();

            CreateMap<UserChallenge, UserChallengeDto>().ForMember("ApproverName", opt => opt.MapFrom(u => u.Approver.UserName));
            CreateMap<UserChallengeDto, UserChallenge>();

            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();

            CreateMap<Achievement, AchievementDto>().ForMember("TypeName", opt => opt.MapFrom(a => a.ChallengeType.Name))
                                                    .ForMember("TypeId", opt => opt.MapFrom(a => a.ChallengeType.Id));
            CreateMap<AchievementDto, Achievement>();

            CreateMap<Location, LocationDto>().ForMember("Latitude", opt => opt.MapFrom(l => l.Coordinates.Coordinate.X))
                                              .ForMember("Longitude", opt => opt.MapFrom(l => l.Coordinates.Coordinate.Y));
            CreateMap<LocationDto, Location>();

            CreateMap<Venue, VenueDto>();
            CreateMap<VenueDto, Venue>();
        }
    }
}

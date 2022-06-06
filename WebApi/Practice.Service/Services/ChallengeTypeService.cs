using Microsoft.EntityFrameworkCore;
using Practice.Data.Interfaces;
using Practice.Entities.Entities;
using Practice.Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Practice.Service.Services
{
    public class ChallengeTypeService : IChallengeTypeService
    {
        private readonly IRepository<ChallengeType> _challengeTypeRepository;

        public ChallengeTypeService(IRepository<ChallengeType> challengeTypeRepository)
        {
            _challengeTypeRepository = challengeTypeRepository;
        }
        public async Task<List<ChallengeType>> GetAllAsync()
        {
            var types = await _challengeTypeRepository.GetAll().ToListAsync();

            return types;
        }
    }
}

using Practice.Entities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Practice.Service.Interfaces
{
    public interface IChallengeTypeService
    {
        public Task<List<ChallengeType>> GetAllAsync();
    }
}

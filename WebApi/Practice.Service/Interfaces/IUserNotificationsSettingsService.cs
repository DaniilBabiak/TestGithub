using Practice.Entities.Entities;
using System;
using System.Threading.Tasks;

namespace Practice.Service.Interfaces
{
    public interface IUserNotificationsSettingsService
    {
        public Task<UserNotificationsSettings> GetSettingsAsync(Guid userId);
        public Task<UserNotificationsSettings> UpdateSettingsAsync(UserNotificationsSettings settings);
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Practice.Data.Interfaces;
using Practice.Entities.Entities;
using Practice.Service.Interfaces;
using System;
using System.Threading.Tasks;

namespace Practice.Service.Services
{
    public class UserNotificationSettingsService : IUserNotificationsSettingsService
    {
        private readonly IRepository<UserNotificationsSettings> _settingsRepository;
        private readonly UserManager<User> _userManager;

        public UserNotificationSettingsService(IRepository<UserNotificationsSettings> settingsRepository, UserManager<User> userManager)
        {
            _settingsRepository = settingsRepository;
            _userManager = userManager;
        }

        public async Task<UserNotificationsSettings> GetSettingsAsync(Guid userId)
        {
            var user = await _userManager.Users.Include(u => u.NotificationsSettings).FirstAsync(u => u.Id == userId);

            return user.NotificationsSettings;
        }

        public async Task<UserNotificationsSettings> UpdateSettingsAsync(UserNotificationsSettings settings)
        {
            await _settingsRepository.UpdateAsync(settings);

            return settings;
        }
    }
}

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Practice.Entities.Entities
{
    public class User : IdentityUser<Guid>, IEntity
    {
        public int Coins { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string About { get; set; }
        public string PhotoPath { get; set; }
        public Location Location { get; set; }
        public UserNotificationsSettings NotificationsSettings { get; set; }
        public virtual ICollection<Achievement> Achievements { get; set; }
        public virtual ICollection<Venue> Venues { get; set; }
    }
}

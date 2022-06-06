using System;

namespace Practice.Entities.Entities
{
    public class UserNotificationsSettings : IEntity
    {
        public Guid Id { get; set; }
        public bool SendAchievementNotification { get; set; }
        public bool SendChallengeNotification { get; set; }
        public bool SendCommitNotification { get; set; }
        public bool SendUserChallengeNotification { get; set; }
        public bool SendVenueNotification { get; set; }
    }
}

using System;

namespace Practice.Entities.Entities
{
    public class UserChallenge : IEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid ChallengeId { get; set; }
        public Challenge Challenge { get; set; }

        public UserChallengeStatuses Status { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime? EndedAt { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public Guid? ApproverId { get; set; }
        public User Approver { get; set; }
    }
}

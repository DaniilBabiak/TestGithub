using System;

namespace Practice.Service
{
    public class UserChallengeDto
    {
        public Guid? Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid? ChallengeId { get; set; }
        public ChallengeDto Challenge { get; set; }

        public string Status { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? EndedAt { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public Guid? ApproverId { get; set; }
        public string ApproverName { get; set; }
    }
}

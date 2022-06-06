using System;

namespace Practice.Entities.Entities
{
    public class ChallengeCommit : IEntity
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid? ApproverId { get; set; }
        public User Approver { get; set; }

        public Guid UserChallengeId { get; set; }
        public UserChallenge UserChallenge { get; set; }

        public string ScreenshotPath { get; set; }

        public string Message { get; set; }

        public DateTime CommitDateTime { get; set; }

        public ChallengeCommitStatuses Status { get; set; }
    }
}

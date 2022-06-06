using System;

namespace Practice.Service
{
    public class ChallengeCommitDto
    {
        public Guid? Id { get; set; }
        public Guid? ApproverId { get; set; }
        public string ApproverName { get; set; }
        public string UserName { get; set; }
        public Guid? UserId { get; set; }
        public UserChallengeDto UserChallenge { get; set; }
        public Guid UserChallengeId { get; set; }
        public string ScreenshotPath { get; set; }
        public DateTime? CommitDateTime { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}

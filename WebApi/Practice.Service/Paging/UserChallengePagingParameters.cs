namespace Practice.Service.Paging
{
    public class UserChallengePagingParameters : QueryStringParameters
    {
        public bool GetWithoutPaging { get; set; } = false;
        public bool GetOnlyStarted { get; set; } = false;
        public bool GetOnlyCompleted { get; set; } = false;
        public bool GetOnlySubmitted { get; set; } = false;
        public UserChallengePagingParameters()
        {
            OrderBy = "StartedAt";
        }
    }
}

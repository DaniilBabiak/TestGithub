namespace Practice.Service.Paging
{
    public class ChallengeCommitPagingParameters : QueryStringParameters
    {
        public ChallengeCommitPagingParameters()
        {
            OrderBy = "Status";
        }
    }
}

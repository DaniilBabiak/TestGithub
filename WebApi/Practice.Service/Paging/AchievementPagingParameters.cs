namespace Practice.Service.Paging
{
    public class AchievementPagingParameters : QueryStringParameters
    {
        public AchievementPagingParameters()
        {
            OrderBy = "Name";
        }
    }
}

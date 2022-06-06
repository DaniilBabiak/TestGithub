using System;

namespace Practice.Service.Paging
{
    public class ChallengePagingParameters : QueryStringParameters
    {
        public bool IsLoggedIn { get; set; } = false;
        public bool IsAdmin { get; set; } = false;
        public Guid? UserId { get; set; }
        public ChallengePagingParameters()
        {
            OrderBy = "Name";
        }
    }
}

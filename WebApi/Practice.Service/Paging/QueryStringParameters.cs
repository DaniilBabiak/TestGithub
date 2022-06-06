namespace Practice.Service.Paging
{
    public abstract class QueryStringParameters
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 6;
        public string OrderBy { get; set; }
    }
}

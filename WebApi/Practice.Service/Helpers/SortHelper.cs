using System.Linq;
using System.Linq.Dynamic.Core;

namespace Practice.Service.Helpers
{
    public class SortHelper<T> : ISortHelper<T>
    {
        public IQueryable<T> ApplySort(IQueryable<T> entities, string orderByQueryString)
        {
            if (!entities.Any())
                return entities;

            if (string.IsNullOrWhiteSpace(orderByQueryString))
            {
                return entities;
            }
            var propertyFromQueryName = orderByQueryString.Split(" ")[0];
            var sortingOrder = orderByQueryString.EndsWith(" desc") ? "descending" : "ascending";

            var orderQuery = propertyFromQueryName + " " + sortingOrder;

            return entities.OrderBy(orderQuery);
        }
    }
}

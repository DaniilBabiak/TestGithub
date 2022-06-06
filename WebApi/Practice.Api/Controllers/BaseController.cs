using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Practice.Entities.Entities;
using Practice.Service.Paging;
using System;
using System.Threading.Tasks;

namespace Practice.Api.Controllers
{
    public class BaseController<T> : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public BaseController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        protected async Task<Guid> GetCurrentUserIdAsync()
        {
            var userName = User.Identity.Name;

            var user = await _userManager.FindByNameAsync(userName);

            return user.Id;
        }

        protected IActionResult PaginatedResult(PagedList<T> items)
        {
            var metadata = new { items.TotalCount, items.PageSize, items.CurrentPage, items.TotalPages, items.HasNext, items.HasPrevious };
            var camelCaseFormatter = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata, camelCaseFormatter));
            Response.Headers.Add("Access-Control-Expose-Headers", "X-Pagination");

            return Ok(items);
        }
    }
}
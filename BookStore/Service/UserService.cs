using System.Security.Claims;

namespace BookStore.Service
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public UserService(IHttpContextAccessor httpContextAccessor) 
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string GetUserId()
        {
            return httpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public bool IsAuthenticated()
        {
            return httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
        }
    }
}

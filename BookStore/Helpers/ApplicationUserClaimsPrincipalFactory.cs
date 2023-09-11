using BookStore.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace BookStore.Helpers
{
    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public ApplicationUserClaimsPrincipalFactory
            (UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            IOptions<IdentityOptions> options) 
            : base(userManager, roleManager, options)
        {
        }
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var claimsIdentity =await base.GenerateClaimsAsync(user);
            claimsIdentity.AddClaim(new Claim("UserFirstName",user.FirstName ?? ""));
            claimsIdentity.AddClaim(new Claim("UserLastName", user.LastName ?? ""));
            return claimsIdentity;
        }
    }
}

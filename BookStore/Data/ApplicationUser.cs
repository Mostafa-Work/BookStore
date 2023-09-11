using Microsoft.AspNetCore.Identity;

namespace BookStore.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string  LastName { get; set; }
        DateTime? DateOfBirth { get; set; }

    }
}

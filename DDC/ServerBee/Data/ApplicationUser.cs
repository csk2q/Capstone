using Microsoft.AspNetCore.Identity;

namespace ServerBee.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public int? CustomerId { get; set; }
    }

}

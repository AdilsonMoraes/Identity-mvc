using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.Authentication
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }

        public string DisplayName { get; set; }

        public string PhoneNumber { get; set; }

        public bool Active { get; set; } = false;
    }
}


using System;
using Microsoft.AspNetCore.Identity;

namespace Authorize.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
          
    }
}

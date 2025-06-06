﻿using Microsoft.AspNetCore.Identity;

namespace ProjectAero96.Data.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Address1 { get; set; } = null!;
        public string? Address2 { get; set; }
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
        public bool Deleted { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }
}

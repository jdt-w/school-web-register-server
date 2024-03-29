﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace SchoolWebRegister.Domain.Entity
{
    [Index(nameof(Id), IsUnique = true)]
    public sealed class ApplicationUser : IdentityUser, IEquatable<ApplicationUser>
    {
        public Profile? Profile { get; set; }
        public ICollection<ApplicationUserRole> UserRoles { get; set; }
        public ICollection<ApplicationUserClaim> Claims { get; set; }
        public ICollection<ActionLog> ActionLogs { get; set; }
        public override string ToString()
        {
            return string.Concat("ID = ", Id, " Login = ", UserName);
        }
        public bool Equals(ApplicationUser? other)
        {
            if (other == null) return false;

            return Id.Equals(other.Id);
        }
    }

    public sealed class ApplicationUserClaim : IdentityUserClaim<string>
    {
        public ApplicationUser User { get; set; }
    }

    public sealed class ApplicationRole : IdentityRole
    {
        public ICollection<ApplicationUserRole> UserRoles { get; set; }
        public ICollection<ApplicationRoleClaim> RoleClaims { get; set; }
    }

    public sealed class ApplicationRoleClaim : IdentityRoleClaim<string>
    {
        public ApplicationRole Role { get; set; }
    }

    public sealed class ApplicationUserRole : IdentityUserRole<string>
    {   
        public ApplicationUser User { get; set; }
        public ApplicationRole Role { get; set; }
    }
}

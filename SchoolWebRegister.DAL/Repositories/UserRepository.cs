using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.DAL.Repositories
{
    public sealed class UserRepository : BaseRepository<ApplicationUser>, IUserRepository
    {
        private readonly DbSet<IdentityUserClaim<string>> Claims;
        private readonly DbSet<IdentityRoleClaim<string>> RoleClaims;
        private readonly DbSet<IdentityUserRole<string>> UserRoles;
        private readonly DbSet<IdentityRole> Roles;

        public UserRepository(ApplicationDbContext db) : base(db)
        {
            Claims = db.Set<IdentityUserClaim<string>>();
            RoleClaims = db.Set<IdentityRoleClaim<string>>();
            UserRoles = db.Set<IdentityUserRole<string>>();
            Roles = db.Set<IdentityRole>();
        }

        public async Task<ApplicationUser?> GetUserByLoginAsync(string login)
        {
            return await Task.FromResult(Select()
                 .Where(user => user.Email.Equals(login))
                 .AsEnumerable()
                 .SingleOrDefault(user => user.Email.Equals(login)));
        }
        public async Task<IList<string>> GetUserRoles(ApplicationUser user)
        {
            var roles = await UserRoles.Where(role => role.UserId.Equals(user.Id)).ToListAsync();

            if (roles == null || !roles.Any()) return null;

            List<string> list = new List<string>();
            foreach (var role in roles) 
            {
                var identityRole = await Roles.FirstOrDefaultAsync(x => x.Id.Equals(role.RoleId));
                list.Add(identityRole.Name);
            }
            return list;
        }
        public async Task<bool> IsUserInRole(ApplicationUser user, UserRole role)
        {
            var userRoles = await UserRoles.AsQueryable().Where(x => x.UserId.Equals(user.Id)).ToListAsync();
            if (!userRoles.Any()) return false;

            return await Roles.AnyAsync(x => x.Name.Equals(role.ToString()) && userRoles.Any(y => y.RoleId.Equals(x)));
        }
        public async Task<IEnumerable<IdentityUserClaim<string>>> GetClaims(ApplicationUser user)
        {
            return await Claims.Where(claim => claim.UserId.Equals(user.Id)).ToListAsync();
        }
        public async Task AddClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims)
        {
            var userClaims = claims.Select(x => new IdentityUserClaim<string>
            {
                UserId = user.Id,
                ClaimType = x.Type,
                ClaimValue = x.Value
            });
            await Claims.AddRangeAsync(userClaims);
        }
        public async Task AddRolesAsync(ApplicationUser user, IEnumerable<string> roles)
        {
            foreach (string role in roles) 
            {
                IdentityRole? identityRole = await Roles.FirstOrDefaultAsync(x => x.Name.Equals(role));
                if (identityRole == null)
                {
                    identityRole = new IdentityRole
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = role.ToString(),
                        NormalizedName = role.ToString()
                    };
                    await Roles.AddAsync(identityRole);
                }
                await UserRoles.AddAsync(new IdentityUserRole<string>
                {
                    UserId = user.Id,
                    RoleId = identityRole.Id
                });
            }
        }
        public async Task RemoveClaimAsync(ApplicationUser user, Claim claim)
        {
            var identityClaim = await Claims.FirstOrDefaultAsync(x => x.ClaimType.Equals(claim.Type) && x.ClaimValue.Equals(claim.Value));

            if (identityClaim != null) Claims.Remove(identityClaim);
        }
    }
}

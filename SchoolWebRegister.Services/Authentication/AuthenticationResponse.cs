using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.Services.Authentication
{
    public struct AuthenticationResponse
    {
        public string UserId { get; set; }
        public string? Email { get; set; }
        public string[] Roles { get; set; }

        public AuthenticationResponse(ApplicationUser user)
        {
            UserId = user.Id;
            Email = user.Email;
        }
        public AuthenticationResponse(ApplicationUser user, IEnumerable<string> roles)
        {
            UserId = user.Id;
            Email = user.Email;
            Roles = roles.ToArray();
        }
    }
}

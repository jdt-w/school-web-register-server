using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.Services.Authentication
{
    public struct AuthenticationResponse
    {
        public string UserId { get; set; }
        public string? Email { get; set; }

        public AuthenticationResponse(ApplicationUser user)
        {
            UserId = user.Id;
            Email = user.Email;
        }
    }
}

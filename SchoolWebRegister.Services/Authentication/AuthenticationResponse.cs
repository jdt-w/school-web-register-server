using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.Services.Authentication
{
    public struct AuthenticationResponse
    {
        public string UserId { get; set; }
        public string? UserName { get; set; }

        public AuthenticationResponse(ApplicationUser user)
        {
            UserId = user.Id;
            UserName = user.UserName;
        }
    }
}

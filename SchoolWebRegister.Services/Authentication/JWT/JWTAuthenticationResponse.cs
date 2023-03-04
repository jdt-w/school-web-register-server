using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.Services.Authentication.JWT
{
    public struct JWTAuthenticationResponse
    {
        public string UserId { get; set; }
        public string? UserName { get; set; }

        public JWTAuthenticationResponse(ApplicationUser user)
        {
            UserId = user.Id;
            UserName = user.UserName;
        }
    }
}

using SchoolWebRegister.Domain.Entity;
using System.Text.Json.Serialization;

namespace SchoolWebRegister.Services.Authentication.JWT
{
    public struct JWTAuthenticationResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string JwtToken { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public JWTAuthenticationResponse(ApplicationUser user, string jwtToken, string refreshToken)
        {
            Id = user.Id;
            UserName = user.UserName;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }

    public struct RevokeTokenRequest
    {
        public string Token { get; set; }
    }
}

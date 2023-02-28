using System.Text.Json.Serialization;

namespace SchoolWebRegister.Services.Authentication
{
    public class JWTAuthResult
    {
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refreshToken")]
        public JWTRefreshToken RefreshToken { get; set; }
    }

    public struct JWTRefreshToken
    {
        [JsonPropertyName("username")]
        public string UserName { get; set; }

        [JsonPropertyName("tokenString")]
        public string TokenString { get; set; }

        [JsonPropertyName("expireAt")]
        public DateTime ExpireAt { get; set; }
    }
}

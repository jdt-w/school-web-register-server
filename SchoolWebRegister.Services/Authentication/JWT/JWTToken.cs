using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SchoolWebRegister.Services.Authentication.JWT
{
    public struct JWTAccessToken
    {
        [Key]
        [JsonIgnore]
        public string UserName { get; set; }
        public string TokenString { get; set; }
        public DateTime ExpireAt { get; set; }
        public string Header { get; set; }
        public string Payload { get; set; }
        public string Signature { get; set; }
    }
    public struct JWTRefreshToken
    {
        [Key]
        [JsonIgnore]
        public string UserName { get; set; }
        public string TokenString { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string CreatedByIp { get; set; }
        public string RevokedByIp { get; set; }
        public string ReplacedByToken { get; set; }
        public string ReasonRevoked { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt != null;
        public bool IsActive => !IsRevoked && !IsExpired;
    }
}

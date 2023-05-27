using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace SchoolWebRegister.Domain.Entity
{
    [PrimaryKey("DateTime")]
    public sealed class ActionLog
    {
        public DateTime DateTime { get; set; }
        public int EventId { get; set; }
        public string UserId { get; set; }
        [JsonIgnore]
        public ApplicationUser User { get; set; }
        public string? InvolvedUserId { get; set; }
        [JsonIgnore]
        public ApplicationUser? InvolvedUser { get; set; }
        public string Context { get; set; }
        public string? Component { get; set; }
        public string EventName { get; set; }
        public string? EventDescription { get; set; }
        public string Source { get; set; }
        public string IPAddress { get; set; }
    }
}

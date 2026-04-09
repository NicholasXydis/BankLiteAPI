using System;

namespace BankLite.Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required string Action { get; set; }
        public required string Details { get; set; }
        public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
    }
}

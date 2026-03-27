using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankLite.Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }
        public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
    }
}

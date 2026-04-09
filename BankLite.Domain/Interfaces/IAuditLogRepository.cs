using BankLite.Domain.Entities;
using System;
using System.Collections.Generic;

namespace BankLite.Domain.Interfaces
{
    public interface IAuditLogRepository
    {
        Task LogAsync(AuditLog auditlog);
        Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId);
    }
}

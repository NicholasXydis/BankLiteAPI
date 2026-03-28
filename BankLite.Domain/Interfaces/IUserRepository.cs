using BankLite.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankLite.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task <User?> GetByIdAsync(Guid id);
        Task <User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task <bool> ExistsAsync(string email);
    }
}

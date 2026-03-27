using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankLite.Domain.Entities
{
    public enum AccountType { Chequing, Savings }

    public class Account
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string AccountNumber { get; set; }
        public AccountType Type { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List <Transaction> Transactions { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace BankLite.Infrastructure.Data
{
    public class BankLiteDbContextFactory : IDesignTimeDbContextFactory<BankLiteDbContext>
    {
        public BankLiteDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BankLiteDbContext>();
            optionsBuilder.UseSqlServer("Data Source=localhost\\SQLEXPRESS;Initial Catalog=BankLiteDb;Integrated Security=True;Trust Server Certificate=True;");

            return new BankLiteDbContext(optionsBuilder.Options);
        }
    }
}

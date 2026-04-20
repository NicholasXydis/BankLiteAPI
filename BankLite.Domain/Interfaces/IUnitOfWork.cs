namespace BankLite.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task SaveAsync();
        Task ExecuteInTransactionAsync(Func<Task> operation);
    }
}

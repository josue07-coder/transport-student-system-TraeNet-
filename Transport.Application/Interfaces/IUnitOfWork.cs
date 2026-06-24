using System.Data;

namespace Transport.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task ExecuteInTransactionAsync(
            Func<Task> operation,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default);

        Task<T> ExecuteInTransactionAsync<T>(
            Func<Task<T>> operation,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default);
    }
}

using System.Data;
using Transport.Application.Interfaces;

namespace Transport.Application.Tests.Testing;

internal sealed class ImmediateUnitOfWork : IUnitOfWork
{
    public Task ExecuteInTransactionAsync(
        Func<Task> operation,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default)
    {
        return operation();
    }

    public Task<T> ExecuteInTransactionAsync<T>(
        Func<Task<T>> operation,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default)
    {
        return operation();
    }
}

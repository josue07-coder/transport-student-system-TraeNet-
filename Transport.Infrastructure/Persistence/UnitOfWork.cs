using Microsoft.EntityFrameworkCore;
using System.Data;
using Transport.Application.Interfaces;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task ExecuteInTransactionAsync(
            Func<Task> operation,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default)
        {
            await ExecuteInTransactionAsync<object?>(async () =>
            {
                await operation();
                return null;
            }, isolationLevel, cancellationToken);
        }

        public async Task<T> ExecuteInTransactionAsync<T>(
            Func<Task<T>> operation,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
                try
                {
                    var result = await operation();
                    await transaction.CommitAsync(cancellationToken);
                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }
    }
}

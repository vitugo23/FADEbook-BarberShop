using Fadebook.DB;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace Fadebook.Repositories;
public class DbTransactionContext : IDbTransactionContext
{
    private readonly FadebookDbContext _fadebookDbContext;
    //private IDbContextTransaction? _currentTransaction;

    public DbTransactionContext(FadebookDbContext fadebookDbContext)
    {
        _fadebookDbContext = fadebookDbContext;
    }

    // This automatically handles transaction state.
    // Operations are performed client side. This groups all operations into a single transaction and
    //   the operations on the database. Thus if any operations fail, the save changes will rollback all operations
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _fadebookDbContext.SaveChangesAsync(cancellationToken);
    }

    //public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    //{
    //    if (_currentTransaction != null)
    //    {
    //        return; // Transaction already started
    //    }
    //    _currentTransaction = await _fadebookDbContext.Database.BeginTransactionAsync(cancellationToken);
    //}

    //public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    //{
    //    if (_currentTransaction == null)
    //    {
    //        return; // No transaction to commit
    //    }
    //    await _currentTransaction.CommitAsync(cancellationToken);
    //    await _currentTransaction.DisposeAsync();
    //    _currentTransaction = null;
    //}

    //public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    //{
    //    if (_currentTransaction == null)
    //    {
    //        return; // No transaction to rollback
    //    }
    //    await _currentTransaction.RollbackAsync(cancellationToken);
    //    await _currentTransaction.DisposeAsync();
    //    _currentTransaction = null;
    //}

    //public IDbContextTransaction? GetCurrentTransaction()
    //{
    //    return _currentTransaction;
    //}
}
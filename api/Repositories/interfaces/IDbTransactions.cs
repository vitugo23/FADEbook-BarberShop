using Microsoft.EntityFrameworkCore.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace Fadebook.Repositories;
public interface IDbTransactionContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    //Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    //Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    //Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    //IDbContextTransaction? GetCurrentTransaction();
}
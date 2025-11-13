using System;
using Fadebook.DB;
using Microsoft.Data.Sqlite;

namespace Fadebook.Api.Tests.TestUtilities;

/// <summary>
/// Base class for repository integration tests using a shared Sqlite in-memory database per test.
/// Ensures proper creation and disposal of the DbContext and underlying connection.
/// </summary>
public abstract class RepositoryTestBase : IDisposable
{
    protected readonly FadebookDbContext _context;
    private readonly SqliteConnection _connection;

    protected RepositoryTestBase()
    {
        (_context, _connection) = TestDbFactory.CreateSqliteInMemoryDb();
    }

    public void Dispose()
    {
        TestDbFactory.Dispose(_context, _connection);
    }
}
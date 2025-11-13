using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Fadebook.DB;

namespace Fadebook.Api.Tests.TestUtilities;

/// <summary>
/// Helpers to create and dispose a test <see cref="FadebookDbContext"/> using in-memory SQLite.
/// </summary>
public static class TestDbFactory
{
    /// <summary>
    /// Creates a <see cref="FadebookDbContext"/> backed by an in-memory SQLite database and returns the open connection.
    /// </summary>
    public static (FadebookDbContext context, SqliteConnection connection) CreateSqliteInMemoryDb()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<FadebookDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new FadebookDbContext(options);
        context.Database.EnsureCreated();
        return (context, connection);
    }

    /// <summary>
    /// Disposes the context and closes/disposes the SQLite connection.
    /// </summary>
    /// <param name="context">Context to dispose.</param>
    /// <param name="connection">Open connection to close and dispose.</param>
    public static void Dispose(FadebookDbContext context, SqliteConnection connection)
    {
        context.Dispose();
        connection.Close();
        connection.Dispose();
    }
}
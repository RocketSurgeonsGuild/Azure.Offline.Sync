using Rocket.Surgery.Azure.Sync.Abstractions.Store;
using SQLite;

namespace Azure.Sync.Benchmarks
{
    public class MockSqlConnection : ISqlConnection
    {
        public SQLiteConnection GetConnection(string fileName) =>
            new SQLiteConnection(string.Empty, SQLiteOpenFlags.ReadOnly);
    }
}
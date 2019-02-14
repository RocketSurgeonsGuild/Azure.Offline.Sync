using System;

namespace Rocket.Surgery.Azure.Sync.Abstractions.Azure.Sync
{
    /// <summary>
    /// Interface reprsentation of a table provider.
    /// </summary>
    public interface ITableProvider
    {
        /// <summary>
        /// Gets the name of the type from table.
        /// </summary>
        Type GetTypeFromTableName(string tableName);
    }
}
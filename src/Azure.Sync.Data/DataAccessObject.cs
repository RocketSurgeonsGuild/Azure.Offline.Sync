using Newtonsoft.Json;
using Rocket.Surgery.Azure.Sync.Abstractions;
using SQLite;
using System;
using Microsoft.WindowsAzure.MobileServices;
using Rocket.Surgery.Azure.Sync.Abstractions.Store;

namespace Rocket.Surgery.Azure.Sync.Data
{
    public abstract class DataAccessObject : IDataAccessObject
    {
        /// <inheritdoc />
        [JsonProperty("CreatedAt")]
        public DateTimeOffset CreatedAt { get; set; }

        /// <inheritdoc />
        [JsonProperty("Deleted")]
        public bool Deleted { get; set; }

        /// <inheritdoc />
        [JsonProperty("Id")]
        [Column("id")]
        [PrimaryKey]
        public string Id { get; set; }

        /// <inheritdoc />
        public long ResourceId { get; set; }

        /// <inheritdoc />
        [JsonProperty("UpdatedAt")]
        public DateTimeOffset UpdatedAt { get; set; }

        /// <inheritdoc />
        [JsonProperty("Version")]
        public string Version { get; set; }

        /// <summary>
        /// Gets the name of the type from table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        /// <remarks>
        /// helper method to allow querying in custom sqlite provider
        /// if adding a new table - one MUST add that table and mapping in this method
        /// </remarks>
        public virtual Type GetTypeFromTableName<T>(string tableName)
        {
            return typeof(T);
        }
    }
}
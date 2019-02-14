using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Query;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json.Linq;
using Rocket.Surgery.Azure.Sync.Abstractions.Store;
using Rocket.Surgery.Azure.Sync.SQLite;
using SQLite;
using Xamarin.Data;

namespace Rocket.Surgery.Azure.Sync
{
    public class SqlStore : MobileServiceLocalStore, ISqlStore
    {
        private readonly ISqlConnection _sqlConnection;
        private readonly string _fileName;

        /// <summary>
        /// The maximum number of parameters allowed in any "upsert" prepared statement.
        /// Note: The default maximum number of parameters allowed by sqlite is 999
        /// See: http://www.sqlite.org/limits.html#max_variable_number
        /// </summary>
        private const int MAX_PARAMETERS_PER_QUERY = 800;

        private readonly Dictionary<string, TableDefinition> _tableMap = new Dictionary<string, TableDefinition>(StringComparer.OrdinalIgnoreCase);
        private readonly SQLiteConnection _connection;
        private readonly SemaphoreSlim _operationSemaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlStore"/> class.
        /// </summary>
        /// <param name="sqlConnection">The SQL connection.</param>
        /// <param name="fileName">Name of the file.</param>
        public SqlStore(ISqlConnection sqlConnection, string fileName)
        {
            _sqlConnection = sqlConnection;
            _fileName = fileName;
            _connection = _sqlConnection.GetConnection(_fileName);
        }

        /// <summary>
        /// Reads data from local store by executing the query.
        /// </summary>
        /// <param name="query">The query to execute on local store.</param>
        /// <returns>A task that will return with results when the query finishes.</returns>
        public override Task<JToken> ReadAsync(MobileServiceTableQueryDescription query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            EnsureInitialized();

            var formatter = new SqlQueryFormatter(query);
            string sql = formatter.FormatSelect();

            return _operationSemaphore.WaitAsync()
                .ContinueWith(t =>
                {
                    try
                    {
                        IList<JObject> rows = ExecuteQueryInternal(query.TableName, sql, formatter.Parameters);
                        JToken result = new JArray(rows.ToArray());

                        if (query.IncludeTotalCount)
                        {
                            sql = formatter.FormatSelectCount();
                            IList<JObject> countRows = null;

                            countRows = ExecuteQueryInternal(query.TableName, sql, formatter.Parameters);

                            long count = countRows[0].Value<long>("count");
                            result = new JObject()
                            {
                                { "results", result },
                                { "count", count }
                            };
                        }

                        return result;
                    }
                    finally
                    {
                        _operationSemaphore.Release();
                    }
                });
        }

        /// <summary>
        /// Updates or inserts data in local table.
        /// </summary>
        /// <param name="tableName">Name of the local table.</param>
        /// <param name="items">A list of items to be inserted.</param>
        /// <param name="ignoreMissingColumns"><code>true</code> if the extra properties on item can be ignored; <code>false</code> otherwise.</param>
        /// <returns>A task that completes when item has been upserted in local table.</returns>
        public override Task UpsertAsync(string tableName, IEnumerable<JObject> items, bool ignoreMissingColumns)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            EnsureInitialized();

            return UpsertAsyncInternal(tableName, items, ignoreMissingColumns);
        }

        /// <summary>
        /// Deletes items from local table that match the given query.
        /// </summary>
        /// <param name="query">A query to find records to delete.</param>
        /// <returns>A task that completes when delete query has executed.</returns>
        /// <exception cref="ArgumentNullException">You must supply a query value</exception>
        public override Task DeleteAsync(MobileServiceTableQueryDescription query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            EnsureInitialized();

            var formatter = new SqlQueryFormatter(query);
            string sql = formatter.FormatDelete();

            return _operationSemaphore.WaitAsync()
                .ContinueWith(t =>
                {
                    try
                    {
                        ExecuteNonQueryInternal(sql, formatter.Parameters);
                    }
                    finally
                    {
                        _operationSemaphore.Release();
                    }
                });
        }

        /// <summary>
        /// Deletes items from local table with the given list of ids
        /// </summary>
        /// <param name="tableName">Name of the local table.</param>
        /// <param name="ids">A list of ids of the items to be deleted</param>
        /// <returns>A task that completes when delete query has executed.</returns>
        public override Task DeleteAsync(string tableName, IEnumerable<string> ids)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            EnsureInitialized();

            string idRange = String.Join(",", ids.Select((_, i) => "@id" + i));

            string sql =
                $"DELETE FROM {SqlHelpers.FormatTableName(tableName)} WHERE {MobileServiceSystemColumns.Id} IN ({idRange})";

            var parameters = new Dictionary<string, object>();

            int j = 0;
            foreach (string id in ids)
            {
                parameters.Add("@id" + (j++), id);
            }

            return _operationSemaphore.WaitAsync()
               .ContinueWith(t =>
               {
                   try
                   {
                       ExecuteNonQueryInternal(sql, parameters);
                   }
                   finally
                   {
                       _operationSemaphore.Release();
                   }
               });
        }

        /// <summary>
        /// Executes a lookup against a local table.
        /// </summary>
        /// <param name="tableName">Name of the local table.</param>
        /// <param name="id">The id of the item to lookup.</param>
        /// <returns>A task that will return with a result when the lookup finishes.</returns>
        public override Task<JObject> LookupAsync(string tableName, string id)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            EnsureInitialized();

            string sql =
                $"SELECT * FROM {SqlHelpers.FormatTableName(tableName)} WHERE {MobileServiceSystemColumns.Id} = @id";
            var parameters = new Dictionary<string, object>
            {
                {"@id", id}
            };

            return _operationSemaphore.WaitAsync()
                .ContinueWith(t =>
                {
                    try
                    {
                        IList<JObject> results = ExecuteQueryInternal(tableName, sql, parameters);
                        return results.FirstOrDefault();
                    }
                    finally
                    {
                        _operationSemaphore.Release();
                    }
                });
        }

        /// <summary>
        /// Executes a sql statement on a given table in local SQLite database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="sql">The SQL query to execute.</param>
        /// <param name="parameters">The query parameters.</param>
        /// <returns>The result of query.</returns>
        protected virtual IList<JObject> ExecuteQueryInternal(string tableName, string sql, IDictionary<string, object> parameters)
        {
            TableDefinition table = GetTable(tableName);
            return ExecuteQueryInternal(tableName, table, sql, parameters);
        }

        /// <inheritdoc />
        protected override async Task OnInitialize()
        {
            CreateAllTables();
            await InitializeConfig();
        }

        /// <summary>
        /// Executes a sql statement on a given table in local SQLite database.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="table">The table definition.</param>
        /// <param name="sql">The SQL query to execute.</param>
        /// <param name="parameters">The query parameters.</param>
        /// <returns>The result of query.</returns>
        protected virtual IList<JObject> ExecuteQueryInternal(string tableName, TableDefinition table, string sql, IDictionary<string, object> parameters)
        {
            table = table ?? new TableDefinition();
            parameters = parameters ?? new Dictionary<string, object>();
            var rows = new List<JObject>();

            Type t = null;

            t = GetTypeFromTableName(tableName);

#if DEBUG
            Debug.WriteLine("sql select - {0}", sql);
            StringBuilder sb = new StringBuilder();
            sb.Append("params ");
            foreach (var p in parameters.Values.ToArray())
            {
                if (p == null)
                {
                    sb.Append(",null");
                }
                else
                {
                    sb.AppendFormat(",{0}", p);
                }
            }
            Debug.WriteLine(sb.ToString());
#endif
            var mapping = _connection.GetMapping(t);

            try
            {
                List<object> list = _connection.Query(mapping, sql, parameters.Values.ToArray());

                foreach (var item in list)
                {
                    JObject record = JObject.FromObject(item);

                    //HACK - need to remove Id property and change to id otherwise updates fail in sql store
                    //in hindsight probably better to have all the Id properties named id in base stores
                    //alas no easy way of changing all xaml comment
                    JProperty IdProperty = record.Property("Id");
                    if (IdProperty != null)
                    {
                        record.Add(new JProperty("id", IdProperty.Value.ToString()));

                        record.Remove("Id");
                    }

                    rows.Add(record);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw ex;
            }

            return rows;
        }

        /// <summary>
        ///     Gets the name of the type from table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <remarks>
        ///     helper method to allow querying in custom sqlite provider
        ///     if adding a new table - one MUST add that table and mapping in this method
        /// </remarks>
        /// <returns></returns>
        protected virtual Type GetTypeFromTableName(string tableName)
        {
            //helper method to allow querying in custom sqlite provider
            //if adding a new table - one MUST add that table and mapping in this method

            Type retVal = null;

            switch (tableName.ToLower())
            {
                case "table_info":
                    retVal = typeof(TableInfo);
                    break;

                case "__config":
                    retVal = typeof(SqlConfig);
                    break;

                case "__errors":
                    retVal = typeof(SqlError);
                    break;

                case "__operations":
                    retVal = typeof(SqlOperation);
                    break;
            }

            if (retVal == null)
            {
                throw new Exception($"DataObjects.BaseDataObject does not have mapping for {tableName}");
            }

            return retVal;
        }

        /// <summary>
        /// Executes a sql statement on a given table in local SQLite database.
        /// </summary>
        /// <param name="sql">The sql statement.</param>
        /// <param name="parameters">The query parameters.</param>
        protected virtual void ExecuteNonQueryInternal(string sql, IDictionary<string, object> parameters)
        {
            try
            {
                if (parameters == null)
                {
                    Debug.WriteLine("sql execute - {0}", sql);
                    _connection.Execute(sql);
                }
                else
                {
#if DEBUG
                    Debug.WriteLine("sql select - {0}", sql);
                    StringBuilder sb = new StringBuilder();
                    sb.Append("params ");
                    foreach (var p in parameters.Values.ToArray())
                    {
                        if (p == null)
                        {
                            sb.Append(",null");
                        }
                        else
                        {
                            sb.AppendFormat(",{0}", p);
                        }
                    }
                    Debug.WriteLine(sb.ToString());
#endif
                    _connection.Execute(sql, parameters.Values.ToArray());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw ex;
            }
        }

        internal virtual async Task SaveSetting(string name, string value)
        {
            var setting = new JObject()
            {
                { "id", name },
                { "value", value }
            };
            await UpsertAsyncInternal(MobileServiceLocalSystemTables.Config, new[] { setting }, ignoreMissingColumns: false);
        }

        internal virtual void CreateTableFromObject(string tableName, IEnumerable<ColumnDefinition> columns)
        {
            ColumnDefinition idColumn = columns.FirstOrDefault(c => c.Name.Equals(MobileServiceSystemColumns.Id));
            var colDefinitions = columns.Where(c => c != idColumn).Select(c =>
                $"{SqlHelpers.FormatMember(c.Name)} {c.StoreType}").ToList();
            if (idColumn != null)
            {
                colDefinitions.Insert(0, $"{SqlHelpers.FormatMember(idColumn.Name)} {idColumn.StoreType} PRIMARY KEY");
            }

            String tblSql =
                $"CREATE TABLE IF NOT EXISTS {SqlHelpers.FormatTableName(tableName)} ({String.Join(", ", colDefinitions)})";
            ExecuteNonQueryInternal(tblSql, parameters: null);

            string infoSql = $"PRAGMA table_info({SqlHelpers.FormatTableName(tableName)});";
            IDictionary<string, JObject> existingColumns = ExecuteQueryInternal("table_info", (TableDefinition)null, infoSql, parameters: null)
                .ToDictionary(c => c.Value<string>("name"), StringComparer.OrdinalIgnoreCase);

            // new columns that do not exist in existing columns
            var columnsToCreate = columns.Where(c => !existingColumns.ContainsKey(c.Name));

            foreach (ColumnDefinition column in columnsToCreate)
            {
                string createSql =
                    $"ALTER TABLE {SqlHelpers.FormatTableName(tableName)} ADD COLUMN {SqlHelpers.FormatMember(column.Name)} {column.StoreType}";
                ExecuteNonQueryInternal(createSql, parameters: null);
            }

            // NOTE: In SQLite you cannot drop columns, only add them.
        }

        private async Task InitializeConfig()
        {
            foreach (KeyValuePair<string, TableDefinition> table in _tableMap)
            {
                if (!MobileServiceLocalSystemTables.All.Contains(table.Key))
                {
                    // preserve system properties setting for non-system tables
                    string name = $"systemProperties|{table.Key}";
                    string value = ((int)table.Value.SystemProperties).ToString();
                    await SaveSetting(name, value);
                }
            }
        }

        private void CreateAllTables()
        {
            foreach (KeyValuePair<string, TableDefinition> table in _tableMap)
            {
                CreateTableFromObject(table.Key, table.Value.Values);
            }
        }

        private TableDefinition GetTable(string tableName)
        {
            TableDefinition table;
            if (!_tableMap.TryGetValue(tableName, out table))
            {
                throw new InvalidOperationException($"Table with name '{tableName}' is not defined.");
            }
            return table;
        }

        private Task UpsertAsyncInternal(string tableName, IEnumerable<JObject> items, bool ignoreMissingColumns)
        {
            TableDefinition table = GetTable(tableName);

            var first = items.FirstOrDefault();
            if (first == null)
            {
                return Task.FromResult(0);
            }

            // Get the columns which we want to map into the database.
            var columns = new List<ColumnDefinition>();
            foreach (var prop in first.Properties())
            {
                ColumnDefinition column;

                // If the column is coming from the server we can just ignore it,
                // otherwise, throw to alert the caller that they have passed an invalid column
                if (!table.TryGetValue(prop.Name, out column) && !ignoreMissingColumns)
                {
                    throw new InvalidOperationException(
                        $"Column with name '{prop.Name}' is not defined on the local table '{tableName}'.");
                }

                if (column != null)
                {
                    columns.Add(column);
                }
            }

            if (columns.Count == 0)
            {
                // no query to execute if there are no columns in the table
                return Task.FromResult(0);
            }

            return _operationSemaphore.WaitAsync()
                .ContinueWith(t =>
                {
                    try
                    {
                        ExecuteNonQueryInternal("BEGIN TRANSACTION", null);

                        BatchInsert(tableName, items, columns.Where(c => c.Name.Equals(MobileServiceSystemColumns.Id)).Take(1).ToList());
                        BatchUpdate(tableName, items, columns);

                        ExecuteNonQueryInternal("COMMIT TRANSACTION", null);
                    }
                    finally
                    {
                        _operationSemaphore.Release();
                    }
                });
        }

        private void BatchInsert(string tableName, IEnumerable<JObject> items, List<ColumnDefinition> columns)
        {
            if (columns.Count == 0) // we need to have some columns to insert the item
            {
                return;
            }

            // Generate the prepared insert statement
            string sqlBase =
                $"INSERT OR IGNORE INTO {SqlHelpers.FormatTableName(tableName)} ({String.Join(", ", columns.Select(c => c.Name).Select(SqlHelpers.FormatMember))}) VALUES ";

            // Use int division to calculate how many times this record will fit into our parameter quota
            int batchSize = ValidateParameterCount(columns.Count);

            foreach (var batch in items.Split(maxLength: batchSize))
            {
                var sql = new StringBuilder(sqlBase);
                var parameters = new Dictionary<string, object>();

                foreach (JObject item in batch)
                {
                    AppendInsertValuesSql(sql, parameters, columns, item);
                    sql.Append(",");
                }

                if (parameters.Any())
                {
                    sql.Remove(sql.Length - 1, 1); // remove the trailing comma
                    ExecuteNonQueryInternal(sql.ToString(), parameters);
                }
            }
        }

        private void BatchUpdate(string tableName, IEnumerable<JObject> items, List<ColumnDefinition> columns)
        {
            if (columns.Count <= 1)
            {
                return; // For update to work there has to be at least once column besides Id that needs to be updated
            }

            ValidateParameterCount(columns.Count);

            string sqlBase = $"UPDATE {SqlHelpers.FormatTableName(tableName)} SET ";

            foreach (JObject item in items)
            {
                var sql = new StringBuilder(sqlBase);
                var parameters = new Dictionary<string, object>();

                ColumnDefinition idColumn = columns.FirstOrDefault(c => c.Name.Equals(MobileServiceSystemColumns.Id));
                if (idColumn == null)
                {
                    continue;
                }

                foreach (var column in columns.Where(c => c != idColumn))
                {
                    string paramName = AddParameter(item, parameters, column);

                    sql.AppendFormat("{0} = {1}", SqlHelpers.FormatMember(column.Name), paramName);
                    sql.Append(",");
                }

                if (parameters.Any())
                {
                    sql.Remove(sql.Length - 1, 1); // remove the trailing comma
                }

                sql.AppendFormat(" WHERE {0} = {1}", SqlHelpers.FormatMember(MobileServiceSystemColumns.Id), AddParameter(item, parameters, idColumn));

                ExecuteNonQueryInternal(sql.ToString(), parameters);
            }
        }

        private static int ValidateParameterCount(int parametersCount)
        {
            int batchSize = MAX_PARAMETERS_PER_QUERY / parametersCount;
            if (batchSize == 0)
            {
                throw new InvalidOperationException(
                    $"The number of fields per entity in an upsert operation is limited to {MAX_PARAMETERS_PER_QUERY}.");
            }
            return batchSize;
        }

        private static void AppendInsertValuesSql(StringBuilder sql, Dictionary<string, object> parameters, List<ColumnDefinition> columns, JObject item)
        {
            sql.Append("(");
            int colCount = 0;
            foreach (var column in columns)
            {
                if (colCount > 0)
                    sql.Append(",");

                sql.Append(AddParameter(item, parameters, column));

                colCount++;
            }
            sql.Append(")");
        }

        private static string AddParameter(JObject item, Dictionary<string, object> parameters, ColumnDefinition column)
        {
            JToken rawValue = item.GetValue(column.Name, StringComparison.OrdinalIgnoreCase);
            object value = SqlHelpers.SerializeValue(rawValue, column.StoreType, column.JsonType);
            string paramName = CreateParameter(parameters, value);
            return paramName;
        }

        private static string CreateParameter(Dictionary<string, object> parameters, object value)
        {
            string paramName = "@p" + parameters.Count;
            parameters[paramName] = value;
            return paramName;
        }
    }
}
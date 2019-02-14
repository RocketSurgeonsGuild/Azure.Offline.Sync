using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Rocket.Surgery.Azure.Sync.Abstractions;
using Rocket.Surgery.Azure.Sync.Abstractions.Store;

namespace Azure.Sync.Tests
{
    internal class TestTable<T> : IMobileServiceSyncTable<T>
        where T : IDataAccessObject, new()
    {
        public List<T> Items = new List<T>
        {
            new T
            {
                Id = "1"
            }
        };

        public TestTable() { _mobileServiceTableQuery = Substitute.For<IMobileServiceTableQuery<T>>(); }

        private readonly IMobileServiceTableQuery<T> _mobileServiceTableQuery;

        public MobileServiceClient MobileServiceClient { get; }

        public MobileServiceRemoteTableOptions SupportedOptions { get; set; }

        public string TableName { get; }

        public IMobileServiceTableQuery<T> CreateQuery() => _mobileServiceTableQuery;

        public async Task DeleteAsync(JObject instance)
        {
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(T instance)
        {
            await Task.CompletedTask;
        }

        public IMobileServiceTableQuery<T> IncludeTotalCount() => _mobileServiceTableQuery;

        public async Task<JObject> InsertAsync(JObject instance)
        {
            return await Task.FromResult(new JObject());
        }

        public async Task InsertAsync(T instance)
        {
            await Task.CompletedTask;
        }

        public IMobileServiceTableQuery<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector) => _mobileServiceTableQuery;

        public IMobileServiceTableQuery<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector) => _mobileServiceTableQuery;

        public async Task PullAsync<U>(string queryId,
            IMobileServiceTableQuery<U> query,
            bool pushOtherTables,
            CancellationToken cancellationToken,
            PullOptions pullOptions)
        {
            await Task.CompletedTask;
        }

        public async Task PullAsync(string queryId,
            string query,
            IDictionary<string, string> parameters,
            bool pushOtherTables,
            CancellationToken cancellationToken,
            PullOptions pullOptions)
        {
            await Task.CompletedTask;
        }

        public async Task PurgeAsync<U>(string queryId, IMobileServiceTableQuery<U> query, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        public async Task PurgeAsync<U>(string queryId, IMobileServiceTableQuery<U> query, bool force, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        public async Task PurgeAsync(string queryId, string query, bool force, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        public async Task<JToken> ReadAsync(string query)
        {
            return await Task.FromResult(new JTokenWriter().CurrentToken);
        }

        public async Task<IEnumerable<T>> ReadAsync()
        {
            return await Task.FromResult(Items.AsEnumerable());
        }

        public async Task<IEnumerable<U>> ReadAsync<U>(IMobileServiceTableQuery<U> query)
        {
            return await Task.FromResult(Enumerable.Empty<U>());
        }

        public async Task RefreshAsync(T instance)
        {
            await Task.CompletedTask;
        }

        public IMobileServiceTableQuery<U> Select<U>(Expression<Func<T, U>> selector) => default(IMobileServiceTableQuery<U>);

        public IMobileServiceTableQuery<T> Skip(int count) => _mobileServiceTableQuery;

        public IMobileServiceTableQuery<T> Take(int count) => _mobileServiceTableQuery;

        public IMobileServiceTableQuery<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector) => _mobileServiceTableQuery;

        public IMobileServiceTableQuery<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector) => _mobileServiceTableQuery;

        public async Task<IEnumerable<T>> ToEnumerableAsync()
        {
            return await Task.FromResult(Items);
        }

        public async Task<List<T>> ToListAsync()
        {
            return await Task.FromResult(Items);
        }

        public async Task UpdateAsync(JObject instance)
        {
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(T instance)
        {
            await Task.CompletedTask;
        }

        public IMobileServiceTableQuery<T> Where(Expression<Func<T, bool>> predicate) => _mobileServiceTableQuery;

        async Task<T> IMobileServiceSyncTable<T>.LookupAsync(string id) => await Task.FromResult(new T());

        async Task<JObject> IMobileServiceSyncTable.LookupAsync(string id) => await Task.FromResult(new JObject());
    }
}
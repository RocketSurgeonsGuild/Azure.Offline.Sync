using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.Query;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json.Linq;

namespace Azure.Sync.Benchmarks
{
    public class MockServiceLocalStore : IMobileServiceLocalStore
    {
        public void Dispose() { }

        public async Task InitializeAsync() => await Task.CompletedTask;

        public async Task<JToken> ReadAsync(MobileServiceTableQueryDescription query) => await Task.FromResult(default(JToken));

        public async Task UpsertAsync(string tableName, IEnumerable<JObject> items, bool ignoreMissingColumns) => await Task.CompletedTask;

        public async Task DeleteAsync(MobileServiceTableQueryDescription query) => await Task.CompletedTask;

        public async Task DeleteAsync(string tableName, IEnumerable<string> ids) => await Task.CompletedTask;

        public async Task<JObject> LookupAsync(string tableName, string id) => await Task.FromResult(default(JObject));
    }
}
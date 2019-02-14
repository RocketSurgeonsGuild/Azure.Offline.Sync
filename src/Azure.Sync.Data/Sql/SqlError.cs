// ReSharper disable InconsistentNaming

using SQLite;

namespace Xamarin.Data
{
    public class SqlError
    {
        public int httpStatus { get; set; }

        [PrimaryKey] public string id { get; set; }

        public string item { get; set; }
        public int operationKind { get; set; }
        public int operationVersion { get; set; }
        public string rawResult { get; set; }
        public int tableKind { get; set; }
        public string tableName { get; set; }
    }
}
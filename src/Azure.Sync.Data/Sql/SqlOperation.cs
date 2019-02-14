using System;
using SQLite;

// ReSharper disable InconsistentNaming

namespace Xamarin.Data
{
    public class SqlOperation
    {
        public DateTime? createdAt { get; set; }

        [PrimaryKey] public string id { get; set; }

        public string item { get; set; }
        public string itemId { get; set; }
        public int kind { get; set; }
        public int sequence { get; set; }
        public int state { get; set; }
        public int tableKind { get; set; }
        public string tableName { get; set; }
        public int version { get; set; }
    }
}
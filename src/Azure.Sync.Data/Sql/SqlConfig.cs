// ReSharper disable InconsistentNaming

using SQLite;

namespace Xamarin.Data
{
    public class SqlConfig
    {
        [PrimaryKey] public string id { get; set; }

        public string value { get; set; }
    }
}
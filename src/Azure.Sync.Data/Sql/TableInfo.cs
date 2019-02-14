// ReSharper disable InconsistentNaming

namespace Xamarin.Data
{
    //this class contains system tables used by Azure - added here to allow support for query/read in azure custom sql provider using sqlcipher
    //used in sqlite to return data for Pragma table_info() calls
    public class TableInfo
    {
        //column id
        public int cid { get; set; }

        public string dflt_value { get; set; }

        //column name
        public string name { get; set; }

        //denotes nullable column
        //default value for each column - should be null
        public int notnull { get; set; }

        //denotes primary key column 1/0
        public int pk { get; set; }

        //column type
        public string type { get; set; }
    }
}
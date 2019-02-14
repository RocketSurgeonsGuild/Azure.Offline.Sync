
namespace Rocket.Surgery.Azure.Sync.Data
{
    public class Lookup : DataAccessObject
    {
        //nvarchar(1000)
        public string LookupDescription { get; set; }

        //nvarchar(1000)
        public string LookupSort { get; set; }

        //nvarchar(128)
        public string LookupType { get; set; }

        //nvarchar(128)
        public string LookupValue { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Rocket.Surgery.Azure.Sync.Abstractions.Store
{
    public interface ISqlConnection
    {
        SQLiteConnection GetConnection(string fileName);
    }
}

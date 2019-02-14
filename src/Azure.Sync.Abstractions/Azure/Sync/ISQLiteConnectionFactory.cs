namespace Rocket.Surgery.Azure.Sync.Abstractions.Azure.Sync
{
    /// <summary>
    /// Interface representation of a <see cref="SQLiteConnection"/> factory.
    /// </summary>
    public interface ISQLiteConnectionFactory
    {
        /// <summary>
        /// Gets the <see cref="SQLiteConnection"/> for the given filename.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        /// <seealso cref="SQLiteConnection GetConnection(string fileName)" />
        string GetConnection(string fileName);
    }
}
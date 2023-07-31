namespace NewStreamSupporter.Contracts
{
    /// <summary>
    /// Rozhraní umožňující práci se soubory ve formě Streamů
    /// </summary>
    public interface IFileStore
    {
        /// <summary>
        /// Stores the given data under the given key
        /// </summary>
        /// <param name="key">The key under which the data should be stored</param>
        /// <param name="data">Data to store</param>
        /// <exception cref="ArgumentException">If the file size exceeds the limit</exception>
        public Task Store(string key, Stream data);
        /// <summary>
        /// Loads data under the given key
        /// </summary>
        /// <param name="key">Key under which the data is stored</param>
        /// <returns>The data stored under the given key, or null if no such data exists</returns>
        public Task<Stream?> Load(string key);
        /// <summary>
        /// Deletes the given key from the store
        /// </summary>
        /// <param name="key">Key for which data should be deleted</param>
        public Task Delete(string key);
        /// <summary>
        /// Checks whether a given key exists in the store
        /// </summary>
        /// <param name="key">The key for which to search</param>
        /// <returns>True if specified key exists, false otherwise</returns>
        public Task<bool> Exists(string key);
    }
}

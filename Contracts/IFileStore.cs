namespace NewStreamSupporter.Contracts
{
    public interface IFileStore
    {
        /// <summary>
        /// Stores the given data under the given key
        /// </summary>
        /// <param name="key">The key under which the data should be stored</param>
        /// <param name="data">Data to store</param>
        /// <exception cref="ArgumentException">If the file size exceeds the limit</exception>
        public Task Store(string key, byte[] data);
        /// <summary>
        /// Loads data under the given key
        /// </summary>
        /// <param name="key">Key under which the data is stored</param>
        /// <returns>The data stored under the given key, or null if no such data exists</returns>
        public Task<byte[]?> Load(string key);
        /// <summary>
        /// Deletes the given key from the store
        /// </summary>
        /// <param name="key">Key for which data should be deleted</param>
        public Task Delete(string key);
    }
}

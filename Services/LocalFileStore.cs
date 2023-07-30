using NewStreamSupporter.Contracts;

namespace NewStreamSupporter.Services
{
    public class LocalFileStore : IFileStore
    {
        private readonly string _basePath;
        private readonly long _sizeLimit;

        public LocalFileStore(string basePath)
        {
            _basePath = basePath;
            if (!Directory.Exists(basePath))
            {
                try
                {
                    Directory.CreateDirectory(basePath);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Could not create the specified base path", ex);
                }
            }
        }

        public Task Delete(string key)
        {
            try
            {
                File.Delete(key);
            }
            catch (Exception)
            {

            }
            return Task.CompletedTask;
        }

        public Task<byte[]?> Load(string key)
        {
            var filePath = GetPath(key);
            if (!File.Exists(filePath))
            {
                return Task.FromResult<byte[]?>(null);
            }
            return File.ReadAllBytesAsync(filePath) as Task<byte[]?>;
        }

        public Task Store(string key, byte[] data)
        {
            if(data.Length > _sizeLimit)
            {
                throw new ArgumentException("Data provided is too large");
            }
            return File.WriteAllBytesAsync(GetPath(key), data);
        }

        private string GetPath(string key)
            => Path.Combine(_basePath, key);
    }
}

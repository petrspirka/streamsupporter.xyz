using NewStreamSupporter.Contracts;

namespace NewStreamSupporter.Services
{
    public class LocalFileStore : IFileStore
    {
        private readonly string _basePath;
        private readonly long _sizeLimit;

        public LocalFileStore(string basePath, long maxFileSize)
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
            _sizeLimit = maxFileSize;
        }

        public Task Delete(string key)
        {
            try
            {
                File.Delete(GetPath(key));
            }
            catch (Exception)
            {

            }
            return Task.CompletedTask;
        }

        public Task<Stream?> Load(string key)
        {
            string filePath = GetPath(key);
            if (!File.Exists(filePath))
            {
                return Task.FromResult<Stream?>(null);
            }
            return Task.FromResult(File.OpenRead(filePath) as Stream) as Task<Stream?>;
        }

        public async Task Store(string key, Stream data)
        {
            if (data.Length > _sizeLimit)
            {
                throw new ArgumentException("Data provided is too large");
            }
            FileStream stream = File.OpenWrite(GetPath(key));
            await data.CopyToAsync(stream);
            stream.Close();
            return;
        }

        public Task<bool> Exists(string key)
            => Task.FromResult(File.Exists(GetPath(key)));

        private string GetPath(string key)
            => Path.Combine(_basePath, key);
    }
}

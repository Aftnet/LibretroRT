using PCLStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RetriX.Shared.StreamProviders
{
    public class SingleFileStreamProvider : IStreamProvider
    {
        private readonly string Path;
        private readonly IFile File;

        public SingleFileStreamProvider(string path, IFile file)
        {
            Path = path;
            File = file;
        }

        public void Dispose()
        {

        }

        public Task<IEnumerable<string>> ListEntriesAsync()
        {
            var output = new string[] { Path };
            return Task.FromResult(output as IEnumerable<string>);
        }

        public Task<Stream> GetFileStreamAsync(string path, PCLStorage.FileAccess accessType)
        {
            if (Path.Equals(path, StringComparison.OrdinalIgnoreCase))
            {
                return File.OpenAsync(accessType);
            }

            return Task.FromResult(null as Stream);
        }
    }
}

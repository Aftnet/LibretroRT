using PCLStorage;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RetriX.Shared.FileProviders
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

        public Task<Stream> GetFileStreamAsync(string path, System.IO.FileAccess accessType)
        {
            if (Path == path)
            {
                return File.OpenAsync(accessType.ToPCLStorageAccess());
            }

            return Task.FromResult(null as Stream);
        }
    }
}

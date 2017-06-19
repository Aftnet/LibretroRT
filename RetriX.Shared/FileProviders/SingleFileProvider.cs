using PCLStorage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RetriX.Shared.FileProviders
{
    public class SingleFileProvider : IFileProvider
    {
        private readonly string Path;
        private readonly IFile File;

        public SingleFileProvider(string path, IFile file)
        {
            Path = path;
            File = file;
        }

        public void Dispose()
        {

        }

        public Task<Stream> GetFileStreamAsync(string path, System.IO.FileAccess accessType)
        {
            if (Path == path)
                return File.OpenAsync(accessType.ToPCLStorageAccess());

            return Task.FromResult(null as Stream);
        }
    }
}

using PCLStorage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RetriX.Shared.FileProviders
{
    public class SingleFileProvider : IFileProvider
    {
        private readonly Uri Uri;
        private readonly IFile File;

        public SingleFileProvider(Uri uri, IFile file)
        {
            Uri = uri;
            File = file;
        }

        public void Dispose()
        {

        }

        public Task<Stream> GetFileStreamAsync(Uri uri, System.IO.FileAccess accessType)
        {
            if (Uri == uri)
                return File.OpenAsync(accessType.ToPCLStorageAccess());

            return Task.FromResult(null as Stream);
        }
    }
}

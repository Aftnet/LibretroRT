using PCLStorage;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace RetriX.Shared.FileProviders
{
    public class ArchiveFileProvider : IFileProvider
    {
        private readonly string HandledScheme;
        private ZipArchive Archive = null;

        public ArchiveFileProvider(string handledScheme, IFile archiveFile)
        {
            HandledScheme = handledScheme;
            archiveFile.OpenAsync(PCLStorage.FileAccess.ReadAndWrite).ContinueWith(d =>
            {
                Archive = new ZipArchive(d.Result);
            });
        }

        public void Dispose()
        {
            Archive?.Dispose();
        }

        public async Task<Stream> GetFileStreamAsync(Uri uri, System.IO.FileAccess accessType)
        {
            if (uri.Scheme != HandledScheme)
            {
                return null;
            }

            while (Archive == null)
            {
                await Task.Delay(50);
            }

            var entry = Archive.GetEntry(uri.LocalPath);
            return entry.Open();
        }
    }
}

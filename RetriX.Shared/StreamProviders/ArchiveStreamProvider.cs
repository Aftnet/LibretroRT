using PCLStorage;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace RetriX.Shared.FileProviders
{
    /*public class ArchiveStreamProvider : IStreamProvider
    {
        private readonly string HandledScheme;
        private ZipArchive Archive = null;

        public ArchiveStreamProvider(string handledScheme, IFile archiveFile)
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

        public async Task<IEnumerable<string>> ListEntriesAsync()
        {
            while (Archive == null)
            {
                await Task.Delay(50);
            }

            var output = Archive.Entries.Select(d => $"{HandledScheme}{d.FullName}").OrderBy(d => d).ToArray();
            return output;
        }

        public async Task<Stream> GetFileStreamAsync(string path, System.IO.FileAccess accessType)
        {
            if (!path.StartsWith(HandledScheme))
            {
                return null;
            }

            path = path.Substring(HandledScheme.Length);
            while (Archive == null)
            {
                await Task.Delay(50);
            }

            var entry = Archive.GetEntry(path);
            return entry.Open();
        }
    }*/
}

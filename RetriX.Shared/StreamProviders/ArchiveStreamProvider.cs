using PCLStorage;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace RetriX.Shared.StreamProviders
{
    public class ArchiveStreamProvider : IStreamProvider
    {
        private readonly string HandledScheme;
        private readonly IFile ArchiveFile;
        private ZipArchive Archive = null;
        private SortedSet<string> EntriesList;

        public ArchiveStreamProvider(string handledScheme, IFile archiveFile)
        {
            HandledScheme = handledScheme;
            ArchiveFile = archiveFile;
        }

        public void Dispose()
        {
            Archive?.Dispose();
        }

        public async Task InitializeAsync()
        {
            var stream = await ArchiveFile.OpenAsync(PCLStorage.FileAccess.ReadAndWrite);
            Archive = new ZipArchive(stream, ZipArchiveMode.Update);
            EntriesList = new SortedSet<string>(Archive.Entries.Select(d => $"{HandledScheme}{d.FullName}"));
        }

        public Task<IEnumerable<string>> ListEntriesAsync()
        {
            return Task.FromResult(EntriesList as IEnumerable<string>);
        }

        public Task<Stream> GetFileStreamAsync(string path, PCLStorage.FileAccess accessType)
        {
            if (!path.StartsWith(HandledScheme))
            {
                return Task.FromResult(null as Stream);
            }

            path = path.Substring(HandledScheme.Length);
            if (!EntriesList.Contains(path))
            {
                return Task.FromResult(null as Stream);
            }

            var entry = Archive.GetEntry(path);
            var output = entry.Open();
            return Task.FromResult(output);
        }
    }
}

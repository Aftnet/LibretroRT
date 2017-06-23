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
        private readonly Dictionary<string, ZipArchiveEntry> EntriesDictionary = new Dictionary<string, ZipArchiveEntry>();

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
            var stream = await ArchiveFile.OpenAsync(PCLStorage.FileAccess.Read);
            Archive = new ZipArchive(stream, ZipArchiveMode.Read);
            foreach (var i in Archive.Entries)
            {
                EntriesDictionary.Add(i.FullName, i);
            }
        }

        public Task<IEnumerable<string>> ListEntriesAsync()
        {
            return Task.FromResult(EntriesDictionary.Keys.Select(d => HandledScheme + d).OrderBy(d => d) as IEnumerable<string>);
        }

        public Task<Stream> GetFileStreamAsync(string path, PCLStorage.FileAccess accessType)
        {
            if (!path.StartsWith(HandledScheme))
            {
                return Task.FromResult(null as Stream);
            }

            path = path.Substring(HandledScheme.Length);
            if (!EntriesDictionary.Keys.Contains(path))
            {
                return Task.FromResult(null as Stream);
            }

            var entry = EntriesDictionary[path];
            var output = entry.Open();
            return Task.FromResult(output);
        }
    }
}

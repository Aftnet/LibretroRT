using PCLStorage;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace RetriX.Shared.StreamProviders
{
    public class ArchiveStreamProvider : IStreamProvider
    {
        private readonly string HandledScheme;
        private readonly IFile ArchiveFile;
        private readonly Dictionary<string, byte[]> EntriesBufferMapping = new Dictionary<string, byte[]>();

        public ArchiveStreamProvider(string handledScheme, IFile archiveFile)
        {
            HandledScheme = handledScheme;
            ArchiveFile = archiveFile;
        }

        public void Dispose()
        {
        }

        public async Task InitializeAsync()
        {
            var stream = await ArchiveFile.OpenAsync(PCLStorage.FileAccess.Read);
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                foreach (var i in archive.Entries)
                {
                    using (var entryStream = i.Open())
                    {
                        var memoryStream = new MemoryStream();
                        await entryStream.CopyToAsync(memoryStream);
                        EntriesBufferMapping.Add(Path.Combine(HandledScheme,i.FullName), memoryStream.ToArray());
                    }
                }
            }
        }

        public Task<IEnumerable<string>> ListEntriesAsync()
        {
            return Task.FromResult(EntriesBufferMapping.Keys.OrderBy(d => d) as IEnumerable<string>);
        }

        public Task<Stream> OpenFileStreamAsync(string path, PCLStorage.FileAccess accessType)
        {
            if (!EntriesBufferMapping.Keys.Contains(path, StringComparer.OrdinalIgnoreCase))
            {
                return Task.FromResult(null as Stream);
            }

            var output = new MemoryStream(EntriesBufferMapping[path]);
            return Task.FromResult(output as Stream);
        }

        public void CloseStream(Stream stream)
        {
        }
    }
}

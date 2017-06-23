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
        private readonly Dictionary<string, Stream> EntriesStreamMapping = new Dictionary<string, Stream>();

        public ArchiveStreamProvider(string handledScheme, IFile archiveFile)
        {
            HandledScheme = handledScheme;
            ArchiveFile = archiveFile;
        }

        public void Dispose()
        {
            foreach(var i in EntriesStreamMapping.Values)
            {
                i.Dispose();
            }
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
                        EntriesStreamMapping.Add(i.FullName, memoryStream);
                    }
                }
            }
        }

        public Task<IEnumerable<string>> ListEntriesAsync()
        {
            return Task.FromResult(EntriesStreamMapping.Keys.Select(d => HandledScheme + d).OrderBy(d => d) as IEnumerable<string>);
        }

        public Task<Stream> OpenFileStreamAsync(string path, PCLStorage.FileAccess accessType)
        {
            if (!path.StartsWith(HandledScheme))
            {
                return Task.FromResult(null as Stream);
            }

            path = path.Substring(HandledScheme.Length);
            if (!EntriesStreamMapping.Keys.Contains(path))
            {
                return Task.FromResult(null as Stream);
            }

            var output = EntriesStreamMapping[path];
            output.Seek(0, SeekOrigin.Begin);
            return Task.FromResult(output);
        }

        public void CloseStream(Stream stream)
        {
            
        }
    }
}

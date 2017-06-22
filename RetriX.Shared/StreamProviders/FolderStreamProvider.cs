using PCLStorage;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RetriX.Shared.StreamProviders
{
    public class FolderStreamProvider : IStreamProvider
    {
        private readonly string HandledScheme;
        private readonly IFolder RootFolder;

        public FolderStreamProvider(string handledScheme, IFolder rootFolder)
        {
            HandledScheme = handledScheme;
            RootFolder = rootFolder;
        }

        public void Dispose()
        {

        }

        public async Task<IEnumerable<string>> ListEntriesAsync()
        {
            var files = await RootFolder.GetFilesAsync();
            var output = files.Select(d => $"{HandledScheme}{d.Path.Substring(0, RootFolder.Path.Length)}").OrderBy(d => d).ToArray();
            return output;
        }

        public async Task<Stream> GetFileStreamAsync(string path, PCLStorage.FileAccess accessType)
        {
            if (!path.StartsWith(HandledScheme))
            {
                return null;
            }

            path = path.Substring(HandledScheme.Length);
            path = Path.Combine(RootFolder.Path, path);

            var existenceCheck = await RootFolder.CheckExistsAsync(path);
            if (existenceCheck != ExistenceCheckResult.FileExists)
            {
                return null;
            }

            var file = await RootFolder.GetFileAsync(path);
            var output = await file.OpenAsync(accessType);
            return output;
        }
    }
}

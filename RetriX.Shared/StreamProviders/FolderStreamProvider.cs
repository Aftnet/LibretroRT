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
            var files = await ListFilesAsync(RootFolder);
            var output = files.Select(d => HandledScheme + d.Path.Substring(RootFolder.Path.Length + 1)).OrderBy(d => d).ToArray();
            return output;
        }

        private async Task<IEnumerable<IFile>> ListFilesAsync(IFolder folder)
        {
            IEnumerable<IFile> files = await folder.GetFilesAsync();
            var subfolders = await folder.GetFoldersAsync();
            foreach(var i in subfolders)
            {
                var subfolderFiles = await ListFilesAsync(i);
                files = files.Concat(subfolderFiles);
            }

            return files;
        }

        public async Task<Stream> GetFileStreamAsync(string path, PCLStorage.FileAccess accessType)
        {
            if (!path.StartsWith(HandledScheme))
            {
                return null;
            }

            path = path.Substring(HandledScheme.Length);

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

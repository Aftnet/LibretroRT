using PCLStorage;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RetriX.Shared.StreamProviders
{
    public class FolderStreamProvider : StreamProviderBase
    {
        private readonly string HandledScheme;
        private readonly IFolder RootFolder;

        public FolderStreamProvider(string handledScheme, IFolder rootFolder)
        {
            HandledScheme = handledScheme;
            RootFolder = rootFolder;
        }

        public override async Task<IEnumerable<string>> ListEntriesAsync()
        {
            var files = await ListFilesRecursiveAsync(RootFolder);
            var output = files.Select(d => HandledScheme + d.Path.Substring(RootFolder.Path.Length + 1)).OrderBy(d => d).ToArray();
            return output;
        }

        protected override async Task<Stream> OpenFileStreamAsyncInternal(string path, PCLStorage.FileAccess accessType)
        {
            if (!path.StartsWith(HandledScheme, System.StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            path = path.Substring(HandledScheme.Length);

            if (accessType == PCLStorage.FileAccess.Read)
            {
                var existenceCheck = await RootFolder.CheckExistsAsync(path);
                if (existenceCheck != ExistenceCheckResult.FileExists)
                {
                    return null;
                }
            }

            var file = await RootFolder.CreateFileAsync(path, CreationCollisionOption.OpenIfExists);
            var output = await file.OpenAsync(accessType);
            return output;
        }

        private async Task<IEnumerable<IFile>> ListFilesRecursiveAsync(IFolder folder)
        {
            IEnumerable<IFile> files = await folder.GetFilesAsync();
            var subfolders = await folder.GetFoldersAsync();
            foreach (var i in subfolders)
            {
                var subfolderFiles = await ListFilesRecursiveAsync(i);
                files = files.Concat(subfolderFiles);
            }

            return files;
        }
    }
}

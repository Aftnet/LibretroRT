using Plugin.FileSystem.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RetriX.Shared.StreamProviders
{
    public class FolderStreamProvider : StreamProviderBase
    {
        private readonly string HandledScheme;
        private readonly IDirectoryInfo RootFolder;

        public FolderStreamProvider(string handledScheme, IDirectoryInfo rootFolder)
        {
            HandledScheme = handledScheme;
            RootFolder = rootFolder;
        }

        public override async Task<IEnumerable<string>> ListEntriesAsync()
        {
            var files = await ListFilesRecursiveAsync(RootFolder);
            var output = files.Select(d => HandledScheme + d.FullName.Substring(RootFolder.FullName.Length + 1)).OrderBy(d => d).ToArray();
            return output;
        }

        protected override async Task<Stream> OpenFileStreamAsyncInternal(string path, FileAccess accessType)
        {
            if (!path.StartsWith(HandledScheme, System.StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            path = path.Substring(HandledScheme.Length + 1);
            var file = (await RootFolder.EnumerateFilesAsync()).FirstOrDefault(d => d.FullName == path);
            if (accessType == FileAccess.Read && file==null)
            {
                return null;
            }

            if (file == null)
            {
                await RootFolder.CreateFileAsync(path);
            }

            var output = await file.OpenAsync(accessType);
            return output;
        }

        private async Task<IEnumerable<IFileInfo>> ListFilesRecursiveAsync(IDirectoryInfo folder)
        {
            IEnumerable<IFileInfo> files = await folder.EnumerateFilesAsync();
            var subfolders = await folder.EnumerateDirectoriesAsync();
            foreach (var i in subfolders)
            {
                var subfolderFiles = await ListFilesRecursiveAsync(i);
                files = files.Concat(subfolderFiles);
            }

            return files;
        }
    }
}

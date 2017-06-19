using PCLStorage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RetriX.Shared.FileProviders
{
    public class FolderFileProvider : IFileProvider
    {
        private readonly string HandledScheme;
        private readonly IFolder RootFolder;

        public FolderFileProvider(string handledScheme, IFolder rootFolder)
        {
            HandledScheme = handledScheme;
            RootFolder = rootFolder;
        }

        public void Dispose()
        {

        }

        public async Task<Stream> GetFileStreamAsync(string path, System.IO.FileAccess accessType)
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
            var output = await file.OpenAsync(accessType.ToPCLStorageAccess());
            return output;
        }
    }
}

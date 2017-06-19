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

        public async Task<Stream> GetFileStreamAsync(Uri uri, System.IO.FileAccess accessType)
        {
            if (uri.Scheme != HandledScheme)
                return null;

            var existenceCheck = await RootFolder.CheckExistsAsync(uri.LocalPath);
            if (existenceCheck != ExistenceCheckResult.FileExists)
                return null;

            var file = await RootFolder.GetFileAsync(uri.LocalPath);
            var output = await file.OpenAsync(accessType.ToPCLStorageAccess());
            return output;
        }
    }
}

using System.IO;
using Windows.Storage;
using Windows.Storage.Streams;

namespace RetriX.UWP.FileProviders
{
    public class FileProviderWrapper : LibretroRT.FrontendComponents.Common.IFileProvider
    {
        private readonly Shared.FileProviders.IFileProvider Provider;

        public FileProviderWrapper(Shared.FileProviders.IFileProvider provider)
        {
            Provider = provider;
        }

        public IRandomAccessStream GetFileStream(string path, FileAccessMode fileAccess)
        {
            var convertedAccess = fileAccess == FileAccessMode.Read ? FileAccess.Read : FileAccess.ReadWrite;
            var managedStream = Provider.GetFileStreamAsync(path, convertedAccess).Result;
            return managedStream.AsRandomAccessStream();
        }
    }
}

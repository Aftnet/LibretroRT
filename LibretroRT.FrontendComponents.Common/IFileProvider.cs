using System;
using Windows.Storage;
using Windows.Storage.Streams;

namespace LibretroRT.FrontendComponents.Common
{
    public interface IFileProvider
    {
        IRandomAccessStream GetFileStream(String path, FileAccessMode fileAccess);
    }
}

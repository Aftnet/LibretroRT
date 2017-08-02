using System.IO;
using Windows.Storage;

namespace RetriX.UWP.FileProviders
{
    public static class FileAccessExtensions
    {
        public static FileAccess ToIOAccess(this FileAccessMode value)
        {
            switch (value)
            {
                case FileAccessMode.Read:
                    return FileAccess.Read;
                default:
                    return FileAccess.ReadWrite;
            }
        }
    }
}

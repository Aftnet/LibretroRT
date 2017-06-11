using RetriX.Shared.Services;
using Windows.Storage;

namespace RetriX.UWP.Services
{
    public class PlatformFileWrapper : IPlatformFileWrapper
    {
        private readonly IStorageFile WrappedFile;
        public object File => WrappedFile;

        public PlatformFileWrapper(IStorageFile wrappedFile)
        {
            WrappedFile = wrappedFile;
        }
    }
}
